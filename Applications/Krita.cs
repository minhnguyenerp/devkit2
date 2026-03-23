using devkit2.Common;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    internal sealed class Krita : BaseApplication
    {
        public override string Name => "Krita";

        public Krita()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "krita");
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
            }
            base.LoadConfig(appPath);
            try
            {
                base.Icon = Icon.ExtractAssociatedIcon(Path.Combine(appPath, InstalledVersions[0].Value, $"krita-x64-{InstalledVersions[0].Value}", "bin", "krita.exe"));
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
                    new ValueName("5.2.16", "5.2.16"),
                };
            }
        }

        public override bool Install(string version, IProgress<DownloadProgress>? progress = null)
        {
            string url = string.Empty;
            string file = string.Empty;
            switch (version)
            {
                case "5.2.16":
                    url = "https://download.kde.org/stable/krita/5.2.16/krita-x64-5.2.16.zip";
                    file = Path.Combine(Path.GetTempPath(), "krita-x64-5.2.16.zip");
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
                new ValueName("PATH", Path.Combine(appPath, version, $"krita-x64-{version}", "bin")),
            };
        }

        public override bool Start(string version, ValueName[] environments, JsonObject? profile = null, string uniqueCode = "")
        {
            var psi = new ProcessStartInfo();
            psi.FileName = Path.Combine(appPath, version, $"krita-x64-{version}", "bin", "krita.exe");
            string workingDir = profile?["WorkingDirectory"]?.ToString() ?? string.Empty;
            if (!string.IsNullOrEmpty(workingDir) && Directory.Exists(workingDir))
            {
                psi.WorkingDirectory = workingDir;
            }
            string startupFile = profile?["StartupFile"]?.ToString() ?? string.Empty;
            if (!string.IsNullOrEmpty(startupFile) && (File.Exists(startupFile) || Directory.Exists(startupFile)))
            {
                psi.ArgumentList.Add(startupFile);
            }
            psi.UseShellExecute = false;
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
