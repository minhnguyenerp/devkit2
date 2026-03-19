using System.IO.Compression;

namespace devkit2.Common
{
    public class ZipHelper
    {
        public static bool ExtractSelectedFiles(string zipPath, string destinationFolder, IEnumerable<string> allowedFiles, out string error)
        {
            error = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(zipPath))
                {
                    error = "zipPath is empty";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(destinationFolder))
                {
                    error = "destinationFolder is empty";
                    return false;
                }

                if (!File.Exists(zipPath))
                {
                    error = "Zip file not found";
                    return false;
                }

                Directory.CreateDirectory(destinationFolder);

                var allowedSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var file in allowedFiles)
                {
                    if (string.IsNullOrWhiteSpace(file))
                        continue;

                    var normalized = file.Replace('\\', '/').TrimStart('/');
                    allowedSet.Add(normalized);
                }

                var destinationRoot = Path.GetFullPath(destinationFolder);
                if (!destinationRoot.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    destinationRoot += Path.DirectorySeparatorChar;

                using var archive = ZipFile.OpenRead(zipPath);

                foreach (var entry in archive.Entries)
                {
                    if (string.IsNullOrEmpty(entry.Name))
                        continue;

                    var entryPath = entry.FullName.Replace('\\', '/').TrimStart('/');

                    if (!allowedSet.Contains(entryPath))
                        continue;

                    var outputPath = Path.GetFullPath(Path.Combine(destinationFolder, entryPath));

                    // Zip Slip check
                    if (!outputPath.StartsWith(destinationRoot, StringComparison.OrdinalIgnoreCase))
                    {
                        error = $"Invalid path: {entry.FullName}";
                        return false;
                    }

                    var outputDir = Path.GetDirectoryName(outputPath);
                    if (!string.IsNullOrEmpty(outputDir))
                        Directory.CreateDirectory(outputDir);

                    entry.ExtractToFile(outputPath, overwrite: true);
                }

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}
