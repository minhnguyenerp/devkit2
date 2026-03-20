using devkit2.Common;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;
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
            try
            {
                base.Icon = Icon.ExtractAssociatedIcon(Path.Combine(appPath, InstalledVersions[0].Value, "Apache24", "bin", "httpd.exe"));
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
                    new ValueName("2.4.66-260223", "2.4.66-260223"),
                };
            }
        }

        public override bool Install(string version, IProgress<DownloadProgress>? progress = null)
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

            bool isOk = true;
            foreach(var tuple in list)
            {
                if (tuple.Url != string.Empty && tuple.File!= string.Empty)
                {
                    if (!base.Download(tuple.Url, tuple.File, progress))
                    {
                        isOk = false;
                        continue;
                    }
                    string extractPath = Path.Combine(appPath, version);
                    Directory.CreateDirectory(extractPath);
                    try
                    {
                        ZipFile.ExtractToDirectory(tuple.File, extractPath, true);
                    }
                    catch (Exception ex)
                    {
                        isOk = false;
                        MessageBox.Show(ex.Message, "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        File.Delete(tuple.File);
                    }
                }
            }

            if (isOk)
            {
                base.SaveNewVersion(version);
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

        public override bool Start(string version, ValueName[] environments, JsonObject? profile = null, string uniqueCode = "")
        {
            string apacheDirSvRoot = Path.Combine(appPath, version, "Apache24");
            string apacheApp = Path.Combine(apacheDirSvRoot, "bin", "httpd.exe");
            string phpApp = string.Empty;
            int phpVer = 8;
            List<string> indexes = new List<string>() { "index.html", "index.htm", "index" };
            foreach (var item in environments)
            {
                if(item.Name.Contains("php"))
                {
                    phpApp = Path.Combine(item.Name, "php.exe");
                    if (item.Name.Contains("8.")) { phpVer = 8; }
                    else if (item.Name.Contains("7.")) { phpVer = 7; }
                    else if (item.Name.Contains("6.")) { phpVer = 6; }
                    else if (item.Name.Contains("5.")) { phpVer = 5; }
                    indexes.Add("index.php");
                }
            }
            string phpDir = Path.GetDirectoryName(phpApp) ?? string.Empty;
            string apacheVer = string.Empty;
            if (version.Split('.').Length > 2)
            {
                apacheVer = version.Split('.')[0] + "_" + version.Split('.')[1];
            }
            
            string phpModule = Path.Combine(phpDir, $"php{phpVer}apache{apacheVer}.dll");

            string instanceDir = profile?["InstanceDirectory"]?.ToString() ?? apacheDirSvRoot;
            string webDir = profile?["WebRootDirectory"]?.ToString() ?? Path.Combine(instanceDir, "www");
            Directory.CreateDirectory(webDir);
            string logsDir = Path.Combine(instanceDir, "logs");
            Directory.CreateDirectory(logsDir);
            string confFile = Path.Combine(instanceDir, "httpd.conf");

            int port = 80;
            if (profile != null && profile["Port"] != null)
            {
                int.TryParse(profile["Port"].ToString(), out port);
            }

            if (!File.Exists(confFile))
            {
                string config = $@"Define SRVROOT ""{apacheDirSvRoot.Replace('\\', '/')}""
ServerRoot ""${{SRVROOT}}""
#begin port
Define PORT {port}
#end port

Listen ${{PORT}}
ServerName localhost:${{PORT}}

#ErrorLog nul
#CustomLog nul common
#begin logdir
ErrorLog ""{logsDir.Replace('\\', '/')}/apache-error.log""
CustomLog ""{logsDir.Replace('\\', '/')}/apache-access.log"" common
#end logdir

LoadModule access_compat_module modules/mod_access_compat.so
LoadModule actions_module modules/mod_actions.so
LoadModule alias_module modules/mod_alias.so
LoadModule allowmethods_module modules/mod_allowmethods.so
LoadModule asis_module modules/mod_asis.so
LoadModule auth_basic_module modules/mod_auth_basic.so
LoadModule authn_core_module modules/mod_authn_core.so
LoadModule authn_file_module modules/mod_authn_file.so
LoadModule authz_core_module modules/mod_authz_core.so
LoadModule authz_groupfile_module modules/mod_authz_groupfile.so
LoadModule authz_host_module modules/mod_authz_host.so
LoadModule authz_user_module modules/mod_authz_user.so
LoadModule autoindex_module modules/mod_autoindex.so
LoadModule dir_module modules/mod_dir.so
LoadModule env_module modules/mod_env.so
LoadModule include_module modules/mod_include.so
LoadModule isapi_module modules/mod_isapi.so
LoadModule log_config_module modules/mod_log_config.so
LoadModule mime_module modules/mod_mime.so
LoadModule negotiation_module modules/mod_negotiation.so
LoadModule setenvif_module modules/mod_setenvif.so
LoadModule proxy_module modules/mod_proxy.so
LoadModule rewrite_module modules/mod_rewrite.so
#begin php
{(File.Exists(phpModule) ? $@"LoadModule php{(phpVer >= 8 ? "" : "7")}_module ""{phpModule.Replace('\\', '/')}""" : "")}
{(File.Exists(phpModule) ? $@"PHPINIDir ""{phpDir.Replace('\\', '/')}""" : "")}
{(File.Exists(phpModule) ? "AddType application/x-httpd-php .php" : "")}
#end php

#begin indexes
Define INDEXES ""{string.Join(" ", indexes)}""
#end indexes

#begin webroot
Define WEBROOT ""{webDir.Replace('\\', '/')}""
#end webroot

DocumentRoot ""${{WEBROOT}}""
DirectoryIndex ${{INDEXES}}

<Directory ""${{WEBROOT}}"">
    AllowOverride All
    Require all granted
</Directory>";

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
                        $"#begin port\r\nDefine PORT {port}\r\n#end port" +
                        config.Substring(nEndPort + "#end port".Length);
                }

                int nBeginPhp = config.IndexOf("#begin php");
                int nEndPhp = config.IndexOf("#end php");
                if (nBeginPhp > 0 && nEndPhp > 0)
                {
                    if (File.Exists(phpModule))
                    {
                        config = config.Substring(0, nBeginPhp) + $@"#begin php
{$@"LoadModule php{(phpVer >= 8 ? "" : "7")}_module ""{phpModule.Replace('\\', '/')}"""}
{$@"PHPINIDir ""{phpDir.Replace('\\', '/')}"""}
{"AddType application/x-httpd-php .php"}
#end php" + config.Substring(nEndPhp + "#end php".Length);
                    }
                    else
                    {
                        config = config.Substring(0, nBeginPhp) + $@"#begin php
#end php" + config.Substring(nEndPhp + "#end php".Length);
                    }
                }

                int nBeginIndexes = config.IndexOf("#begin indexes");
                int nEndIndexes = config.IndexOf("#end indexes");
                if (nBeginIndexes > 0 && nEndIndexes > 0)
                {
                    config = config.Substring(0, nBeginIndexes) + $@"#begin indexes
Define INDEXES ""{string.Join(" ", indexes)}""
#end indexes" + config.Substring(nEndIndexes + "#end indexes".Length);
                }

                int nBeginWebRoot = config.IndexOf("#begin webroot");
                int nEndWebRoot = config.IndexOf("#end webroot");
                if (nBeginWebRoot > 0 && nEndWebRoot > 0)
                {
                    config = config.Substring(0, nBeginWebRoot) + $@"#begin webroot
Define WEBROOT ""{webDir.Replace('\\', '/')}""
#end webroot" + config.Substring(nEndWebRoot + "#end webroot".Length);
                }

                int nBeginLogDir = config.IndexOf("#begin logdir");
                int nEndLogDir = config.IndexOf("#end logdir");
                if (nBeginLogDir > 0 && nEndLogDir > 0)
                {
                    config = config.Substring(0, nBeginLogDir) + $@"#begin logdir
ErrorLog ""{logsDir.Replace('\\', '/')}/apache-error.log""
CustomLog ""{logsDir.Replace('\\', '/')}/apache-access.log"" common
#end logdir" + config.Substring(nEndLogDir + "#end logdir".Length);
                }

                File.WriteAllText(confFile, config, Encoding.ASCII);
            }

            var runPsi = new ProcessStartInfo();
            runPsi.FileName = apacheApp;
            runPsi.Arguments = $"-f \"{confFile}\"";
            runPsi.WorkingDirectory = instanceDir;
            runPsi.UseShellExecute = false;
            runPsi.CreateNoWindow = true;
            runPsi.RedirectStandardOutput = true;
            runPsi.RedirectStandardError = true;
            LoadEnvironments(ref runPsi, environments);
            var proc = Process.Start(runPsi);
            if (proc == null)
                return false;

            Sysconf.Instance.AddRunningApplication(new RunningApplication {
                UniqueCode = uniqueCode,
                Pid = proc.Id,
                Sessionid = proc.SessionId,
                ProcessName = proc.ProcessName,
                StartTime = proc.StartTime,
                ApplicationName = Name,
                RuntimeDirectory = instanceDir,
                ApplicationVersion = version,
            });
            return true;
        }

        public override JsonObject? ProfileEdit(JsonObject? init = null)
        {
            using (var dlg = new ApacheProfile())
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
