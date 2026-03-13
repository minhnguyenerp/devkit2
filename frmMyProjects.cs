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

        private void toolStripButtonNewProject_Click(object sender, EventArgs e)
        {
            frmProject project = new frmProject();
            project.ShowDialog();
            projects.Add(new JsonObject()
            {
                ["Type"] = "VSCODE",
                ["ProjectLocation"] = "Programfiles",
                ["Environments"] = new JsonArray { "aa", "bb" }
            });
            SaveProjects();
            foreach (var app in Sysconf.Instance.Applications)
            {
                ListViewItem item = new ListViewItem(app.Name);
                item.ImageKey = app.Name;
                listView1.Items.Add(item);
            }
        }
    }
}
