using devkit2.Common;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    internal sealed class VSCode : BaseApplication
    {
        public override string Name => "VSCode";

        public VSCode()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "vscode");
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
                base.Icon = Icon.ExtractAssociatedIcon(
                    Path.Combine(appPath, InstalledVersions[0].Value, "Code.exe")
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
                    new ValueName("1.130.0", "1.130.0") { Tag = "https://vscode.download.prss.microsoft.com/dbazure/download/stable/1b6a188127eeaf9194f945eb6eb89a657e93c54c/VSCode-win32-x64-1.130.0.zip" },
                    new ValueName("1.123.0", "1.123.0") { Tag = "https://vscode.download.prss.microsoft.com/dbazure/download/stable/6a44c352bd24569c417e530095901b649960f9f8/VSCode-win32-x64-1.123.0.zip" },
                    new ValueName("1.122.1", "1.122.1") { Tag = "https://vscode.download.prss.microsoft.com/dbazure/download/stable/8761a5560cfd65fdd19ce7e2bd18dab5c0a4d84e/VSCode-win32-x64-1.122.1.zip" },
                    new ValueName("1.119.0", "1.119.0") { Tag = "https://vscode.download.prss.microsoft.com/dbazure/download/stable/8b640eef5a6c6089c029249d48efa5c99adf7d51/VSCode-win32-x64-1.119.0.zip" },
                    new ValueName("1.118.1", "1.118.1") { Tag = "https://vscode.download.prss.microsoft.com/dbazure/download/stable/034f571df509819cc10b0c8129f66ef77a542f0e/VSCode-win32-x64-1.118.1.zip" },
                    new ValueName("1.117.0", "1.117.0") { Tag = "https://vscode.download.prss.microsoft.com/dbazure/download/stable/10c8e557c8b9f9ed0a87f61f1c9a44bde731c409/VSCode-win32-x64-1.117.0.zip" },
                    new ValueName("1.116.0", "1.116.0") { Tag = "https://vscode.download.prss.microsoft.com/dbazure/download/stable/560a9dba96f961efea7b1612916f89e5d5d4d679/VSCode-win32-x64-1.116.0.zip" },
                    new ValueName("1.115.0", "1.115.0") { Tag = "https://vscode.download.prss.microsoft.com/dbazure/download/stable/41dd792b5e652393e7787322889ed5fdc58bd75b/VSCode-win32-x64-1.115.0.zip" },
                    new ValueName("1.114.0", "1.114.0") { Tag = "https://vscode.download.prss.microsoft.com/dbazure/download/stable/e7fb5e96c0730b9deb70b33781f98e2f35975036/VSCode-win32-x64-1.114.0.zip" },
                    new ValueName("1.112.0", "1.112.0") { Tag = "https://vscode.download.prss.microsoft.com/dbazure/download/stable/07ff9d6178ede9a1bd12ad3399074d726ebe6e43/VSCode-win32-x64-1.112.0.zip" },
                    new ValueName("1.111.0", "1.111.0") { Tag = "https://vscode.download.prss.microsoft.com/dbazure/download/stable/ce099c1ed25d9eb3076c11e4a280f3eb52b4fbeb/VSCode-win32-x64-1.111.0.zip" },
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

            file = Path.Combine(Path.GetTempPath(), $"VSCode-win32-x64-{version}.zip");

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
                    string exePath = Path.Combine(appPath, installed[0].Value, "Code.exe");
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
                string exePath = Path.Combine(appPath, installed[0].Value, "Code.exe");
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
            psi.FileName = Path.Combine(appPath, version, @"Code.exe");
            string workingDir = profile?["WorkingDirectory"]?.ToString() ?? string.Empty;
            if(!string.IsNullOrEmpty(workingDir) && Directory.Exists(workingDir))
            {
                psi.WorkingDirectory = workingDir;
            }
            string startupFile = profile?["StartupFile"]?.ToString() ?? string.Empty;
            if (!string.IsNullOrEmpty(startupFile) && (File.Exists(startupFile) || Directory.Exists(startupFile)))
            {
                psi.ArgumentList.Add(startupFile);
            }
            psi.UseShellExecute = false;
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return false;
        }
    }
}
