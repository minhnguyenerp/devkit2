using devkit2.Common;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    internal sealed class Nim : BaseApplication
    {
        public override string Name => "Nim";

        public Nim()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "nim");
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
                    new ValueName("2.2.8", "2.2.8"),
                };
            }
        }

        public override bool Install(string version)
        {
            string url = string.Empty;
            string file = string.Empty;
            switch (version)
            {
                case "2.2.8":
                    url = "https://nim-lang.org/download/nim-2.2.8_x64.zip";
                    file = Path.Combine(Path.GetTempPath(), "nim-2.2.8_x64.zip");
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
                new ValueName("PATH", Path.Combine(appPath, version, $"nim-{version}", "bin")),
            };
        }

        public override bool Start(string version, ValueName[] environments, JsonObject? profile = null)
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
                            _icon = Icon.ExtractAssociatedIcon(
                                Path.Combine(appPath, InstalledVersions[0].Value, $"nim-{InstalledVersions[0].Value}", "bin", "nim.exe")
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
