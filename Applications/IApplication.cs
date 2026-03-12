using dekit2.Common;

namespace dekit2.Applications
{
    public interface IApplication
    {
        string Name { get; }
        ValueName[] InstalledVersions { get; }
        ValueName[] AvailableVersions { get; }
        bool Valid { get; }
        bool IsInstalled(string version);
        bool IsRunning(string version);
        bool Install(string version);
        bool Uninstall(string version);
        ValueName[] GetEnvironments(string version);
        bool Start(string version, ValueName[] environments);
        bool Stop(string version);
    }
}
