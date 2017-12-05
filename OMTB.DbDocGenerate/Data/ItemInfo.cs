namespace DbDocGenerate.Data
{
    public class ItemInfo
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Style { get; set; }
        public ItemInfo()
        {
        }
        public ItemInfo(string name, string value)
        {
            Name = name;
            Value = value;
        }
        public ItemInfo(string name, string value, string style)
        {
            Name = name;
            Value = value;
            Style = style;
        }

        public override string ToString()
        {
            return "<span style=\"color:" + Style + "\">" + Name + "</span>";
        }
    }
}