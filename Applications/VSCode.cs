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
                    new ValueName("1.111.0", "1.111.0"),
                };
            }
        }

        public override bool Install(string version)
        {
            string url = string.Empty;
            string file = string.Empty;
            switch (version)
            {
                case "1.111.0":
                    url = "https://vscode.download.prss.microsoft.com/dbazure/download/stable/ce099c1ed25d9eb3076c11e4a280f3eb52b4fbeb/VSCode-win32-x64-1.111.0.zip";
                    file = Path.Combine(Path.GetTempPath(), "VSCode-win32-x64-1.111.0.zip");
                    break;
            }

            if (url != string.Empty && file != string.Empty)
            {
                if (!File.Exists(file))
                {
                    if (!base.Download(url, file))
                    {
                        return false;
                    }
                }

                string extractPath = Path.Combine(appPath, version);
                Directory.CreateDirectory(extractPath);
                ZipFile.ExtractToDirectory(file, extractPath, true);
                Directory.CreateDirectory(Path.Combine(extractPath, "data"));

                if (!IsInstalled(version) && Config != null && Config["InstalledVersions"] != null && Config["InstalledVersions"] is JsonArray)
                {
                    ((JsonArray)Config["InstalledVersions"]).Add(version);
                }
                base.SaveConfig(Config, appPath);

                return true;
            }
            return false;
        }

        public override ValueName[] GetEnvironments(string version)
        {
            return new ValueName[] {
                new ValueName("PATH", Path.Combine(appPath, version)),
            };
        }

        public override bool Start(string version, ValueName[] environments, JsonObject? profile = null)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = Path.Combine(appPath, version, @"Code.exe");
            if (profile != null)
            {
                string workingDir = profile["WorkingDirectory"]?.ToString() ?? string.Empty;
                if(!string.IsNullOrEmpty(workingDir) && Directory.Exists(workingDir))
                {
                    psi.WorkingDirectory = workingDir;
                    psi.ArgumentList.Add(workingDir);
                }
            }
            LoadEnvironments(ref psi, environments);

            try
            {
                if (Process.Start(psi) != null)
                {
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

        public override bool Stop(string version)
        {
            return false;
        }

        public override Icon Icon
        {
            get
            {
                if (_icon == null)
                {
                    if (InstalledVersions.Length > 0)
                    {
                        try
                        {
                            _icon = Icon.ExtractAssociatedIcon(
                                Path.Combine(appPath, InstalledVersions[0].Value, "Code.exe")
                            );
                        }
                        catch { }
                    }
                }
                if (_icon == null)
                {
                    _icon = base.Icon;
                }
                return _icon;
            }
        }
    }
}
