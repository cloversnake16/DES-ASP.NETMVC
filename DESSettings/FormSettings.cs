using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DESSettings
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
        }

        private void buttonSet_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(textUserName.Text.Trim()) || string.IsNullOrEmpty(textPassword.Text.Trim()))
            {
                MessageBox.Show("Required field.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            WriteRegistryKey("Software\\DES", "UserName", textUserName.Text.Trim());
            WriteRegistryKey("Software\\DES", "Password", textPassword.Text.Trim());
        }

        private string ReadRegistryKey(string subKeyPath, string key)
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

        private bool WriteRegistryKey(string subKeyPath, string key, string value)
        {
            try
            {
                RegistryKey baseRegistry = Registry.LocalMachine;
                RegistryKey subKey = baseRegistry.CreateSubKey(subKeyPath);
                subKey.SetValue(key, value);
                return true;
            }
            catch { }
            return false;
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            textUserName.Text = ReadRegistryKey("Software\\DES", "UserName");
        }
    }
}
