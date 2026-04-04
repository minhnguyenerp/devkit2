using devkit2.Applications;
using devkit2.Common;
using devkit2.Properties;
using System.ComponentModel;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

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
                if (app.AvailableVersions.Length > 0)
                {
                    comboBoxProgram.Items.Add(new ValueName(app.Name, app.Name) { Tag = app });

                    newRowIndex = dataGridView1.Rows.Add();
                    var newRow = dataGridView1.Rows[newRowIndex];
                    newRow.Cells[colProgram.Index].Value = app.Name;
                    var comboCell = (DataGridViewComboBoxCell)newRow.Cells[colEnvironment.Index];
                    var list = app?.AvailableVersions?.ToList() ?? new List<ValueName>();
                    list.Insert(0, new ValueName("", ""));
                    comboCell.DataSource = list;
                    comboCell.DisplayMember = "Name";
                    comboCell.ValueMember = "Value";
                    newRow.Tag = app;
                }
            }

            if (Project != null)
            {
                if (Project["ProjectName"] != null)
                {
                    txtProjectName.Text = Project["ProjectName"]?.ToString();
                }
                if (Project["Program"] != null)
                {
                    foreach (var item in comboBoxProgram.Items)
                    {
                        if (Project["Program"]?.ToString() == item.ToString())
                        {
                            comboBoxProgram.SelectedItem = item;
                            break;
                        }
                    }
                }
                if (Project["Version"] != null)
                {
                    foreach (var item in comboBoxVersion.Items)
                    {
                        if (Project["Version"]?.ToString() == item.ToString())
                        {
                            comboBoxVersion.SelectedItem = item;
                            break;
                        }
                    }
                }
                if (Project["Profile"] != null)
                {
                    btnProfile.Tag = Project["Profile"] as JsonObject;
                    btnProfile.Text = Regex.Replace(Project["Profile"]?.ToString() ?? "...", @"\r\n?|\n", "");
                }
                if (Project["Environments"] != null && Project["Environments"] is JsonArray)
                {
                    foreach (var env in Project["Environments"] as JsonArray)
                    {
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            var app = row.Tag as IApplication;
                            if (app != null && env != null && app.Name == env["Program"]?.ToString())
                            {
                                var cell = row.Cells[colEnvironment.Index] as DataGridViewComboBoxCell;
                                if (cell != null)
                                {
                                    var version = env["Version"]?.ToString();
                                    var dataSource = cell.DataSource as IEnumerable<ValueName>;
                                    if (!string.IsNullOrEmpty(version) && dataSource != null)
                                    {
                                        var item = dataSource.FirstOrDefault(x => x.Value == version);
                                        if (item != null)
                                        {
                                            cell.Value = item.Value;
                                        }
                                    }
                                }
                                var cellRun = row.Cells[colRun.Index];
                                if(cellRun != null)
                                {
                                    cellRun.Value = (env["Run"] != null && (env["Run"]?.ToString().ToLower() == "true"));
                                }
                                if (env["Profile"] != null)
                                {
                                    row.Cells[colProfile.Index].Tag = env["Profile"];
                                    row.Cells[colProfile.Index].Value = env["Profile"]?.ToString();
                                }
                                break;
                            }
                        }
                    }
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
                    foreach (var one in app.AvailableVersions)
                    {
                        comboBoxVersion.Items.Add(one);
                    }
                    if (comboBoxVersion.Items.Count > 0)
                        comboBoxVersion.SelectedIndex = 0;
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
                var cell = row.Cells[colEnvironment.Index].Value as string;
                if (!string.IsNullOrEmpty(cell))
                {
                    jsonArray.Add(new JsonObject
                    {
                        ["Program"] = (row.Tag as IApplication)?.Name ?? string.Empty,
                        ["Version"] = cell,
                        ["Profile"] = (row.Cells[colProfile.Index].Tag as JsonObject)?.DeepClone(),
                        ["Run"] = ((row.Cells[colRun.Index].Value as bool?) == true)
                    });
                }
            }
            Project["Environments"] = jsonArray;
            Project["Profile"] = btnProfile.Tag as JsonObject;

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

        private void btnProfile_Click(object sender, EventArgs e)
        {
            var app = (comboBoxProgram.SelectedItem as ValueName)?.Tag as IApplication;
            if (app != null)
            {
                JsonObject? result = app.ProfileEdit(btnProfile.Tag as JsonObject);
                btnProfile.Tag = result;
                btnProfile.Text = Regex.Replace(result?.ToString() ?? "...", @"\r\n?|\n", "");
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

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            bool validClick = (e.RowIndex != -1 && e.ColumnIndex != -1); //Make sure the clicked row/column is valid.
            var datagridview = sender as DataGridView;
            if (datagridview == null) { return; }

            // Check to make sure the cell clicked is the cell containing the combobox 
            if (validClick && e.ColumnIndex == colProfile.Index)
            {
                var row = datagridview.Rows[e.RowIndex];
                var app = row?.Tag as IApplication;
                if (app != null)
                {
                    DataGridViewCell? cell = row?.Cells[colProfile.Index];
                    if (cell != null)
                    {
                        JsonObject? result = app.ProfileEdit(cell.Tag as JsonObject);
                        cell.Tag = result;
                        if(result != null)
                        {
                            cell.Value = result.ToString();
                        }
                        else
                        {
                            cell.Value = string.Empty;
                        }
                    }
                }
            }
        }
    }
}
