using devkit2.Common;
using devkit2.Properties;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    internal sealed class Bombardier : BaseApplication
    {
        public override string Name => "Bombardier";

        public Bombardier()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "bombardier");
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
                base.Icon = Icon.ExtractAssociatedIcon(Path.Combine(appPath, InstalledVersions[0].Value, "bombardier-windows-amd64.exe"));
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
                    new ValueName("2.0.2", "2.0.2"),
                };
            }
        }

        public override bool Install(string version, IProgress<InstallProgress>? progress = null)
        {
            string url = string.Empty;
            string file = string.Empty;
            switch (version)
            {
                case "2.0.2":
                    url = "https://github.com/codesenberg/bombardier/releases/download/v2.0.2/bombardier-windows-amd64.exe";
                    file = Path.Combine(appPath, version, "bombardier-windows-amd64.exe");
                    break;
            }

            if (url != string.Empty && file != string.Empty)
            {
                Directory.CreateDirectory(Path.Combine(appPath, version));
                if (!base.Download(url, file, progress))
                {
                    return false;
                }
                File.WriteAllText(Path.Combine(appPath, version, "bombardier.bat"),
@"@echo off
""%~dp0bombardier-windows-amd64.exe"" %*");

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
