namespace devkit2.Common
{
    public class RunningApplication
    {
        public int Pid { get; set; }
        public int Sessionid { get; set; }
        public DateTime StartTime { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public string UniqueCode { get; set; } = string.Empty;

        public override bool Equals(object obj)
        {
            if (obj is not RunningApplication other)
                return false;

            return Pid == other.Pid &&
                   Sessionid == other.Sessionid &&
                   ProcessName == other.ProcessName &&
                   StartTime == other.StartTime;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Pid, Sessionid, ProcessName, StartTime);
        }
    }
}
