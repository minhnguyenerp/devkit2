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
            // Determine mysqld path inside installed folder
            string mysqldPath = Path.Combine(appPath, version, $"mariadb-{version}-winx64", "bin", "mysqld.exe");

            if (!File.Exists(mysqldPath))
                return false;

            // Read profile values (port, datadir)
            string dataDir = profile? ["DataDir"]?.ToString() ?? Path.Combine(appPath, version, $"mariadb-{version}-winx64", "data");
            int port = 3306;
            if (profile != null && profile["Port"] != null)
            {
                int.TryParse(profile["Port"].ToString(), out port);
            }

            try
            {
                // Ensure data directory exists
                if (!Directory.Exists(dataDir))
                {
                    Directory.CreateDirectory(dataDir);
                }

                // Prepare a my.ini defaults file in the data directory so mysqld uses correct paths
                string baseDir = Path.Combine(appPath, version, $"mariadb-{version}-winx64");
                string configPath = Path.Combine(dataDir, "my.ini");
                if (!File.Exists(configPath))
                {
                    var cfg = new System.Text.StringBuilder();
                    cfg.AppendLine("[mysqld]");
                    cfg.AppendLine($"basedir=\"{baseDir}\"");
                    cfg.AppendLine($"datadir=\"{dataDir}\"");
                    cfg.AppendLine($"port={port}");
                    cfg.AppendLine("bind-address=127.0.0.1");
                    cfg.AppendLine("skip-name-resolve");
                    File.WriteAllText(configPath, cfg.ToString());
                }

                // Initialize database if data directory does not contain mysql system files
                bool hasMeaningfulFiles = Directory.EnumerateFileSystemEntries(dataDir)
                    .Any(p => !string.Equals(Path.GetFileName(p), Path.GetFileName(configPath), StringComparison.OrdinalIgnoreCase));
                if (!hasMeaningfulFiles)
                {
                    var initPsi = new ProcessStartInfo();
                    initPsi.FileName = mysqldPath;
                    initPsi.Arguments = $"--defaults-file=\"{configPath}\" --initialize-insecure --datadir=\"{dataDir}\"";
                    initPsi.UseShellExecute = false;
                    initPsi.CreateNoWindow = true;
                    initPsi.WorkingDirectory = Path.GetDirectoryName(mysqldPath) ?? string.Empty;
                    LoadEnvironments(ref initPsi, environments);

                    using (var initProc = Process.Start(initPsi))
                    {
                        initProc?.WaitForExit(60000);
                    }
                }

                // Start mysqld in background using the defaults file
                var runPsi = new ProcessStartInfo();
                runPsi.FileName = mysqldPath;
                runPsi.Arguments = $"--defaults-file=\"{configPath}\" --datadir=\"{dataDir}\" --port={port} --bind-address=127.0.0.1";
                runPsi.UseShellExecute = false;
                runPsi.CreateNoWindow = true;
                runPsi.RedirectStandardOutput = true;
                runPsi.RedirectStandardError = true;
                runPsi.WorkingDirectory = Path.GetDirectoryName(mysqldPath) ?? string.Empty;
                LoadEnvironments(ref runPsi, environments);

                var proc = Process.Start(runPsi);
                if (proc != null)
                {
                    // detach from the process by not waiting on it; let it run in background
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        public override bool Stop(string version)
        {
            string mysqldPath = Path.Combine(appPath, version, $"mariadb-{version}-winx64", "bin", "mysqld.exe");

            try
            {
                var procs = Process.GetProcessesByName("mysqld");
                foreach (var p in procs)
                {
                    try
                    {
                        // Try to match exact executable path if possible
                        string? mainModule = null;
                        try { mainModule = p.MainModule?.FileName; } catch { }
                        if (mainModule != null)
                        {
                            if (string.Equals(Path.GetFullPath(mainModule), Path.GetFullPath(mysqldPath), StringComparison.OrdinalIgnoreCase))
                            {
                                p.Kill();
                                p.WaitForExit(5000);
                            }
                        }
                        else
                        {
                            // If cannot get main module, attempt to kill anyway
                            p.Kill();
                            p.WaitForExit(5000);
                        }
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
