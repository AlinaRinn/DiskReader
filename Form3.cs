using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiskReader
{
    public partial class Form3 : Form
    {
        public Form3(Form2 f1)
        {
            InitializeComponent();
        }
        public string tmp;
        public void button1_Click(object sender, EventArgs e)
        {
            tmp = textBox2.Text;
            this.Close();
        }
    }
}
