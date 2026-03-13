using devkit2.Common;
using System.Diagnostics;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    //https://getcomposer.org/download/2.9.5/composer.phar
    internal sealed class Composer : BaseApplication
    {
        public override string Name => "Composer";

        public Composer()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "composer");
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
                    new ValueName("2.9.5", "2.9.5"),
                };
            }
        }

        public override bool Install(string version)
        {
            string url = string.Empty;
            string file = string.Empty;
            switch (version)
            {
                case "2.9.5":
                    url = "https://getcomposer.org/download/2.9.5/composer.phar";
                    file = Path.Combine(appPath, version, "composer.phar");
                    break;
            }

            if (url != string.Empty && file != string.Empty)
            {
                if (!File.Exists(file))
                {
                    Directory.CreateDirectory(Path.Combine(appPath, version));
                    if (!base.Download(url, file))
                    {
                        return false;
                    }
                    File.WriteAllText(Path.Combine(appPath, version, "composer.bat"),
@"@echo off
php.exe ""%~dp0composer.phar"" %*");
                }

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

        public override bool Start(string version, ValueName[] environments)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            psi.UseShellExecute = false;
            LoadEnvironments(ref psi, environments);

            try
            {
                if (Process.Start(psi) != null)
                {
                    return true;
                }
            }
            catch { return false; }
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
                            _icon = Icon.ExtractAssociatedIcon(Environment.SystemDirectory + @"\cmd.exe");
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
