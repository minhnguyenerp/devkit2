using devkit2.Common;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    public interface IApplication
    {
        string Name { get; }
        string AppPath { get; }
        ValueName[] InstalledVersions { get; }
        ValueName[] AvailableVersions { get; }
        bool Valid { get; }
        bool IsInstalled(string version);
        bool Install(string version, IProgress<InstallProgress>? progress = null);
        bool Uninstall(string version);
        ValueName[] GetEnvironments(string version);
        bool Start(string version, ValueName[] environments, JsonObject? profile = null, string uniqueCode = "");
        bool Stop(RunningApplication runningApplication);
        void ReloadIcon();
        Icon? Icon { get; set; }
        Icon? RunningIcon { get; }
        JsonObject? ProfileEdit(JsonObject? init = null);
    }
}
