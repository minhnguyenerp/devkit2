using devkit2.Common;
using System.Diagnostics;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    internal sealed class Spyder : Python
    {
        public override string Name => "Spyder";

        public Spyder() : base()
        {
            ReloadInstalledVersions();
        }

        private void ReloadInstalledVersions()
        {
            JsonArray arr = new JsonArray();
            foreach (var item in base.InstalledVersions)
            {
                string path = Path.Combine(base.appPath, item.Value, $"python-{item.Value}-embed-amd64", "Scripts", "spyder.exe");
                if (File.Exists(path))
                {
                    arr.Add(item.Value);
                }
            }
            Config?["InstalledVersions"] = arr;
        }

        public override bool Install(string version, IProgress<InstallProgress>? progress = null)
        {
            if(!base.InstalledVersions.Any(v => v.Value == version))
            {
                base.Install(version, progress);
            }

            if (base.InstalledVersions.Any(v => v.Value == version))
            {
                string path = Path.Combine(base.appPath, version, $"python-{version}-embed-amd64", "Scripts", "pip.exe");
                if (File.Exists(path))
                {
                    var psi = new ProcessStartInfo();
                    psi.FileName = path;
                    psi.UseShellExecute = false;
                    psi.Arguments = "install spyder";
                    var proc = Process.Start(psi);
                    proc?.WaitForExit();
                    if (proc?.ExitCode != 0)
                    {
                        return false;
                    }
                    else
                    {
                        ReloadInstalledVersions();
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool Uninstall(string version)
        {
            foreach (var item in base.InstalledVersions)
            {
                if (item.Value == version)
                {
                    string path = Path.Combine(base.appPath, item.Value, $"python-{item.Value}-embed-amd64", "Scripts", "pip.exe");
                    if (File.Exists(path))
                    {
                        var psi = new ProcessStartInfo();
                        psi.FileName = path;
                        psi.UseShellExecute = false;
                        psi.Arguments = "uninstall spyder";
                        var proc = Process.Start(psi);
                        proc?.WaitForExit();
                        if (proc?.ExitCode != 0)
                        {
                            return false;
                        }
                        else
                        {
                            ReloadInstalledVersions();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public override bool Start(string version, ValueName[] environments, JsonObject? profile = null, string uniqueCode = "")
        {
            var psi = new ProcessStartInfo();
            psi.FileName = Path.Combine(appPath, version, $"python-{version}-embed-amd64", "Scripts", "spyder.exe");
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            string workingDir = profile?["WorkingDirectory"]?.ToString() ?? string.Empty;
            if (!string.IsNullOrEmpty(workingDir) && Directory.Exists(workingDir))
            {
                psi.WorkingDirectory = workingDir;
            }
            string startupFile = profile?["StartupFile"]?.ToString() ?? string.Empty;
            if (!string.IsNullOrEmpty(startupFile) && (File.Exists(startupFile) || Directory.Exists(startupFile)))
            {
                psi.ArgumentList.Add("-p");
                psi.ArgumentList.Add(startupFile);
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
