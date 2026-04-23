using devkit2.Applications;
using devkit2.Properties;
using System.Runtime;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace devkit2
{
    public partial class frmSSHSettings : Form
    {
        public frmSSHSettings()
        {
            InitializeComponent();
            Icon = Resources.dev_23828;
        }

        private void frmSSHSettings_Load(object sender, EventArgs e)
        {
            string settingPath = Path.Combine(BaseApplication.LocalApplicationData, "settings");
            Directory.CreateDirectory(settingPath);
            string settingFile = Path.Combine(settingPath, "ssh-settings.json");
            if (File.Exists(settingFile))
            {
                string strContent = File.ReadAllText(settingFile);
                JsonObject? obj = JsonSerializer.Deserialize<JsonObject>(strContent);
                if (obj != null)
                {
                    txtSshPort.Text = obj["ssh-port"]?.ToString();
                    checkBoxSshGlobal.Checked = (obj["ssh-global"] != null ? (bool)obj["ssh-global"] : false);
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            int nPort = 22;
            int.TryParse(txtSshPort.Text.Trim(), out nPort);
            JsonObject obj = new JsonObject();
            obj["ssh-port"] = nPort;
            obj["ssh-global"] = checkBoxSshGlobal.Checked;
            string json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
            string settingPath = Path.Combine(BaseApplication.LocalApplicationData, "settings");
            Directory.CreateDirectory(settingPath);
            string settingFile = Path.Combine(settingPath, "ssh-settings.json");
            File.WriteAllText(settingFile, json);
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
