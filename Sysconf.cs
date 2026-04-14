using devkit2.Applications;
using devkit2.Common;
using System.Diagnostics;
using System.Text.Json;

namespace devkit2
{
    public sealed class Sysconf
    {
        private static Sysconf? instance = null;
        private static readonly object padlock = new object();
        //private static JsonArray runningPids = new JsonArray();

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
                            List<RunningApplication>? runningApps = JsonSerializer.Deserialize<List<RunningApplication>>(strRunning);
                            if (runningApps != null && runningApps.Any())
                            {
                                List<string> lstCodes = new List<string>();
                                foreach (RunningApplication app in runningApps)
                                {
                                    instance.AddRunningApplication(app, false);
                                    lstCodes.Add(app.UniqueCode);
                                }

                                foreach (var one in lstCodes)
                                {
                                    instance.GetRunningApplication(one);
                                }
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
                        string configFile = Path.Combine(BaseApplication.LocalApplicationData, "settings", "runnings.json");
                        string json = JsonSerializer.Serialize(runningApplications, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(configFile, json);
                    }
                    return true;
                }
                return false;
            }
        }

        public IApplication? GetApplication(string appName)
        {
            foreach (var app in applications)
            {
                if(app.Name == appName)
                {
                    return app;
                }
            }
            return null;
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
                            string configFile = Path.Combine(BaseApplication.LocalApplicationData, "settings", "runnings.json");
                            string json = JsonSerializer.Serialize(runningApplications, new JsonSerializerOptions { WriteIndented = true });
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
            bool bResult = false;
            lock (runningAppLock)
            {
                foreach (RunningApplication app in runningApplications)
                {
                    if (app.UniqueCode == uniqueCode)
                    {
                        if (!string.IsNullOrEmpty(app.ApplicationName))
                        {
                            foreach (var application in applications)
                            {
                                if (application.Name == app.ApplicationName)
                                {
                                    bResult = application.Stop(app);
                                    break;
                                }
                            }
                        }

                        if (bResult)
                        {
                            runningApplications.Remove(app);
                        }
                        break;
                    }
                }
            }
            return bResult;
        }
    }
}
