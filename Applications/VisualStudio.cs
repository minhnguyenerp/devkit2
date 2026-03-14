using devkit2.Common;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.ConstrainedExecution;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    internal sealed class VisualStudio : BaseApplication
    {
        public override string Name => "VisualStudio";

        public VisualStudio()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "visualstudio");
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
            }
            base.LoadConfig(appPath);

            foreach (var ver in AvailableVersions)
            {
                if (File.Exists(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    "Microsoft Visual Studio",
                    ver.Value,
                    "Community",
                    "Common7",
                    "IDE",
                    "devenv.exe"
                )))
                {
                    if (Config != null && Config["InstalledVersions"] == null) { Config["InstalledVersions"] = new JsonArray(); }
                    if (!IsInstalled(ver.Value) && Config != null && Config["InstalledVersions"] != null && Config["InstalledVersions"] is JsonArray)
                    {
                        ((JsonArray)Config["InstalledVersions"]).Add(ver.Value);
                    }
                }
            }
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
                    new ValueName("18", "18"),
                    new ValueName("2022", "2022"),
                };
            }
        }

        public override bool Install(string version)
        {
            string url = string.Empty;
            switch (version)
            {
                case "18":
                    url = "https://visualstudio.microsoft.com/downloads/";
                    break;
                case "2022":
                    url = "https://visualstudio.microsoft.com/downloads/";
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
            return new ValueName[] {
                new ValueName("PATH", Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    "Microsoft Visual Studio",
                    version,
                    "Community",
                    "Common7",
                    "IDE"
                )),
            };
        }

        public override bool Start(string version, ValueName[] environments, JsonObject? profile = null)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "Microsoft Visual Studio",
                version,
                "Community",
                "Common7",
                "IDE",
                "devenv.exe"
            );
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
                if (Process.Start(psi) != null)
                {
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

        public override bool Stop(string version)
        {
            return false;
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
                            _icon = Icon.ExtractAssociatedIcon(
                                Path.Combine(
                                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                                    "Microsoft Visual Studio",
                                    InstalledVersions[0].Value,
                                    "Community",
                                    "Common7",
                                    "IDE",
                                    "devenv.exe"
                                )
                            );
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
