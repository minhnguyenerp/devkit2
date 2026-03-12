using dekit2.Applications;
using dekit2.Properties;
using System.Reflection;
using System.Text;

namespace dekit2
{
    public partial class frmStart : Form
    {
        public frmStart()
        {
            InitializeComponent();
            Icon = Resources.dev_23828;
        }

        private void frmStart_Load(object sender, EventArgs e)
        {
            var apps = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IApplication).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Select(t => (IApplication)Activator.CreateInstance(t));
            int rowIndex = -1;
            int i = 1;
            foreach (var app in apps)
            {
                if (app != null && app.Valid && app.InstalledVersions.Length > 0)
                {
                    rowIndex = dataGridViewPrograms.Rows.Add();
                    var row = dataGridViewPrograms.Rows[rowIndex];
                    row.Cells[colNo.Index].Value = (i++).ToString();
                    row.Cells[colStart.Index].Value = "Run";
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
                //row.Cells[colVersion.Index].Value = string.Join(", ", app?.InstalledVersions);
                var comboCell = (DataGridViewComboBoxCell)row.Cells[colSelect.Index];
                comboCell.DataSource = app?.AvailableVersions;
                comboCell.DisplayMember = "Name";
                comboCell.ValueMember = "Value";
                var selected = app?.InstalledVersions?.LastOrDefault()?.Value;
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

            StringBuilder envPath = new StringBuilder();
            foreach(DataGridViewRow row in datagridview.Rows)
            {
                if (row.Tag != null && row.Tag is IApplication)
                {
                    var app = (IApplication)row.Tag;
                    var version = row.Cells[colSelect.Index].Value?.ToString() ?? "";
                    bool isEnv = (bool)(row.Cells[colEnv.Index].Value ?? false);
                    if (!string.IsNullOrEmpty(version) && isEnv)
                    {
                        var path = app.GetPaths(version);
                        if (!string.IsNullOrEmpty(path))
                        {
                            if (envPath.Length > 0)
                            {
                                envPath.Append(";");
                                envPath.Append(path);
                            }
                            else
                            {
                                envPath.Append(path);
                            }
                        }
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
                        app.Start(version, envPath.ToString());
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
    }
}
