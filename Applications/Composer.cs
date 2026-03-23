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
            ReloadIcon();
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
                    new ValueName("2.9.5", "2.9.5"),
                };
            }
        }

        public override bool Install(string version, IProgress<DownloadProgress>? progress = null)
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
                Directory.CreateDirectory(Path.Combine(appPath, version));
                if (!base.Download(url, file, progress))
                {
                    return false;
                }
                File.WriteAllText(Path.Combine(appPath, version, "composer.bat"),
@"@echo off
php.exe ""%~dp0composer.phar"" %*");

                base.SaveNewVersion(version);

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

        public override bool Start(string version, ValueName[] environments, JsonObject? profile = null, string uniqueCode = "")
        {
            var psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
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
