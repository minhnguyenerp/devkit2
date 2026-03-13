using devkit2.Applications;
using devkit2.Common;
using devkit2.Properties;
using System.ComponentModel;
using System.Text.Json.Nodes;

namespace devkit2
{
    public partial class frmProject : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public JsonObject? Project { get; set; } = null;

        public frmProject()
        {
            InitializeComponent();
            Icon = Resources.dev_23828;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
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
            if (txtProjectName.Text.Trim().Length <= 0)
            {
                MessageBox.Show("Please input the project name!", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (comboBoxProgram.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a program!", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (comboBoxVersion.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a program version!", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (Project == null)
            {
                Project = new JsonObject();
                Project["GUID"] = Guid.NewGuid().ToString();
            }
            Project["ProjectName"] = txtProjectName.Text.Trim();
            Project["Program"] = (comboBoxProgram.SelectedItem as ValueName)?.Value ?? string.Empty;
            Project["Version"] = (comboBoxVersion.SelectedItem as ValueName)?.Value ?? string.Empty;

            var jsonArray = new JsonArray();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var cell = row.Cells[colVersion.Index].Value as string;
                if (cell != null && !string.IsNullOrEmpty(cell))
                {
                    jsonArray.Add(new JsonObject
                    {
                        ["Program"] = (row.Tag as IApplication)?.Name ?? string.Empty,
                        ["Version"] = cell,
                    });
                }
            }
            Project["Environments"] = jsonArray;
            Project["ProjectDirectory"] = txtProjectLocation.Text.Trim();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            var datagridview = sender as DataGridView;
            if (datagridview != null && datagridview.IsCurrentCellDirty)
            {
                datagridview.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            bool validClick = (e.RowIndex != -1 && e.ColumnIndex != -1); //Make sure the clicked row/column is valid.
            var datagridview = sender as DataGridView;
            if (datagridview == null) { return; }

            // Check to make sure the cell clicked is the cell containing the combobox 
            if (validClick && datagridview.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn)
            {
                datagridview.BeginEdit(true);
                ((ComboBox)datagridview.EditingControl).DroppedDown = true;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a Folder";
                dialog.UseDescriptionForTitle = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtProjectLocation.Text = dialog.SelectedPath;
                }
            }
        }
    }
}
