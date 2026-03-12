using dekit2.Common;
using System.Net;
using System.Net.Sockets;
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
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DevKit2");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        protected void LoadConfig(string applicationPath)
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
                Config = JsonSerializer.Deserialize<JsonObject>(strConfig);
            }
            catch
            {
                Config = null;
            }
        }

        protected bool SaveConfig(JsonObject? config, string applicationPath)
        {
            if (config != null)
            {
                string configFile = Path.Combine(applicationPath, "config.json");
                string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configFile, json);
            }
            return false;
        }

        public virtual bool Valid => false;
        public virtual string Name => string.Empty;

        protected JsonObject? Config = null;
        protected string appPath = string.Empty;

        public virtual ValueName[] InstalledVersions
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

        public virtual ValueName[] AvailableVersions => Array.Empty<ValueName>();

        public virtual bool IsInstalled(string version)
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

        public virtual bool IsRunning(string version) { return false; }

        public virtual bool Install(string version) { return false; }

        public virtual bool Start(string version, string envPath) { return false; }

        public virtual bool Stop(string version) { return false; }

        public virtual string GetPaths(string version) { return string.Empty; }

        public virtual bool Uninstall(string version)
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
                    SaveConfig(Config, appPath);
                }
                return true;
            }
            return false;
        }

        protected virtual bool Download(string url, string file)
        {
            try
            {
                var handler = new SocketsHttpHandler
                {
                    AllowAutoRedirect = true,
                    ConnectCallback = async (context, cancellationToken) =>
                    {
                        var addresses = await Dns.GetHostAddressesAsync(context.DnsEndPoint.Host);

                        // Sắp xếp: IPv4 trước, IPv6 sau
                        var ordered = addresses
                            .OrderBy(a => a.AddressFamily == AddressFamily.InterNetwork ? 0 : 1)
                            .ToList();

                        Exception? lastEx = null;

                        foreach (var ip in ordered)
                        {
                            try
                            {
                                var socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                                await socket.ConnectAsync(ip, context.DnsEndPoint.Port, cancellationToken);
                                return new NetworkStream(socket, ownsSocket: true);
                            }
                            catch (Exception ex)
                            {
                                lastEx = ex;
                            }
                        }
                        throw lastEx ?? new Exception("Unable to connect to any resolved address.");
                    }
                };

                using var client = new HttpClient(handler);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

                using var response = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead)
                                           .GetAwaiter()
                                           .GetResult();

                response.EnsureSuccessStatusCode();

                using var input = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
                using var output = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None);

                input.CopyTo(output);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
