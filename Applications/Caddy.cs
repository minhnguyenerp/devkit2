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
            string webDir = profile?["WebRootDirectory"]?.ToString() ?? Path.Combine(instanceDir, "www");
            Directory.CreateDirectory(webDir);
            string logsDir = Path.Combine(instanceDir, "logs");
            Directory.CreateDirectory(logsDir);
            string confFile = Path.Combine(instanceDir, "Caddyfile");

            int port = 80;
            if (profile != null && profile["Port"] != null)
            {
                int.TryParse(profile["Port"].ToString(), out port);
            }

            if (!File.Exists(confFile))
            {
                // Genereate Caddyfile
                string config = $@"{{
    admin off
}}

#begin port
:{port} {{
#end port
    #begin webroot
    root * ""{webDir}""
    #end webroot
    #begin logdir
    log {{
        output file ""{Path.Combine(logsDir, "caddy-access.log")}"" {{
            roll_size 10MB
            roll_keep 5
        }}
    }}
    #end logdir
    encode gzip zstd
    try_files {{path}} {{path}}/ /index.php
    #begin fastcgi port
    {(!string.IsNullOrEmpty(phpCgiApp) ? $@"php_fastcgi 127.0.0.1:{port + 1}" : "")}
    #end fastcgi port
    file_server
}}
";
                File.WriteAllText(confFile, config, Encoding.ASCII);
            }
            else
            {
                string config = File.ReadAllText(confFile, Encoding.ASCII);
                int nBeginPort = config.IndexOf("#begin port");
                int nEndPort = config.IndexOf("#end port");
                if (nBeginPort > 0 && nEndPort > 0)
                {
                    config = config.Substring(0, nBeginPort) +
                        $"#begin port\r\n:{port} {{\r\n#end port" +
                        config.Substring(nEndPort + "#end port".Length);
                }

                int nBeginFastCgiPort = config.IndexOf("#begin fastcgi port");
                int nEndFastCgiPort = config.IndexOf("#end fastcgi port");
                if (nBeginFastCgiPort > 0 && nEndFastCgiPort > 0)
                {
                    config = config.Substring(0, nBeginFastCgiPort) + $@"#begin fastcgi port
{(!string.IsNullOrEmpty(phpCgiApp) ? $@"php_fastcgi 127.0.0.1:{port + 1}" : "")}
#end fastcgi port" + config.Substring(nEndFastCgiPort + "#end fastcgi port".Length);
                }

                int nBeginWebRoot = config.IndexOf("#begin webroot");
                int nEndWebRoot = config.IndexOf("#end webroot");
                if (nBeginWebRoot > 0 && nEndWebRoot > 0)
                {
                    config = config.Substring(0, nBeginWebRoot) + $@"#begin webroot
    root * ""{webDir}""
#end webroot" + config.Substring(nEndWebRoot + "#end webroot".Length);
                }

                int nBeginLogDir = config.IndexOf("#begin logdir");
                int nEndLogDir = config.IndexOf("#end logdir");
                if (nBeginLogDir > 0 && nEndLogDir > 0)
                {
                    config = config.Substring(0, nBeginLogDir) + $@"#begin logdir
    log {{
        output file ""{Path.Combine(logsDir, "caddy-access.log")}"" {{
            roll_size 10MB
            roll_keep 5
        }}
    }}
#end logdir" + config.Substring(nEndLogDir + "#end logdir".Length);
                }

                File.WriteAllText(confFile, config, Encoding.ASCII);
            }

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
            runPsi.WorkingDirectory = instanceDir;
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
