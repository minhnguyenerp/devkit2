using devkit2.Applications;
using devkit2.Common;
using devkit2.Properties;
using System.Diagnostics;
using System.Security.Policy;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace devkit2
{
    public partial class frmManualLaunch : Form
    {
        private ImageList imgList = new ImageList();
        private JsonArray projects = new JsonArray();
        private string strConfigFile = string.Empty;
        private ContextMenuStrip listViewMenu;

        public frmManualLaunch()
        {
            InitializeComponent();
            Icon = Resources.dev_23828;

            imgList.ImageSize = new Size(32, 32);
            imgList.ColorDepth = ColorDepth.Depth32Bit;
            foreach (var app in Sysconf.Instance.Applications)
            {
                imgList.Images.Add(app.Name, app.Icon);
                imgList.Images.Add(app.Name + "_Running", app.RunningIcon);
            }
            listView1.LargeImageList = imgList;
            listView1.View = View.LargeIcon;
            listView1.MultiSelect = false;

            string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DevKit2", "settings");
            if (!Directory.Exists(configPath))
            {
                Directory.CreateDirectory(configPath);
            }
            strConfigFile = Path.Combine(configPath, "launchs.json");
            if (!File.Exists(strConfigFile))
            {
                string json = JsonSerializer.Serialize(projects, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(strConfigFile, json);
            }

            string strConfig = File.ReadAllText(strConfigFile);
            try
            {
                projects = JsonSerializer.Deserialize<JsonArray>(strConfig);
            }
            catch
            {
                projects = new JsonArray();
            }

            RefreshApps();

            listViewMenu = new ContextMenuStrip();
            listViewMenu.Items.Add("Start", null, listView1_Start_Click);
            listViewMenu.Items.Add("Stop", null, listView1_Stop_Click);
            listViewMenu.Items.Add(new ToolStripSeparator());
            listViewMenu.Items.Add("Open Application Directory", null, listView1_OpenApplicationDirectory_Click);
            listViewMenu.Items.Add("Open Working Directory", null, listView1_OpenWorkingDirectory_Click);
            listViewMenu.Items.Add("Open Command Prompt", null, listView1_OpenCommandPrompt_Click);
            listViewMenu.Items.Add(new ToolStripSeparator());
            listViewMenu.Items.Add("Edit", null, listView1_Edit_Click);
            listView1.ContextMenuStrip = listViewMenu;
        }

        private void listView1_OpenWorkingDirectory_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0) { return; }

            var item = listView1.SelectedItems[0];
            string guid = item.Tag?.ToString() ?? string.Empty;
            JsonObject? profile = null;
            if (!string.IsNullOrEmpty(guid))
            {
                JsonObject? proj = null;
                foreach (var one in projects)
                {
                    if (one != null && guid == (one["GUID"]?.ToString() ?? string.Empty))
                    {
                        proj = (JsonObject)one;
                        break;
                    }
                }
                if (proj != null)
                {
                    foreach (var app in Sysconf.Instance.Applications)
                    {
                        if (app.Name == proj["Program"]?.ToString())
                        {
                            profile = proj["Profile"] as JsonObject;
                            break;
                        }
                    }
                }
            }

            var psi = new ProcessStartInfo();
            psi.FileName = "explorer.exe";
            string workingDir = profile?["WorkingDirectory"]?.ToString() ?? string.Empty;
            if (!string.IsNullOrEmpty(workingDir) && Directory.Exists(workingDir))
            {
                psi.Arguments = workingDir;
            }
            psi.UseShellExecute = true;
            Process.Start(psi);
        }

        private void listView1_OpenApplicationDirectory_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0) { return; }

            var item = listView1.SelectedItems[0];
            string guid = item.Tag?.ToString() ?? string.Empty;
            string appPath = string.Empty;
            if (!string.IsNullOrEmpty(guid))
            {
                JsonObject? proj = null;
                foreach (var one in projects)
                {
                    if (one != null && guid == (one["GUID"]?.ToString() ?? string.Empty))
                    {
                        proj = (JsonObject)one;
                        break;
                    }
                }
                if (proj != null)
                {
                    foreach (var app in Sysconf.Instance.Applications)
                    {
                        if (app.Name == proj["Program"]?.ToString())
                        {
                            appPath = app.AppPath;
                            break;
                        }
                    }
                }
            }

            var psi = new ProcessStartInfo();
            psi.FileName = "explorer.exe";
            if (!string.IsNullOrEmpty(appPath) && Directory.Exists(appPath))
            {
                psi.Arguments = appPath;
            }
            psi.UseShellExecute = true;
            Process.Start(psi);
        }

        private void listView1_OpenCommandPrompt_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0) { return; }
            RunSelectedProject(true);
        }

        private void listView1_Start_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0) { return; }
            RunSelectedProject();
        }

        private void listView1_Stop_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0) { return; }
            StopSelectedProject();
        }

        private void listView1_Edit_Click(object sender, EventArgs e)
        {
            EditSelectedProject();
        }

        private void SaveProjects()
        {
            string json = JsonSerializer.Serialize(projects, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(strConfigFile, json);
        }

        private void RefreshApps()
        {
            JsonArray refreshedApps = new JsonArray();
            foreach (var app in Sysconf.Instance.Applications)
            {
                if (app != null && app.Valid && app.InstalledVersions.Length > 0)
                {
                    string md5 = MD5.Hash(app.Name);
                    if (projects != null)
                    {
                        JsonObject? appFound = null;
                        foreach (var japp in projects)
                        {
                            if (japp?["GUID"]?.ToString() == md5)
                            {
                                appFound = japp as JsonObject;
                                break;
                            }
                        }
                        if (appFound != null)
                        {
                            refreshedApps.Add(appFound.DeepClone());
                        }
                        else
                        {
                            refreshedApps.Add(new JsonObject
                            {
                                ["GUID"] = md5,
                                ["ProjectName"] = app.Name,
                                ["Program"] = app.Name,
                                ["Version"] = app.AvailableVersions?.FirstOrDefault()?.ToString() ?? string.Empty,
                                ["Environments"] = new JsonArray(),
                                ["Profile"] = null,
                            });
                        }
                    }
                }
            }
            projects = refreshedApps;
        }

        private void LoadProjects()
        {
            List<string> guids = new List<string>();
            foreach (var project in projects)
            {
                if (project != null)
                {
                    ListViewItem? oneFound = null;
                    foreach (ListViewItem? one in listView1.Items)
                    {
                        if (one != null && one.Tag != null &&
                            !string.IsNullOrEmpty(one.Tag.ToString()) &&
                            one.Tag.ToString() == project["GUID"]?.ToString())
                        {
                            oneFound = one;
                            break;
                        }
                    }

                    if (oneFound != null)
                    {
                        oneFound.Text = project["ProjectName"]?.ToString() ?? "Noname";
                    }
                    else
                    {
                        ListViewItem item = new ListViewItem(project["ProjectName"]?.ToString() ?? "Noname");
                        item.Tag = project["GUID"]?.ToString() ?? "GUID";
                        listView1.Items.Add(item);
                        oneFound = item;

                        // Update image icon
                        foreach(var one in Sysconf.Instance.Applications)
                        {
                            if(item.Tag.ToString() == MD5.Hash(one.Name))
                            {
                                listView1.LargeImageList?.Images.RemoveByKey(one.Name);
                                listView1.LargeImageList?.Images.RemoveByKey($"{one.Name}_Running");
                                if (one.Icon != null)
                                {
                                    listView1.LargeImageList?.Images.Add(one.Name, one.Icon);
                                }
                                if (one.RunningIcon != null)
                                {
                                    listView1.LargeImageList?.Images.Add($"{one.Name}_Running", one.RunningIcon);
                                }
                            }    
                        }    
                    }

                    string guid = oneFound?.Tag?.ToString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(guid))
                    {
                        guids.Add(guid);
                    }

                    var runningApp = Sysconf.Instance.GetRunningApplication(oneFound?.Tag?.ToString() ?? "");
                    if (runningApp != null)
                    {
                        oneFound?.ImageKey = project["Program"]?.ToString() + "_Running" ?? "";
                    }
                    else
                    {
                        oneFound?.ImageKey = project["Program"]?.ToString() ?? "";
                    }
                }
            }

            for (int i = listView1.Items.Count - 1; i >= 0; i--)
            {
                ListViewItem? one = listView1.Items[i];
                if (one != null && one.Tag != null &&
                    !guids.Contains(one?.Tag?.ToString() ?? string.Empty))
                {
                    listView1.Items.RemoveAt(i);
                }
            }
        }

        private bool StopSelectedProject()
        {
            var item = listView1.SelectedItems[0];
            string guid = item.Tag?.ToString() ?? string.Empty;
            bool result = false;
            if (!string.IsNullOrEmpty(guid))
            {
                JsonObject? proj = null;
                foreach (var one in projects)
                {
                    if (one != null && guid == (one["GUID"]?.ToString() ?? string.Empty))
                    {
                        proj = (JsonObject)one;
                        break;
                    }
                }
                if (proj != null)
                {
                    int guidBegin = 0;
                    foreach (var env in proj["Environments"] as JsonArray)
                    {
                        if (env != null && env["Program"] != null && env["Version"] != null && env["Run"]?.ToString() == "true")
                        {
                            Sysconf.Instance.CloseApplication(guid + (guidBegin++).ToString());
                        }
                    }

                    result = Sysconf.Instance.CloseApplication(guid);
                    if (result == true)
                    {
                        item?.ImageKey = proj["Program"]?.ToString() ?? "";
                    }
                    else
                    {
                        item?.ImageKey = proj["Program"]?.ToString() + "_Running" ?? "";
                    }
                }
            }
            return result;
        }

        private void RunSelectedProject(bool isCommandPrompt = false)
        {
            var item = listView1.SelectedItems[0];
            string guid = item.Tag?.ToString() ?? string.Empty;
            if (!string.IsNullOrEmpty(guid))
            {
                JsonObject? proj = null;
                foreach (var one in projects)
                {
                    if (one != null && guid == (one["GUID"]?.ToString() ?? string.Empty))
                    {
                        proj = (JsonObject)one;
                        break;
                    }
                }
                if (proj != null)
                {
                    IApplication? primaryApplication = null;
                    string primaryVersion = string.Empty;
                    JsonObject? primaryProfile = null;
                    bool needToSaveProjectConfig = false;
                    foreach (var app in Sysconf.Instance.Applications)
                    {
                        if (app.Name == proj["Program"]?.ToString())
                        {
                            primaryApplication = app;
                            primaryVersion = proj["Version"]?.ToString() ?? string.Empty;
                            primaryProfile = proj["Profile"] as JsonObject;
                            break;
                        }
                    }

                    if (isCommandPrompt)
                    {
                        foreach (var app in Sysconf.Instance.Applications)
                        {
                            if (app.Name == "Cmd")
                            {
                                primaryApplication = app;
                                primaryVersion = app.InstalledVersions.FirstOrDefault()?.ToString() ?? "";
                                break;
                            }
                        }
                    }

                    if (primaryApplication != null && !string.IsNullOrEmpty(primaryVersion))
                    {
                        List<ValueName> listEnv = new List<ValueName>();
                        if (proj["Environments"] != null)
                        {
                            foreach (var env in proj["Environments"] as JsonArray)
                            {
                                if (env != null && env["Program"] != null && env["Version"] != null)
                                {
                                    IApplication? subapp = null;
                                    foreach (var app in Sysconf.Instance.Applications)
                                    {
                                        if (app.Name == env["Program"]?.ToString() && app.Name != primaryApplication.Name)
                                        {
                                            subapp = app;
                                            break;
                                        }
                                    }
                                    if (subapp != null)
                                    {
                                        listEnv.AddRange(subapp.GetEnvironments(env["Version"]?.ToString() ?? string.Empty));
                                    }
                                }
                            }
                            listEnv.AddRange(primaryApplication.GetEnvironments(primaryVersion));

                            int guidBegin = 0;
                            foreach (var env in proj["Environments"] as JsonArray)
                            {
                                if (env != null && env["Program"] != null && env["Version"] != null && env["Run"]?.ToString() == "true")
                                {
                                    foreach (var app in Sysconf.Instance.Applications)
                                    {
                                        if (app.Name == env["Program"]?.ToString())
                                        {
                                            string strversion = env["Version"]?.ToString() ?? string.Empty;
                                            if (!app.IsInstalled(strversion) && !string.IsNullOrEmpty(app.InstalledVersions?.FirstOrDefault()?.ToString()))
                                            {
                                                if (MessageBox.Show($"{app.Name} version {strversion} is no longer existed, do you want to switch to {app.InstalledVersions?.FirstOrDefault()?.ToString()}", "DevKit2", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                                                {
                                                    strversion = app.InstalledVersions?.FirstOrDefault()?.ToString() ?? string.Empty;
                                                    env["Version"] = strversion;
                                                    needToSaveProjectConfig = true;
                                                }
                                            }
                                            if (app.IsInstalled(strversion))
                                            {
                                                app.Start(strversion, listEnv.ToArray(), env["Profile"] as JsonObject, guid + (guidBegin++).ToString());
                                            }
                                            else
                                            {
                                                MessageBox.Show($"Cannot start {app.Name} version {strversion}", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (!primaryApplication.IsInstalled(primaryVersion) && !string.IsNullOrEmpty(primaryApplication.InstalledVersions?.FirstOrDefault()?.ToString()))
                        {
                            if (MessageBox.Show($"{primaryApplication.Name} version {primaryVersion} is no longer existed, do you want to switch to {primaryApplication.InstalledVersions?.FirstOrDefault()?.ToString()}", "DevKit2", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                            {
                                primaryVersion = primaryApplication.InstalledVersions?.FirstOrDefault()?.ToString() ?? string.Empty;
                                proj["Version"] = primaryVersion;
                                needToSaveProjectConfig = true;
                            }
                        }

                        if (primaryApplication.IsInstalled(primaryVersion))
                        {
                            if (isCommandPrompt)
                            {
                                primaryApplication.Start(primaryVersion, listEnv.ToArray(), primaryProfile, guid);
                            }
                            else
                            {
                                if (primaryApplication.Start(primaryVersion, listEnv.ToArray(), primaryProfile, guid))
                                {
                                    item?.ImageKey = proj["Program"]?.ToString() + "_Running" ?? "";
                                }
                                else
                                {
                                    item?.ImageKey = proj["Program"]?.ToString() ?? "";
                                }
                            }
                        }

                        if (needToSaveProjectConfig)
                        {
                            SaveProjects();
                        }
                    }
                }
            }
        }

        private void EditSelectedProject()
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem item = listView1.SelectedItems[0];
                string? guid = item.Tag as string;
                if (!string.IsNullOrEmpty(guid))
                {
                    JsonObject? projFound = null;
                    foreach (var proj in projects)
                    {
                        if (guid == proj?["GUID"]?.ToString())
                        {
                            projFound = proj as JsonObject;
                            break;
                        }
                    }

                    if (projFound != null)
                    {
                        frmProject project = new frmProject() { Project = projFound };
                        if (project.ShowDialog() == DialogResult.OK)
                        {
                            SaveProjects();
                            LoadProjects();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Can not update project!", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a project!", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void frmManualLaunch_Load(object sender, EventArgs e)
        {
            LoadProjects();
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0) { return; }
            RunSelectedProject();
        }

        public override void Refresh()
        {
            RefreshApps();
            LoadProjects();
            base.Refresh();
        }
    }
}
