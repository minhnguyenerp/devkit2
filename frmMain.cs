using devkit2.Applications;
using devkit2.Properties;
using System.Reflection;

namespace devkit2
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
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                var titleAttr = entryAssembly?.GetCustomAttribute<AssemblyTitleAttribute>();
                var title = titleAttr?.Title;
                var version = entryAssembly?.GetName().Version?.ToString() ?? "";
                this.Text = title + " - " + version;
            }
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
            trayIcon.Text = "DevKit2";
            trayIcon.Icon = this.Icon;
            trayIcon.Visible = false;
            trayIcon.ContextMenuStrip = trayMenu;

            trayIcon.DoubleClick += TrayIcon_DoubleClick;

            //this.Resize += MainForm_Resize;
            //this.FormClosing += MainForm_FormClosing;
            //trayIcon.Visible = true;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                //HideToTray();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var frm = tabPageManualLaunch.Controls[0] as Form;
            if (frm != null)
            {
                frm.Close();
            }
            /*if (!reallyExit)
            {
                e.Cancel = true;
                HideToTray();
            }*/
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

            foreach (var app in apps)
            {
                if (app != null && app.Valid)
                {
                    Sysconf.Instance.AddApplication(app);
                }
            }

            var projects = new frmMyProjects();
            projects.TopLevel = false;
            projects.FormBorderStyle = FormBorderStyle.None;
            tabPageMyProjects.Controls.Add(projects);
            projects.Show();
            projects.Dock = DockStyle.Fill;

            var programs = new frmPrograms();
            programs.TopLevel = false;
            programs.FormBorderStyle = FormBorderStyle.None;
            tabPagePrograms.Controls.Add(programs);
            programs.Show();
            programs.Dock = DockStyle.Fill;

            //var start = new frmStart();
            var start = new frmManualLaunch();
            start.TopLevel = false;
            start.FormBorderStyle = FormBorderStyle.None;
            tabPageManualLaunch.Controls.Add(start);
            start.Show();
            start.Dock = DockStyle.Fill;

            var document = new frmDocument();
            document.TopLevel = false;
            document.FormBorderStyle = FormBorderStyle.None;
            tabPageDocument.Controls.Add(document);
            document.Show();
            document.Dock = DockStyle.Fill;

            tabControlContainer.TabPages.Remove(tabPageFileExplorer);
            /*var explorer = new frmFileExplorer();
            explorer.TopLevel = false;
            explorer.FormBorderStyle = FormBorderStyle.None;
            tabPageFileExplorer.Controls.Add(explorer);
            explorer.Show();
            explorer.Dock = DockStyle.Fill;*/
        }

        private void tabControlContainer_Selected(object sender, TabControlEventArgs e)
        {
            TabPage? page = e.TabPage;
            if (page != null)
            {
                if (page.Controls.Count > 0)
                {
                    Form? form = null;
                    foreach (var control in page.Controls)
                    {
                        if(control is Form)
                        {
                            form = (Form)control;
                        }
                    }
                    if(form != null)
                    {
                        form.Refresh();
                    }
                }
            }
        }
    }
}
