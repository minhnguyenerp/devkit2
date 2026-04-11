using devkit2.Common;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    internal sealed class Php : BaseApplication
    {
        public override string Name => "PHP";

        public Php()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "php");
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
                    Path.Combine(appPath, InstalledVersions[0].Value, "php.exe")
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
                    new ValueName("8.5.5", "8.5.5"),
                    new ValueName("8.5.4", "8.5.4"),
                    new ValueName("7.4.33", "7.4.33"),
                };
            }
        }

        public override bool Install(string version, IProgress<InstallProgress>? progress = null)
        {
            string url = string.Empty;
            string file = string.Empty;
            string iniUrl = string.Empty;
            List<(string Url, string File, string[] Extracts)> extensions = new List<(string Url, string File, string[] Extracts)>();
            switch (version)
            {
                case "8.5.5":
                    url = "https://downloads.php.net/~windows/releases/archives/php-8.5.5-Win32-vs17-x64.zip";
                    file = Path.Combine(Path.GetTempPath(), "php-8.5.5-Win32-vs17-x64.zip");
                    iniUrl = "https://github.com/minhnguyenerp/devkit2/releases/download/bin1.0.1/php.ini-8.5.4"; // Use the same php.ini with the 8.5.4 version
                    extensions.Add((
                        Url: "https://downloads.php.net/~windows/pecl/releases/apcu/5.1.28/php_apcu-5.1.28-8.5-ts-vs17-x64.zip",
                        File: Path.Combine(Path.GetTempPath(), "php_apcu-5.1.28-8.5-ts-vs17-x64.zip"),
                        Extracts: new string[] { "php_apcu.dll" }
                    ));
                    break;
                case "8.5.4":
                    url = "https://github.com/minhnguyenerp/devkit2/releases/download/bin1.0.1/php-8.5.4-Win32-vs17-x64.zip";
                    file = Path.Combine(Path.GetTempPath(), "php-8.5.4-Win32-vs17-x64.zip");
                    iniUrl = "https://github.com/minhnguyenerp/devkit2/releases/download/bin1.0.1/php.ini-8.5.4";
                    extensions.Add((
                        Url: "https://downloads.php.net/~windows/pecl/releases/apcu/5.1.28/php_apcu-5.1.28-8.5-ts-vs17-x64.zip",
                        File: Path.Combine(Path.GetTempPath(), "php_apcu-5.1.28-8.5-ts-vs17-x64.zip"),
                        Extracts: new string[] { "php_apcu.dll" }
                    ));
                    break;
                case "7.4.33":
                    url = "https://github.com/minhnguyenerp/devkit2/releases/download/bin1.0.1/php-7.4.33-Win32-vc15-x64.zip";
                    file = Path.Combine(Path.GetTempPath(), "php-7.4.33-Win32-vc15-x64.zip");
                    iniUrl = "https://github.com/minhnguyenerp/devkit2/releases/download/bin1.0.1/php.ini-7.4.33";
                    extensions.Add((
                        Url: "https://downloads.php.net/~windows/pecl/releases/apcu/5.1.28/php_apcu-5.1.28-7.4-ts-vc15-x64.zip",
                        File: Path.Combine(Path.GetTempPath(), "php_apcu-5.1.28-7.4-ts-vc15-x64.zip"),
                        Extracts: new string[] { "php_apcu.dll" }
                    ));
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
                    string phpIniFile = Path.Combine(extractPath, "php.ini");
                    base.Download(iniUrl, phpIniFile, progress);
                    string strToReplace = $";extension_dir = \"ext\"";
                    string strPhpIni = File.ReadAllText(phpIniFile);
                    strPhpIni = strPhpIni.Replace(strToReplace, $"extension_dir=\"{Path.Combine(extractPath, "ext")}\"");
                    File.WriteAllText(phpIniFile, strPhpIni);
                    foreach (var extension in extensions)
                    {
                        if(base.Download(extension.Url, extension.File, progress))
                        {
                            ZipHelper.ExtractSelectedFiles(
                                extension.File,
                                Path.Combine(extractPath, "ext"),
                                extension.Extracts, out string error);
                        }
                    }    
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
                new ValueName("PATH", Path.Combine(appPath, version)),
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
