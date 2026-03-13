using devkit2.Applications;
using devkit2.Common;
using devkit2.Properties;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace devkit2
{
    public partial class frmMyProjects : Form
    {
        private ImageList imgList = new ImageList();
        private JsonArray projects = new JsonArray();
        private string strConfigFile = string.Empty;

        public frmMyProjects()
        {
            InitializeComponent();
            Icon = Resources.dev_23828;
            imgList.ImageSize = new Size(32, 32);
            imgList.ColorDepth = ColorDepth.Depth32Bit;
            foreach (var app in Sysconf.Instance.Applications)
            {
                imgList.Images.Add(app.Name, app.Icon);
            }
            listView1.LargeImageList = imgList;
            listView1.View = View.LargeIcon;
            listView1.MultiSelect = false;

            string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DevKit2", "projects");
            if (!Directory.Exists(configPath))
            {
                Directory.CreateDirectory(configPath);
            }
            strConfigFile = Path.Combine(configPath, "projects.json");
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
        }

        private void SaveProjects()
        {
            string json = JsonSerializer.Serialize(projects, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(strConfigFile, json);
        }

        private void LoadProjects()
        {
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
                        oneFound.ImageKey = project["Program"]?.ToString() ?? "";
                    }
                    else
                    {
                        ListViewItem item = new ListViewItem(project["ProjectName"]?.ToString() ?? "Noname");
                        item.Tag = project["GUID"]?.ToString() ?? "GUID";
                        item.ImageKey = project["Program"]?.ToString() ?? "";
                        listView1.Items.Add(item);
                    }
                }
            }
        }

        private void toolStripButtonNewProject_Click(object sender, EventArgs e)
        {
            frmProject project = new frmProject();
            if (project.ShowDialog() == DialogResult.OK)
            {
                projects.Add(project.Project);
                SaveProjects();
                LoadProjects();
            }
        }

        private void frmMyProjects_Load(object sender, EventArgs e)
        {
            LoadProjects();
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0) { return; }
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
                    foreach (var app in Sysconf.Instance.Applications)
                    {
                        if(app.Name == proj["Program"]?.ToString())
                        {
                            primaryApplication = app;
                            primaryVersion = proj["Version"]?.ToString() ?? string.Empty;
                            break;
                        }
                    }
                    if (primaryApplication != null && !string.IsNullOrEmpty(primaryVersion))
                    {
                        List<ValueName> listEnv = new List<ValueName>();
                        if (proj["Environments"] != null)
                        {
                            foreach(var env in proj["Environments"] as JsonArray)
                            {
                                if (env != null && env["Program"] != null && env["Version"] != null)
                                {
                                    IApplication? subapp = null;
                                    foreach (var app in Sysconf.Instance.Applications)
                                    {
                                        if (app.Name == env["Program"]?.ToString())
                                        {
                                            subapp = app;
                                            break;
                                        }
                                    }
                                    if(subapp != null)
                                    {
                                        listEnv.AddRange(subapp.GetEnvironments(env["Version"]?.ToString() ?? string.Empty));
                                    }
                                }
                            }
                        }
                        primaryApplication.Start(primaryVersion, listEnv.ToArray());
                    }
                }
            }
        }
    }
}
