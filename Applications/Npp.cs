using devkit2.Common;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    internal sealed class Npp : BaseApplication
    {
        public override string Name => "Notepad++";

        public Npp()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "npp");
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
                    Path.Combine(appPath, InstalledVersions[0].Value, "notepad++.exe")
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
                    new ValueName("8.9.3", "8.9.3"),
                    new ValueName("8.9.2", "8.9.2"),
                };
            }
        }

        public override bool Install(string version, IProgress<InstallProgress>? progress = null)
        {
            string url = string.Empty;
            string file = string.Empty;
            switch (version)
            {
                case "8.9.3":
                    url = "https://github.com/notepad-plus-plus/notepad-plus-plus/releases/download/v8.9.3/npp.8.9.3.portable.x64.zip";
                    file = Path.Combine(Path.GetTempPath(), "npp.8.9.3.portable.x64.zip");
                    break;
                case "8.9.2":
                    url = "https://github.com/notepad-plus-plus/notepad-plus-plus/releases/download/v8.9.2/npp.8.9.2.portable.x64.zip";
                    file = Path.Combine(Path.GetTempPath(), "npp.8.9.2.portable.x64.zip");
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

                var installed = InstalledVersions;
                if (installed.Length > 0)
                {
                    string exePath = Path.Combine(appPath, installed[0].Value, "notepad++.exe");
                    base.RegisterContextMenu(exePath, false);
                }

                return true;
            }
            return false;
        }

        public override bool Uninstall(string version)
        {
            bool bResult = base.Uninstall(version);
            var installed = InstalledVersions;
            if (installed.Length > 0)
            {
                string exePath = Path.Combine(appPath, installed[0].Value, "notepad++.exe");
                base.RegisterContextMenu(exePath, false);
            }
            else
            {
                base.UnregisterContextMenu();
            }
            return bResult;
        }

        public override ValueName[] GetEnvironments(string version)
        {
            return new ValueName[] {
                new ValueName("PATH", Path.Combine(appPath, version)),
            };
        }

        public override bool Start(string version, ValueName[] environments, JsonObject? profile = null, string uniqueCode = "")
        {
            var psi = new ProcessStartInfo();
            psi.FileName = Path.Combine(appPath, version, "notepad++.exe");
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
