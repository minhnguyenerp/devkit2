using dekit2.Applications;
using dekit2.Properties;
using System.Reflection;

namespace dekit2
{
    public partial class frmPrograms : Form
    {
        public frmPrograms()
        {
            InitializeComponent();
            Icon = Resources.dev_23828;
        }

        private void frmPrograms_Load(object sender, EventArgs e)
        {
            var apps = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IApplication).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Select(t => (IApplication)Activator.CreateInstance(t));
            int rowIndex = -1;
            int i = 1;
            foreach (var app in apps)
            {
                if (app != null && app.Valid)
                {
                    rowIndex = dataGridViewPrograms.Rows.Add();
                    var row = dataGridViewPrograms.Rows[rowIndex];
                    row.Cells[colNo.Index].Value = (i++).ToString();
                    row.Tag = app;
                    RowRefresh(row);
                }
            }
        }

        private void RowRefresh(DataGridViewRow row)
        {
            if (row.Tag != null && row.Tag is IApplication)
            {
                var app = (IApplication)row.Tag;
                row.Cells[colProgram.Index].Value = app?.Name ?? "Unknown";
                row.Cells[colVersion.Index].Value = string.Join(", ", app?.InstalledVersions);
                var comboCell = (DataGridViewComboBoxCell)row.Cells[colSelect.Index];
                comboCell.DataSource = app?.AvailableVersions;
                comboCell.DisplayMember = "Name";
                comboCell.ValueMember = "Value";
                var selected = app?.InstalledVersions?.LastOrDefault()?.Value;
                var actionCell = row.Cells[colAction.Index];
                if (selected != null)
                {
                    var checkCell = row.Cells[colInstalled.Index];
                    if (app.IsInstalled(selected.ToString()))
                    {
                        checkCell.Value = true;
                        actionCell.Value = "Un-install";
                    }
                    else
                    {
                        checkCell.Value = false;
                        actionCell.Value = "Install";
                    }
                }
                else
                {
                    actionCell.Value = "";
                }
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

            if (e.ColumnIndex == colAction.Index)
            {
                var row = datagridview.Rows[e.RowIndex];
                bool isChecked = (bool)(row.Cells[colInstalled.Index].Value ?? false);
                var app = row.Tag as IApplication;
                if (app != null)
                {
                    string version = row.Cells[colSelect.Index]?.Value?.ToString() ?? "";
                    if (isChecked)
                    {
                        //Uninstall software
                        if (string.IsNullOrEmpty(version))
                        {
                            MessageBox.Show("Please select a version to install", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            app.Uninstall(version);
                            RowRefresh(row);
                        }
                    }
                    else
                    {
                        //Install software
                        if (string.IsNullOrEmpty(version))
                        {
                            MessageBox.Show("Please select a version to install", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            app.Install(version);
                            RowRefresh(row);
                        }
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

        private void dataGridViewPrograms_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var datagridview = sender as DataGridView;
            bool validClick = (e.RowIndex != -1 && e.ColumnIndex != -1);
            if (!validClick || datagridview == null) { return; }

            if (e.ColumnIndex == colSelect.Index)
            {
                var row = datagridview.Rows[e.RowIndex];
                string version = row.Cells[e.ColumnIndex]?.Value?.ToString() ?? "";
                var app = row.Tag as IApplication;
                if (app != null && app.IsInstalled(version))
                {
                    row.Cells[colInstalled.Index].Value = true;
                    row.Cells[colAction.Index].Value = "Un-install";
                }
                else
                {
                    row.Cells[colInstalled.Index].Value = false;
                    row.Cells[colAction.Index].Value = "Install";
                }
            }
        }
    }
}
