using devkit2.Common;
using System.Text.Json.Nodes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace devkit2.Applications
{
    public interface IApplication
    {
        string Name { get; }
        ValueName[] InstalledVersions { get; }
        ValueName[] AvailableVersions { get; }
        bool Valid { get; }
        bool IsInstalled(string version);
        bool IsRunning(string version, string uniqueCode = "");
        bool Install(string version, IProgress<DownloadProgress>? progress = null);
        bool Uninstall(string version);
        ValueName[] GetEnvironments(string version);
        bool Start(string version, ValueName[] environments, JsonObject? profile = null, string uniqueCode = "");
        bool Stop(string version);
        Icon Icon { get; }
        Icon RunningIcon { get; }
        JsonObject? ProfileEdit(JsonObject? init = null);
    }
}
