using devkit2.Properties;
using System.ComponentModel;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
namespace devkit2.Applications
{
    public partial class CaddyProfile : Form
    {
        public CaddyProfile()
        {
            InitializeComponent();
            Icon = Resources.dev_23828;
            this.ActiveControl = txtInstanceDirectory;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public JsonObject? Profile { get; set; } = null;

        private void btnBrowseInstanceDirectory_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a Folder";
                dialog.UseDescriptionForTitle = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtInstanceDirectory.Text = dialog.SelectedPath;
                    if (File.Exists(Path.Combine(txtInstanceDirectory.Text, "Caddyfile")))
                    {
                        string config = File.ReadAllText(Path.Combine(txtInstanceDirectory.Text, "Caddyfile"));
                        int nBeginWebRoot = config.IndexOf("#begin webroot");
                        int nEndWebRoot = config.IndexOf("#end webroot");
                        if (nBeginWebRoot > 0 && nEndWebRoot > 0)
                        {
                            string str = config.Substring(nBeginWebRoot, nEndWebRoot + "#end webroot".Length - nBeginWebRoot);
                            string[] lines = str.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                            foreach (string line in lines)
                            {
                                if (line.Contains("\""))
                                {
                                    string[] sParams = line.Split('"');
                                    if (sParams.Length >= 2)
                                    {
                                        txtWebRootDirectory.Text = (sParams[1]).Trim().Replace('/', '\\');
                                        break;
                                    }
                                }
                            }
                        }

                        int nBeginPort = config.IndexOf("#begin port");
                        int nEndPort = config.IndexOf("#end port");
                        if (nBeginPort > 0 && nEndPort > 0)
                        {
                            string str = config.Substring(nBeginPort, nEndPort + "#end port".Length - nBeginPort);
                            var match = Regex.Match(str, @"(?m)^:(\d+)");
                            if (match.Success)
                            {
                                txtPort.Text = match.Groups[1].Value;
                            }
                        }
                    }
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (Profile == null) { Profile = new JsonObject(); }
            Profile["InstanceDirectory"] = txtInstanceDirectory.Text;
            Profile["WebRootDirectory"] = txtWebRootDirectory.Text;
            Profile["Port"] = txtPort.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void CaddyProfile_Load(object sender, EventArgs e)
        {
            if (Profile != null)
            {
                txtInstanceDirectory.Text = Profile["InstanceDirectory"]?.ToString();
                txtWebRootDirectory.Text = Profile["WebRootDirectory"]?.ToString();
                txtPort.Text = Profile["Port"]?.ToString();
            }
        }

        private void btnBrowseWebRootDirectory_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a Folder";
                dialog.UseDescriptionForTitle = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtWebRootDirectory.Text = dialog.SelectedPath;
                }
            }
        }
    }
}
