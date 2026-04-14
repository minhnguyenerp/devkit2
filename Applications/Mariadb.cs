using devkit2.Common;
using devkit2.Properties;
using IniParser.Model;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    internal sealed class Mariadb : BaseApplication
    {
        public override string Name => "Mariadb";

        public Mariadb()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "mariadb");
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
                base.Icon = Resources.file_type_mariadb_icon_130403;
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
                    new ValueName("12.2.2", "12.2.2"),
                };
            }
        }

        public override bool Install(string version, IProgress<InstallProgress>? progress = null)
        {
            string url = string.Empty;
            string file = string.Empty;
            switch (version)
            {
                case "12.2.2":
                    url = "https://github.com/minhnguyenerp/devkit2/releases/download/bin1.0.1/mariadb-12.2.2-winx64.zip";
                    file = Path.Combine(Path.GetTempPath(), "mariadb-12.2.2-winx64.zip");
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
                new ValueName("PATH", Path.Combine(appPath, version, $"mariadb-{version}-winx64", "bin")),
            };
        }

        public override bool Start(string version, ValueName[] environments, JsonObject? profile = null, string uniqueCode = "")
        {
            if (Sysconf.Instance.GetRunningApplication(uniqueCode) != null)
            {
                MessageBox.Show("Mariadb is already running.", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            // Use the same layout as the scripts: baseDir contains bin, my.ini located at baseDir, data under baseDir/data
            string baseDir = Path.Combine(appPath, version, $"mariadb-{version}-winx64");
            string binDir = Path.Combine(baseDir, "bin");
            string mariadbdApp = Path.Combine(binDir, "mariadbd.exe");
            string installDbApp = Path.Combine(binDir, "mariadb-install-db.exe");

            if (!File.Exists(mariadbdApp))
                return false;
            if (!File.Exists(installDbApp))
                return false;

            // Read profile values (port, datadirectory)
            string dataDir = profile?["DataDirectory"]?.ToString() ?? Path.Combine(baseDir, "data");
            int port = 3306;
            if (profile != null && profile["Port"] != null)
            {
                int.TryParse(profile["Port"].ToString(), out port);
            }

            string mysqlSystemDir = Path.Combine(dataDir, "mysql");
            bool hasSystemTables = Directory.Exists(mysqlSystemDir) && Directory.EnumerateFileSystemEntries(mysqlSystemDir).Any();

            if (!hasSystemTables)
            {
                var initPsi = new ProcessStartInfo();
                initPsi.FileName = installDbApp;
                initPsi.Arguments = $"--datadir=\"{dataDir}\"";
                initPsi.UseShellExecute = false;
                initPsi.CreateNoWindow = true;
                initPsi.WorkingDirectory = binDir;
                using (var initProc = Process.Start(initPsi))
                {
                    initProc?.WaitForExit(120000);
                }
            }

            if (!File.Exists(Path.Combine(dataDir, "my.ini")))
            {
                File.WriteAllText(Path.Combine(dataDir, "my.ini"), $"[mariadbd]");
            }

            var iniParser = new IniParser.FileIniDataParser();
            IniData iniData = iniParser.ReadFile(Path.Combine(dataDir, "my.ini"));
            iniData["mariadbd"]["basedir"] = $"\"{baseDir}\"";
            iniData["mariadbd"]["datadir"] = $"\"{dataDir}\"";
            iniData["mariadbd"]["port"] = port.ToString();
            iniData["mariadbd"]["socket"] = "mysql.sock";
            iniData["mariadbd"]["skip-grant-tables"] = "0";
            iniData["mariadbd"]["skip-networking"] = "0";
            iniData["mariadbd"]["character-set-server"] = "utf8mb4";
            iniData["mariadbd"]["collation-server"] = "utf8mb4_general_ci";
            iniData["client"]["port"] = port.ToString();
            iniData["client"]["user"] = "root";
            iniData["client"]["password"] = "";
            iniParser.WriteFile(Path.Combine(dataDir, "my.ini"), iniData, Encoding.ASCII);

            string errLog = Path.Combine(dataDir, "mariadb-error-log.err");
            var runPsi = new ProcessStartInfo();
            runPsi.FileName = mariadbdApp;
            runPsi.Arguments = $"--defaults-file=\"{Path.Combine(dataDir, "my.ini")}\" --log-error=\"{errLog}\"";
            runPsi.UseShellExecute = false;
            runPsi.CreateNoWindow = true;
            runPsi.RedirectStandardOutput = true;
            runPsi.RedirectStandardError = true;
            runPsi.WorkingDirectory = binDir;
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
                ApplicationName = Name,
                ApplicationVersion = version,
                RuntimeDirectory = dataDir,
                Profile = profile,
            });
            return true;
        }

        public override JsonObject? ProfileEdit(JsonObject? init = null)
        {
            using (var dlg = new MariadbProfile())
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
            string baseDir = Path.Combine(appPath, runningApplication.ApplicationVersion, $"mariadb-{runningApplication.ApplicationVersion}-winx64");
            string binDir = Path.Combine(baseDir, "bin");
            string mariadbdApp = Path.Combine(binDir, "mariadb-admin.exe");
            var stopPsi = new ProcessStartInfo();
            stopPsi.FileName = mariadbdApp;
            stopPsi.Arguments = $"--defaults-file=\"{Path.Combine(runningApplication.RuntimeDirectory, "my.ini")}\" shutdown";
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
