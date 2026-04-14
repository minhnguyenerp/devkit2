using devkit2.Common;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    internal class Python : BaseApplication
    {
        public override string Name => "Python";

        public Python()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "python");
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
                    Path.Combine(appPath, InstalledVersions[0].Value, $"python-{InstalledVersions[0].Value}-embed-amd64", "python.exe")
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
                    new ValueName("3.14.4", "3.14.4"),
                    new ValueName("3.14.3", "3.14.3"),
                };
            }
        }

        public override bool Install(string version, IProgress<InstallProgress>? progress = null)
        {
            string url = string.Empty;
            string file = string.Empty;
            switch (version)
            {
                case "3.14.4":
                    url = "https://github.com/minhnguyenerp/devkit2/releases/download/bin1.0.1/python-3.14.4-embed-amd64.zip";
                    file = Path.Combine(Path.GetTempPath(), "python-3.14.4-embed-amd64.zip");
                    break;
                case "3.14.3":
                    url = "https://github.com/minhnguyenerp/devkit2/releases/download/bin1.0.1/python-3.14.3-embed-amd64.zip";
                    file = Path.Combine(Path.GetTempPath(), "python-3.14.3-embed-amd64.zip");
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

                var psi = new ProcessStartInfo
                {
                    FileName = Path.Combine(appPath, version, $"python-{version}-embed-amd64", "python.exe"),
                    Arguments = "-m pip install --force-reinstall pip",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                {
                    //string output = process.StandardOutput.ReadToEnd();
                    //string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();
                    //Console.WriteLine("OUTPUT:\n" + output);
                    //Console.WriteLine("ERROR:\n" + error);
                }

                base.SaveNewVersion(version);

                return true;
            }
            return false;
        }

        public override ValueName[] GetEnvironments(string version)
        {
            return new ValueName[] {
                new ValueName("PATH", Path.Combine(appPath, version, $"python-{version}-embed-amd64")),
                new ValueName("PATH", Path.Combine(appPath, version, $"python-{version}-embed-amd64", "Scripts")),
            };
        }

        public override bool Start(string version, ValueName[] environments, JsonObject? profile = null, string uniqueCode = "")
        {
            var psi = new ProcessStartInfo();
            psi.FileName = Path.Combine(appPath, version, $"python-{version}-embed-amd64", "python.exe");
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
                        Profile = profile,
                    });
                    return true;
                }
            }
            catch { return false; }
            return false;
        }
    }
}
