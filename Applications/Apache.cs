using devkit2.Common;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    internal sealed class Apache : BaseApplication
    {
        public override string Name => "Apache";

        public Apache()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "apache");
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
                    new ValueName("2.4.66-260223", "2.4.66-260223"),
                };
            }
        }

        public override bool Install(string version)
        {
            List<(string Url, string File)> list = new List<(string Url, string File)>();
            switch (version)
            {
                case "2.4.66-260223":
                    list.Add((
                        "https://www.apachelounge.com/download/VS18/binaries/httpd-2.4.66-260223-Win64-VS18.zip",
                        Path.Combine(Path.GetTempPath(), "httpd-2.4.66-260223-Win64-VS18.zip")
                    ));
                    list.Add((
                        "https://www.apachelounge.com/download/VS18/modules/mod_jk-1.2.50-win64-VS18.zip",
                        Path.Combine(Path.GetTempPath(), "mod_jk-1.2.50-win64-VS18.zip")
                    ));
                    list.Add((
                        "https://www.apachelounge.com/download/VS18/modules/mod_qos-11.76-win64-VS18.zip",
                        Path.Combine(Path.GetTempPath(), "mod_qos-11.76-win64-VS18.zip")
                    ));
                    list.Add((
                        "https://www.apachelounge.com/download/VS18/modules/mod_fcgid-2.3.10-win64-VS18.zip",
                        Path.Combine(Path.GetTempPath(), "mod_fcgid-2.3.10-win64-VS18.zip")
                    ));
                    list.Add((
                        "https://www.apachelounge.com/download/VS18/modules/mod_security-2.9.12-win64-VS18.zip",
                        Path.Combine(Path.GetTempPath(), "mod_security-2.9.12-win64-VS18.zip")
                    ));
                    list.Add((
                        "https://www.apachelounge.com/download/VS18/modules/mod_xsendfile-0.12-win64-VS18.zip",
                        Path.Combine(Path.GetTempPath(), "mod_xsendfile-0.12-win64-VS18.zip")
                    ));
                    list.Add((
                        "https://www.apachelounge.com/download/VS18/modules/mod_log_rotate-1.0.2-win64-VS18.zip",
                        Path.Combine(Path.GetTempPath(), "mod_log_rotate-1.0.2-win64-VS18.zip")
                    ));
                    list.Add((
                        "https://www.apachelounge.com/download/VS18/modules/dbd_modules-1.0.6-win64-VS18.zip",
                        Path.Combine(Path.GetTempPath(), "dbd_modules-1.0.6-win64-VS18.zip")
                    ));
                    list.Add((
                        "https://www.apachelounge.com/download/VS18/modules/mod_bw-0.92-win64-VS18.zip",
                        Path.Combine(Path.GetTempPath(), "mod_bw-0.92-win64-VS18.zip")
                    ));
                    list.Add((
                        "https://www.apachelounge.com/download/VS18/modules/mod_view-2.2-win64-VS18.zip",
                        Path.Combine(Path.GetTempPath(), "mod_view-2.2-win64-VS18.zip")
                    ));
                    list.Add((
                        "https://www.apachelounge.com/download/VS18/modules/mod_watch-4.3-win64-VS18.zip",
                        Path.Combine(Path.GetTempPath(), "mod_watch-4.3-win64-VS18.zip")
                    ));
                    list.Add((
                        "https://www.apachelounge.com/download/VS18/modules/mod_evasive-2.4.0-win64-VS18.zip",
                        Path.Combine(Path.GetTempPath(), "mod_evasive-2.4.0-win64-VS18.zip")
                    ));
                    break;
            }

            bool isOk = false;
            foreach(var tuple in list)
            {
                if (tuple.Url != string.Empty && tuple.File!= string.Empty)
                {
                    if (!File.Exists(tuple.File))
                    {
                        if (!base.Download(tuple.Url, tuple.File))
                        {
                            break;
                        }
                    }

                    string extractPath = Path.Combine(appPath, version);
                    Directory.CreateDirectory(extractPath);
                    ZipFile.ExtractToDirectory(tuple.File, extractPath, true);
                    isOk = true;
                }
            }

            if (isOk)
            {
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
                new ValueName("PATH", Path.Combine(appPath, version, "Apache24", "bin")),
            };
        }

        public override bool Start(string version, ValueName[] environments, JsonObject? profile = null)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = Path.Combine(appPath, version, "Apache24", "bin", "httpd.exe");
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
                            _icon = Icon.ExtractAssociatedIcon(Path.Combine(appPath, InstalledVersions[InstalledVersions.Length - 1].Value, "Apache24", "bin", "httpd.exe"));
                        } catch { }
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
