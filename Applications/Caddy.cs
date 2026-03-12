using devkit2.Common;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    internal sealed class Caddy : BaseApplication
    {
        public override string Name => "Caddy";

        public Caddy()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "caddy");
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
                    new ValueName("2.11.2", "2.11.2"),
                };
            }
        }

        public override bool Install(string version)
        {
            string url = string.Empty;
            string file = string.Empty;
            switch (version)
            {
                case "2.11.2":
                    url = "https://github.com/caddyserver/caddy/releases/download/v2.11.2/caddy_2.11.2_windows_amd64.zip";
                    file = Path.Combine(Path.GetTempPath(), "caddy_2.11.2_windows_amd64.zip");
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
            psi.FileName = Path.Combine(appPath, version, "caddy.exe");
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
    }
}
