using devkit2.Applications;
using devkit2.Properties;

namespace devkit2
{
    public partial class frmSSHImport : Form
    {
        public frmSSHImport()
        {
            InitializeComponent();
            Icon = Resources.dev_23828;
            this.Text = $"Place import data in base64 format";
            this.ActiveControl = txtBase64;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                string input = txtBase64.Text.Trim();
                if (string.IsNullOrWhiteSpace(input))
                {
                    MessageBox.Show("Please paste exported key data first!", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!input.StartsWith("DevKit2|", StringComparison.Ordinal))
                {
                    MessageBox.Show("Invalid key format!", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string[] parts = input.Split(new[] { '|' }, 3);
                if (parts.Length != 3)
                {
                    MessageBox.Show("Invalid key data format!", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string fileName = parts[1];
                string base64 = parts[2];
                string password = txtPassword.Text;

                string cleanedBase64 = base64
                    .Replace("\r", "")
                    .Replace("\n", "")
                    .Trim();

                byte[] rawBytes;
                try
                {
                    rawBytes = Convert.FromBase64String(cleanedBase64);
                }
                catch
                {
                    MessageBox.Show("Base64 data is invalid!", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                bool isEncrypted = KeyTransferService.IsEncryptedDevKitPayload(rawBytes);

                string credentialsPath = Path.Combine(BaseApplication.LocalApplicationData, "credentials");
                Directory.CreateDirectory(credentialsPath);
                string outputPath = Path.Combine(credentialsPath, fileName);

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

                if (isEncrypted)
                {
                    if (string.IsNullOrWhiteSpace(password))
                    {
                        MessageBox.Show(
                            "This key was exported with password protection. Please enter the password to import it.",
                            "DevKit2",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                    KeyTransferService.ImportKeyFromBase64(
                        cleanedBase64,
                        password,
                        credentialsPath,
                        fileName);
                }
                else
                {
                    File.WriteAllBytes(outputPath, rawBytes);
                }

                MessageBox.Show("Import successful!", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
