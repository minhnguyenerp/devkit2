using devkit2.Common;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;
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
            try
            {
                base.Icon = Icon.ExtractAssociatedIcon(Path.Combine(appPath, InstalledVersions[0].Value, "caddy.exe"));
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
                    new ValueName("2.11.2", "2.11.2"),
                };
            }
        }

        public override bool Install(string version, IProgress<DownloadProgress>? progress = null)
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

        public override bool Start(string version, ValueName[] environments, JsonObject? profile = null, string uniqueCode = "")
        {
            string caddyDirSvRoot = Path.Combine(appPath, version);
            string caddyApp = Path.Combine(caddyDirSvRoot, "caddy.exe");
            string phpCgiApp = string.Empty;
            foreach (var item in environments)
            {
                if (item.Name.Contains("php"))
                {
                    phpCgiApp = Path.Combine(item.Name, "php-cgi.exe");
                }
            }

            string instanceDir = profile?["InstanceDirectory"]?.ToString() ?? caddyDirSvRoot;
            string webDir = Path.Combine(instanceDir, "www");
            Directory.CreateDirectory(webDir);
            string confFile = Path.Combine(instanceDir, "Caddyfile");

            int port = 80;
            if (profile != null && profile["Port"] != null)
            {
                int.TryParse(profile["Port"].ToString(), out port);
            }

            // Genereate Caddyfile
            string config = $@"{{
    admin off
}}

:{port} {{
    root * ""{webDir}""
    encode gzip zstd
    php_fastcgi 127.0.0.1:{port + 1}
    file_server
}}
";
            File.WriteAllText(confFile, config, Encoding.ASCII);

            // Start PHP CGI if available
            if (!string.IsNullOrEmpty(phpCgiApp))
            {
                var psiCgi = new ProcessStartInfo
                {
                    FileName = phpCgiApp,
                    Arguments = $"-b 127.0.0.1:{port + 1}",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process.Start(psiCgi);
            }

            var runPsi = new ProcessStartInfo();
            runPsi.FileName = caddyApp;
            runPsi.Arguments = $"run --adapter caddyfile --config \"{confFile}\"";
            runPsi.UseShellExecute = false;
            runPsi.CreateNoWindow = true;
            runPsi.RedirectStandardOutput = true;
            runPsi.RedirectStandardError = true;
            LoadEnvironments(ref runPsi, environments);
            var proc = Process.Start(runPsi);
            if (proc == null)
                return false;
            Sysconf.Instance.AddRunningApplication(new RunningApplication
            {
                UniqueCode = uniqueCode,
                Pid = proc.Id,
                Sessionid = proc.SessionId,
                ProcessName = proc.ProcessName,
                StartTime = proc.StartTime,
            });
            return true;
        }

        public override bool Stop(string version)
        {
            return false;
        }

        public override JsonObject? ProfileEdit(JsonObject? init = null)
        {
            using (var dlg = new CaddyProfile())
            {
                dlg.Profile = init;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    return dlg.Profile;
                }
                return init;
            }
        }
    }
}
