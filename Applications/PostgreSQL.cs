using devkit2.Common;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    internal sealed class PostgreSQL : BaseApplication
    {
        public override string Name => "PostgreSQL";

        public PostgreSQL()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "postgresql");
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
                base.Icon = Icon.ExtractAssociatedIcon(Path.Combine(appPath, InstalledVersions[0].Value, "pgsql", "bin", "psql.exe"));
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
                    new ValueName("18.3-2", "18.3-2"),
                };
            }
        }

        public override bool Install(string version, IProgress<InstallProgress>? progress = null)
        {
            string url = string.Empty;
            string file = string.Empty;
            switch (version)
            {
                case "18.3-2":
                    url = "https://github.com/minhnguyenerp/devkit2/releases/download/bin1.0.1/postgresql-18.3-2-windows-x64-binaries.zip";
                    file = Path.Combine(Path.GetTempPath(), "postgresql-18.3-2-windows-x64-binaries.zip");
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
                new ValueName("PATH", Path.Combine(appPath, version, "pgsql", "pgAdmin 4", "runtime")),
                new ValueName("PATH", Path.Combine(appPath, version, "pgsql", "bin")),
            };
        }

        public override bool Start(string version, ValueName[] environments, JsonObject? profile = null, string uniqueCode = "")
        {
            if (Sysconf.Instance.GetRunningApplication(uniqueCode) != null)
            {
                MessageBox.Show("PostgreSQL is already running.", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            // Use the same layout as the scripts: baseDir contains bin, my.ini located at baseDir, data under baseDir/data
            string baseDir = Path.Combine(appPath, version, "pgsql");
            string binDir = Path.Combine(baseDir, "bin");
            string postgresApp = Path.Combine(binDir, "postgres.exe");
            string installDbApp = Path.Combine(binDir, "initdb.exe");

            if (!File.Exists(postgresApp))
                return false;
            if (!File.Exists(installDbApp))
                return false;

            // Read profile values (port, datadirectory)
            string dataDir = profile?["DataDirectory"]?.ToString() ?? Path.Combine(baseDir, "data");
            int port = 5432;
            if (profile != null && profile["Port"] != null)
            {
                int.TryParse(profile["Port"].ToString(), out port);
            }

            string postgresSystemDir = Path.Combine(dataDir, "global");
            bool hasSystemTables = Directory.Exists(postgresSystemDir) && Directory.EnumerateFileSystemEntries(postgresSystemDir).Any();

            if (!hasSystemTables)
            {
                var initPsi = new ProcessStartInfo();
                initPsi.FileName = installDbApp;
                initPsi.Arguments = $"-D \"{dataDir}\" -U postgres -E UTF8 --auth=trust";
                initPsi.UseShellExecute = false;
                initPsi.CreateNoWindow = true;
                initPsi.WorkingDirectory = binDir;
                using (var initProc = Process.Start(initPsi))
                {
                    initProc?.WaitForExit(120000);
                }
                if (!Directory.Exists(Path.Combine(dataDir, "log")))
                {
                    Directory.CreateDirectory(Path.Combine(dataDir, "log"));
                }
            }

            var runPsi = new ProcessStartInfo();
            runPsi.FileName = postgresApp;
            runPsi.Arguments = $"-D \"{dataDir}\" -c port={port} -c logging_collector=on -c log_directory=log -c log_filename=postgresql.log -c log_truncate_on_rotation=on -c log_rotation_age=1d";
            runPsi.UseShellExecute = false;
            runPsi.CreateNoWindow = true;
            runPsi.RedirectStandardOutput = true;
            runPsi.RedirectStandardError = true;
            string workingDir = profile?["WorkingDirectory"]?.ToString() ?? binDir;
            if (!string.IsNullOrEmpty(workingDir) && Directory.Exists(workingDir))
            {
                runPsi.WorkingDirectory = workingDir;
            }
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
                Profile = profile,
            });
            return true;
        }

        public override JsonObject? ProfileEdit(JsonObject? init = null)
        {
            using (var dlg = new PostgreSQLProfile())
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
            string baseDir = Path.Combine(appPath, runningApplication.ApplicationVersion, "pgsql");
            string binDir = Path.Combine(baseDir, "bin");
            string postgresApp = Path.Combine(binDir, "pg_ctl.exe");
            var stopPsi = new ProcessStartInfo();
            stopPsi.FileName = postgresApp;
            stopPsi.Arguments = $"-D \"{runningApplication.Profile?["DataDirectory"]?.ToString()}\" stop";
            stopPsi.WorkingDirectory = runningApplication.Profile?["DataDirectory"]?.ToString();
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
