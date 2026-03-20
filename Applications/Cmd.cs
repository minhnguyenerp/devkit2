using devkit2.Common;
using devkit2.Properties;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    internal sealed class Cmd : BaseApplication
    {
        public override string Name => "Cmd";

        public Cmd()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "cmd");
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
            }
            base.LoadConfig(appPath);

            foreach (var ver in AvailableVersions)
            {
                if (Config != null && Config["InstalledVersions"] == null) { Config["InstalledVersions"] = new JsonArray(); }
                if (!IsInstalled(ver.Value) && Config != null && Config["InstalledVersions"] != null && Config["InstalledVersions"] is JsonArray)
                {
                    ((JsonArray)Config["InstalledVersions"]).Add(ver.Value);
                }
            }

            try
            {
                base.Icon = Icon.ExtractAssociatedIcon(
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.System),
                        "cmd.exe"
                    )
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
                    new ValueName("Any", "Any"),
                };
            }
        }

        public override bool Install(string version, IProgress<DownloadProgress>? progress = null)
        {
            string url = string.Empty;
            switch (version)
            {
                case "Any":
                    url = "https://learn.microsoft.com/en-us/windows-server/administration/windows-commands/cmd";
                    break;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
            return true;
        }

        public override ValueName[] GetEnvironments(string version)
        {
            return Array.Empty<ValueName>();
        }

        public override bool Start(string version, ValueName[] environments, JsonObject? profile = null, string uniqueCode = "")
        {
            var psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            psi.UseShellExecute = false;
            if (profile != null)
            {
                string workingDir = profile["WorkingDirectory"]?.ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(workingDir) && Directory.Exists(workingDir))
                {
                    psi.WorkingDirectory = workingDir;
                }
                string startupFile = profile["StartupFile"]?.ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(startupFile) && File.Exists(startupFile))
                {
                    psi.ArgumentList.Add(startupFile);
                }
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return false;
        }
    }
}
