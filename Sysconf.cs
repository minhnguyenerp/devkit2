using devkit2.Applications;
using devkit2.Common;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace devkit2
{
    public sealed class Sysconf
    {
        private static Sysconf? instance = null;
        private static readonly object padlock = new object();
        private static JsonArray runningPids = new JsonArray();

        Sysconf() { }
        public static Sysconf Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Sysconf();
                        string settingPath = Path.Combine(BaseApplication.LocalApplicationData, "settings");
                        Directory.CreateDirectory(settingPath);
                        string runningFile = Path.Combine(settingPath, "runnings.json");
                        if (File.Exists(runningFile))
                        {
                            string strRunning = File.ReadAllText(runningFile);
                            try
                            {
                                runningPids = JsonSerializer.Deserialize<JsonArray>(strRunning);
                            }
                            catch
                            {
                                runningPids = new JsonArray();
                            }

                            foreach(var onePid in runningPids)
                            {
                                if (onePid != null && onePid is JsonObject)
                                {
                                    RunningApplication? running = JsonSerializer.Deserialize<RunningApplication>(onePid.ToString());
                                    if (running != null)
                                    {
                                        instance.AddRunningApplication(running, false);
                                    }
                                }
                            }

                            List<string> lstCodes = new List<string>();
                            foreach (var onePid in runningPids)
                            {
                                lstCodes.Add(onePid?["UniqueCode"]?.ToString() ?? "");
                            }
                            foreach(var one in lstCodes)
                            {
                                instance.GetRunningApplication(one);
                            }
                        }
                    }
                    return instance;
                }
            }
        }

        private List<IApplication> applications = new List<IApplication>();
        public bool AddApplication(IApplication app)
        {
            lock (padlock)
            {
                if (!applications.Contains(app))
                {
                    applications.Add(app);
                    return true;
                }
                return false;
            }
        }

        public List<IApplication> Applications => applications;

        private object runningAppLock = new object();
        private List<RunningApplication> runningApplications = new List<RunningApplication>();
        public bool AddRunningApplication(RunningApplication running, bool isSave = true)
        {
            lock (runningAppLock)
            {
                if (!runningApplications.Contains(running) && !string.IsNullOrEmpty(running.UniqueCode))
                {
                    runningApplications.Add(running);
                    if (isSave)
                    {
                        runningPids.Add(JsonSerializer.SerializeToNode(running));
                        string configFile = Path.Combine(BaseApplication.LocalApplicationData, "settings", "runnings.json");
                        string json = JsonSerializer.Serialize(runningPids, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(configFile, json);
                    }
                    return true;
                }
                return false;
            }
        }

        public RunningApplication? GetRunningApplication(string uniqueCode)
        {
            if(string.IsNullOrEmpty(uniqueCode)) { return null; }
            lock (runningAppLock)
            {
                foreach (RunningApplication app in runningApplications)
                {
                    if (app.UniqueCode == uniqueCode)
                    {
                        try
                        {
                            var proc = Process.GetProcessById(app.Pid);
                            if (proc != null && !proc.HasExited)
                            {
                                return app;
                            }
                        }
                        catch
                        {
                            runningApplications.Remove(app);
                            foreach(var onePid in runningPids)
                            {
                                if (onePid != null && onePid is JsonObject && onePid?["UniqueCode"]?.ToString() == uniqueCode)
                                {
                                    runningPids.Remove(onePid);
                                    break;
                                }
                            }
                            string configFile = Path.Combine(BaseApplication.LocalApplicationData, "settings", "runnings.json");
                            string json = JsonSerializer.Serialize(runningPids, new JsonSerializerOptions { WriteIndented = true });
                            File.WriteAllText(configFile, json);
                            return null;
                        }
                    }
                }
            }
            return null;
        }

        public bool CloseApplication(string uniqueCode)
        {
            if (string.IsNullOrEmpty(uniqueCode)) { return false; }
            lock (runningAppLock)
            {
                foreach (RunningApplication app in runningApplications)
                {
                    if (app.UniqueCode == uniqueCode)
                    {
                        bool bResult = false;
                        try
                        {
                            var proc = Process.GetProcessById(app.Pid);
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
                            bResult = false;
                        }

                        if(bResult)
                        {
                            runningApplications.Remove(app);
                            foreach (var onePid in runningPids)
                            {
                                if (onePid != null && onePid is JsonObject && onePid?["UniqueCode"]?.ToString() == uniqueCode)
                                {
                                    runningPids.Remove(onePid);
                                    break;
                                }
                            }
                        }    

                        return bResult;
                    }
                }
            }
            return false;
        }
    }
}
