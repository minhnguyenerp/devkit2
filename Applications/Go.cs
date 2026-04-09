using devkit2.Common;
using devkit2.Properties;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    internal sealed class Go : BaseApplication
    {
        public override string Name => "Go";

        public Go()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "go");
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
                base.Icon = Resources.golang_logo_icon_171073;
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
                    new ValueName("1.26.2", "1.26.2"),
                    new ValueName("1.26.1", "1.26.1"),
                };
            }
        }

        public override bool Install(string version, IProgress<InstallProgress>? progress = null)
        {
            string url = string.Empty;
            string file = string.Empty;
            switch (version)
            {
                case "1.26.2":
                    url = "https://go.dev/dl/go1.26.2.windows-amd64.zip";
                    file = Path.Combine(Path.GetTempPath(), "go1.26.2.windows-amd64.zip");
                    break;
                case "1.26.1":
                    url = "https://go.dev/dl/go1.26.1.windows-amd64.zip";
                    file = Path.Combine(Path.GetTempPath(), "go1.26.1.windows-amd64.zip");
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
                Directory.CreateDirectory(Path.Combine(extractPath, "go", "gopath"));
                Directory.CreateDirectory(Path.Combine(extractPath, "go", "gocache"));
                Directory.CreateDirectory(Path.Combine(extractPath, "go", "gotelemetry"));

                base.SaveNewVersion(version);

                return true;
            }
            return false;
        }

        public override ValueName[] GetEnvironments(string version)
        {
            return new ValueName[] {
                new ValueName("PATH", Path.Combine(appPath, version, "go", "bin")),
                new ValueName("GOPATH", Path.Combine(appPath, version, "go", "gopath")),
                new ValueName("GOCACHE", Path.Combine(appPath, version, "go", "gocache")),
                new ValueName("GOTELEMETRYDIR", Path.Combine(appPath, version, "go", "gotelemetry")),
            };
        }

        public override bool Start(string version, ValueName[] environments, JsonObject? profile = null, string uniqueCode = "")
        {
            var psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            psi.UseShellExecute = false;
            string workingDir = profile?["WorkingDirectory"]?.ToString() ?? string.Empty;
            if (!string.IsNullOrEmpty(workingDir) && Directory.Exists(workingDir))
            {
                psi.WorkingDirectory = workingDir;
            }
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
                        ApplicationVersion = version,
                    });
                    return true;
                }
            }
            catch { return false; }
            return false;
        }
    }
}
