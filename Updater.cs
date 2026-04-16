using devkit2.Applications;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace devkit2
{
    internal class GithubRelease
    {
        public string tag_name { get; set; } = "";
        public List<GithubAsset> assets { get; set; } = new();
    }

    internal class GithubAsset
    {
        public string name { get; set; } = "";
        public string browser_download_url { get; set; } = "";
    }

    internal class Updater
    {
        private Version GetCurrentVersion()
        {
            string? versionText =
                Assembly.GetExecutingAssembly()
                        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                        .InformationalVersion;

            if (!string.IsNullOrWhiteSpace(versionText))
            {
                versionText = versionText.Split('+')[0].Trim().TrimStart('v', 'V');
                if (Version.TryParse(versionText, out var v))
                    return v;
            }

            string fallback = Application.ProductVersion;
            fallback = fallback.Trim().TrimStart('v', 'V');

            if (Version.TryParse(fallback, out var fv))
                return fv;

            return new Version(1, 0, 0, 0);
        }

        private async Task<GithubRelease?> GetLatestReleaseAsync()
        {
            try
            {
                using var client = new HttpClient();

                client.DefaultRequestHeaders.UserAgent.ParseAdd("DevKit2-Updater");
                client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");

                string url = "https://api.github.com/repos/minhnguyenerp/devkit2/releases/latest";

                using var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                string json = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<GithubRelease>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch (TaskCanceledException)
            {
                return null;
            }
            catch (HttpRequestException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private Version ParseVersion(string tag)
        {
            tag = tag.Trim().TrimStart('v', 'V');

            if (Version.TryParse(tag, out var version))
                return version;

            return new Version(0, 0, 0, 0);
        }

        private async Task DownloadFileAsync(string url, string savePath)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("DevKit2-Updater");

            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            await using var fs = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await response.Content.CopyToAsync(fs);
        }

        private void RunInstallerAndExit(string installerPath)
        {
            var psi = new ProcessStartInfo
            {
                FileName = installerPath,
                UseShellExecute = true
            };
            Process.Start(psi);
            Application.Exit();
        }

        private void CleanUpdateFolder(string path)
        {
            if (!Directory.Exists(path)) return;

            foreach (var file in Directory.GetFiles(path, "*.exe"))
            {
                try
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                catch { }
            }
        }

        public async Task CheckForUpdate()
        {
            try
            {
                string updaterPath = Path.Combine(BaseApplication.LocalApplicationData, "updater");
                Directory.CreateDirectory(updaterPath);

                string lastUpdateFile = Path.Combine(updaterPath, "lastupdate.txt");
                if (File.Exists(lastUpdateFile))
                {
                    try
                    {
                        string text = File.ReadAllText(lastUpdateFile);
                        if (DateTimeOffset.TryParse(text, out var lastCheckTime))
                        {
                            if (DateTimeOffset.UtcNow - lastCheckTime < TimeSpan.FromMinutes(20))
                            {
                                return;
                            }
                        }
                    }
                    catch { }
                }

                Version currentVersion = GetCurrentVersion();
                GithubRelease? latestRelease = await GetLatestReleaseAsync();

                try
                {
                    File.WriteAllText(lastUpdateFile, DateTime.UtcNow.ToString("o"));
                }
                catch { }

                if (latestRelease == null)
                    return;

                Version latestVersion = ParseVersion(latestRelease.tag_name);

                if (latestVersion <= currentVersion)
                {
                    CleanUpdateFolder(updaterPath);
                    return;
                }

                var asset = latestRelease.assets.FirstOrDefault(a =>
                    a.name.Contains("Setup", StringComparison.OrdinalIgnoreCase));

                if (asset == null)
                {
                    return;
                }

                string setupFile = Path.Combine(updaterPath, $"DevKit2-{latestVersion}-Setup.exe");
                bool alreadyDownloaded = File.Exists(setupFile);

                if (!alreadyDownloaded)
                {
                    await DownloadFileAsync(asset.browser_download_url, setupFile);
                }

                Application.OpenForms[0]?.Invoke(new Action(() =>
                {
                    var result = MessageBox.Show("Found new version, do you want to update now?", "DevKit2 - Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        RunInstallerAndExit(setupFile);
                    }
                }));
            }
            catch (Exception ex)
            {
                Application.OpenForms[0]?.Invoke(new Action(() =>
                {
                    MessageBox.Show("Check for update failed. " + ex.Message, "DevKit2", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }));
            }
        }
    }
}
