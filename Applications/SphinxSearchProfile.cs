using devkit2.Properties;
using System.ComponentModel;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace devkit2.Applications
{
    public partial class SphinxSearchProfile : Form
    {
        public SphinxSearchProfile()
        {
            InitializeComponent();
            Icon = Resources.dev_23828;
            this.ActiveControl = txtConfigDirectory;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public JsonObject? Profile { get; set; } = null;

        private void ApacheProfile_Load(object sender, EventArgs e)
        {
            if (Profile != null)
            {
                txtConfigDirectory.Text = Profile["ConfigDirectory"]?.ToString();
                txtDataDirectory.Text = Profile["DataDirectory"]?.ToString();
                txtPort.Text = Profile["Port"]?.ToString();
            }
        }

        private void btnBrowseConfigDirectory_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a Folder";
                dialog.UseDescriptionForTitle = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtConfigDirectory.Text = dialog.SelectedPath;
                    if (File.Exists(Path.Combine(txtConfigDirectory.Text, "sphinx.conf")))
                    {
                        string config = File.ReadAllText(Path.Combine(txtConfigDirectory.Text, "sphinx.conf"));
                        int nBeginPort = config.IndexOf("#begin port");
                        int nEndPort = config.IndexOf("#end port");
                        if (nBeginPort > 0 && nEndPort > 0)
                        {
                            string str = config.Substring(nBeginPort, nEndPort + "#end port".Length - nBeginPort);
                            string[] lines = str.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                            foreach (string line in lines)
                            {
                                var match = Regex.Match(line, @"listen\s*=\s*(\d+)");
                                if (match.Success)
                                {
                                    txtPort.Text = match.Groups[1].Value;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (Profile == null) { Profile = new JsonObject(); }
            Profile["ConfigDirectory"] = txtConfigDirectory.Text;
            Profile["DataDirectory"] = txtDataDirectory.Text;
            Profile["Port"] = txtPort.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Profile = null;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnBrowseDataDirectory_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a Folder";
                dialog.UseDescriptionForTitle = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtDataDirectory.Text = dialog.SelectedPath;
                }
            }
        }
    }
}
