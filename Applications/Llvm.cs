using devkit2.Common;
using devkit2.Properties;
using SharpCompress.Common;
using SharpCompress.Readers;
using System.Diagnostics;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    internal sealed class Llvm : BaseApplication
    {
        public override string Name => "Llvm";

        public Llvm()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "llvm");
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
            }
            base.LoadConfig(appPath);
#if DEBUG
            Task.Run(async () => { ReloadIcon(); });
#endif
        }

        public override void ReloadIcon()
        {
            try
            {
                base.Icon = Icon.ExtractAssociatedIcon(Environment.SystemDirectory + @"\cmd.exe");
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
                    new ValueName("22.1.8", "22.1.8") { Tag = "https://github.com/llvm/llvm-project/releases/download/llvmorg-22.1.8/clang+llvm-22.1.8-x86_64-pc-windows-msvc.tar.xz" },
                };
            }
        }

        public override bool Install(string version, IProgress<InstallProgress>? progress = null)
        {
            string url = string.Empty;
            string file = string.Empty;

            foreach (var one in AvailableVersions)
            {
                if (one.Value == version)
                {
                    url = one.Tag?.ToString() ?? string.Empty;
                    break;
                }
            }

            file = Path.Combine(Path.GetTempPath(), $"clang+llvm-{version}-x86_64-pc-windows-msvc.tar.xz");

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
                            progress?.Report(new InstallProgress { Message = reader.Entry.Key ?? "" });
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
                new ValueName("LLVM_HOME", Path.Combine(appPath, version, $"clang+llvm-{version}-x86_64-pc-windows-msvc")),
                new ValueName("LIBCLANG_PATH", Path.Combine(appPath, version, $"clang+llvm-{version}-x86_64-pc-windows-msvc", "bin")),
                new ValueName("PATH", Path.Combine(appPath, version, $"clang+llvm-{version}-x86_64-pc-windows-msvc", "bin")),
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
            LoadEnvironments(ref psi, environments, profile);

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
                        Profile = profile,
                    });
                    return true;
                }
            }
            catch { return false; }
            return false;
        }
    }
}
