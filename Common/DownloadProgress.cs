namespace devkit2.Common
{
    public class InstallProgress
    {
        public string Message { get; set; } = string.Empty;
        public long BytesReceived { get; set; } = 0;
        public long? TotalBytes { get; set; } = null;
        public double SpeedBytesPerSecond { get; set; }
        public double ProgressPercentage =>
            TotalBytes.HasValue && TotalBytes.Value > 0
                ? (double)BytesReceived * 100d / TotalBytes.Value
                : 0;
        public string ProgressText
        {
            get
            {
                if (Message != string.Empty)
                {
                    return Message;
                }
                else
                {
                    string downloaded = Format.FormatSize(BytesReceived);
                    string total = TotalBytes.HasValue ? Format.FormatSize(TotalBytes.Value) : "?";
                    string speed = Format.FormatSpeed(SpeedBytesPerSecond);
                    return $"{ProgressPercentage:F2}% ({downloaded}/{total}) - {speed}";
                }    
            }
        }
    }
}
