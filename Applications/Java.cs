using devkit2.Common;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    internal sealed class Java : BaseApplication
    {
        public override string Name => "Java";

        public Java()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "java");
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
                base.Icon = Icon.ExtractAssociatedIcon(Path.Combine(appPath, InstalledVersions[0].Value, $"jdk-{InstalledVersions[0].Value}", "bin", "java.exe"));
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
                    new ValueName("26", "26"),
                    new ValueName("25.0.2", "25.0.2"),
                    new ValueName("24.0.2", "24.0.2"),
                };
            }
        }

        public override bool Install(string version, IProgress<InstallProgress>? progress = null)
        {
            string url = string.Empty;
            string file = string.Empty;
            switch (version)
            {
                case "26":
                    url = "https://download.java.net/java/GA/jdk26/c3cc523845074aa0af4f5e1e1ed4151d/35/GPL/openjdk-26_windows-x64_bin.zip";
                    file = Path.Combine(Path.GetTempPath(), "openjdk-26_windows-x64_bin.zip");
                    break;
                case "25.0.2":
                    url = "https://download.java.net/java/GA/jdk25.0.2/b1e0dfa218384cb9959bdcb897162d4e/10/GPL/openjdk-25.0.2_windows-x64_bin.zip";
                    file = Path.Combine(Path.GetTempPath(), "openjdk-25.0.2_windows-x64_bin.zip");
                    break;
                case "24.0.2":
                    url = "https://download.java.net/java/GA/jdk24.0.2/fdc5d0102fe0414db21410ad5834341f/12/GPL/openjdk-24.0.2_windows-x64_bin.zip";
                    file = Path.Combine(Path.GetTempPath(), "openjdk-24.0.2_windows-x64_bin.zip");
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
                new ValueName("PATH", Path.Combine(appPath, version, $"jdk-{version}", "bin")),
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
