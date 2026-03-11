using dekit2.Applications;
using dekit2.Properties;
using System.Reflection;

namespace dekit2
{
    public partial class frmMain : Form
    {
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private bool reallyExit = false;
        private bool ballonFirstTime = true;

        public frmMain()
        {
            InitializeComponent();
            Icon = Resources.dev_23828;
            InitTray();
        }

        private void InitTray()
        {
            ToolStripMenuItem openItem = new ToolStripMenuItem("Open", null, OpenFromTray);
            ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit", null, ExitApplication);
            openItem.Font = new Font(openItem.Font, FontStyle.Bold);
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add(openItem);
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add(exitItem);

            trayIcon = new NotifyIcon();
            trayIcon.Text = "Minh Nguyen DevKit2";
            trayIcon.Icon = this.Icon;
            trayIcon.Visible = false;
            trayIcon.ContextMenuStrip = trayMenu;

            trayIcon.DoubleClick += TrayIcon_DoubleClick;

            this.Resize += MainForm_Resize;
            this.FormClosing += MainForm_FormClosing;
            trayIcon.Visible = true;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                HideToTray();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
#if !DEBUG
            if (!reallyExit)
            {
                e.Cancel = true;
                HideToTray();
            }
#endif
        }

        private void HideToTray()
        {
            this.Hide();
            trayIcon.Visible = true;
            if (ballonFirstTime)
            {
                ballonFirstTime = false;
                trayIcon.ShowBalloonTip(3000, "Application is still running", "The application is running in the system tray. Right-click the tray icon and select Exit to close it.", ToolTipIcon.Info);
            }
        }

        private void ShowFromTray()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.Activate();
            trayIcon.Visible = true; // có thể để true hoặc false tùy ý
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowFromTray();
        }

        private void OpenFromTray(object sender, EventArgs e)
        {
            ShowFromTray();
        }

        private void ExitApplication(object sender, EventArgs e)
        {
            reallyExit = true;
            trayIcon.Visible = false;
            Application.Exit();
        }

        private void frmMain_Load(object sender, EventArgs e)
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
                __internalTrigger = true;
                comboCell.Value = selected;
            }
        }

        private void dataGridViewPrograms_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            bool validClick = (e.RowIndex != -1 && e.ColumnIndex != -1); //Make sure the clicked row/column is valid.
            var datagridview = sender as DataGridView;

            // Check to make sure the cell clicked is the cell containing the combobox 
            if (validClick && datagridview.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn)
            {
                datagridview.BeginEdit(true);
                ((ComboBox)datagridview.EditingControl).DroppedDown = true;
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

        private bool __internalTrigger = false;
        private void dataGridViewPrograms_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var datagridview = sender as DataGridView;
            bool validClick = (e.RowIndex != -1 && e.ColumnIndex != -1);
            if (!validClick || datagridview == null) { return; }

            if (e.ColumnIndex == colInstall.Index && datagridview.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn)
            {
                if (__internalTrigger)
                {
                    __internalTrigger = false;
                    return;
                }
                var row = datagridview.Rows[e.RowIndex];
                bool isChecked = (bool)(row.Cells[e.ColumnIndex].Value ?? false);
                if (isChecked)
                {
                    //Install software
                    var app = row.Tag as IApplication;
                    if (app != null)
                    {
                        string version = row.Cells[colSelect.Index]?.Value?.ToString() ?? "";
                        if (string.IsNullOrEmpty(version))
                        {
                            MessageBox.Show("Please select a version to install", "Minh Nguyen DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            app.Install(version);
                            RowRefresh(row);
                        }
                    }
                }
                else
                {
                    //Uninstall software
                    MessageBox.Show("Uninstall");
                }
            }
            else if (e.ColumnIndex == colSelect.Index)
            {
                __internalTrigger = true;
                var row = datagridview.Rows[e.RowIndex];
                string version = row.Cells[e.ColumnIndex]?.Value?.ToString() ?? "";
                var app = row.Tag as IApplication;
                if(app != null && app.IsInstalled(version))
                {
                    row.Cells[colInstall.Index].Value = true;
                }
                else
                {
                    row.Cells[colInstall.Index].Value = true;
                }
            }
        }
    }
}
