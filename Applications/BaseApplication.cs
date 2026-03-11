using dekit2.Common;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace dekit2.Applications
{
    public abstract class BaseApplication : IApplication
    {
        public static string LocalApplicationData
        {
            get
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MinhNguyenDevKit2");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        protected JsonObject? LoadConfig(string applicationPath)
        {
            string configFile = Path.Combine(applicationPath, "config.json");
            if (!File.Exists(configFile))
            {
                JsonObject config = new JsonObject();
                config["InstalledVersions"] = new JsonArray();
                string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configFile, json);
            }

            string strConfig = File.ReadAllText(configFile);
            try
            {
                return JsonSerializer.Deserialize<JsonObject>(strConfig);
            }
            catch
            {
                return null;
            }
        }

        protected bool SaveConfig(JsonObject? config, string applicationPath)
        {
            if(config != null)
            {
                string configFile = Path.Combine(applicationPath, "config.json");
                string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configFile, json);
            }
            return false;
        }

        public virtual bool Valid => false;
        public virtual string Name => string.Empty;
        public virtual ValueName[] InstalledVersions => Array.Empty<ValueName>();
        public virtual ValueName[] AvailableVersions => Array.Empty<ValueName>();
        public virtual bool IsInstalled(string version) { return false; }

        public virtual bool IsRunning(string version) { return false; }

        public virtual bool Install(string version) { return false; }

        public virtual bool Start(string version) { return false; }

        public virtual bool Stop(string version) { return false; }

        public virtual bool Uninstall(string version) { return false; }
    }
}
