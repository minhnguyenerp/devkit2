using devkit2.Applications;
using devkit2.Common;
using devkit2.Properties;
using System.Text.Json.Nodes;

namespace devkit2
{
    public partial class frmStart : Form
    {
        public frmStart()
        {
            InitializeComponent();
            Icon = Resources.dev_23828;
            dataGridViewPrograms.AllowUserToAddRows = false;
            dataGridViewPrograms.RowHeadersVisible = false;
            dataGridViewPrograms.AllowUserToResizeRows = false;
            dataGridViewPrograms.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewPrograms.MultiSelect = false;
        }

        private void frmStart_Load(object sender, EventArgs e)
        {
            LoadApplications();
        }

        public override void Refresh()
        {
            LoadApplications();
            base.Refresh();
        }

        private void LoadApplications()
        {
            int newRowIndex = -1;
            foreach (var app in Sysconf.Instance.Applications)
            {
                DataGridViewRow? existed = null;
                foreach (DataGridViewRow row in dataGridViewPrograms.Rows)
                {
                    if (row.Tag != null && row.Tag == app)
                    {
                        existed = row;
                        break;
                    }
                }
                if (existed != null)
                {
                    if (app.InstalledVersions.Length <= 0) { dataGridViewPrograms.Rows.Remove(existed); }
                }
                else
                {
                    if (app != null && app.Valid && app.InstalledVersions.Length > 0)
                    {
                        newRowIndex = dataGridViewPrograms.Rows.Add();
                        var newRow = dataGridViewPrograms.Rows[newRowIndex];
                        newRow.Cells[colNo.Index].Value = (dataGridViewPrograms.Rows.Count).ToString();
                        newRow.Cells[colStart.Index].Value = "Run";
                        newRow.Tag = app;
                        RowRefresh(newRow);
                    }
                }
            }
        }

        private void RowRefresh(DataGridViewRow row)
        {
            if (row.Tag != null && row.Tag is IApplication)
            {
                var app = (IApplication)row.Tag;
                row.Cells[colProgram.Index].Value = app?.Name ?? "Unknown";
                //row.Cells[colVersion.Index].Value = string.Join(", ", app?.InstalledVersions);
                var comboCell = (DataGridViewComboBoxCell)row.Cells[colSelect.Index];
                comboCell.DataSource = app?.InstalledVersions;
                comboCell.DisplayMember = "Name";
                comboCell.ValueMember = "Value";
                var selected = app?.InstalledVersions?.FirstOrDefault()?.Value;
                //__internalTrigger = true;
                comboCell.Value = selected;
            }
        }

        private void dataGridViewPrograms_CellClick(object sender, DataGridViewCellEventArgs e)
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

            List<ValueName> environments = new List<ValueName>();
            foreach (DataGridViewRow row in datagridview.Rows)
            {
                if (row.Tag != null && row.Tag is IApplication)
                {
                    var app = (IApplication)row.Tag;
                    var version = row.Cells[colSelect.Index].Value?.ToString() ?? "";
                    bool isEnv = (bool)(row.Cells[colEnv.Index].Value ?? false);
                    if (!string.IsNullOrEmpty(version) && isEnv)
                    {
                        environments.AddRange(app.GetEnvironments(version));
                    }
                }
            }

            if (e.ColumnIndex == colStart.Index)
            {
                var row = datagridview.Rows[e.RowIndex];
                var app = row.Tag as IApplication;
                if (app != null)
                {
                    string version = row.Cells[colSelect.Index]?.Value?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(version))
                    {
                        app.Start(version, environments.ToArray(), row.Cells[colProfile.Index]?.Tag as JsonObject);
                    }
                    else
                    {
                        MessageBox.Show("Please select a version to start", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void dataGridViewPrograms_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            var datagridview = sender as DataGridView;
            if (datagridview != null && datagridview.IsCurrentCellDirty)
            {
                datagridview.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dataGridViewPrograms_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
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
                        if (result != null)
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
