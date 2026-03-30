using devkit2.Common;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    internal sealed class Nginx : BaseApplication
    {
        public override string Name => "Nginx";

        public Nginx()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "nginx");
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
                base.Icon = Icon.ExtractAssociatedIcon(Path.Combine(appPath, InstalledVersions[0].Value, $"nginx-{InstalledVersions[0].Value}", "nginx.exe"));
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
                    new ValueName("1.28.2", "1.28.2"),
                };
            }
        }

        public override bool Install(string version, IProgress<InstallProgress>? progress = null)
        {
            string url = string.Empty;
            string file = string.Empty;
            switch (version)
            {
                case "1.28.2":
                    url = "https://nginx.org/download/nginx-1.28.2.zip";
                    file = Path.Combine(Path.GetTempPath(), "nginx-1.28.2.zip");
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
                new ValueName("PATH", Path.Combine(appPath, version, $"nginx-{version}")),
            };
        }

        public override bool Start(string version, ValueName[] environments, JsonObject? profile = null, string uniqueCode = "")
        {
            if (Sysconf.Instance.GetRunningApplication(uniqueCode) != null)
            {
                MessageBox.Show("Nginx is already running.", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            string nginxDirSvRoot = Path.Combine(appPath, version, $"nginx-{version}");
            string nginxApp = Path.Combine(nginxDirSvRoot, "nginx.exe");
            string phpCgiApp = string.Empty;
            List<string> indexes = new List<string>() { "index.html", "index.htm" };
            foreach (var item in environments)
            {
                if (item.Name.Contains("php"))
                {
                    phpCgiApp = Path.Combine(item.Name, "php-cgi.exe");
                    indexes.Add("index.php");
                }
            }

            string instanceDir = profile?["InstanceDirectory"]?.ToString() ?? nginxDirSvRoot;
            string webDir = profile?["WebRootDirectory"]?.ToString() ?? Path.Combine(instanceDir, "www");
            Directory.CreateDirectory(webDir);
            string logsDir = Path.Combine(instanceDir, "logs");
            Directory.CreateDirectory(logsDir);
            string bodyTempDir = Path.Combine(instanceDir, "temp", "client_body_temp");
            Directory.CreateDirectory(bodyTempDir);
            string confFile = Path.Combine(instanceDir, "nginx.conf");

            int port = 80;
            if (profile != null && profile["Port"] != null)
            {
                int.TryParse(profile["Port"].ToString(), out port);
            }

            if (!File.Exists(confFile))
            {
                // Genereate nginx.conf
                string config = $@"events {{
    worker_connections 1024;
}}

http {{
    types {{
        text/html                                        html htm shtml;
        text/css                                         css;
        text/xml                                         xml;
        image/gif                                        gif;
        image/jpeg                                       jpeg jpg;
        application/javascript                           js;
        application/atom+xml                             atom;
        application/rss+xml                              rss;

        text/mathml                                      mml;
        text/plain                                       txt;
        text/vnd.sun.j2me.app-descriptor                 jad;
        text/vnd.wap.wml                                 wml;
        text/x-component                                 htc;

        image/avif                                       avif;
        image/png                                        png;
        image/svg+xml                                    svg svgz;
        image/tiff                                       tif tiff;
        image/vnd.wap.wbmp                               wbmp;
        image/webp                                       webp;
        image/x-icon                                     ico;
        image/x-jng                                      jng;
        image/x-ms-bmp                                   bmp;

        font/woff                                        woff;
        font/woff2                                       woff2;

        application/java-archive                         jar war ear;
        application/json                                 json;
        application/mac-binhex40                         hqx;
        application/msword                               doc;
        application/pdf                                  pdf;
        application/postscript                           ps eps ai;
        application/rtf                                  rtf;
        application/vnd.apple.mpegurl                    m3u8;
        application/vnd.google-earth.kml+xml             kml;
        application/vnd.google-earth.kmz                 kmz;
        application/vnd.ms-excel                         xls;
        application/vnd.ms-fontobject                    eot;
        application/vnd.ms-powerpoint                    ppt;
        application/vnd.oasis.opendocument.graphics      odg;
        application/vnd.oasis.opendocument.presentation  odp;
        application/vnd.oasis.opendocument.spreadsheet   ods;
        application/vnd.oasis.opendocument.text          odt;
        application/vnd.openxmlformats-officedocument.presentationml.presentation
                                                         pptx;
        application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
                                                         xlsx;
        application/vnd.openxmlformats-officedocument.wordprocessingml.document
                                                         docx;
        application/vnd.wap.wmlc                         wmlc;
        application/wasm                                 wasm;
        application/x-7z-compressed                      7z;
        application/x-cocoa                              cco;
        application/x-java-archive-diff                  jardiff;
        application/x-java-jnlp-file                     jnlp;
        application/x-makeself                           run;
        application/x-perl                               pl pm;
        application/x-pilot                              prc pdb;
        application/x-rar-compressed                     rar;
        application/x-redhat-package-manager             rpm;
        application/x-sea                                sea;
        application/x-shockwave-flash                    swf;
        application/x-stuffit                            sit;
        application/x-tcl                                tcl tk;
        application/x-x509-ca-cert                       der pem crt;
        application/x-xpinstall                          xpi;
        application/xhtml+xml                            xhtml;
        application/xspf+xml                             xspf;
        application/zip                                  zip;

        application/octet-stream                         bin exe dll;
        application/octet-stream                         deb;
        application/octet-stream                         dmg;
        application/octet-stream                         iso img;
        application/octet-stream                         msi msp msm;

        audio/midi                                       mid midi kar;
        audio/mpeg                                       mp3;
        audio/ogg                                        ogg;
        audio/x-m4a                                      m4a;
        audio/x-realaudio                                ra;

        video/3gpp                                       3gpp 3gp;
        video/mp2t                                       ts;
        video/mp4                                        mp4;
        video/mpeg                                       mpeg mpg;
        video/quicktime                                  mov;
        video/webm                                       webm;
        video/x-flv                                      flv;
        video/x-m4v                                      m4v;
        video/x-mng                                      mng;
        video/x-ms-asf                                   asx asf;
        video/x-ms-wmv                                   wmv;
        video/x-msvideo                                  avi;
    }}

    default_type application/octet-stream;

    #begin logdir
    error_log ""{logsDir.Replace('\\', '/')}/nginx-error.log"";
    access_log ""{logsDir.Replace('\\', '/')}/nginx-access.log"";
    client_body_temp_path ""{bodyTempDir.Replace('\\', '/')}"";
    #end logdir
    #access_log off;
    #error_log off;

    server {{
        #begin port
        listen {port};
        #end port
        server_name localhost;

        #begin webroot
        root ""{webDir.Replace('\\', '/')}"";
        #end webroot
        #begin indexes
        index {string.Join(" ", indexes)};
        #end indexes

        gzip on;
        gzip_types text/plain text/css application/json application/javascript application/xml text/javascript image/svg+xml;

        location / {{
            try_files $uri $uri/ /index.php?$query_string;
        }}

        location ~ \.php$ {{
            #begin fastcgi port
            {(!string.IsNullOrEmpty(phpCgiApp) ? $@"fastcgi_pass 127.0.0.1:{port + 1};" : "")}
            #end fastcgi port
            fastcgi_index index.php;
            fastcgi_param  SCRIPT_FILENAME    $document_root$fastcgi_script_name;
            fastcgi_param  QUERY_STRING       $query_string;
            fastcgi_param  REQUEST_METHOD     $request_method;
            fastcgi_param  CONTENT_TYPE       $content_type;
            fastcgi_param  CONTENT_LENGTH     $content_length;

            fastcgi_param  SCRIPT_NAME        $fastcgi_script_name;
            fastcgi_param  REQUEST_URI        $request_uri;
            fastcgi_param  DOCUMENT_URI       $document_uri;
            fastcgi_param  DOCUMENT_ROOT      $document_root;
            fastcgi_param  SERVER_PROTOCOL    $server_protocol;
            fastcgi_param  REQUEST_SCHEME     $scheme;
            fastcgi_param  HTTPS              $https if_not_empty;

            fastcgi_param  GATEWAY_INTERFACE  CGI/1.1;
            fastcgi_param  SERVER_SOFTWARE    nginx/$nginx_version;

            fastcgi_param  REMOTE_ADDR        $remote_addr;
            fastcgi_param  REMOTE_PORT        $remote_port;
            fastcgi_param  REMOTE_USER        $remote_user;
            fastcgi_param  SERVER_ADDR        $server_addr;
            fastcgi_param  SERVER_PORT        $server_port;
            fastcgi_param  SERVER_NAME        $server_name;

            # PHP only, required if PHP was built with --enable-force-cgi-redirect
            fastcgi_param  REDIRECT_STATUS    200;
        }}

        location ~ /\.(?!well-known).* {{
            deny all;
        }}
    }}
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
                        $"#begin port\r\n\tlisten {port};\r\n\t#end port" +
                        config.Substring(nEndPort + "#end port".Length);
                }

                int nBeginFastCgiPort = config.IndexOf("#begin fastcgi port");
                int nEndFastCgiPort = config.IndexOf("#end fastcgi port");
                if (nBeginFastCgiPort > 0 && nEndFastCgiPort > 0)
                {
                    config = config.Substring(0, nBeginFastCgiPort) + $@"#begin fastcgi port
    {(!string.IsNullOrEmpty(phpCgiApp) ? $@"fastcgi_pass 127.0.0.1:{port + 1};" : "")}
    #end fastcgi port" + config.Substring(nEndFastCgiPort + "#end fastcgi port".Length);
                }

                int nBeginWebRoot = config.IndexOf("#begin webroot");
                int nEndWebRoot = config.IndexOf("#end webroot");
                if (nBeginWebRoot > 0 && nEndWebRoot > 0)
                {
                    config = config.Substring(0, nBeginWebRoot) + $@"#begin webroot
    root ""{webDir.Replace('\\', '/')}"";
    #end webroot" + config.Substring(nEndWebRoot + "#end webroot".Length);
                }

                int nBeginIndexes = config.IndexOf("#begin indexes");
                int nEndIndexes = config.IndexOf("#end indexes");
                if (nBeginIndexes > 0 && nEndIndexes > 0)
                {
                    config = config.Substring(0, nBeginIndexes) + $@"#begin indexes
    index {string.Join(" ", indexes)};
    #end indexes" + config.Substring(nEndIndexes + "#end indexes".Length);
                }

                int nBeginLogDir = config.IndexOf("#begin logdir");
                int nEndLogDir = config.IndexOf("#end logdir");
                if (nBeginLogDir > 0 && nEndLogDir > 0)
                {
                    config = config.Substring(0, nBeginLogDir) + $@"#begin logdir
    error_log ""{logsDir.Replace('\\', '/')}/nginx-error.log"";
    access_log ""{logsDir.Replace('\\', '/')}/nginx-access.log"";
    client_body_temp_path ""{bodyTempDir.Replace('\\', '/')}"";
#end logdir" + config.Substring(nEndLogDir + "#end logdir".Length);
                }

                File.WriteAllText(confFile, config, Encoding.ASCII);
            }

            // Start PHP CGI if available
            RunningApplication? runningCgi = null;
            if (!string.IsNullOrEmpty(phpCgiApp))
            {
                var psiCgi = new ProcessStartInfo
                {
                    FileName = phpCgiApp,
                    Arguments = $"-b 127.0.0.1:{port + 1}",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var cgiproc = Process.Start(psiCgi);
                if (cgiproc != null)
                {
                    runningCgi = new RunningApplication
                    {
                        UniqueCode = uniqueCode,
                        Pid = cgiproc.Id,
                        Sessionid = cgiproc.SessionId,
                        ProcessName = cgiproc.ProcessName,
                        StartTime = cgiproc.StartTime,
                    };
                }
            }

            var runPsi = new ProcessStartInfo();
            runPsi.FileName = nginxApp;
            runPsi.Arguments = $"-c \"{confFile}\" -p \"{instanceDir}\"";
            runPsi.WorkingDirectory = instanceDir;
            runPsi.UseShellExecute = false;
            runPsi.CreateNoWindow = true;
            runPsi.RedirectStandardOutput = true;
            runPsi.RedirectStandardError = true;
            LoadEnvironments(ref runPsi, environments);
            var proc = Process.Start(runPsi);
            if (proc == null)
                return false;
            var runningApp = new RunningApplication
            {
                UniqueCode = uniqueCode,
                Pid = proc.Id,
                Sessionid = proc.SessionId,
                ProcessName = proc.ProcessName,
                StartTime = proc.StartTime,
                ApplicationName = Name,
                RuntimeDirectory = instanceDir,
                ApplicationVersion = version,
            };
            if (runningCgi != null)
            {
                runningApp.Childs.Add(runningCgi);
            }
            Sysconf.Instance.AddRunningApplication(runningApp);
            return true;
        }

        public override JsonObject? ProfileEdit(JsonObject? init = null)
        {
            using (var dlg = new NginxProfile())
            {
                dlg.Profile = init;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    return dlg.Profile;
                }
                return init;
            }
        }

        public override bool Stop(RunningApplication runningApplication)
        {
            string nginxDirSvRoot = Path.Combine(appPath, runningApplication.ApplicationVersion, $"nginx-{runningApplication.ApplicationVersion}");
            string nginxApp = Path.Combine(nginxDirSvRoot, "nginx.exe");
            string confFile = Path.Combine(runningApplication.RuntimeDirectory, "nginx.conf");
            var stopPsi = new ProcessStartInfo();
            stopPsi.FileName = nginxApp;
            stopPsi.Arguments = $"-s stop -c \"{confFile}\" -p \"{runningApplication.RuntimeDirectory}\"";
            stopPsi.WorkingDirectory = runningApplication.RuntimeDirectory;
            stopPsi.UseShellExecute = false;
            stopPsi.CreateNoWindow = true;
            stopPsi.RedirectStandardOutput = true;
            stopPsi.RedirectStandardError = true;
            var proc = Process.Start(stopPsi);
            proc?.WaitForExit(5000);
            base.Stop(runningApplication);
            if (proc == null)
                return false;
            return true;
        }
    }
}
