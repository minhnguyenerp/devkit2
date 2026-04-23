using devkit2.Applications;
using devkit2.Properties;

namespace devkit2
{
    public partial class frmSSHImportRawKey : Form
    {
        public frmSSHImportRawKey()
        {
            InitializeComponent();
            Icon = Resources.dev_23828;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            string credentialsPath = Path.Combine(BaseApplication.LocalApplicationData, "credentials");
            Directory.CreateDirectory(credentialsPath);
            string fileName = textBoxKeyName.Text.Trim();
            string outputPath = Path.Combine(credentialsPath, fileName);

            if(string.IsNullOrWhiteSpace(fileName))
            {
                MessageBox.Show("Please input the key file name", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            /*if(File.Exists(outputPath))
            {
                MessageBox.Show("Key file name is already existed", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }*/

            string input = textBoxPrivateKey.Text.Trim();
            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Please input the private key data!", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (File.Exists(outputPath))
            {
                var result = MessageBox.Show(
                    $"File '{fileName}' already exists. Overwrite?",
                    "DevKit2",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                    return;
            }

            File.WriteAllText(outputPath, input + "\n");

            MessageBox.Show("Import successful!", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
