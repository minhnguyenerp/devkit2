using devkit2.Applications;
using devkit2.Properties;

namespace devkit2
{
    public partial class frmSSHExport : Form
    {
        private string _sshkeyfile = string.Empty;
        public frmSSHExport(string sshkey = "")
        {
            _sshkeyfile = sshkey;
            InitializeComponent();
            Icon = Resources.dev_23828;
            this.Text = $"Export: {_sshkeyfile}";
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                string credentialsPath = Path.Combine(BaseApplication.LocalApplicationData, "credentials");
                string fullPath = Path.Combine(credentialsPath, _sshkeyfile);
                if (!File.Exists(fullPath))
                {
                    MessageBox.Show("Cannot open key file!", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }

                txtExported.Text = "";
                string password = txtPassword.Text;
                string base64;

                if (string.IsNullOrWhiteSpace(password))
                {
                    byte[] data = File.ReadAllBytes(fullPath);
                    base64 = Convert.ToBase64String(data);
                }
                else
                {
                    base64 = KeyTransferService.ExportKeyToBase64(fullPath, password);
                }

                string finalText = $"DevKit2|{_sshkeyfile}|{base64}";

                txtExported.Text = finalText;
                txtExported.Focus();
                txtExported.SelectAll();
                MessageBox.Show("Export successful!", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
