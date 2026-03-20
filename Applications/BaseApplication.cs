using devkit2.Common;
using devkit2.Properties;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace devkit2.Applications
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

        public virtual bool IsRunning(string version, string uniqueCode = "")
        {
            return false;
        }

        public virtual bool Install(string version, IProgress<DownloadProgress>? progress = null) { return false; }

        public virtual bool Start(string version, ValueName[] environments, JsonObject? profile = null, string uniqueCode = "") { return false; }

        protected bool CloseChildProcesses(RunningApplication app)
        {
            bool bResult = true;
            foreach (var child in app.Childs)
            {
                try
                {
                    var proc = Process.GetProcessById(child.Pid);
                    if (proc != null && !proc.HasExited)
                    {
                        if (proc.HasExited)
                        {
                            continue;
                        }
                        else
                        {
                            // 1. nếu có window → đóng nhẹ
                            if (proc.MainWindowHandle != IntPtr.Zero)
                            {
                                if (proc.CloseMainWindow())
                                {
                                    if (proc.WaitForExit(5000))
                                        continue;
                                }
                            }
                            proc.Kill();
                            bResult = proc.WaitForExit(5000);
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }
            return bResult;
        }

        public virtual bool Stop(RunningApplication runningApplication)
        {
            CloseChildProcesses(runningApplication);

            bool bResult = false;
            try
            {
                var proc = Process.GetProcessById(runningApplication.Pid);
                if (proc != null && !proc.HasExited)
                {
                    if (proc.HasExited)
                    {
                        bResult = true;
                    }
                    else
                    {
                        // 1. nếu có window → đóng nhẹ
                        if (proc.MainWindowHandle != IntPtr.Zero)
                        {
                            if (proc.CloseMainWindow())
                            {
                                if (proc.WaitForExit(5000))
                                    bResult = true;
                            }
                        }

                        if (!bResult)
                        {
                            proc.Kill();
                            bResult = proc.WaitForExit(5000);
                        }
                    }
                }
            }
            catch
            {
                bResult = true;
            }
            return bResult;
        }

        public virtual ValueName[] GetEnvironments(string version) { return Array.Empty<ValueName>(); }

        public virtual bool Uninstall(string version)
        {
            string extractPath = Path.Combine(appPath, version);
            if (Directory.Exists(extractPath))
            {
                try
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
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                return true;
            }
            return false;
        }

        protected bool DownloadSync(string url, string file)
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
                if (File.Exists(file))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                }
                return false;
            }
        }

        protected virtual bool Download(string url, string file, IProgress<DownloadProgress>? progress)
        {
            try
            {
                if (File.Exists(file))
                {
                    var fileInfo = new FileInfo(file);
                    long size = fileInfo.Length;
                    progress?.Report(new DownloadProgress
                    {
                        BytesReceived = size,
                        TotalBytes = size,
                        SpeedBytesPerSecond = 0
                    });
                    return true;
                }

                var handler = new SocketsHttpHandler
                {
                    AllowAutoRedirect = true,
                    ConnectCallback = async (context, cancellationToken) =>
                    {
                        var addresses = await Dns.GetHostAddressesAsync(context.DnsEndPoint.Host);

                        // IPv4 trước, IPv6 sau
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

                var totalBytes = response.Content.Headers.ContentLength;

                using var input = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
                using var output = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None);

                byte[] buffer = new byte[81920];
                int bytesRead;
                long totalRead = 0;

                var stopwatch = Stopwatch.StartNew();
                long lastReportedBytes = 0;
                var lastReportTime = stopwatch.Elapsed;

                while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, bytesRead);
                    totalRead += bytesRead;

                    var elapsed = stopwatch.Elapsed.TotalSeconds;
                    double avgSpeed = elapsed > 0 ? totalRead / elapsed : 0;

                    // report liên tục, hoặc có thể giới hạn theo thời gian
                    progress?.Report(new DownloadProgress
                    {
                        BytesReceived = totalRead,
                        TotalBytes = totalBytes,
                        SpeedBytesPerSecond = avgSpeed
                    });

                    lastReportedBytes = totalRead;
                    lastReportTime = stopwatch.Elapsed;
                }

                // report lần cuối để chắc chắn đủ 100%
                progress?.Report(new DownloadProgress
                {
                    BytesReceived = totalRead,
                    TotalBytes = totalBytes,
                    SpeedBytesPerSecond = stopwatch.Elapsed.TotalSeconds > 0
                        ? totalRead / stopwatch.Elapsed.TotalSeconds
                        : 0
                });

                return true;
            }
            catch
            {
                if (File.Exists(file))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                }

                return false;
            }
        }

        protected void LoadEnvironments(ref ProcessStartInfo processStartInfo, ValueName[] environments)
        {
            Dictionary<string, string> distinc = new Dictionary<string, string>();
            foreach (var env in environments)
            {
                if (distinc.ContainsKey(env.Value))
                {
                    distinc[env.Value] = env.Name + ";" + distinc[env.Value];
                }
                else
                {
                    distinc[env.Value] = env.Name;
                }
            }

            foreach (var one in distinc)
            {
                string? currentValue = Environment.GetEnvironmentVariable(one.Key);
                if (currentValue != null && currentValue.Length > 0)
                {
                    processStartInfo.EnvironmentVariables[one.Key] = one.Value + ";" + currentValue;
                }
                else
                {
                    processStartInfo.EnvironmentVariables[one.Key] = one.Value;
                }
            }
        }

        private Icon? _icon = null;
        public virtual Icon? Icon
        {
            get
            {
                if (_icon == null)
                    _icon = Resources.dev_23828;
                return _icon;
            }
            set
            {
                if (value != null)
                {
                    _icon = value;
                    _runningIcon = IconUtil.MakeOverlay(_icon, Resources.play);
                }
            }
        }

        private Icon? _runningIcon = null;
        public virtual Icon? RunningIcon
        {
            get
            {
                if (_runningIcon == null)
                    _runningIcon = IconUtil.MakeOverlay(Resources.dev_23828, Resources.play);
                return _runningIcon;
            }
        }

        public virtual JsonObject? ProfileEdit(JsonObject? init = null)
        {
            BaseApplicationProfile dlgProfile = new BaseApplicationProfile() { Profile = init };
            dlgProfile.ShowDialog();
            return dlgProfile.Profile;
        }

        protected virtual void SaveNewVersion(string version)
        {
            if (!IsInstalled(version) && Config != null && Config["InstalledVersions"] != null && Config["InstalledVersions"] is JsonArray)
            {
                ((JsonArray)Config["InstalledVersions"]).Add(version);
            }

            JsonArray sortedVersion = new JsonArray();
            foreach(var one in AvailableVersions)
            {
                foreach(var installed in Config["InstalledVersions"] as JsonArray)
                {
                    if (installed?.ToString() == one.Value)
                    {
                        sortedVersion.Add(installed?.ToString() ?? "");
                        break;
                    }
                }
            }
            Config["InstalledVersions"] = sortedVersion;
            SaveConfig(Config, appPath);
        }
    }
}
