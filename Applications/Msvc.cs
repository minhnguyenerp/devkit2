using devkit2.Common;
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace devkit2.Applications
{
    internal sealed class Msvc : BaseApplication
    {
        public override string Name => "Msvc";

        public Msvc()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "msvc");
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
            }
            base.LoadConfig(appPath);
            ReloadIcon();
        }

        public override void ReloadIcon()
        {
            try
            {
                base.Icon = Icon.ExtractAssociatedIcon(
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.System),
                        "cmd.exe"
                    )
                );
            }
            catch { }
        }

        public override bool Valid
        {
            get
            {
                if (Config != null)
                    return true;
                return false;
            }
        }

        public override ValueName[] AvailableVersions
        {
            get
            {
                return new ValueName[]
                {
                    new ValueName("2026", "2026"),
                    new ValueName("2022", "2022"),
                    new ValueName("2019", "2019"),
                    new ValueName("2017", "2017"),
                };
            }
        }

        public override bool Install(string version, IProgress<InstallProgress>? progress = null)
        {
            using var builder = new PortableMsvcBuilder(new PortableMsvcOptions
            {
                OutputDirectory = Path.Combine(appPath, version),
                DownloadsDirectory = Path.Combine(Path.GetTempPath(), $"msvc-{version}"),
                VsVersion = version,
                UseInsiders = false,
                HostArchitecture = "x64",
                TargetArchitecturesCsv = "x64,x86",
                AcceptLicense = true,
                Verbose = true,
                DownloadRetryCount = 3,
                DownloadRetryDelayMs = 2000,
                Logger = s => { Debug.WriteLine($"[MSVC] {s}"); progress?.Report(new InstallProgress { Message = s }); }
            });

            JsonObject jConf = new JsonObject();
            var result = builder.BuildAsync(progress, jConf).GetAwaiter().GetResult();

            if (!result.Success)
            {
                var msg = result.Errors.Count > 0
                    ? string.Join(Environment.NewLine, result.Errors)
                    : "Portable MSVC build failed.";
                MessageBox.Show(msg, "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (Config != null)
                {
                    if (Config["Version"] == null) { Config["Version"] = new JsonObject(); }
                    Config["Version"]?[version] = jConf;
                }
                base.SaveNewVersion(version);
                return true;
            }    
            return false;
        }

        public override ValueName[] GetEnvironments(string version)
        {
            if(Config?["Version"]?[version] != null)
            {
                string host = Config?["Version"]?[version]?["host"]?.ToString() ?? "";
                string target = Config?["Version"]?[version]?["target"]?.ToString() ?? "";
                string msvcv = Config?["Version"]?[version]?["msvcv"]?.ToString() ?? "";
                string sdkv = Config?["Version"]?[version]?["sdkv"]?.ToString() ?? "";
                return new ValueName[] {
                    new ValueName("VSCMD_ARG_HOST_ARCH", host),
                    new ValueName("VSCMD_ARG_TGT_ARCH", target),
                    new ValueName("VCToolsVersion", msvcv),
                    new ValueName("WindowsSDKVersion", $"{sdkv}\\"),
                    new ValueName("VCToolsInstallDir", $"{Path.Combine(appPath, version)}\\VC\\Tools\\MSVC\\{msvcv}\\"),
                    new ValueName("WindowsSdkBinPath", $"{Path.Combine(appPath, version)}\\Windows Kits\\10\\bin\\"),
                    new ValueName("PATH", $"{Path.Combine(appPath, version)}\\VC\\Tools\\MSVC\\{msvcv}\\bin\\Host{host}\\{target}"),
                    new ValueName("PATH", $"{Path.Combine(appPath, version)}\\Windows Kits\\10\\bin\\{sdkv}\\{host}"),
                    new ValueName("PATH", $"{Path.Combine(appPath, version)}\\Windows Kits\\10\\bin\\{sdkv}\\{host}\\ucrt"),
                    new ValueName("INCLUDE", $"{Path.Combine(appPath, version)}\\VC\\Tools\\MSVC\\{msvcv}\\include"),
                    new ValueName("INCLUDE", $"{Path.Combine(appPath, version)}\\Windows Kits\\10\\Include\\{sdkv}\\ucrt"),
                    new ValueName("INCLUDE", $"{Path.Combine(appPath, version)}\\Windows Kits\\10\\Include\\{sdkv}\\shared"),
                    new ValueName("INCLUDE", $"{Path.Combine(appPath, version)}\\Windows Kits\\10\\Include\\{sdkv}\\um"),
                    new ValueName("INCLUDE", $"{Path.Combine(appPath, version)}\\Windows Kits\\10\\Include\\{sdkv}\\winrt"),
                    new ValueName("INCLUDE", $"{Path.Combine(appPath, version)}\\Windows Kits\\10\\Include\\{sdkv}\\cppwinrt"),
                    new ValueName("LIB", $"{Path.Combine(appPath, version)}\\VC\\Tools\\MSVC\\{msvcv}\\lib\\{target}"),
                    new ValueName("LIB", $"{Path.Combine(appPath, version)}\\Windows Kits\\10\\Lib\\{sdkv}\\ucrt\\{target}"),
                    new ValueName("LIB", $"{Path.Combine(appPath, version)}\\Windows Kits\\10\\Lib\\{sdkv}\\um\\{target}"),
                };
            }

            return Array.Empty<ValueName>();
        }

        public override bool Start(string version, ValueName[] environments, JsonObject? profile = null, string uniqueCode = "")
        {
            var psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            psi.UseShellExecute = false;
            string workingDir = profile?["WorkingDirectory"]?.ToString() ?? string.Empty;
            if (!string.IsNullOrEmpty(workingDir) && Directory.Exists(workingDir))
            {
                psi.WorkingDirectory = workingDir;
            }
            LoadEnvironments(ref psi, environments);

            try
            {
                var proc = Process.Start(psi);
                if (proc != null)
                {
                    Sysconf.Instance.AddRunningApplication(new RunningApplication
                    {
                        UniqueCode = uniqueCode,
                        Pid = proc.Id,
                        Sessionid = proc.SessionId,
                        ProcessName = proc.ProcessName,
                        StartTime = proc.StartTime,
                        ApplicationName = Name,
                        ApplicationVersion = version,
                    });
                    return true;
                }
            }
            catch { return false; }
            return false;
        }
    }

    internal sealed class PortableMsvcOptions
    {
        public string OutputDirectory { get; set; } = "msvc";
        public string DownloadsDirectory { get; set; } = "downloads";

        public string VsVersion { get; set; } = "latest"; // 2019, 2022, 2026, latest
        public bool UseInsiders { get; set; } = false;

        public string HostArchitecture { get; set; } = "x64"; // x64, x86, arm64
        public string TargetArchitecturesCsv { get; set; } = "x64"; // x64,x86,arm,arm64

        public string MsvcVersion { get; set; } = "";
        public string WindowsSdkVersion { get; set; } = "";

        public bool AcceptLicense { get; set; } = true;
        public bool ShowVersionsOnly { get; set; } = false;
        public bool Verbose { get; set; } = true;

        public TimeSpan HttpTimeout { get; set; } = TimeSpan.FromMinutes(30);
        public int DownloadRetryCount { get; set; } = 3;
        public int DownloadRetryDelayMs { get; set; } = 1500;

        public string LessMsiUrl { get; set; } =
            "https://github.com/activescott/lessmsi/releases/download/v2.12.6/lessmsi-v2.12.6.zip";

        public Action<string>? Logger { get; set; }
    }

    internal sealed class PortableMsvcResult
    {
        public bool Success { get; set; }
        public bool UsedMsiExec { get; set; }
        public bool UsedLessMsiFallback { get; set; }

        public string OutputDirectory { get; set; } = "";
        public string DownloadsDirectory { get; set; } = "";
        public string SelectedMsvcVersion { get; set; } = "";
        public string SelectedWindowsSdkVersion { get; set; } = "";
        public string LicenseUrl { get; set; } = "";

        public long TotalDownloadedBytes { get; set; }

        public List<string> Logs { get; } = new();
        public List<string> Errors { get; } = new();
        public List<string> AvailableMsvcVersions { get; } = new();
        public List<string> AvailableWindowsSdkVersions { get; } = new();

        public void Log(string message)
        {
            Logs.Add(message);
        }

        public void Error(string message)
        {
            Errors.Add(message);
            Logs.Add("ERROR: " + message);
        }
    }

    internal sealed class PortableMsvcBuilder : IDisposable
    {
        private static readonly string[] AllHosts = { "x64", "x86", "arm64" };
        private static readonly string[] AllTargets = { "x64", "x86", "arm", "arm64" };

        private static readonly Dictionary<string, string[]> ManifestUrls = new(StringComparer.OrdinalIgnoreCase)
        {
            ["latest"] = new[] { "https://aka.ms/vs/stable/channel", "https://aka.ms/vs/insiders/channel" },
            ["2026"] = new[] { "https://aka.ms/vs/18/stable/channel", "https://aka.ms/vs/18/insiders/channel" },
            ["2022"] = new[] { "https://aka.ms/vs/17/release/channel", "https://aka.ms/vs/17/pre/channel" },
            ["2019"] = new[] { "https://aka.ms/vs/16/release/channel", "https://aka.ms/vs/16/pre/channel" },
        };

        private readonly PortableMsvcOptions _options;
        private readonly HttpClient _http;
        private readonly string _outputDir;
        private readonly string _downloadsDir;
        private readonly MsiExtractor _msiExtractor;

        private long _totalDownloadedBytes;

        public PortableMsvcBuilder(PortableMsvcOptions? options = null)
        {
            _options = options ?? new PortableMsvcOptions();
            _outputDir = Path.GetFullPath(_options.OutputDirectory);
            _downloadsDir = Path.GetFullPath(_options.DownloadsDirectory);

            Directory.CreateDirectory(_outputDir);
            Directory.CreateDirectory(_downloadsDir);

            _http = new HttpClient
            {
                Timeout = _options.HttpTimeout
            };

            _msiExtractor = new MsiExtractor(_downloadsDir, _options);
        }

        public async Task<PortableMsvcResult> BuildAsync(IProgress<InstallProgress>? progress = null, JsonObject? jConf = null, CancellationToken cancellationToken = default)
        {
            var result = new PortableMsvcResult
            {
                OutputDirectory = _outputDir,
                DownloadsDirectory = _downloadsDir
            };

            try
            {
                var host = Normalize(_options.HostArchitecture);
                var targets = ParseTargets(_options.TargetArchitecturesCsv);

                if (!ValidateHostTarget(host, targets, result))
                    return result;

                progress?.Report(new InstallProgress { Message = "Parse manifest urls" });
                if (!ManifestUrls.TryGetValue(_options.VsVersion, out var channelUrls))
                {
                    AddError(result, $"Unsupported VS version selector: {_options.VsVersion}", progress);
                    return result;
                }

                var channelUrl = channelUrls[_options.UseInsiders ? 1 : 0];
                AddLog(result, "Loading channel manifest: " + channelUrl);

                var channelManifest = await TryDownloadJsonAsync(channelUrl, result, progress, cancellationToken).ConfigureAwait(false);
                if (channelManifest == null)
                    return result;

                using (channelManifest)
                {
                    if (!channelManifest.RootElement.TryGetProperty("channelItems", out var channelItems) ||
                        channelItems.ValueKind != JsonValueKind.Array)
                    {
                        AddError(result, "channelItems not found in channel manifest.", progress);
                        return result;
                    }

                    var vsItemName = _options.UseInsiders
                        ? "Microsoft.VisualStudio.Manifests.VisualStudioPreview"
                        : "Microsoft.VisualStudio.Manifests.VisualStudio";

                    if (!TryFirst(channelItems.EnumerateArray(),
                            x => StringEquals(GetStringSafe(x, "id"), vsItemName),
                            out var vsManifestItem))
                    {
                        AddError(result, $"Could not find manifest item '{vsItemName}'.", progress);
                        return result;
                    }

                    string vsManifestPayloadUrl;
                    try
                    {
                        var payloads = vsManifestItem.GetProperty("payloads");
                        if (payloads.ValueKind != JsonValueKind.Array || payloads.GetArrayLength() == 0)
                        {
                            AddError(result, "Visual Studio manifest item has no payloads.", progress);
                            return result;
                        }

                        vsManifestPayloadUrl = GetStringSafe(payloads[0], "url");
                        if (string.IsNullOrWhiteSpace(vsManifestPayloadUrl))
                        {
                            AddError(result, "Could not read Visual Studio manifest payload URL.", progress);
                            return result;
                        }
                    }
                    catch
                    {
                        AddError(result, "Could not read Visual Studio manifest payload URL.", progress);
                        return result;
                    }

                    AddLog(result, "Loading VS manifest: " + vsManifestPayloadUrl);

                    var vsManifest = await TryDownloadJsonAsync(vsManifestPayloadUrl, result, progress, cancellationToken).ConfigureAwait(false);
                    if (vsManifest == null)
                        return result;

                    using (vsManifest)
                    {
                        if (!vsManifest.RootElement.TryGetProperty("packages", out var packagesArray) ||
                            packagesArray.ValueKind != JsonValueKind.Array)
                        {
                            AddError(result, "packages not found in VS manifest.", progress);
                            return result;
                        }

                        var packages = BuildPackageMap(packagesArray);

                        var msvcMap = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        var sdkMap = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                        BuildVersionMaps(packages, msvcMap, sdkMap);

                        if (!_options.UseInsiders)
                            RemovePreviewMsvcFromStable(packages, msvcMap);

                        result.AvailableMsvcVersions.AddRange(msvcMap.Keys);
                        result.AvailableWindowsSdkVersions.AddRange(sdkMap.Keys);

                        if (_options.ShowVersionsOnly)
                        {
                            result.Success = true;
                            return result;
                        }

                        if (msvcMap.Count == 0)
                        {
                            AddError(result, "No MSVC versions found.", progress);
                            return result;
                        }

                        if (sdkMap.Count == 0)
                        {
                            AddError(result, "No Windows SDK versions found.", progress);
                            return result;
                        }

                        var selectedMsvcShort = string.IsNullOrWhiteSpace(_options.MsvcVersion)
                            ? msvcMap.Keys.Last()
                            : _options.MsvcVersion.Trim();

                        var selectedSdkShort = string.IsNullOrWhiteSpace(_options.WindowsSdkVersion)
                            ? sdkMap.Keys.Last()
                            : _options.WindowsSdkVersion.Trim();

                        if (!msvcMap.TryGetValue(selectedMsvcShort, out var selectedMsvcPid))
                        {
                            AddError(result, $"Unknown MSVC version: {selectedMsvcShort}", progress);
                            return result;
                        }

                        if (!sdkMap.TryGetValue(selectedSdkShort, out var selectedSdkPid))
                        {
                            AddError(result, $"Unknown Windows SDK version: {selectedSdkShort}", progress);
                            return result;
                        }

                        var selectedMsvcFull = selectedMsvcPid
                            .Replace("microsoft.vc.", "", StringComparison.OrdinalIgnoreCase)
                            .Replace(".tools.hostx64.targetx64.base", "", StringComparison.OrdinalIgnoreCase);

                        result.SelectedMsvcVersion = selectedMsvcFull;
                        result.SelectedWindowsSdkVersion = selectedSdkShort;

                        AddLog(result, "Selected MSVC version: " + selectedMsvcFull);
                        AddLog(result, "Selected Windows SDK version: " + selectedSdkShort);

                        result.LicenseUrl = GetBuildToolsLicenseUrlSafe(channelItems);
                        if (!_options.AcceptLicense && !string.IsNullOrWhiteSpace(result.LicenseUrl))
                        {
                            AddError(result, "License not accepted. Set AcceptLicense=true or handle prompting outside.", progress);
                            return result;
                        }

                        var okMsvc = await DownloadAndExtractMsvcAsync(
                            packages, host, targets, selectedMsvcFull, result, progress, cancellationToken).ConfigureAwait(false);

                        if (!okMsvc)
                            return result;

                        var okSdk = await DownloadAndExtractWindowsSdkAsync(
                            packages, selectedSdkPid, selectedSdkShort, targets, result, progress, cancellationToken).ConfigureAwait(false);

                        if (!okSdk)
                            return result;

                        var msvcv = GetSingleSubdirectoryNameSafe(Path.Combine(_outputDir, "VC", "Tools", "MSVC"));
                        if (string.IsNullOrWhiteSpace(msvcv))
                        {
                            AddError(result, "Could not determine extracted MSVC version folder.", progress);
                            return result;
                        }

                        var sdkv = GetSingleSubdirectoryNameSafe(Path.Combine(_outputDir, "Windows Kits", "10", "bin"));
                        if (string.IsNullOrWhiteSpace(sdkv))
                        {
                            AddError(result, "Could not determine extracted Windows SDK version folder.", progress);
                            return result;
                        }

                        var okPost = await PostProcessAsync(host, targets, msvcv, sdkv, result, progress, cancellationToken).ConfigureAwait(false);
                        if (!okPost)
                            return result;

                        if(jConf != null && targets.Count > 0)
                        {
                            jConf["host"] = host;
                            jConf["target"] = targets[0];
                            jConf["msvcv"] = msvcv;
                            jConf["sdkv"] = sdkv;
                        }

                        result.TotalDownloadedBytes = _totalDownloadedBytes;
                        result.Success = true;
                        return result;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                AddError(result, "Operation canceled.", progress);
                return result;
            }
            catch (Exception ex)
            {
                AddError(result, "Unhandled error: " + ex.Message, progress);
                return result;
            }
        }

        private async Task<bool> DownloadAndExtractMsvcAsync(
            Dictionary<string, List<JsonElement>> packages,
            string host,
            List<string> targets,
            string msvcFullVersion,
            PortableMsvcResult result,
            IProgress<InstallProgress>? progress,
            CancellationToken cancellationToken)
        {
            AddLog(result, "Downloading MSVC payloads...");

            var msvcPackages = new List<string>
            {
                "microsoft.visualcpp.dia.sdk",
                $"microsoft.vc.{msvcFullVersion}.crt.headers.base",
                $"microsoft.vc.{msvcFullVersion}.crt.source.base",
                $"microsoft.vc.{msvcFullVersion}.asan.headers.base",
                $"microsoft.vc.{msvcFullVersion}.pgo.headers.base",
            };

            foreach (var target in targets)
            {
                msvcPackages.AddRange(new[]
                {
                    $"microsoft.vc.{msvcFullVersion}.tools.host{host}.target{target}.base",
                    $"microsoft.vc.{msvcFullVersion}.tools.host{host}.target{target}.res.base",
                    $"microsoft.vc.{msvcFullVersion}.crt.{target}.desktop.base",
                    $"microsoft.vc.{msvcFullVersion}.crt.{target}.store.base",
                    $"microsoft.vc.{msvcFullVersion}.premium.tools.host{host}.target{target}.base",
                    $"microsoft.vc.{msvcFullVersion}.pgo.{target}.base",
                });

                if (target == "x86" || target == "x64")
                    msvcPackages.Add($"microsoft.vc.{msvcFullVersion}.asan.{target}.base");

                var redistSuffix = target == "arm" ? ".onecore.desktop" : "";
                var redistPkg = $"microsoft.vc.{msvcFullVersion}.crt.redist.{target}{redistSuffix}.base";

                if (!packages.ContainsKey(redistPkg))
                {
                    var redistName = $"microsoft.visualcpp.crt.redist.{target}{redistSuffix}";
                    if (!packages.TryGetValue(redistName, out var redistCandidates) || redistCandidates.Count == 0)
                    {
                        AddError(result, $"Could not resolve redist package '{redistName}'.", progress);
                        return false;
                    }

                    string dependency = "";
                    try
                    {
                        dependency = redistCandidates[0].GetProperty("dependencies")
                            .EnumerateArray()
                            .Select(x => (x.GetString() ?? "").ToLowerInvariant())
                            .FirstOrDefault(x => x.EndsWith(".base", StringComparison.OrdinalIgnoreCase)) ?? "";
                    }
                    catch
                    {
                        dependency = "";
                    }

                    if (string.IsNullOrWhiteSpace(dependency))
                    {
                        AddError(result, $"Could not resolve base dependency for '{redistName}'.", progress);
                        return false;
                    }

                    redistPkg = dependency;
                }

                msvcPackages.Add(redistPkg);
            }

            foreach (var pkg in msvcPackages.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!packages.TryGetValue(pkg, out var candidates))
                {
                    AddLog(result, pkg + " ... MISSING");
                    continue;
                }

                var package = SelectPackageByLanguage(candidates);

                JsonElement payloads;
                try
                {
                    payloads = package.GetProperty("payloads");
                }
                catch
                {
                    AddError(result, $"Package '{pkg}' has no payloads.", progress);
                    return false;
                }

                foreach (var payload in payloads.EnumerateArray())
                {
                    var fileName = GetStringSafe(payload, "fileName");
                    var url = GetStringSafe(payload, "url");
                    var sha = GetStringSafe(payload, "sha256");

                    if (string.IsNullOrWhiteSpace(fileName) ||
                        string.IsNullOrWhiteSpace(url) ||
                        string.IsNullOrWhiteSpace(sha))
                    {
                        AddError(result, $"Invalid payload metadata in package '{pkg}'.", progress);
                        return false;
                    }

                    var ok = await DownloadWithHashAsync(url, sha, fileName, result, progress, cancellationToken).ConfigureAwait(false);
                    if (!ok)
                        return false;

                    var zipPath = Path.Combine(_downloadsDir, fileName);
                    if (!File.Exists(zipPath))
                    {
                        AddError(result, "Downloaded zip file not found: " + zipPath, progress);
                        return false;
                    }

                    try
                    {
                        using var archive = ZipFile.OpenRead(zipPath);
                        foreach (var entry in archive.Entries)
                        {
                            if (!entry.FullName.StartsWith("Contents/", StringComparison.OrdinalIgnoreCase))
                                continue;

                            var relative = entry.FullName.Substring("Contents/".Length)
                                .Replace('/', Path.DirectorySeparatorChar);

                            var outPath = Path.Combine(_outputDir, relative);
                            var outDir = Path.GetDirectoryName(outPath);
                            if (!string.IsNullOrWhiteSpace(outDir))
                                Directory.CreateDirectory(outDir);

                            if (!string.IsNullOrEmpty(entry.Name))
                                entry.ExtractToFile(outPath, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        AddError(result, $"Failed to extract '{fileName}': {ex.Message}", progress);
                        return false;
                    }
                }
            }

            return true;
        }

        private async Task<bool> DownloadAndExtractWindowsSdkAsync(
            Dictionary<string, List<JsonElement>> packages,
            string sdkPid,
            string sdkVersionShort,
            List<string> targets,
            PortableMsvcResult result,
            IProgress<InstallProgress>? progress,
            CancellationToken cancellationToken)
        {
            AddLog(result, "Downloading Windows SDK payloads...");

            var sdkPackages = new List<string>
            {
                "Windows SDK for Windows Store Apps Tools-x86_en-us.msi",
                "Windows SDK for Windows Store Apps Headers-x86_en-us.msi",
                "Windows SDK for Windows Store Apps Headers OnecoreUap-x86_en-us.msi",
                "Windows SDK for Windows Store Apps Libs-x86_en-us.msi",
                "Universal CRT Headers Libraries and Sources-x86_en-us.msi",
            };

            foreach (var arch in AllTargets)
            {
                sdkPackages.Add($"Windows SDK Desktop Headers {arch}-x86_en-us.msi");
                sdkPackages.Add($"Windows SDK OnecoreUap Headers {arch}-x86_en-us.msi");
            }

            foreach (var target in targets)
                sdkPackages.Add($"Windows SDK Desktop Libs {target}-x86_en-us.msi");

            if (!ResolveWindowsSdkInstallPackage(
                    packages,
                    sdkPid,
                    sdkVersionShort,
                    out var sdkInstallPackage,
                    out var sdkResolveLog))
            {
                AddError(result, sdkResolveLog, progress);
                return false;
            }

            AddLog(result, sdkResolveLog);

            var msiPaths = new List<string>();
            var cabNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var pkg in sdkPackages.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var payload = FindPayloadByFileName(sdkInstallPackage, @"Installers\" + pkg);
                if (payload == null)
                    continue;

                var fileName = Path.GetFileName(pkg);
                var ok = await DownloadWithHashAsync(
                    GetStringSafe(payload.Value, "url"),
                    GetStringSafe(payload.Value, "sha256"),
                    fileName,
                    result,
                    progress,
                    cancellationToken).ConfigureAwait(false);

                if (!ok)
                    return false;

                var msiPath = Path.Combine(_downloadsDir, fileName);
                if (!File.Exists(msiPath))
                {
                    AddError(result, "Downloaded MSI file not found: " + msiPath, progress);
                    return false;
                }

                msiPaths.Add(msiPath);

                try
                {
                    foreach (var cab in EnumerateMsiCabNames(msiPath))
                        cabNames.Add(cab);
                }
                catch (Exception ex)
                {
                    AddError(result, $"Failed to inspect MSI '{fileName}' for CABs: {ex.Message}", progress);
                    return false;
                }
            }

            foreach (var cabName in cabNames.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var payload = FindPayloadByFileName(sdkInstallPackage, @"Installers\" + cabName);
                if (payload == null)
                    continue;

                var ok = await DownloadWithHashAsync(
                    GetStringSafe(payload.Value, "url"),
                    GetStringSafe(payload.Value, "sha256"),
                    cabName,
                    result,
                    progress,
                    cancellationToken).ConfigureAwait(false);

                if (!ok)
                    return false;
            }

            foreach (var msiPath in msiPaths)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var extractResult = await _msiExtractor.ExtractAsync(msiPath, _outputDir, cancellationToken).ConfigureAwait(false);

                foreach (var log in extractResult.Logs)
                    AddLog(result, log);

                foreach (var err in extractResult.Errors)
                    AddError(result, err, progress);

                result.UsedMsiExec |= extractResult.UsedMsiExec;
                result.UsedLessMsiFallback |= extractResult.UsedLessMsiFallback;

                if (!extractResult.Success)
                    return false;

                var extractedMsiShadow = Path.Combine(_outputDir, Path.GetFileName(msiPath));
                SafeDeleteFile(extractedMsiShadow);
            }

            return true;
        }

        private static bool ResolveWindowsSdkInstallPackage(
            Dictionary<string, List<JsonElement>> packages,
            string sdkPid,
            string sdkVersionShort,
            out JsonElement sdkInstallPackage,
            out string resolutionLog)
        {
            sdkInstallPackage = default;
            resolutionLog = "";

            static HashSet<string> GetInstallerFileNames(JsonElement pkg)
            {
                var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                try
                {
                    if (!pkg.TryGetProperty("payloads", out var payloads) ||
                        payloads.ValueKind != JsonValueKind.Array)
                        return set;

                    foreach (var payload in payloads.EnumerateArray())
                    {
                        var fileName = GetStringSafe(payload, "fileName");
                        if (string.IsNullOrWhiteSpace(fileName))
                            continue;

                        if (fileName.StartsWith(@"Installers\", StringComparison.OrdinalIgnoreCase))
                            set.Add(fileName);
                    }
                }
                catch
                {
                }

                return set;
            }

            static int ScoreInstallerPackage(JsonElement pkg, string sdkVersionShort)
            {
                int score = 0;

                var id = GetStringSafe(pkg, "id");
                var version = GetStringSafe(pkg, "version");
                var files = GetInstallerFileNames(pkg);

                if (files.Count == 0)
                    return -1;

                // match version signals
                if (!string.IsNullOrWhiteSpace(id) &&
                    id.Contains(sdkVersionShort, StringComparison.OrdinalIgnoreCase))
                    score += 5;

                if (!string.IsNullOrWhiteSpace(version) &&
                    version.Contains(sdkVersionShort, StringComparison.OrdinalIgnoreCase))
                    score += 5;

                // strong signals: real SDK installer payloads
                string[] mustLike =
                {
                    @"Installers\Windows SDK for Windows Store Apps Tools-x86_en-us.msi",
                    @"Installers\Universal CRT Headers Libraries and Sources-x86_en-us.msi",
                    @"Installers\Windows SDK Desktop Headers x64-x86_en-us.msi",
                    @"Installers\Windows SDK OnecoreUap Headers x64-x86_en-us.msi",
                };

                foreach (var f in mustLike)
                {
                    if (files.Contains(f))
                        score += 20;
                }

                // prefer packages with many MSI installers
                score += files.Count;

                return score;
            }

            JsonElement? best = null;
            int bestScore = int.MinValue;
            string bestId = "";

            foreach (var kv in packages)
            {
                foreach (var candidate in kv.Value)
                {
                    int score = ScoreInstallerPackage(candidate, sdkVersionShort);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        best = candidate;
                        bestId = GetStringSafe(candidate, "id");
                    }
                }
            }

            if (best.HasValue && bestScore > 0)
            {
                sdkInstallPackage = best.Value;
                resolutionLog = $"Resolved SDK install package by payload scoring: {bestId} (score={bestScore})";
                return true;
            }

            resolutionLog = $"Could not resolve install package for SDK '{sdkPid}' / version '{sdkVersionShort}'.";
            return false;
        }

        private async Task<bool> PostProcessAsync(
            string host,
            List<string> targets,
            string msvcv,
            string sdkv,
            PortableMsvcResult result,
            IProgress<InstallProgress>? progress,
            CancellationToken cancellationToken)
        {
            try
            {
                var redist = Path.Combine(_outputDir, "VC", "Redist");
                if (Directory.Exists(redist))
                {
                    var redistMsvcDir = Path.Combine(redist, "MSVC");
                    var redistVersion = GetSingleSubdirectoryNameSafe(redistMsvcDir);
                    if (!string.IsNullOrWhiteSpace(redistVersion))
                    {
                        var src = Path.Combine(redistMsvcDir, redistVersion, "debug_nonredist");

                        foreach (var target in targets)
                        {
                            var targetSrc = Path.Combine(src, target);
                            if (!Directory.Exists(targetSrc))
                                continue;

                            foreach (var file in Directory.GetFiles(targetSrc, "*.dll", SearchOption.AllDirectories))
                            {
                                var dstDir = Path.Combine(_outputDir, "VC", "Tools", "MSVC", msvcv, "bin", $"Host{host}", target);
                                Directory.CreateDirectory(dstDir);

                                var dstFile = Path.Combine(dstDir, Path.GetFileName(file));
                                SafeDeleteFile(dstFile);
                                File.Move(file, dstFile);
                            }
                        }
                    }

                    SafeDeleteDirectory(redist);
                }

                var msdiaMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["x86"] = "msdia140.dll",
                    ["x64"] = Path.Combine("amd64", "msdia140.dll"),
                    ["arm"] = Path.Combine("arm", "msdia140.dll"),
                    ["arm64"] = Path.Combine("arm64", "msdia140.dll"),
                };

                var diaSrc = Path.Combine(_outputDir, "DIA%20SDK", "bin", msdiaMap[host]);
                var diaDstBase = Path.Combine(_outputDir, "VC", "Tools", "MSVC", msvcv, "bin", $"Host{host}");

                if (File.Exists(diaSrc))
                {
                    foreach (var target in targets)
                    {
                        var dstDir = Path.Combine(diaDstBase, target);
                        Directory.CreateDirectory(dstDir);
                        File.Copy(diaSrc, Path.Combine(dstDir, Path.GetFileName(diaSrc)), true);
                    }
                }

                SafeDeleteDirectory(Path.Combine(_outputDir, "DIA%20SDK"));

                SafeDeleteDirectory(Path.Combine(_outputDir, "Common7"));
                SafeDeleteDirectory(Path.Combine(_outputDir, "VC", "Tools", "MSVC", msvcv, "Auxiliary"));

                foreach (var target in targets)
                {
                    foreach (var extra in new[] { "store", "uwp", "enclave", "onecore" })
                        SafeDeleteDirectory(Path.Combine(_outputDir, "VC", "Tools", "MSVC", msvcv, "lib", target, extra));

                    SafeDeleteDirectory(Path.Combine(_outputDir, "VC", "Tools", "MSVC", msvcv, "bin", $"Host{host}", target, "onecore"));
                }

                foreach (var extra in new[]
                {
                    Path.Combine("Catalogs"),
                    Path.Combine("DesignTime"),
                    Path.Combine("bin", sdkv, "chpe"),
                    Path.Combine("Lib", sdkv, "ucrt_enclave"),
                })
                {
                    SafeDeleteDirectory(Path.Combine(_outputDir, "Windows Kits", "10", extra));
                }

                foreach (var arch in AllTargets)
                {
                    if (!targets.Contains(arch, StringComparer.OrdinalIgnoreCase))
                    {
                        SafeDeleteDirectory(Path.Combine(_outputDir, "Windows Kits", "10", "Lib", sdkv, "ucrt", arch));
                        SafeDeleteDirectory(Path.Combine(_outputDir, "Windows Kits", "10", "Lib", sdkv, "um", arch));
                    }

                    if (!StringEquals(arch, host))
                    {
                        SafeDeleteDirectory(Path.Combine(_outputDir, "VC", "Tools", "MSVC", msvcv, "bin", $"Host{arch}"));
                        SafeDeleteDirectory(Path.Combine(_outputDir, "Windows Kits", "10", "bin", sdkv, arch));
                    }
                }

                foreach (var target in targets)
                {
                    SafeDeleteFile(Path.Combine(_outputDir, "VC", "Tools", "MSVC", msvcv, "bin", $"Host{host}", target, "vctip.exe"));
                }

                var buildDir = Path.Combine(_outputDir, "VC", "Auxiliary", "Build");
                Directory.CreateDirectory(buildDir);

                await File.WriteAllTextAsync(
                    Path.Combine(buildDir, "vcvarsall.bat"),
                    "rem both bat files are here only for nvcc, do not call them manually",
                    Encoding.ASCII,
                    cancellationToken).ConfigureAwait(false);

                using (File.Create(Path.Combine(buildDir, "vcvars64.bat")))
                {
                }

                foreach (var target in targets)
                {
                    var setup = $@"@echo off

set VSCMD_ARG_HOST_ARCH={host}
set VSCMD_ARG_TGT_ARCH={target}

set VCToolsVersion={msvcv}
set WindowsSDKVersion={sdkv}\

set VCToolsInstallDir=%~dp0VC\Tools\MSVC\{msvcv}\
set WindowsSdkBinPath=%~dp0Windows Kits\10\bin\

set PATH=%~dp0VC\Tools\MSVC\{msvcv}\bin\Host{host}\{target};%~dp0Windows Kits\10\bin\{sdkv}\{host};%~dp0Windows Kits\10\bin\{sdkv}\{host}\ucrt;%PATH%
set INCLUDE=%~dp0VC\Tools\MSVC\{msvcv}\include;%~dp0Windows Kits\10\Include\{sdkv}\ucrt;%~dp0Windows Kits\10\Include\{sdkv}\shared;%~dp0Windows Kits\10\Include\{sdkv}\um;%~dp0Windows Kits\10\Include\{sdkv}\winrt;%~dp0Windows Kits\10\Include\{sdkv}\cppwinrt
set LIB=%~dp0VC\Tools\MSVC\{msvcv}\lib\{target};%~dp0Windows Kits\10\Lib\{sdkv}\ucrt\{target};%~dp0Windows Kits\10\Lib\{sdkv}\um\{target}
";
                    await File.WriteAllTextAsync(
                        Path.Combine(_outputDir, $"setup_{target}.bat"),
                        setup,
                        Encoding.ASCII,
                        cancellationToken).ConfigureAwait(false);
                }

                return true;
            }
            catch (Exception ex)
            {
                AddError(result, "Post-process failed: " + ex.Message, progress);
                return false;
            }
        }

        private async Task<JsonDocument?> TryDownloadJsonAsync(
            string url,
            PortableMsvcResult result,
            IProgress<InstallProgress>? progress,
            CancellationToken cancellationToken)
        {
            var bytes = await TryDownloadBytesWithRetryAsync(url, result, progress, cancellationToken).ConfigureAwait(false);
            if (bytes == null)
                return null;

            try
            {
                return JsonDocument.Parse(bytes);
            }
            catch (Exception ex)
            {
                AddError(result, $"Failed to parse JSON from '{url}': {ex.Message}", progress);
                return null;
            }
        }

        private async Task<byte[]?> TryDownloadBytesWithRetryAsync(
            string url,
            PortableMsvcResult result,
            IProgress<InstallProgress>? progress,
            CancellationToken cancellationToken)
        {
            Exception? lastError = null;
            var retryCount = Math.Max(1, _options.DownloadRetryCount);

            for (int attempt = 1; attempt <= retryCount; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    using var response = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    lastError = ex;
                    AddLog(result, $"Download attempt {attempt}/{retryCount} failed for {url}: {ex.Message}");

                    if (attempt < retryCount)
                        await Task.Delay(_options.DownloadRetryDelayMs, cancellationToken).ConfigureAwait(false);
                }
            }

            AddError(result, $"Failed to download '{url}': {lastError?.Message}", progress);
            return null;
        }

        private async Task<bool> DownloadWithHashAsync(
            string url,
            string sha256,
            string fileName,
            PortableMsvcResult result,
            IProgress<InstallProgress>? progress,
            CancellationToken cancellationToken)
        {
            var path = Path.Combine(_downloadsDir, fileName);
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(dir))
                Directory.CreateDirectory(dir);

            var expected = sha256.Trim().ToLowerInvariant();

            try
            {
                if (File.Exists(path))
                {
                    await using var existingStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                    var existingHashBytes = await SHA256.HashDataAsync(existingStream, cancellationToken).ConfigureAwait(false);
                    var existingHash = Convert.ToHexString(existingHashBytes).ToLowerInvariant();

                    if (existingHash == expected)
                    {
                        AddLog(result, $"{fileName} ... OK (cached)");
                        return true;
                    }

                    SafeDeleteFile(path);
                }
            }
            catch (Exception ex)
            {
                AddLog(result, $"Cache check failed for '{fileName}': {ex.Message}");
                SafeDeleteFile(path);
            }

            Exception? lastError = null;
            var retryCount = Math.Max(1, _options.DownloadRetryCount);

            for (int attempt = 1; attempt <= retryCount; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    using var response = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();

                    var total = response.Content.Headers.ContentLength ?? -1L;
                    await using var input = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

                    {
                        await using var output = new FileStream(
                            path,
                            FileMode.Create,
                            FileAccess.Write,
                            FileShare.None,
                            1024 * 1024,
                            useAsync: true);

                        var buffer = new byte[1024 * 1024];
                        long readTotal = 0;

                        while (true)
                        {
                            var read = await input.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken).ConfigureAwait(false);
                            if (read == 0)
                                break;

                            await output.WriteAsync(buffer.AsMemory(0, read), cancellationToken).ConfigureAwait(false);
                            readTotal += read;

                            if (_options.Verbose && total > 0)
                            {
                                var percent = (int)(readTotal * 100 / total);
                                Console.Write($"\r{fileName} ... {percent}%");
                            }
                        }

                        await output.FlushAsync(cancellationToken).ConfigureAwait(false);
                    }

                    if (_options.Verbose)
                        Console.WriteLine();

                    await using var verifyStream = new FileStream(
                        path,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read,
                        1024 * 1024,
                        useAsync: true);

                    var actualHashBytes = await SHA256.HashDataAsync(verifyStream, cancellationToken).ConfigureAwait(false);
                    var actual = Convert.ToHexString(actualHashBytes).ToLowerInvariant();

                    if (actual != expected)
                    {
                        SafeDeleteFile(path);
                        AddError(result, $"Hash mismatch for {fileName}", progress);
                        return false;
                    }

                    _totalDownloadedBytes += new FileInfo(path).Length;
                    return true;
                }
                catch (Exception ex)
                {
                    lastError = ex;
                    SafeDeleteFile(path);
                    AddLog(result, $"Download attempt {attempt}/{retryCount} failed for '{fileName}': {ex.Message}");

                    if (attempt < retryCount)
                        await Task.Delay(_options.DownloadRetryDelayMs, cancellationToken).ConfigureAwait(false);
                }
            }

            AddError(result, $"Failed to download '{fileName}': {lastError?.Message}", progress);
            return false;
        }

        private static IEnumerable<string> EnumerateMsiCabNames(string msiPath)
        {
            using var fs = new FileStream(msiPath, FileMode.Open, FileAccess.Read, FileShare.Read, 64 * 1024);
            const int chunkSize = 64 * 1024;
            const int overlapSize = 64;

            var buffer = new byte[chunkSize];
            var overlap = Array.Empty<byte>();
            var results = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            while (true)
            {
                var read = fs.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                    break;

                var combined = new byte[overlap.Length + read];
                if (overlap.Length > 0)
                    Buffer.BlockCopy(overlap, 0, combined, 0, overlap.Length);

                Buffer.BlockCopy(buffer, 0, combined, overlap.Length, read);

                foreach (var cab in GetMsiCabNames(combined))
                    results.Add(cab);

                var keep = Math.Min(overlapSize, combined.Length);
                overlap = new byte[keep];
                Buffer.BlockCopy(combined, combined.Length - keep, overlap, 0, keep);
            }

            return results;
        }

        private static bool ValidateHostTarget(string host, List<string> targets, PortableMsvcResult result)
        {
            if (!AllHosts.Contains(host, StringComparer.OrdinalIgnoreCase))
            {
                result.Error($"Unsupported host architecture: {host}");
                return false;
            }

            foreach (var target in targets)
            {
                if (!AllTargets.Contains(target, StringComparer.OrdinalIgnoreCase))
                {
                    result.Error($"Unsupported target architecture: {target}");
                    return false;
                }
            }

            return true;
        }

        private static Dictionary<string, List<JsonElement>> BuildPackageMap(JsonElement packagesArray)
        {
            var map = new Dictionary<string, List<JsonElement>>(StringComparer.OrdinalIgnoreCase);

            foreach (var pkg in packagesArray.EnumerateArray())
            {
                var id = GetStringSafe(pkg, "id").ToLowerInvariant();
                if (string.IsNullOrWhiteSpace(id))
                    continue;

                if (!map.TryGetValue(id, out var list))
                {
                    list = new List<JsonElement>();
                    map[id] = list;
                }

                list.Add(pkg);
            }

            return map;
        }

        private static void BuildVersionMaps(
            Dictionary<string, List<JsonElement>> packages,
            SortedDictionary<string, string> msvc,
            SortedDictionary<string, string> sdk)
        {
            foreach (var pid in packages.Keys)
            {
                if (pid.StartsWith("microsoft.vc.", StringComparison.OrdinalIgnoreCase) &&
                    pid.EndsWith(".tools.hostx64.targetx64.base", StringComparison.OrdinalIgnoreCase) &&
                    !pid.Contains("premium", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = pid.Split('.');
                    if (parts.Length >= 4)
                    {
                        var version = parts[2] + "." + parts[3];
                        if (!string.IsNullOrWhiteSpace(version) && char.IsDigit(version[0]))
                            msvc[version] = pid;
                    }
                }
                else if (pid.StartsWith("microsoft.visualstudio.component.windows10sdk.", StringComparison.OrdinalIgnoreCase) ||
                         pid.StartsWith("microsoft.visualstudio.component.windows11sdk.", StringComparison.OrdinalIgnoreCase))
                {
                    var version = pid.Split('.').LastOrDefault() ?? "";
                    if (version.All(char.IsDigit))
                        sdk[version] = pid;
                }
            }
        }

        private static void RemovePreviewMsvcFromStable(
            Dictionary<string, List<JsonElement>> packages,
            SortedDictionary<string, string> msvc)
        {
            const string previewId = "microsoft.vc.preview.tools.hostx64.targetx64";
            if (!packages.TryGetValue(previewId, out var previewPackages) || previewPackages.Count == 0)
                return;

            var version = GetStringSafe(previewPackages[0], "version");
            if (string.IsNullOrWhiteSpace(version))
                return;

            var shortVersion = string.Join(".", version.Split('.').Take(2));
            if (!string.IsNullOrWhiteSpace(shortVersion))
                msvc.Remove(shortVersion);
        }

        private static JsonElement SelectPackageByLanguage(List<JsonElement> candidates)
        {
            foreach (var c in candidates)
            {
                if (!c.TryGetProperty("language", out var lang) || lang.ValueKind == JsonValueKind.Null)
                    return c;

                var s = lang.GetString();
                if (string.Equals(s, "en-US", StringComparison.OrdinalIgnoreCase))
                    return c;
            }

            return candidates[0];
        }

        private static JsonElement? FindPayloadByFileName(JsonElement package, string expectedFileName)
        {
            try
            {
                foreach (var payload in package.GetProperty("payloads").EnumerateArray())
                {
                    var fileName = GetStringSafe(payload, "fileName");
                    if (string.Equals(fileName, expectedFileName, StringComparison.OrdinalIgnoreCase))
                        return payload;
                }
            }
            catch
            {
            }

            return null;
        }

        private static IEnumerable<string> GetMsiCabNames(byte[] msi)
        {
            var results = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var needle = Encoding.ASCII.GetBytes(".cab");

            for (int i = 0; i <= msi.Length - needle.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < needle.Length; j++)
                {
                    if (msi[i + j] != needle[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (!match)
                    continue;

                var start = Math.Max(0, i - 32);
                var len = Math.Min(36, msi.Length - start);

                string text;
                try
                {
                    text = Encoding.ASCII.GetString(msi, start, len);
                }
                catch
                {
                    continue;
                }

                var index = text.IndexOf(".cab", StringComparison.OrdinalIgnoreCase);
                if (index < 0)
                    continue;

                var value = text.Substring(0, index + 4).Trim('\0', ' ', '\r', '\n', '\t');
                if (value.EndsWith(".cab", StringComparison.OrdinalIgnoreCase))
                    results.Add(value);
            }

            return results;
        }

        private static string GetBuildToolsLicenseUrlSafe(JsonElement channelItems)
        {
            if (!TryFirst(channelItems.EnumerateArray(),
                    x => StringEquals(GetStringSafe(x, "id"), "Microsoft.VisualStudio.Product.BuildTools"),
                    out var buildTools))
                return "";

            try
            {
                foreach (var resource in buildTools.GetProperty("localizedResources").EnumerateArray())
                {
                    if (StringEquals(GetStringSafe(resource, "language"), "en-us"))
                        return GetStringSafe(resource, "license");
                }
            }
            catch
            {
            }

            return "";
        }

        private static bool TryFirst<T>(IEnumerable<T> items, Func<T, bool> predicate, out T value)
        {
            foreach (var item in items)
            {
                if (predicate(item))
                {
                    value = item;
                    return true;
                }
            }

            value = default!;
            return false;
        }

        private static string GetSingleSubdirectoryNameSafe(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                    return "";

                return Directory.GetDirectories(path)
                    .Select(Path.GetFileName)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .OrderByDescending(x => x, StringComparer.OrdinalIgnoreCase)
                    .FirstOrDefault() ?? "";
            }
            catch
            {
                return "";
            }
        }

        private static List<string> ParseTargets(string csv)
        {
            return csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(Normalize)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string Normalize(string value) => (value ?? "").Trim().ToLowerInvariant();

        private static bool StringEquals(string a, string b) =>
            string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

        private static string GetStringSafe(JsonElement element, string propertyName)
        {
            try
            {
                if (!element.TryGetProperty(propertyName, out var prop))
                    return "";

                return prop.GetString() ?? "";
            }
            catch
            {
                return "";
            }
        }

        private void AddLog(PortableMsvcResult result, string message)
        {
            result.Log(message);
            _options.Logger?.Invoke(message);
            if (_options.Verbose)
                Console.WriteLine(message);
        }

        private void AddError(PortableMsvcResult result, string message, IProgress<InstallProgress>? progress)
        {
            result.Error(message);
            _options.Logger?.Invoke("ERROR: " + message);
            if (_options.Verbose)
                Console.WriteLine("ERROR: " + message);
            if(progress != null)
            {
                progress?.Report(new InstallProgress { Message = message });
            }
        }

        private static void SafeDeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
            }
            catch
            {
            }
        }

        private static void SafeDeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            _http.Dispose();
            _msiExtractor.Dispose();
        }

        private sealed class MsiExtractor : IDisposable
        {
            private readonly string _toolsRoot;
            private readonly string _lessMsiDir;
            private readonly PortableMsvcOptions _options;
            private readonly HttpClient _http;

            public MsiExtractor(string downloadsRoot, PortableMsvcOptions options)
            {
                _options = options;
                _toolsRoot = Path.Combine(Path.GetFullPath(downloadsRoot), "tools");
                _lessMsiDir = Path.Combine(_toolsRoot, "lessmsi");

                Directory.CreateDirectory(_toolsRoot);

                _http = new HttpClient
                {
                    Timeout = options.HttpTimeout
                };
            }

            public async Task<PortableMsvcResult> ExtractAsync(string msiPath, string outputDir, CancellationToken cancellationToken)
            {
                var result = new PortableMsvcResult();

                try
                {
                    if (!File.Exists(msiPath))
                    {
                        result.Error("MSI file not found: " + msiPath);
                        return result;
                    }

                    if (HasMsiExec())
                    {
                        result.UsedMsiExec = true;
                        result.Log("Using msiexec for " + Path.GetFileName(msiPath));
                        result.Success = await ExtractWithMsiexecAsync(msiPath, outputDir, result, cancellationToken).ConfigureAwait(false);
                        return result;
                    }

                    result.UsedLessMsiFallback = true;
                    result.Log("msiexec not available, fallback to lessmsi for " + Path.GetFileName(msiPath));

                    var okEnsure = await EnsureLessMsiAsync(result, cancellationToken).ConfigureAwait(false);
                    if (!okEnsure)
                        return result;

                    result.Success = await ExtractWithLessMsiAsync(msiPath, outputDir, result, cancellationToken).ConfigureAwait(false);
                    return result;
                }
                catch (OperationCanceledException)
                {
                    result.Error("MSI extraction canceled.");
                    return result;
                }
                catch (Exception ex)
                {
                    result.Error("MSI extraction failed: " + ex.Message);
                    return result;
                }
            }

            private static bool HasMsiExec()
            {
                try
                {
                    var systemDir = Environment.GetFolderPath(Environment.SpecialFolder.System);
                    var systemPath = Path.Combine(systemDir, "msiexec.exe");
                    if (File.Exists(systemPath))
                        return true;

                    var path = Environment.GetEnvironmentVariable("PATH") ?? "";
                    foreach (var dir in path.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
                    {
                        try
                        {
                            var candidate = Path.Combine(dir.Trim(), "msiexec.exe");
                            if (File.Exists(candidate))
                                return true;
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }

                return false;
            }

            private async Task<bool> ExtractWithMsiexecAsync(
                string msiPath,
                string outputDir,
                PortableMsvcResult result,
                CancellationToken cancellationToken)
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "msiexec.exe",
                        Arguments = $"/a \"{msiPath}\" /quiet /qn TARGETDIR=\"{Path.GetFullPath(outputDir)}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = false,
                        RedirectStandardError = false
                    };

                    using var process = Process.Start(psi);
                    if (process == null)
                    {
                        result.Error("Failed to start msiexec.exe.");
                        return false;
                    }

                    await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);

                    if (process.ExitCode != 0)
                    {
                        result.Error($"msiexec failed for '{Path.GetFileName(msiPath)}' with exit code {process.ExitCode}.");
                        return false;
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    result.Error("msiexec extraction failed: " + ex.Message);
                    return false;
                }
            }

            private async Task<bool> EnsureLessMsiAsync(PortableMsvcResult result, CancellationToken cancellationToken)
            {
                var exePath = Path.Combine(_lessMsiDir, "lessmsi.exe");
                if (File.Exists(exePath))
                    return true;

                Exception? lastError = null;
                var retryCount = Math.Max(1, _options.DownloadRetryCount);

                for (int attempt = 1; attempt <= retryCount; attempt++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        SafeDeleteDirectory(_lessMsiDir);
                        Directory.CreateDirectory(_toolsRoot);

                        var zipPath = Path.Combine(_toolsRoot, "lessmsi.zip");
                        result.Log("Downloading lessmsi...");

                        using var response = await _http.GetAsync(_options.LessMsiUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                        response.EnsureSuccessStatusCode();

                        await using (var output = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 256, useAsync: true))
                        {
                            await using var input = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                            await input.CopyToAsync(output, 1024 * 256, cancellationToken).ConfigureAwait(false);
                        }

                        Directory.CreateDirectory(_lessMsiDir);
                        ZipFile.ExtractToDirectory(zipPath, _lessMsiDir, true);
                        SafeDeleteFile(zipPath);

                        if (!File.Exists(exePath))
                        {
                            result.Error("lessmsi.exe not found after extraction.");
                            return false;
                        }

                        return true;
                    }
                    catch (Exception ex)
                    {
                        lastError = ex;
                        result.Log($"lessmsi download attempt {attempt}/{retryCount} failed: {ex.Message}");

                        if (attempt < retryCount)
                            await Task.Delay(_options.DownloadRetryDelayMs, cancellationToken).ConfigureAwait(false);
                    }
                }

                result.Error("Failed to prepare lessmsi: " + lastError?.Message);
                return false;
            }

            private async Task<bool> ExtractWithLessMsiAsync(
                string msiPath,
                string outputDir,
                PortableMsvcResult result,
                CancellationToken cancellationToken)
            {
                try
                {
                    var exe = Path.Combine(_lessMsiDir, "lessmsi.exe");
                    if (!File.Exists(exe))
                    {
                        result.Error("lessmsi.exe not found.");
                        return false;
                    }

                    var psi = new ProcessStartInfo
                    {
                        FileName = exe,
                        Arguments = $"x \"{msiPath}\" \"{Path.GetFullPath(outputDir)}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    using var process = Process.Start(psi);
                    if (process == null)
                    {
                        result.Error("Failed to start lessmsi.exe.");
                        return false;
                    }

                    var stdoutTask = process.StandardOutput.ReadToEndAsync();
                    var stderrTask = process.StandardError.ReadToEndAsync();

                    await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);

                    var stdout = await stdoutTask.ConfigureAwait(false);
                    var stderr = await stderrTask.ConfigureAwait(false);

                    if (!string.IsNullOrWhiteSpace(stdout))
                        result.Log(stdout.Trim());

                    if (process.ExitCode != 0)
                    {
                        result.Error($"lessmsi failed for '{Path.GetFileName(msiPath)}' with exit code {process.ExitCode}. {stderr}".Trim());
                        return false;
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    result.Error("lessmsi extraction failed: " + ex.Message);
                    return false;
                }
            }

            private static void SafeDeleteDirectory(string path)
            {
                try
                {
                    if (Directory.Exists(path))
                        Directory.Delete(path, true);
                }
                catch
                {
                }
            }

            private static void SafeDeleteFile(string path)
            {
                try
                {
                    if (File.Exists(path))
                        File.Delete(path);
                }
                catch
                {
                }
            }

            public void Dispose()
            {
                _http.Dispose();
            }
        }
    }
}
