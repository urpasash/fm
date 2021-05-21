using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace project_01_total
{
    public partial class Info : Form
    {
        string strFrm = "";
        public Info(string str, string name)
        {
            InitializeComponent();
            strFrm = str;
            Text = name;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = strFrm;
        }
    }
}
