using devkit2.Common;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    internal sealed class VSCodium : BaseApplication
    {
        public override string Name => "VSCodium";

        public VSCodium()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "vscodium");
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
                    Path.Combine(appPath, InstalledVersions[0].Value, "VSCodium.exe")
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
                    new ValueName("1.112.01907", "1.112.01907"),
                };
            }
        }

        public override bool Install(string version, IProgress<InstallProgress>? progress = null)
        {
            string url = string.Empty;
            string file = string.Empty;
            switch (version)
            {
                case "1.112.01907":
                    url = "https://github.com/VSCodium/vscodium/releases/download/1.112.01907/VSCodium-win32-x64-1.112.01907.zip";
                    file = Path.Combine(Path.GetTempPath(), "VSCodium-win32-x64-1.112.01907.zip");
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

                Directory.CreateDirectory(Path.Combine(extractPath, "data"));
                string settingPath = Path.Combine(extractPath, "data", "user-data", "User");
                Directory.CreateDirectory(settingPath);
                File.WriteAllText(Path.Combine(settingPath, "settings.json"), """
{
    "telemetry.telemetryLevel": "off",
    "extensions.autoUpdate": false,
    "editor.minimap.enabled": false,
    "editor.smoothScrolling": false,
    "workbench.list.smoothScrolling": false,
    "terminal.integrated.smoothScrolling": false,
    "files.watcherExclude": {
        "**/.git/objects/**": true,
        "**/.git/subtree-cache/**": true,
        "**/node_modules/**": true,
        "**/bower_components/**": true,
        "**/env/**": true,
        "**/venv/**": true,
        "**/dist/**": true,
        "**/build/**": true,
        "**/.DS_Store": true,
        "env-*": true
    },
    "search.exclude": {
        "**/node_modules": true,
        "**/bower_components": true,
        "**/dist": true,
        "**/build": true,
        "**/env": true,
        "**/venv": true
    }
}
""");

                base.SaveNewVersion(version);

                var installed = InstalledVersions;
                if (installed.Length > 0)
                {
                    string exePath = Path.Combine(appPath, installed[0].Value, "VSCodium.exe");
                    base.RegisterContextMenu(exePath);
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
                string exePath = Path.Combine(appPath, installed[0].Value, "VSCodium.exe");
                base.RegisterContextMenu(exePath);
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
            psi.FileName = Path.Combine(appPath, version, "VSCodium.exe");
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return false;
        }
    }
}
