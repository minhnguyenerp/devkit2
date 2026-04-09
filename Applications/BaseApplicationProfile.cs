using devkit2.Properties;
using System.ComponentModel;
using System.Text.Json.Nodes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace devkit2.Applications
{
    public partial class BaseApplicationProfile : Form
    {
        public BaseApplicationProfile()
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
                    txtWorkingDirectory.Text = dialog.SelectedPath;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (Profile == null) { Profile = new JsonObject(); }
            Profile["WorkingDirectory"] = txtWorkingDirectory.Text;
            Profile["StartupFile"] = txtStartupFile.Text;
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

        private void BaseApplicationProfile_Load(object sender, EventArgs e)
        {
            if (Profile != null)
            {
                txtWorkingDirectory.Text = Profile["WorkingDirectory"]?.ToString();
                txtStartupFile.Text = Profile["StartupFile"]?.ToString();
            }
        }

        private void btnStartupFileBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Select a File or Folder";
                dialog.FileName = "[Folder]";
                dialog.Multiselect = false;
                dialog.CheckFileExists = false;
                dialog.ValidateNames = false;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string path = dialog.FileName;
                    if (File.Exists(path))
                    {
                        txtStartupFile.Text = path;
                    }
                    else if (Directory.Exists(Path.GetDirectoryName(path)))
                    {
                        txtStartupFile.Text = Path.GetDirectoryName(path);
                    }
                }
            }
        }
    }
}
