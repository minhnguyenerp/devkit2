namespace dekit2.Common
{
    public class ValueName
    {
        public string Value { get; set; }
        public string Name { get; set; }

        public ValueName(string value, string name)
        {
            Value = value;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
