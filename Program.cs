namespace devkit2
{
    internal static class Program
    {
        static Mutex global_mutex = new Mutex(true, "a57b812d-be3c-4106-979d-39f66c1e89e1");
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            if (global_mutex.WaitOne(TimeSpan.Zero, true))
            {
                try
                {
                    ApplicationConfiguration.Initialize();
                    Application.Run(new frmMain());
                }
                finally
                {
                    global_mutex.ReleaseMutex();
                }
            }
            else
            {
                MessageBox.Show("DevKit2 is already running. Only one instance at a time is allowed.", "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}