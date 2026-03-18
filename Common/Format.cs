namespace devkit2.Common
{
    internal class Format
    {
        public static string FormatSize(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB" };
            double size = bytes;
            int unit = 0;

            while (size >= 1024 && unit < units.Length - 1)
            {
                size /= 1024;
                unit++;
            }

            return $"{size:F1} {units[unit]}";
        }

        public static string FormatSpeed(double bytesPerSecond)
        {
            string[] units = { "B/s", "KB/s", "MB/s", "GB/s" };
            double speed = bytesPerSecond;
            int unit = 0;

            while (speed >= 1024 && unit < units.Length - 1)
            {
                speed /= 1024;
                unit++;
            }

            return $"{speed:F1} {units[unit]}";
        }
    }
}
