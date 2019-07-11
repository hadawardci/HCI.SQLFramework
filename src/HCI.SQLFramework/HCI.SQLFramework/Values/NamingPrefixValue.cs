namespace HCI.SQLFramework.Values
{
    internal static class NamingPrefixValue
    {
        public static string Key => "Key@";
        public static string WithoutKey(string value) => value.Replace(Key, string.Empty);
        public static string NotMapped => "NotMapped@";
    }
}
