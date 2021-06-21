using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DiskReader
{
    public partial class Form8 : Form
    {
        public Form8()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RegistryKey currentUserKey = Registry.CurrentUser;
            RegistryKey Key = currentUserKey.CreateSubKey(textBox1.Text.ToString());
            Key.SetValue(textBox2.Text.ToString(), textBox3.Text.ToString());
            Key.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RegistryKey currentUserKey = Registry.CurrentUser;
            currentUserKey.DeleteSubKey(textBox1.Text.ToString());
        }
    }
}
