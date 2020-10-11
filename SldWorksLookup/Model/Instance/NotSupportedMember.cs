namespace SldWorksLookup.Model
{
    public struct NotSupportedMember
    {
        public NotSupportedMember(string name, PropertyClsfi propertyClsfi, string msg)
        {
            Name = name;
            PropertyClsfi = propertyClsfi;
            Msg = msg;
        }

        public string Name { get; set; }

        public PropertyClsfi PropertyClsfi { get; set; }

        public string Msg { get; set; }

        public bool IsVaildObject()
        {
            return !string.IsNullOrEmpty(Name);
        }
    }
}
