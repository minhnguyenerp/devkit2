using dekit2.Common;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Text.Json.Nodes;

namespace dekit2.Applications
{
    internal sealed class SumatraPDF : BaseApplication
    {
        public override string Name => "SumatraPDF";

        public SumatraPDF()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "sumatrapdf");
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
                    new ValueName("3.5.2", "3.5.2"),
                };
            }
        }

        public override bool Install(string version)
        {
            string url = string.Empty;
            string file = string.Empty;
            switch (version)
            {
                case "3.5.2":
                    url = "https://www.sumatrapdfreader.org/dl/rel/3.5.2/SumatraPDF-3.5.2-64.zip";
                    file = Path.Combine(Path.GetTempPath(), "SumatraPDF-3.5.2-64.zip");
                    break;
            }

            if (url != string.Empty && file != string.Empty)
            {
                if (!File.Exists(file))
                {
                    if(!base.Download(url, file))
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

        public override string GetPaths(string version)
        {
            return Path.Combine(appPath, version);
        }

        public override bool Start(string version, string envPath)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = Path.Combine(appPath, version, "SumatraPDF-3.5.2-64.exe");
            psi.UseShellExecute = false;
            string currentPath = Environment.GetEnvironmentVariable("PATH") ?? "";
            psi.EnvironmentVariables["PATH"] = envPath + ";" + currentPath;
            try
            {
                if (Process.Start(psi) != null)
                {
                    return true;
                }
            } catch { return false; }
            return false;
        }

        public override bool Stop(string version)
        {
            return false;
        }
    }
}
