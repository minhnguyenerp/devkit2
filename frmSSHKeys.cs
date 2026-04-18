using devkit2.Applications;
using devkit2.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace devkit2
{
    public partial class frmSSHKeys : Form
    {
        public frmSSHKeys()
        {
            InitializeComponent();
            Icon = Resources.dev_23828;
        }

        private void toolStripButtonImport_Click(object sender, EventArgs e)
        {
            frmSSHImport dlg = new frmSSHImport();
            dlg.ShowDialog();
            LoadKeysToListView();
        }

        private void toolStripButtonExport_Click(object sender, EventArgs e)
        {
            if (listViewKeys.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a key first!", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            frmSSHExport dlg = new frmSSHExport(listViewKeys.SelectedItems[0].Text);
            dlg.ShowDialog();
        }

        private void frmSSHKeys_Load(object sender, EventArgs e)
        {
            listViewKeys.View = View.Details;
            listViewKeys.FullRowSelect = true;
            listViewKeys.GridLines = true;
            listViewKeys.Columns.Clear();
            listViewKeys.Columns.Add("Key Name", listViewKeys.ClientSize.Width - 26);
            listViewKeys.HeaderStyle = ColumnHeaderStyle.None;
            LoadKeysToListView();
        }

        private void LoadKeysToListView()
        {
            string credentialsPath = Path.Combine(BaseApplication.LocalApplicationData, "credentials");
            Directory.CreateDirectory(credentialsPath);

            listViewKeys.Items.Clear();
            foreach (string file in Directory.GetFiles(credentialsPath))
            {
                if (Path.GetExtension(file).Equals(".pub", StringComparison.OrdinalIgnoreCase))
                    continue;

                string fileName = Path.GetFileName(file);
                var item = new ListViewItem(fileName);
                listViewKeys.Items.Add(item);
            }
        }

        private void listViewKeys_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewKeys.SelectedItems.Count == 0)
                return;

            string fileName = listViewKeys.SelectedItems[0].Text;
            string credentialsPath = Path.Combine(BaseApplication.LocalApplicationData, "credentials");
            string fullPath = Path.Combine(credentialsPath, fileName);
            string normalizedPath = fullPath.Replace("\\", "/");
            string gitCommand =
                "=== Setup SSH for Git ===\r\n\r\n" +

                "[PowerShell]\r\n" +
                $"git config --global core.sshCommand 'ssh -i \"{normalizedPath}\" -p 22'\r\n\r\n" +

                "[CMD / Bash]\r\n" +
                $"git config --global core.sshCommand \"ssh -i \\\"{normalizedPath}\\\" -p 22\"\r\n\r\n" +

                "=== Useful Commands ===\r\n\r\n" +

                "# Check current remote URL\r\n" +
                "git remote -v\r\n\r\n" +

                "# Change remote to SSH (example)\r\n" +
                "git remote set-url origin git@github.com:username/repo.git\r\n";
            textBox1.Text = gitCommand;
        }

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            if (listViewKeys.SelectedItems.Count == 0)
                return;

            string fileName = listViewKeys.SelectedItems[0].Text;

            if (!string.IsNullOrEmpty(fileName))
            {
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to delete this key?",
                    "DevKit2",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    string credentialsPath = Path.Combine(BaseApplication.LocalApplicationData, "credentials");
                    string fullPath = Path.Combine(credentialsPath, fileName);
                    File.Delete(fullPath);
                    LoadKeysToListView();
                }
            }
        }
    }
}
