using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace DESNotification.Helpers
{
    public class RegistryHelper
    {
        public static string ReadRegistryKey(string subKeyPath, string key)
        {
            try
            {
                RegistryKey baseRegistry = Registry.LocalMachine;
                RegistryKey subKey = baseRegistry.OpenSubKey(subKeyPath);
                object value = subKey.GetValue(key);
                if (value != null) return value.ToString();
            }
            catch { }
            return null;
        }
    }
}
