using devkit2.Applications;
using devkit2.Common;
using devkit2.Properties;
using System.Text.Json;
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
            string configFile = Path.Combine(BaseApplication.LocalApplicationData, "settings", "starts.json");
            if (File.Exists(configFile))
            {
                string strConfig = File.ReadAllText(configFile);
                try
                {
                    var starts = JsonSerializer.Deserialize<JsonArray>(strConfig);
                    foreach(var one in starts)
                    {
                        string? appName = one?["ApplicationName"]?.ToString();
                        var profile = one?["Profile"] as JsonObject;
                        if (!string.IsNullOrEmpty(appName))
                        {
                            foreach (DataGridViewRow row in dataGridViewPrograms.Rows)
                            {
                                if (row.Cells[colProgram.Index]?.Value?.ToString() == appName)
                                {
                                    row.Cells[colProfile.Index].Tag = profile?.DeepClone();
                                    if (profile != null)
                                    {
                                        row.Cells[colProfile.Index].Value = profile.ToString();
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                catch { }
            }
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
                var comboCell = (DataGridViewComboBoxCell)row.Cells[colEnvironment.Index];
                var list = app?.InstalledVersions?.ToList() ?? new List<ValueName>();
                list.Insert(0, new ValueName("", ""));
                comboCell.DataSource = list;
                comboCell.DisplayMember = "Name";
                comboCell.ValueMember = "Value";
                //var selected = app?.InstalledVersions?.FirstOrDefault()?.Value;
                //__internalTrigger = true;
                //comboCell.Value = selected;
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
                    var version = row.Cells[colEnvironment.Index].Value?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(version))
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
                    string version = row.Cells[colEnvironment.Index]?.Value?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(version))
                    {
                        app.Start(version, environments.ToArray(), row.Cells[colProfile.Index]?.Tag as JsonObject, MD5.Hash(app.Name + version + ((row.Cells[colProfile.Index]?.Tag as JsonArray)?.ToString()) ?? ""));
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

        private void frmStart_FormClosing(object sender, FormClosingEventArgs e)
        {
            string configFile = Path.Combine(BaseApplication.LocalApplicationData, "settings", "starts.json");
            JsonArray array = new JsonArray();
            foreach (DataGridViewRow one in dataGridViewPrograms.Rows)
            {
                array.Add(new JsonObject
                {
                    ["ApplicationName"] = one.Cells[colProgram.Index]?.Value?.ToString(),
                    ["Profile"] = one.Cells[colProfile.Index]?.Tag as JsonObject,
                });
            }
            string json = JsonSerializer.Serialize(array, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configFile, json);
        }
    }
}
