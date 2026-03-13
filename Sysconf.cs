using devkit2.Applications;
using System.Security.Cryptography.Pkcs;

namespace devkit2
{
    public sealed class Sysconf
    {
        private static Sysconf? instance = null;
        private static readonly object padlock = new object();
        Sysconf() { }
        public static Sysconf Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Sysconf();
                    }
                    return instance;
                }
            }
        }

        private List<IApplication> applications = new List<IApplication>();
        public bool AddApplication(IApplication app)
        {
            lock (padlock)
            {
                if (!applications.Contains(app))
                {
                    applications.Add(app);
                    return true;
                }
                return false;
            }
        }

        public List<IApplication> Applications => applications;
    }
}
