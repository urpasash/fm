using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Ionic.Zip;


namespace project_01_total
{
    public partial class Window : Form
    {
        private SystemIconsImageList sysIcons = new SystemIconsImageList();
        bool foc = true;
        bool hid = true;
        string cop = "";
        string pastPath = "";

        ListView currentListView = new ListView();
        TextBox currentTextBox = new TextBox();

        public Window()
        {
            InitializeComponent();

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void callOutNewFolderFunc()
        {

            try
            {
                string fileAddr = currentTextBox.Text + "Новый файл";
                if (Directory.Exists(fileAddr))
                {
                    int i = 1;
                    for (; Directory.Exists(fileAddr + " (" + i.ToString() + ")"); i++)
                    {
                    };
                    fileAddr += " (" + i.ToString() + ")";

                }
                Directory.CreateDirectory(fileAddr);
                refreshListview();
            }
            catch
            {
                MessageBox.Show("Fail");
                
            }
            
        }

        private void newFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            callOutNewFolderFunc();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DriveInfo[] drives = DriveInfo.GetDrives();

            LoadDisk(drives, comboBox1);
            LoadDisk(drives, comboBox2);
            currentListView = listView1;
            currentTextBox = textBox1;
            LoadInfo(listView1, textBox1);
            LoadInfo(listView2, textBox2);

        }
        private void LoadDisk(DriveInfo[] drives, ComboBox comboBox)
        {
            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady)
                    comboBox.Items.Add(drive.Name);
            }
            comboBox.SelectedIndex = 0;
        }

        private void LoadInfo(ListView listView, TextBox textBox)
        {
            listView.Items.Clear();
            pastPath = textBox.Text;

            try
            {
                string[] ourDir = Directory.GetDirectories(textBox.Text);
                if (textBox.Text != Path.GetPathRoot(textBox.Text))
                    backFolder(listView);
                foreach (string folder in ourDir)
                {
                    if (hid)
                    {
                        if ((new FileInfo(folder).Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                        {

                            string[] array = new string[5];
                            array[0] = Path.GetFileName(folder);
                            array[1] = "";
                            array[2] = "<DIR>";
                            array[3] = File.GetCreationTime(folder).ToString();

                            ListViewItem item = new ListViewItem(array);
                            item.ImageIndex = 0;
                            listView.Items.Add(item);
                        }
                    }
                    else
                    {
                        string[] array = new string[5];
                        array[0] = Path.GetFileName(folder);
                        array[1] = "";
                        array[2] = "<DIR>";
                        array[3] = File.GetCreationTime(folder).ToString();
                        ListViewItem item = new ListViewItem(array);
                        item.ImageIndex = 0;
                        listView.Items.Add(item);
                    }
                }
                ourDir = Directory.GetFiles(textBox.Text);
                foreach (string files in ourDir)
                {
                    if (hid)
                    {
                        if ((new FileInfo(files).Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                        {
                            string[] array = new string[4];
                            array[0] = Path.GetFileNameWithoutExtension(files);
                            array[1] = Path.GetExtension(files);
                            double size = new FileInfo(files).Length;
                            array[2] = size.ToString();
                            array[3] = File.GetCreationTime(files).ToString();

                            ListViewItem item = new ListViewItem(array);
                            imageList1.Images.Add(files, Icon.ExtractAssociatedIcon(files));
                            item.ImageIndex = imageList1.Images.Keys.IndexOf(files);
                            listView.Items.Add(item);
                        }
                    }
                    else
                    {
                        string[] array = new string[4];
                        array[0] = Path.GetFileNameWithoutExtension(files);
                        array[1] = Path.GetExtension(files);
                        double size = new FileInfo(files).Length;
                        array[2] = size.ToString();
                        array[3] = File.GetCreationTime(files).ToString();

                        ListViewItem item = new ListViewItem(array);
                        imageList1.Images.Add(files, Icon.ExtractAssociatedIcon(files));
                        item.ImageIndex = imageList1.Images.Keys.IndexOf(files);
                        listView.Items.Add(item);
                    }
                }
            }
            catch
            {
                LoadInfo(listView, textBox);
            }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.Text = comboBox1.Text;
            LoadInfo(listView1, textBox1);
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            foc = false;
        }
        private void backFolder(ListView listView)
        {
            ListViewItem item = new ListViewItem("..");
            item.ImageIndex = 1;
            listView.Items.Add(item);
        }
   
        private void selectItem(ListView listView, TextBox textBox)
        {
            try
            {
                int pos = listView.SelectedIndices[0];
                if (pos == 0 && listView.SelectedItems[0].Text == "..")
                {
                    try
                    {
                        if (pastPath != null)
                        {
                            DirectoryInfo di = new DirectoryInfo(textBox.Text);
                            if (di.Parent.FullName != Path.GetPathRoot(textBox.Text))
                                textBox.Text = di.Parent.FullName + (char)92;
                            else textBox.Text = di.Parent.FullName;
                            LoadInfo(listView, textBox);
                        }
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        string addr = getPathFileExt(textBox, listView, 0);
                        if (File.Exists(addr))
                            selectFiles(listView, textBox);
                        else
                            textBox.Text += listView.Items[pos].Text + (char)92;
                        LoadInfo(listView, textBox);
                    }
                    catch
                    {
                        MessageBox.Show("Error");
                    }
                }
            }
            catch
            {
                MessageBox.Show("Выберете другой файл");
            }

        }

        private string getPathFileExt(TextBox textBox, ListView listView, int index)
        {
            return textBox.Text + listView.SelectedItems[index].Text + listView.SelectedItems[index].SubItems[1].Text;
        }

        private void selectFiles(ListView listView, TextBox textBox)
        {
            Process.Start(textBox.Text + listView.SelectedItems[0].Text + listView.SelectedItems[0].SubItems[1].Text);
        }
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            selectItem(listView1, textBox1);
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            foc = true;
            currentListView = listView1;
            currentTextBox = textBox1;
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    if (File.Exists(getPathFileExt(textBox1, listView1, 0)))
                    {
                        setContextMenu("0");
                    }
                    else if (Directory.Exists(getPathFileExt(textBox1, listView1, 0)))
                    {
                        setContextMenu("1");
                    }

                }
                catch
                {
                    setContextMenu("2");
                }
            }
        }

        private void listView2_MouseClick(object sender, MouseEventArgs e)
        {
            foc = false;
            currentListView = listView2;
            currentTextBox = textBox2;
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    if (File.Exists(getPathFileExt(textBox2, listView2, 0)))
                    {
                        setContextMenu("0");
                    }
                    else if (Directory.Exists(getPathFileExt(textBox2, listView2, 0)))
                    {
                        setContextMenu("1");
                    }

                }
                catch
                {
                    setContextMenu("2");
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            foc = true;
        }


        private void comboBox2_TextChanged_1(object sender, EventArgs e)
        {
            textBox2.Text = comboBox2.Text;
            LoadInfo(listView2, textBox2);
        }

        private void listView2_DoubleClick(object sender, EventArgs e)
        {
            selectItem(listView2, textBox2);

        }

        private void detailButton_Click(object sender, EventArgs e)
        {
            currentListView.View = View.Details;
        }

        private void viewToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string strFilePath = currentTextBox.Text
                              + currentListView.SelectedItems[0].SubItems[0].Text
                              + currentListView.SelectedItems[0].SubItems[1].Text;

            if (File.Exists(strFilePath))
            {
                FileInfo oFileInfo = new FileInfo(strFilePath);
                MessageBox.Show("Name: " + oFileInfo.Name + "\nLength: " + oFileInfo.Length.ToString() + " Bytes", "Properties");
            }
            else
            {
                DirectoryInfo oDirInfo = new DirectoryInfo(strFilePath);
                long filesLength = DirectoryLength(strFilePath);
                int dirCount = DirectoriesCount(strFilePath);
                MessageBox.Show("Name: " + oDirInfo.Name + "\nLength: " + filesLength.ToString() + " Bytes" + "\nDirectories: " + dirCount.ToString() + "  |  Files: " + FilesCount(strFilePath), "Properties");
            }
        }

        private void listButton_Click(object sender, EventArgs e)
        {
            currentListView.View = View.List;
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            refreshListview();
        }

        private void refreshListview()
        {
            Refresh();
            LoadInfo(listView1, textBox1);
            LoadInfo(listView2, textBox2);
        }

        private bool askBeforeCopy(string fileName)
        {

            DialogResult text;
            text = MessageBox.Show("Вы хотите скопировать файл" + $" {fileName} ?", "Copy", MessageBoxButtons.YesNo);
            if (text == DialogResult.Yes)
                return true;

            return false;
        }

        private short formCopyFunc(string sourcePath, int pos)
        {

                short index = 0;

                string destinationPath = "";
                string text = "";
                if (textBox1.Text == textBox2.Text)
                    MessageBox.Show("Это тоже место");
                else
                {
                    if (foc)
                    {
                        text = textBox2.Text;
                        destinationPath = textBox2.Text + Path.GetFileName(sourcePath);
                    }
                    else
                    {
                        text = textBox1.Text;
                        destinationPath = textBox1.Text + Path.GetFileName(sourcePath);
                    }

                }
                return index;   
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentListView.SelectedItems.Count > 0)
            {
                int index = currentListView.SelectedItems.Count - 1;
                string sourcePath = currentTextBox.Text;
                while (index != -1)
                {
                    if (currentListView.SelectedItems[index].Text != "..")
                    {
                        string fileName = currentListView.SelectedItems[index].Text + currentListView.SelectedItems[index].SubItems[1].Text;
                        if (askBeforeCopy(fileName))
                        {
                            formCopyFunc(sourcePath + fileName, index);
                        }
                    }
                    index--;
                }
                refreshListview();
            }
            else MessageBox.Show("Выберете файл");
        }

        private bool IsFolderEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }
        private bool askBeforeDelete(string path, string fileName)
        {
            DialogResult text;
            if (File.Exists(path))
            {
                text = MessageBox.Show("Вы хотите удалить файл " + $"{fileName} ?", "Delete", MessageBoxButtons.YesNo);
                if (text == DialogResult.Yes)
                    return true;
            }
            else if (!IsFolderEmpty(path))
            {
                text = MessageBox.Show("Вы хотите удалить файл " + $"{ fileName} ?", "Delete", MessageBoxButtons.YesNo);
                if (text == DialogResult.Yes)
                    return true;
            }
            else
            {
                text = MessageBox.Show("Вы хотите удалить файл " + $" {fileName} ?", "Delete", MessageBoxButtons.YesNo);
                if (text == DialogResult.Yes)
                    return true;
            }
            return false;
        }
        private void deleteFunc(string path)
        {

            if (File.Exists(path))
                File.Delete(path);
            else
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
                Directory.Delete(path);
            }
        }

        private void callOutDeleteFunc()
        {
            if (currentListView.SelectedItems.Count > 0)
            {
                int index = currentListView.SelectedItems.Count - 1;

                string sourcePath = currentTextBox.Text;
                while (index != -1)
                {
                    if (currentListView.SelectedItems[index].Text != "..")
                    {
                        string fileName = currentListView.SelectedItems[index].Text + currentListView.SelectedItems[index].SubItems[1].Text;
                        string path = getPathFileExt(currentTextBox, currentListView, index);                   
                        if (askBeforeDelete(path, fileName))
                        {
                            deleteFunc(path);

                        }
                    }
                    index--;
                }
                refreshListview();
            }
            else
                MessageBox.Show("Выберете файл");

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            callOutDeleteFunc();
        }

        private long DirectoryLength(string path)
        {
            long filesLength = 0;
            try
            {
                DirectoryInfo fDir = new DirectoryInfo(path);
                foreach (FileInfo file in fDir.GetFiles("*.*", SearchOption.AllDirectories))
                {
                    filesLength += file.Length;
                }
            }
            catch (Exception exc) { MessageBox.Show(exc.Message); }
            return filesLength;
        }
        private int DirectoriesCount(string path)
        {
            int dirCount = 0;
            try
            {
                DirectoryInfo fDir = new DirectoryInfo(path);
                foreach (DirectoryInfo dir in fDir.GetDirectories("*.*", SearchOption.AllDirectories))
                {
                    dirCount++;
                }
            }
            catch { }
            return dirCount;
        }
        private int FilesCount(string path)
        {
            int filesCount = 0;
            try
            {
                DirectoryInfo fDir = new DirectoryInfo(path);
                foreach (FileInfo file in fDir.GetFiles("*.*", SearchOption.AllDirectories))
                {
                    filesCount++;
                }
            }
            catch { }
            return filesCount;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                LoadInfo(listView1, textBox1);
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                LoadInfo(listView2, textBox2);
        }

        private void reName(string path, string newName)
        {
            if (currentListView.SelectedItems.Count == 1)
            {
                string oldName = currentListView.SelectedItems[0].Text.ToString();

                string newPath = Path.Combine(path, newName);
                string oldPath = getPathFileExt(currentTextBox, currentListView, 0);

                if (newName != oldName)
                {
                    if (File.Exists(oldPath))
                        File.Move(oldPath, newPath + currentListView.SelectedItems[0].SubItems[1].Text);
                    else if (Directory.Exists(oldPath))
                        Directory.Move(oldPath, newPath);
                }
                else
                {
                    MessageBox.Show("Это старое имя");
                }

            }
        }

        private void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            try
            {
                if (e.Label == "" || e.Label == "..")
                {
                    e.CancelEdit = true;
                    MessageBox.Show("Недействительное имя");
                }
                else
                {
                    reName(textBox1.Text, e.Label.ToString());
                    refreshListview();
                }
            }
            catch
            {
                e.CancelEdit = true;

            }

        }

        private void listView2_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            try
            {
                if (e.Label == ".." || e.Label == "")
                {
                    e.CancelEdit = true;
                    MessageBox.Show("Недействительное имя");
                }
                else
                {
                    reName(textBox2.Text, e.Label.ToString());
                    refreshListview();
                }
            }
            catch
            {
                e.CancelEdit = true;
            }
        }

        private void viewToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            view.PerformClick();
        }


        private void deleteToolStripMenuItem1_Click_2(object sender, EventArgs e)
        {
            delete.PerformClick();
        }

        private void newFolderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            newFolder.PerformClick();
        }

        private void moveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentListView.SelectedItems.Count > 0)
            {
                int index = currentListView.SelectedItems.Count - 1;
                string sourcePath = currentTextBox.Text;
                while (index != -1)
                {
                    if (currentListView.SelectedItems[index].Text != "..")
                    {
                        string fileName = currentListView.SelectedItems[index].Text + currentListView.SelectedItems[index].SubItems[1].Text;
                        if (askBeforeCopy(fileName))
                        {
                            formCopyFunc(sourcePath + fileName, index);
                        }
                    }
                    index--;
                }
                refreshListview();
            }
            else MessageBox.Show("Выьерете файл");

        }

        private void listView1_BeforeLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (currentListView.SelectedItems[0].Text == "..")
                e.CancelEdit = true;
        }

        private void renameToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (foc)
                {
                    if (listView1.SelectedItems.Count == 1 && listView1.SelectedItems[0].Text.ToString() != "[..]")
                        listView1.SelectedItems[0].BeginEdit();
                }
                else
                {
                    if (listView2.SelectedItems.Count == 1 && listView2.SelectedItems[0].Text.ToString() != "[..]")
                        listView2.SelectedItems[0].BeginEdit();
                }
            }
            catch
            {
                MessageBox.Show("Нельзя переименовать");
            }
        }

        private void setContextMenu(string type)
        {
            if (type == "0")
            {
                viewToolStrip.Enabled = true;
                copyToolStrip.Enabled = true;
                renameToolStrip.Enabled = true;
                deleteToolStrip.Enabled = true;
                newFolderToolStrip.Enabled = false;
            }
            else if (type == "1")
            {
                viewToolStrip.Enabled = true;
                copyToolStrip.Enabled = true;
                renameToolStrip.Enabled = true;
                deleteToolStrip.Enabled = true;
                newFolderToolStrip.Enabled = false;
            }
            else if (type == "2")
            {
                viewToolStrip.Enabled = false;
                copyToolStrip.Enabled = false;
                renameToolStrip.Enabled = false;
                deleteToolStrip.Enabled = false;
                newFolderToolStrip.Enabled = true;
            }
        }

        private void listView1_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    if (File.Exists(getPathFileExt(textBox1, listView1, 0)))
                    {
                        setContextMenu("0");
                    }
                    else if (Directory.Exists(getPathFileExt(textBox1, listView1, 0)))
                    {
                        setContextMenu("1");
                    }

                }
                catch
                {
                    if (cop == "0" || cop == "1")
                    {
                        viewToolStrip.Enabled = false;
                        copyToolStrip.Enabled = false;
                        renameToolStrip.Enabled = false;
                        deleteToolStrip.Enabled = false;
                        newFolderToolStrip.Enabled = true;
                    }
                    else
                        setContextMenu("2");
                }
            }
        }

        private void listView2_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    if (File.Exists(getPathFileExt(textBox2, listView2, 0)))
                    {
                        setContextMenu("0");
                    }
                    else if (Directory.Exists(getPathFileExt(textBox2, listView2, 0)))
                    {
                        setContextMenu("1");
                    }

                }
                catch
                {

                    if (cop == "0" || cop == "1")
                    {
                        viewToolStrip.Enabled = false;
                        copyToolStrip.Enabled = false;
                        renameToolStrip.Enabled = false;
                        deleteToolStrip.Enabled = false;
                        newFolderToolStrip.Enabled = true;
                    }
                    else
                        setContextMenu("2");
                }
            }
        }

        private async void listView1_DragDrop(object sender, DragEventArgs e)
        {
            Clipboard.SetData(DataFormats.FileDrop, e.Data.GetData(DataFormats.FileDrop));

            foreach (string file in Clipboard.GetFileDropList())
            {
                string fileName = Path.GetFileName(file);
                if (File.Exists(file))
                {
                    await Task.Run(() => File.Copy(file, textBox1.Text + fileName));
                }
                else
                {
                    Directory.CreateDirectory(textBox1.Text + Path.GetFileName(file));
                    await DirectoryCopy(file, textBox1.Text + Path.GetFileName(file));
                }
                
            }
            refreshListview();
        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private async Task DirectoryCopy(string src, string dstn)
        {
            DirectoryInfo srcDir = new DirectoryInfo(src);
            DirectoryInfo dstnDir = new DirectoryInfo(dstn);
            Directory.CreateDirectory(dstn);
            foreach (DirectoryInfo dir in srcDir.GetDirectories())
            {
                await DirectoryCopy(dir.FullName, Path.Combine(dstn, dir.Name));
                if (textBox1.Text == dir.FullName)
                {
                    LoadInfo(listView1, textBox1);
                }
                if (textBox2.Text == dir.FullName)
                {
                    LoadInfo(listView2, textBox2);
                }
            }
            foreach (FileInfo file in srcDir.GetFiles())
            {
                await Task.Run(() => file.CopyTo(Path.Combine(dstnDir.FullName, file.Name), false));
            }
        }

        private async void listView2_DragDrop(object sender, DragEventArgs e)
        {
            Clipboard.SetData(DataFormats.FileDrop, e.Data.GetData(DataFormats.FileDrop));

            foreach (string file in Clipboard.GetFileDropList())
            {
                string fileName = Path.GetFileName(file);
                Directory.CreateDirectory(textBox2.Text + Path.GetFileName(file));
                await DirectoryCopy(file, textBox2.Text + Path.GetFileName(file));
            }
            refreshListview();
        }

        private void listView2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }
        private async Task Compress()
        {
            try
            {
                using (ZipFile zip = new ZipFile())
                {
                    zip.AlternateEncodingUsage = ZipOption.Always;
                    zip.AlternateEncoding = Encoding.GetEncoding(866);
                    foreach (ListViewItem item in currentListView.SelectedItems)
                    {
                        string path = currentTextBox.Text + item.SubItems[0].Text + item.SubItems[1].Text;
                        if (File.Exists(path))
                        {
                            await Task.Run(() => zip.AddFile(path, ""));
                        }
                        else
                        {

                            await Task.Run(() => zip.AddDirectory(path, Path.GetFileName(path)));
                        }
                    }
                    ArchiveForm archiveForm = new ArchiveForm(zip, currentTextBox,currentListView);
                    archiveForm.ShowDialog();
                    LoadInfo(currentListView, currentTextBox);

                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private async void compressToolStripMenuItem_Click(object sender, EventArgs e)
        {
           await Compress();
        }
    }
}
