using devkit2.Common;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    internal sealed class Zig : BaseApplication
    {
        public override string Name => "Zig";

        public Zig()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "zig");
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
                base.Icon = Icon.ExtractAssociatedIcon(
                    Path.Combine(appPath, InstalledVersions[0].Value, $"zig-x86_64-windows-{InstalledVersions[0].Value}", "zig.exe")
                );
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
                    new ValueName("0.16.0-dev.3153+d6f43caad", "0.16.0-dev.3153+d6f43caad"),
                    new ValueName("0.16.0-dev.3133+5ec8e45f3", "0.16.0-dev.3133+5ec8e45f3"),
                    new ValueName("0.16.0-dev.3070+b22eb176b", "0.16.0-dev.3070+b22eb176b"),
                    new ValueName("0.16.0-dev.2973+06b85a4fd", "0.16.0-dev.2973+06b85a4fd"),
                    new ValueName("0.16.0-dev.2736+3b515fbed", "0.16.0-dev.2736+3b515fbed"),
                };
            }
        }

        public override bool Install(string version, IProgress<InstallProgress>? progress = null)
        {
            string url = string.Empty;
            string file = string.Empty;
            switch (version)
            {
                case "0.16.0-dev.3153+d6f43caad":
                    url = "https://ziglang.org/builds/zig-x86_64-windows-0.16.0-dev.3153+d6f43caad.zip";
                    file = Path.Combine(Path.GetTempPath(), "zig-x86_64-windows-0.16.0-dev.3153+d6f43caad.zip");
                    break;
                case "0.16.0-dev.3133+5ec8e45f3":
                    url = "https://ziglang.org/builds/zig-x86_64-windows-0.16.0-dev.3133+5ec8e45f3.zip";
                    file = Path.Combine(Path.GetTempPath(), "zig-x86_64-windows-0.16.0-dev.3133+5ec8e45f3.zip");
                    break;
                case "0.16.0-dev.3070+b22eb176b":
                    url = "https://ziglang.org/builds/zig-x86_64-windows-0.16.0-dev.3070+b22eb176b.zip";
                    file = Path.Combine(Path.GetTempPath(), "zig-x86_64-windows-0.16.0-dev.3070+b22eb176b.zip");
                    break;
                case "0.16.0-dev.2973+06b85a4fd":
                    url = "https://ziglang.org/builds/zig-x86_64-windows-0.16.0-dev.2973+06b85a4fd.zip";
                    file = Path.Combine(Path.GetTempPath(), "zig-x86_64-windows-0.16.0-dev.2973+06b85a4fd.zip");
                    break;
                case "0.16.0-dev.2736+3b515fbed":
                    url = "https://ziglang.org/builds/zig-x86_64-windows-0.16.0-dev.2736+3b515fbed.zip";
                    file = Path.Combine(Path.GetTempPath(), "zig-x86_64-windows-0.16.0-dev.2736+3b515fbed.zip");
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
                    ZipFile.ExtractToDirectory(file, extractPath, true);
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
                new ValueName("PATH", Path.Combine(appPath, version, $"zig-x86_64-windows-{version}")),
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
