using devkit2.Applications;
using devkit2.Common;
using devkit2.Properties;

namespace devkit2
{
    public partial class frmProject : Form
    {
        public frmProject()
        {
            InitializeComponent();
            Icon = Resources.dev_23828;
        }

        private void frmProject_Load(object sender, EventArgs e)
        {
            int newRowIndex = -1;
            foreach (var app in Sysconf.Instance.Applications)
            {
                if (app.InstalledVersions.Length > 0)
                {
                    comboBoxProgram.Items.Add(new ValueName(app.Name, app.Name) { Tag = app });

                    newRowIndex = dataGridView1.Rows.Add();
                    var newRow = dataGridView1.Rows[newRowIndex];
                    newRow.Cells[colProgram.Index].Value = app.Name;
                    var comboCell = (DataGridViewComboBoxCell)newRow.Cells[colVersion.Index];
                    var list = app?.InstalledVersions?.ToList() ?? new List<ValueName>();
                    list.Insert(0, new ValueName("", ""));
                    comboCell.DataSource = list;
                    comboCell.DisplayMember = "Name";
                    comboCell.ValueMember = "Value";
                    newRow.Tag = app;
                }
            }
        }

        private void comboBoxProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxVersion.Items.Clear();
            var selected = comboBoxProgram.SelectedItem as ValueName;
            if (selected != null && selected.Tag != null)
            {
                IApplication? app = selected.Tag as IApplication;
                if (app != null)
                {
                    int nIdx = -1;
                    foreach (var one in app.InstalledVersions)
                    {
                        nIdx = comboBoxVersion.Items.Add(one);
                    }
                    if (nIdx > -1)
                        comboBoxVersion.SelectedIndex = nIdx;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(txtProjectName.Text.Trim().Length <= 0)
            {
                MessageBox.Show("Please input the project name!", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }    
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
