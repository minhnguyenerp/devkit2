using devkit2.Common;
using devkit2.Properties;
using SharpCompress.Common;
using SharpCompress.Readers;
using System.Diagnostics;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    internal sealed class Rust : BaseApplication
    {
        public override string Name => "Rust";

        public Rust()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "rust");
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
            }
            base.LoadConfig(appPath);
            ReloadIcon();
        }

        public override void ReloadIcon()
        {
            try
            {
                base.Icon = Resources.file_type_rust_icon_130185;
            }
            catch { }
        }

        public override bool Valid
        {
            get
            {
                if (Config != null)
                    return true;
                return false;
            }
        }

        public override ValueName[] AvailableVersions
        {
            get
            {
                return new ValueName[]
                {
                    new ValueName("1.94.0", "1.94.0"),
                };
            }
        }

        public override bool Install(string version, IProgress<DownloadProgress>? progress = null)
        {
            string url = string.Empty;
            string file = string.Empty;
            switch (version)
            {
                case "1.94.0":
                    url = "https://static.rust-lang.org/dist/rust-1.94.0-x86_64-pc-windows-gnu.tar.xz";
                    file = Path.Combine(Path.GetTempPath(), "rust-1.94.0-x86_64-pc-windows-gnu.tar.xz");
                    break;
            }

            if (url != string.Empty && file != string.Empty)
            {
                if (!base.Download(url, file, progress))
                {
                    return false;
                }

                string extractPath = Path.Combine(appPath, version);
                Directory.CreateDirectory(extractPath);
                try
                {
                    using var stream = File.OpenRead(file);
                    using var reader = ReaderFactory.OpenReader(stream);

                    while (reader.MoveToNextEntry())
                    {
                        if (!reader.Entry.IsDirectory)
                        {
                            reader.WriteEntryToDirectory(extractPath, new ExtractionOptions
                            {
                                ExtractFullPath = true,
                                Overwrite = true
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    File.Delete(file);
                    return false;
                }

                base.SaveNewVersion(version);

                return true;
            }
            return false;
        }

        public override ValueName[] GetEnvironments(string version)
        {
            return new ValueName[] {
                new ValueName("PATH", Path.Combine(appPath, version, $"rust-{version}-x86_64-pc-windows-gnu", "rustc", "bin")),
                new ValueName("PATH", Path.Combine(appPath, version, $"rust-{version}-x86_64-pc-windows-gnu", "cargo", "bin")),
                new ValueName("RUSTFLAGS", "--sysroot=" + Path.Combine(appPath, version, $"rust-{version}-x86_64-pc-windows-gnu", "rust-std-x86_64-pc-windows-gnu")),
                new ValueName("CARGO_HOME", Path.Combine(appPath, version, $"rust-{version}-x86_64-pc-windows-gnu", "cargo")),
            };
        }

        public override bool Start(string version, ValueName[] environments, JsonObject? profile = null, string uniqueCode = "")
        {
            var psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            psi.UseShellExecute = false;
            string workingDir = profile?["WorkingDirectory"]?.ToString() ?? string.Empty;
            if (!string.IsNullOrEmpty(workingDir) && Directory.Exists(workingDir))
            {
                psi.WorkingDirectory = workingDir;
            }
            LoadEnvironments(ref psi, environments);

            try
            {
                var proc = Process.Start(psi);
                if (proc != null)
                {
                    Sysconf.Instance.AddRunningApplication(new RunningApplication
                    {
                        UniqueCode = uniqueCode,
                        Pid = proc.Id,
                        Sessionid = proc.SessionId,
                        ProcessName = proc.ProcessName,
                        StartTime = proc.StartTime,
                        ApplicationName = Name,
                        ApplicationVersion = version,
                    });
                    return true;
                }
            }
            catch { return false; }
            return false;
        }
    }
}
