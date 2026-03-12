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
        string GetPaths(string version);
        bool Start(string version, string envPath);
        bool Stop(string version);
    }
}
