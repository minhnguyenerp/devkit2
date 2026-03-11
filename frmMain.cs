using dekit2.Properties;

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
            if (!reallyExit)
            {
                e.Cancel = true;
                HideToTray();
            }
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
    }
}
