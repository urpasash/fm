using System;
using System.Windows.Forms;
using Ionic.Zip;

namespace project_01_total
{
    public partial class ArchiveForm : Form
    {
        private string text;
        private ZipFile zip;
        private TextBox mainTextBox;
        private ListView listView;
        public ArchiveForm(ZipFile zip, TextBox textBox, ListView listView)
        {
            this.zip = zip;
            mainTextBox = textBox;
            this.listView = listView;
            InitializeComponent();
        }
        private void saveButton_Click(object sender, EventArgs e)
        {
            text = textBox.Text;
            zip.Save(mainTextBox.Text + text +".zip");
            Close();
        }

    }
}
