using devkit2.Common;
using System.Diagnostics;
using System.IO.Compression;
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

        public override bool Install(string version)
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
                new ValueName("PATH", Path.Combine(appPath, version, $"mariadb-{version}-winx64", "bin")),
            };
        }

        public override bool Start(string version, ValueName[] environments, JsonObject? profile = null)
        {
            // Use the same layout as the scripts: baseDir contains bin, my.ini located at baseDir, data under baseDir/data
            string baseDir = Path.Combine(appPath, version, $"mariadb-{version}-winx64");
            string binDir = Path.Combine(baseDir, "bin");
            string mariadbdPath = Path.Combine(binDir, "mariadbd.exe");
            string installDbPath = Path.Combine(binDir, "mariadb-install-db.exe");
            string adminPath = Path.Combine(binDir, "mariadb-admin.exe");

            if (!File.Exists(mariadbdPath))
                return false;

            // Read profile values (port, datadir)
            string dataDir = profile? ["DataDir"]?.ToString() ?? Path.Combine(baseDir, "data");
            int port = 3306;
            if (profile != null && profile["Port"] != null)
            {
                int.TryParse(profile["Port"].ToString(), out port);
            }

            // Put my.ini inside the data directory by default so each data folder can have its own config
            string configPath = profile? ["Config"]?.ToString() ?? Path.Combine(dataDir, "my.ini");
            string errLog = Path.Combine(dataDir, "mariadb-error-log.err");

            try
            {
                // Ensure data directory exists for later use
                if (!Directory.Exists(dataDir))
                {
                    Directory.CreateDirectory(dataDir);
                }

                // If config does not exist, create a my.ini in the data folder based on Scripts\my.ini
                if (!File.Exists(configPath))
                {
                    var cfg = new System.Text.StringBuilder();
                    cfg.AppendLine("[mariadbd]");
                    cfg.AppendLine($"basedir=\"{baseDir}\"");
                    cfg.AppendLine($"datadir=\"{dataDir}\"");
                    cfg.AppendLine($"port={port}");
                    cfg.AppendLine("socket=mysql.sock");
                    cfg.AppendLine("skip-grant-tables=0");
                    cfg.AppendLine("skip-networking=0");
                    cfg.AppendLine("character-set-server=utf8mb4");
                    cfg.AppendLine("collation-server=utf8mb4_general_ci");
                    cfg.AppendLine();
                    cfg.AppendLine("[client]");
                    cfg.AppendLine($"port={port}");
                    cfg.AppendLine("user=root");
                    cfg.AppendLine("password=");
                    File.WriteAllText(configPath, cfg.ToString());
                }

                // Initialize database if mysql system tables are missing.
                // Previous heuristic checked for any file in data dir which could be wrong when temporary InnoDB files exist
                string mysqlSystemDir = Path.Combine(dataDir, "mysql");
                bool hasSystemTables = Directory.Exists(mysqlSystemDir) && Directory.EnumerateFileSystemEntries(mysqlSystemDir).Any();

                if (!hasSystemTables)
                {
                    // Try mariadb-install-db first (script does this)
                    if (File.Exists(installDbPath))
                    {
                        var initPsi = new ProcessStartInfo();
                        initPsi.FileName = installDbPath;
                        initPsi.Arguments = $"--datadir=\"{dataDir}\"";
                        initPsi.UseShellExecute = false;
                        initPsi.CreateNoWindow = true;
                        initPsi.WorkingDirectory = binDir;
                        LoadEnvironments(ref initPsi, environments);

                        using (var initProc = Process.Start(initPsi))
                        {
                            initProc?.WaitForExit(60000);
                        }
                    }
                    else
                    {
                        // Fallback: use mariadbd --initialize-insecure
                        try
                        {
                            var initPsi = new ProcessStartInfo();
                            initPsi.FileName = mariadbdPath;
                            initPsi.Arguments = $"--defaults-file=\"{configPath}\" --initialize-insecure --datadir=\"{dataDir}\" --basedir=\"{baseDir}\"";
                            initPsi.UseShellExecute = false;
                            initPsi.CreateNoWindow = true;
                            initPsi.WorkingDirectory = binDir;
                            LoadEnvironments(ref initPsi, environments);

                            using (var initProc = Process.Start(initPsi))
                            {
                                initProc?.WaitForExit(60000);
                            }
                        }
                        catch { }
                    }
                }

                // Start mariadbd hidden/detached with defaults-file and log-error
                var runPsi = new ProcessStartInfo();
                runPsi.FileName = mariadbdPath;
                runPsi.Arguments = $"--defaults-file=\"{configPath}\" --log-error=\"{errLog}\"";
                runPsi.UseShellExecute = false;
                runPsi.CreateNoWindow = true;
                runPsi.RedirectStandardOutput = true;
                runPsi.RedirectStandardError = true;
                runPsi.WorkingDirectory = binDir;
                LoadEnvironments(ref runPsi, environments);

                var proc = Process.Start(runPsi);
                if (proc == null)
                    return false;

                // Poll for readiness using mariadb-admin ping (up to ~15s)
                if (File.Exists(adminPath))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        try
                        {
                            var pingPsi = new ProcessStartInfo();
                            pingPsi.FileName = adminPath;
                            pingPsi.Arguments = $"-uroot -h127.0.0.1 -P{port} --protocol=tcp ping";
                            pingPsi.UseShellExecute = false;
                            pingPsi.CreateNoWindow = true;
                            pingPsi.RedirectStandardOutput = true;
                            pingPsi.RedirectStandardError = true;
                            pingPsi.WorkingDirectory = binDir;
                            LoadEnvironments(ref pingPsi, environments);

                            using (var pingProc = Process.Start(pingPsi))
                            {
                                if (pingProc != null && pingProc.WaitForExit(1000) && pingProc.ExitCode == 0)
                                {
                                    return true;
                                }
                            }
                        }
                        catch { }
                        Thread.Sleep(1000);
                    }
                }

                // If admin not available or ping failed, still consider started if process was launched
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool Stop(string version)
        {
            string baseDir = Path.Combine(appPath, version, $"mariadb-{version}-winx64");
            string binDir = Path.Combine(baseDir, "bin");
            string adminPath = Path.Combine(binDir, "mariadb-admin.exe");

            int port = 3306;

            try
            {
                // First try a graceful shutdown using mariadb-admin if available
                if (File.Exists(adminPath))
                {
                    try
                    {
                        var shutPsi = new ProcessStartInfo();
                        shutPsi.FileName = adminPath;
                        shutPsi.Arguments = $"-uroot -h127.0.0.1 -P{port} --protocol=tcp shutdown";
                        shutPsi.UseShellExecute = false;
                        shutPsi.CreateNoWindow = true;
                        shutPsi.RedirectStandardOutput = true;
                        shutPsi.RedirectStandardError = true;
                        shutPsi.WorkingDirectory = binDir;
                        var shutProc = Process.Start(shutPsi);
                        if (shutProc != null)
                        {
                            if (shutProc.WaitForExit(5000) && shutProc.ExitCode == 0)
                            {
                                return true;
                            }
                        }
                    }
                    catch { }
                }

                // If graceful shutdown didn't work, kill any mariadbd.exe processes
                var procs = Process.GetProcessesByName("mariadbd");
                foreach (var p in procs)
                {
                    try
                    {
                        p.Kill();
                        p.WaitForExit(5000);
                    }
                    catch { }
                }

                return true;
            }
            catch { return false; }
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
                            _icon = Icon.ExtractAssociatedIcon(Path.Combine(appPath, InstalledVersions[0].Value, $"mariadb-{InstalledVersions[0].Value}-winx64", "bin", "mariadb.exe"));
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
