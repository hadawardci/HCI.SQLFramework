using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI.SQLFramework.Values
{
    internal static class NamingPrefixValue
    {
        public static string Key => "Key@";
        public static string WithoutKey(string value) => value.Replace(Key, string.Empty);
        public static string NotMapped => "NotMapped@";
    }
}
