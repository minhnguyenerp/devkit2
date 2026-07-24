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
            Task.Run(async () => { ReloadIcon(); });
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
                    new ValueName("0.17.0-dev.1454+5faa79730", "0.17.0-dev.1454+5faa79730") { Tag = "https://ziglang.org/builds/zig-x86_64-windows-0.17.0-dev.1454+5faa79730.zip" },
                    new ValueName("0.17.0-dev.704+b8cb78023", "0.17.0-dev.704+b8cb78023") { Tag = "https://ziglang.org/builds/zig-x86_64-windows-0.17.0-dev.704+b8cb78023.zip" },
                    new ValueName("0.17.0-dev.633+9c5655093", "0.17.0-dev.633+9c5655093") { Tag = "https://ziglang.org/builds/zig-x86_64-windows-0.17.0-dev.633+9c5655093.zip" },
                    new ValueName("0.17.0-dev.292+fc1c83a36", "0.17.0-dev.292+fc1c83a36") { Tag = "https://ziglang.org/builds/zig-x86_64-windows-0.17.0-dev.292+fc1c83a36.zip" },
                    new ValueName("0.17.0-dev.248+95507faf1", "0.17.0-dev.248+95507faf1") { Tag = "https://ziglang.org/builds/zig-x86_64-windows-0.17.0-dev.248+95507faf1.zip" },
                    new ValueName("0.17.0-dev.135+9df02121d", "0.17.0-dev.135+9df02121d") { Tag = "https://ziglang.org/builds/zig-x86_64-windows-0.17.0-dev.135+9df02121d.zip" },
                    new ValueName("0.16.0-dev.3153+d6f43caad", "0.16.0-dev.3153+d6f43caad") { Tag = "https://ziglang.org/builds/zig-x86_64-windows-0.16.0-dev.3153+d6f43caad.zip" },
                    new ValueName("0.16.0-dev.3133+5ec8e45f3", "0.16.0-dev.3133+5ec8e45f3") { Tag = "https://ziglang.org/builds/zig-x86_64-windows-0.16.0-dev.3133+5ec8e45f3.zip" },
                    new ValueName("0.16.0-dev.3070+b22eb176b", "0.16.0-dev.3070+b22eb176b") { Tag = "https://ziglang.org/builds/zig-x86_64-windows-0.16.0-dev.3070+b22eb176b.zip" },
                    new ValueName("0.16.0-dev.2973+06b85a4fd", "0.16.0-dev.2973+06b85a4fd") { Tag = "https://ziglang.org/builds/zig-x86_64-windows-0.16.0-dev.2973+06b85a4fd.zip" },
                    new ValueName("0.16.0-dev.2736+3b515fbed", "0.16.0-dev.2736+3b515fbed") { Tag = "https://ziglang.org/builds/zig-x86_64-windows-0.16.0-dev.2736+3b515fbed.zip" },
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

            file = Path.Combine(Path.GetTempPath(), $"zig-x86_64-windows-{version}.zip");

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
