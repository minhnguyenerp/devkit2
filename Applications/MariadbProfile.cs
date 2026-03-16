using devkit2.Properties;
using System.ComponentModel;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    public partial class MariadbProfile : Form
    {
        public MariadbProfile()
        {
            InitializeComponent();
            Icon = Resources.dev_23828;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public JsonObject? Profile { get; set; } = null;

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (Profile == null) { Profile = new JsonObject(); }
            Profile["DataDir"] = txtDataDirectory.Text;
            Profile["Port"] = txtPort.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Profile = null;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void MariadbProfile_Load(object sender, EventArgs e)
        {
            if (Profile != null)
            {
                txtDataDirectory.Text = Profile["DataDir"]?.ToString();
                txtPort.Text = Profile["Port"]?.ToString();
            }
        }

        private void btnBrowseWorkingDirectory_Click(object sender, EventArgs e)
        {

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
