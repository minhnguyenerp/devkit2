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
                        var match = Regex.Match(config, @"(?m)^:(\d+)");
                        if (match.Success)
                        {
                            txtPort.Text = match.Groups[1].Value;
                        }
                    }
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (Profile == null) { Profile = new JsonObject(); }
            Profile["InstanceDirectory"] = txtInstanceDirectory.Text;
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
                txtPort.Text = Profile["Port"]?.ToString();
            }
        }
    }
}
