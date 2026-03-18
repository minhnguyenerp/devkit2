namespace devkit2.Common
{
    public class DownloadProgress
    {
        public long BytesReceived { get; set; }
        public long? TotalBytes { get; set; }
        public double SpeedBytesPerSecond { get; set; }
        public double ProgressPercentage =>
            TotalBytes.HasValue && TotalBytes.Value > 0
                ? (double)BytesReceived * 100d / TotalBytes.Value
                : 0;
    }
}
