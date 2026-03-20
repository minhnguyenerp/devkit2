using devkit2.Properties;
using System.ComponentModel;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    public partial class PostgreSQLProfile : Form
    {
        public PostgreSQLProfile()
        {
            InitializeComponent();
            Icon = Resources.dev_23828;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public JsonObject? Profile { get; set; } = null;

        private void btnBrowseWorkingDirectory_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a Folder";
                dialog.UseDescriptionForTitle = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtWorkingDirectory.Text = dialog.SelectedPath;
                }
            }
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (Profile == null) { Profile = new JsonObject(); }
            Profile["WorkingDirectory"] = txtWorkingDirectory.Text;
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

        private void PostgreSQLProfile_Load(object sender, EventArgs e)
        {
            if (Profile != null)
            {
                txtWorkingDirectory.Text = Profile["WorkingDirectory"]?.ToString();
                txtDataDirectory.Text = Profile["DataDirectory"]?.ToString();
                txtPort.Text = Profile["Port"]?.ToString();
            }
        }
    }
}
