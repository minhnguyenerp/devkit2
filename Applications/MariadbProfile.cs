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

        private void btnWorkingDirectoryBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a Folder";
                dialog.UseDescriptionForTitle = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtDataDir.Text = dialog.SelectedPath;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (Profile == null) { Profile = new JsonObject(); }
            Profile["DataDir"] = txtDataDir.Text;
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
                txtDataDir.Text = Profile["DataDir"]?.ToString();
                txtPort.Text = Profile["Port"]?.ToString();
            }
        }
    }
}
