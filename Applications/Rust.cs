using dekit2.Common;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using System.Text.Json.Nodes;

namespace dekit2.Applications
{
    internal sealed class Rust : BaseApplication
    {
        public override string Name => "Rust";

        public Rust()
        {
            appPath = Path.Combine(BaseApplication.LocalApplicationData, "apps", "rust");
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
            }
            base.LoadConfig(appPath);
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
                    new ValueName("1.94.0", "1.94.0"),
                };
            }
        }

        public override bool Install(string version)
        {
            string url = string.Empty;
            string file = string.Empty;
            switch (version)
            {
                case "1.94.0":
                    url = "https://static.rust-lang.org/dist/rust-1.94.0-x86_64-pc-windows-gnu.tar.xz";
                    file = Path.Combine(Path.GetTempPath(), "rust-1.94.0-x86_64-pc-windows-gnu.tar.xz");
                    break;
            }

            if (url != string.Empty && file != string.Empty)
            {
                if (!File.Exists(file))
                {
                    if (!base.Download(url, file))
                    {
                        return false;
                    }
                }

                string extractPath = Path.Combine(appPath, version);
                Directory.CreateDirectory(extractPath);
                using var stream = File.OpenRead(file);
                using var reader = ReaderFactory.OpenReader(stream);

                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        reader.WriteEntryToDirectory(extractPath, new ExtractionOptions
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });
                    }
                }

                if (!IsInstalled(version) && Config != null && Config["InstalledVersions"] != null && Config["InstalledVersions"] is JsonArray)
                {
                    ((JsonArray)Config["InstalledVersions"]).Add(version);
                }
                base.SaveConfig(Config, appPath);

                return true;
            }
            return false;
        }

        public override string GetPaths(string version)
        {
            return Path.Combine(appPath, version, $"rust-{version}-x86_64-pc-windows-gnu", "rustc", "bin") + ";" +
                Path.Combine(appPath, version, $"rust-{version}-x86_64-pc-windows-gnu", "cargo", "bin");
        }

        public override bool Start(string version, string envPath)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            psi.UseShellExecute = false;
            string currentPath = Environment.GetEnvironmentVariable("PATH") ?? "";
            psi.EnvironmentVariables["PATH"] = envPath + ";" + currentPath;
            psi.EnvironmentVariables["RUSTFLAGS"] = "--sysroot=" + Path.Combine(appPath, version, $"rust-{version}-x86_64-pc-windows-gnu", "rust-std-x86_64-pc-windows-gnu");
            psi.EnvironmentVariables["CARGO_HOME"] = Path.Combine(appPath, version, $"rust-{version}-x86_64-pc-windows-gnu", "cargo");
            try
            {
                if (Process.Start(psi) != null)
                {
                    return true;
                }
            }
            catch { return false; }
            return false;
        }

        public override bool Stop(string version)
        {
            return false;
        }
    }
}
