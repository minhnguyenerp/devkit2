using devkit2.Common;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    internal sealed class SphinxSearch : BaseApplication
    {
        public override string Name => "SphinxSearch";

        public SphinxSearch()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "sphinxsearch");
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
                base.Icon = Icon.ExtractAssociatedIcon(
                    Path.Combine(appPath, InstalledVersions[0].Value, $"sphinx-{InstalledVersions[0].Value}", "bin", "searchd.exe")
                );
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
                    new ValueName("3.9.1", "3.9.1"),
                };
            }
        }

        public override bool Install(string version, IProgress<InstallProgress>? progress = null)
        {
            string url = string.Empty;
            string file = string.Empty;
            switch (version)
            {
                case "3.9.1":
                    url = "https://github.com/minhnguyenerp/devkit2/releases/download/bin1.0.1/sphinx-3.9.1-141d2ea-windows-amd64.zip";
                    file = Path.Combine(Path.GetTempPath(), "sphinx-3.9.1-141d2ea-windows-amd64.zip");
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
                new ValueName("PATH", Path.Combine(appPath, version, $"sphinx-{version}", "bin")),
            };
        }

        public override bool Start(string version, ValueName[] environments, JsonObject? profile = null, string uniqueCode = "")
        {
            if (Sysconf.Instance.GetRunningApplication(uniqueCode) != null)
            {
                MessageBox.Show("SphinxSearch is already running.", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }

            string configDir = profile?["ConfigDirectory"]?.ToString() ?? "";
            string dataDir = profile?["DataDirectory"]?.ToString() ?? "";
            int port = 9312;
            if (profile != null && profile["Port"] != null)
            {
                int.TryParse(profile["Port"].ToString(), out port);
            }

            if (!string.IsNullOrEmpty(configDir) && !string.IsNullOrEmpty(dataDir))
            {
                Directory.CreateDirectory(configDir);
                Directory.CreateDirectory(dataDir);
                string confFile = Path.Combine(configDir, "sphinx.conf");
                if (!File.Exists(confFile))
                {
                    string config = $@"
searchd
{{
    #begin port
    listen = {port}:mysql41
    #end port
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
                            $"#begin port\r\nlisten = {port}:mysql41\r\n#end port" +
                            config.Substring(nEndPort + "#end port".Length);
                    }
                    File.WriteAllText(confFile, config, Encoding.ASCII);
                }

                var psi = new ProcessStartInfo();
                psi.FileName = Path.Combine(appPath, version, $"sphinx-{version}", "bin", "searchd.exe");
                psi.Arguments = $"--config \"{confFile}\" --datadir \"{dataDir}\"";
                psi.WorkingDirectory = configDir;
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;
               
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
                            RuntimeDirectory = configDir,
                            ApplicationVersion = version,
                            Profile = profile,
                        });
                        return true;
                    }
                }
                catch { return false; }
            }
            return false;
        }

        public override JsonObject? ProfileEdit(JsonObject? init = null)
        {
            using (var dlg = new SphinxSearchProfile())
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
            string baseDir = Path.Combine(appPath, runningApplication.ApplicationVersion, $"sphinx-{runningApplication.ApplicationVersion}");
            string binDir = Path.Combine(baseDir, "bin");
            string sphinxApp = Path.Combine(binDir, "searchd.exe");
            var stopPsi = new ProcessStartInfo();
            stopPsi.FileName = sphinxApp;
            stopPsi.Arguments = $"--stop --config \"{Path.Combine(runningApplication.RuntimeDirectory, "sphinx.conf")}\"";
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
