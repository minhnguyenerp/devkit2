using dekit2.Common;
using System.Diagnostics;
using System.Net;
using System.Text.Json.Nodes;

namespace dekit2.Applications
{
    internal sealed class Git : BaseApplication
    {
        public override string Name => "Git";
        private JsonObject? Config = null;
        private string appPath = string.Empty;

        public Git()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "git");
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
            }
            Config = base.LoadConfig(appPath);
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

        public override ValueName[] InstalledVersions
        {
            get
            {
                if (Config != null && Config["InstalledVersions"] != null && Config["InstalledVersions"] is JsonArray)
                {
                    JsonArray installedVersions = (JsonArray?)Config["InstalledVersions"] ?? new JsonArray();
                    List<ValueName> list = new List<ValueName>();
                    foreach (var one in installedVersions)
                    {
                        if (one != null)
                        {
                            list.Add(new ValueName(one.ToString(), one.ToString()));
                        }
                    }
                    return list.ToArray();
                }
                else
                {
                    return Array.Empty<ValueName>();
                }
            }
        }

        public override ValueName[] AvailableVersions
        {
            get
            {
                return new ValueName[]
                {
                    new ValueName("2.53.0", "2.53.0"),
                };
            }
        }

        public override bool IsInstalled(string version)
        {
            if (Config != null && Config["InstalledVersions"] != null && Config["InstalledVersions"] is JsonArray)
            {
                JsonArray installedVersions = (JsonArray?)Config["InstalledVersions"] ?? new JsonArray();
                foreach (var one in installedVersions)
                {
                    if (one != null && one.ToString() == version)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool Install(string version)
        {
            string url = string.Empty;
            string file = string.Empty;
            switch (version)
            {
                case "2.53.0":
                    url = "https://github.com/git-for-windows/git/releases/download/v2.53.0.windows.1/PortableGit-2.53.0-64-bit.7z.exe";
                    file = Path.Combine(Path.GetTempPath(), "PortableGit-2.53.0-64-bit.7z.exe");
                    break;
            }

            if (url != string.Empty && file != string.Empty)
            {
                if (!File.Exists(file))
                {
                    using HttpClient client = new HttpClient();
                    var response = client.GetAsync(url).GetAwaiter().GetResult();
                    response.EnsureSuccessStatusCode();
                    using (var fs = new FileStream(file, FileMode.Create, FileAccess.Write))
                    {
                        response.Content.CopyToAsync(fs).GetAwaiter().GetResult();
                    }
                }

                string extractPath = Path.Combine(appPath, version);
                Directory.CreateDirectory(extractPath);

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = file,
                    Arguments = $"-y -o\"{extractPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process.Start(psi).WaitForExit();

                if(!IsInstalled(version) && Config != null && Config["InstalledVersions"] != null && Config["InstalledVersions"] is JsonArray)
                {
                    ((JsonArray)Config["InstalledVersions"]).Add(version);
                }
                base.SaveConfig(Config, appPath);

                return true;
            }
            return false;
        }

        public override bool Start(string version)
        {
            MessageBox.Show("Start");
            return false;
        }

        public override bool Stop(string version)
        {
            return false;
        }

        public override bool Uninstall(string version)
        {
            string extractPath = Path.Combine(appPath, version);
            if (Directory.Exists(extractPath))
            {
                Directory.Delete(extractPath, true);
                if (Config != null && Config["InstalledVersions"] != null && Config["InstalledVersions"] is JsonArray)
                {
                    JsonArray installedVersions = (JsonArray?)Config["InstalledVersions"] ?? new JsonArray();
                    JsonArray arr = new JsonArray();
                    foreach (var one in installedVersions)
                    {
                        if (one != null && one.ToString() != version)
                        {
                            arr.Add(one.ToString());
                        }
                    }
                    Config["InstalledVersions"] = arr;
                    base.SaveConfig(Config, appPath);
                }
                return true;
            }
            return false;
        }
    }
}
