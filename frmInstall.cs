using devkit2.Applications;
using devkit2.Common;
using devkit2.Properties;

namespace devkit2
{
    public partial class frmInstall : Form
    {
        private List<(string AppName, string AppVersion)> list = new List<(string AppName, string AppVersion)>();
        public frmInstall(List<(string AppName, string AppVersion)> apps)
        {
            InitializeComponent();
            this.Icon = Icon = Resources.dev_23828;
            this.list = apps;
            dataGridViewPrograms.AllowUserToAddRows = false;
            dataGridViewPrograms.RowHeadersVisible = false;
            dataGridViewPrograms.AllowUserToResizeRows = false;
            dataGridViewPrograms.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewPrograms.MultiSelect = false;
        }

        private void frmInstall_Load(object sender, EventArgs e)
        {
            if (list.Count > 0)
            {
                var apps = Sysconf.Instance.Applications;
                int rowIndex = -1;
                int i = 1;

                foreach (var app in apps)
                {
                    if (app != null && app.Valid)
                    {
                        foreach (var item in list)
                        {
                            if (item.AppName == app.Name)
                            {
                                rowIndex = dataGridViewPrograms.Rows.Add();
                                var row = dataGridViewPrograms.Rows[rowIndex];
                                row.Cells[colNo.Index].Value = (i++).ToString();
                                row.Tag = app;
                                row.Cells[colVersion.Index].Tag = item.AppVersion;
                                RowRefresh(row);
                            }
                        }
                    }
                }

                Install();
            }
            else
            {
                this.Close();
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
                var selected = row.Cells[colVersion.Index].Tag;
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

                var list = comboCell.DataSource as IEnumerable<ValueName>;
                bool exists = list?.Any(x => x.Value.Equals(selected)) ?? false;
                if (exists)
                {
                    comboCell.Value = selected;
                }
                else
                {
                    comboCell.Value = null;
                }
            }
        }

        private void Install()
        {
            int total = dataGridViewPrograms.Rows.Count;
            if (total <= 0) { return; }
            IProgress<int> overallProgress = new Progress<int>(value =>
            {
                total += value;
                if (total <= 0)
                {
                    this.Close();
                }
            });
            foreach (DataGridViewRow row in dataGridViewPrograms.Rows)
            {
                bool isChecked = (bool)(row.Cells[colInstalled.Index].Value ?? false);
                var app = row.Tag as IApplication;
                if (app != null)
                {
                    string version = row.Cells[colSelect.Index]?.Value?.ToString() ?? "";
                    if (!isChecked && !string.IsNullOrEmpty(version))
                    {

                        var progress = new Progress<InstallProgress>(info =>
                        {
                            if (info.ProgressPercentage < 100)
                            {

                                row.Cells[colProgram.Index].Value = $"{app.Name} [{info.ProgressText}]";
                            }
                            else
                            {
                                row.Cells[colProgram.Index].Value = app.Name + (string.IsNullOrEmpty(info.Message) ? " [Installing...]" : $" [{info.Message}]");
                            }
                        });

                        Task.Run(() =>
                        {
                            row.Cells[colAction.Index].Value = "Installing...";
                            app.Install(version, progress);
                            app.ReloadIcon();
                        }).ContinueWith(t =>
                        {
                            if (t.Exception != null)
                            {
                                MessageBox.Show(t.Exception?.InnerException?.Message, "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            RowRefresh(row);
                            overallProgress.Report(-1);
                        }, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                }
            }
        }
    }
}
