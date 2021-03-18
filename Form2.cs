using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace DiskReader
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            PopulateTreeView();
            this.treeView1.NodeMouseClick += new TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
        }

        private void diskInfoToolStripMenuItem_Click(object sender, EventArgs e)                                                                                    // Open DiskInfo
        {
            Form1 Form = new Form1();
            Form.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)                                                                                      // Open About
        {
            Form4 Form = new Form4();
            Form.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)                                                                                                        // Close form2
        {
            this.Close();
        }

        public string path;
        private void PopulateTreeView()                                                                                                                            // Create Tree
        {
            TreeNode rootNode;
            DirectoryInfo info = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
            path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info.GetDirectories(), rootNode);
                treeView1.Nodes.Add(rootNode);
            }
        }

        private void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo)                                                                                // Fulling Tree
        {
            this.Cursor = Cursors.WaitCursor;
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
            foreach (DirectoryInfo subDir in subDirs)
            {
                aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "folder";
                try
                {
                    subSubDirs = subDir.GetDirectories();
                    if (subSubDirs.Length != 0)
                    {
                        GetDirectories(subSubDirs, aNode);
                    }
                    nodeToAddTo.Nodes.Add(aNode);
                }
                catch (System.UnauthorizedAccessException)
                {
                    MessageBox.Show("System: Access denied", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            this.Cursor = Cursors.Default;
        }

        void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)                                                                              // For Node's use
        {
            TreeNode newSelected = e.Node;
            listView1.Items.Clear();
            DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;
            //int i = 0;
            //List<string> listpath = new List<string>();
            foreach (DirectoryInfo dir in nodeDirInfo.GetDirectories())
            {
                item = new ListViewItem(dir.Name, 0);
                subItems = new ListViewItem.ListViewSubItem[] {new ListViewItem.ListViewSubItem(item, "Directory"), new ListViewItem.ListViewSubItem(item, dir.LastAccessTime.ToShortDateString())};
                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
                //listpath.Add(dir.FullName.ToString());                                                                                                         // Fulling listView
                //i++;
            }
            foreach (FileInfo file in nodeDirInfo.GetFiles())
            {
                item = new ListViewItem(file.Name, 1);
                subItems = new ListViewItem.ListViewSubItem[] { new ListViewItem.ListViewSubItem(item, "File"), new ListViewItem.ListViewSubItem(item, file.LastAccessTime.ToShortDateString()) };
                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
                //string filepath = Path.GetFullPath(item.Text);
                //MessageBox.Show(filepath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //int[] test = listView1.SelectedListView;
            }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void button_refresh_Click(object sender, EventArgs e)                                                                                            // Refresh Tree
        {
            this.Cursor = Cursors.WaitCursor; 
            DirectoryInfo info = new DirectoryInfo(path);
            treeView1.Nodes.Clear();
            TreeNode rootNode;
            rootNode = new TreeNode(info.Name);
            rootNode.Tag = info;
            GetDirectories(info.GetDirectories(), rootNode);
            treeView1.Nodes.Add(rootNode);
            this.Cursor = Cursors.Default;
        }

        private void button_set_Click(object sender, EventArgs e)                                                                                              // Set custom path
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.ShowDialog();
            path = folder.SelectedPath.ToString();
            if (path == "")
            {
                MessageBox.Show("Set path", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            button_refresh_Click(sender, e);
        }

        public string selectedNodeText;
        public string sourcedir;
        private void button_rename_Click(object sender, EventArgs e)                                                                                          // Renaming
        {
            if (treeView1.SelectedNode != null)
            {
                TreeNode node = this.treeView1.SelectedNode as TreeNode;
                selectedNodeText = node.Text;
                this.treeView1.LabelEdit = true;
                node.BeginEdit();
            }
            else
            {
                MessageBox.Show("Choose folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void treeView1_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                sourcedir = path + @"..\" + @"..\" + treeView1.SelectedNode.FullPath.ToString();
            }
            else
            {
                MessageBox.Show("Choose folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }


        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode as TreeNode;
            this.treeView1.LabelEdit = false;

            if (e.Label.IndexOfAny(new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' }) != -1)                                                           // Check 1
            {
                MessageBox.Show("Invalid Name.\n" + "The step Name must not contain " + "following characters:\n \\ / : * ? \" < > |", "Edit Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (string.IsNullOrWhiteSpace(e.Label))                                                                                                         // Check 2
            {
                MessageBox.Show("Invalid name", "Error");
                e.CancelEdit = true;
                return;
            }
            else
            {
                //string label = (!string.IsNullOrEmpty(e.Label) ? e.Label : selectedNodeText);
                if (null != e.Label)
                {
                    try
                    {
                        //MessageBox.Show(sourcedir, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        DirectoryInfo di = new DirectoryInfo(sourcedir);
                        string sous = sourcedir + @"..\" + @"..\" + @"\" + e.Label;
                        //MessageBox.Show(sous, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        di.MoveTo(sous);
                    }
                    catch (IOException er)
                    {
                        MessageBox.Show(er.Message);
                    }
                }
            }
        }

        private void button_delete_Click(object sender, EventArgs e)                                                                                           // Delete
        {
            if (treeView1.SelectedNode != null) {
                string sourcedir = path + @"..\" + @"..\" + treeView1.SelectedNode.FullPath.ToString();
            }
            else
            {
                MessageBox.Show("Choose folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                Directory.Delete(sourcedir, true);
            }
            catch (System.IO.IOException)
            {
                MessageBox.Show("Deleting Error: Incorrect path or access denied", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            button_refresh_Click(sender, e);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)                                                        // Copy
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();     
            Directory.CreateDirectory(destDirName);
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }

        private void button_copy_Click(object sender, EventArgs e)                                                                                            // Copy
        {
            if (treeView1.SelectedNode != null)
            {
                sourcedir = path + @"..\" + @"..\" + treeView1.SelectedNode.FullPath.ToString();
            }
            else
            {
                MessageBox.Show("Choose folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FolderBrowserDialog folder = new FolderBrowserDialog();
            MessageBox.Show("Choose destination folder", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            folder.ShowDialog();
            string dest = folder.SelectedPath.ToString();
            DirectoryCopy(sourcedir, dest, true);
            button_refresh_Click(sender, e);
        }

        private void button_new_Click(object sender, EventArgs e)                                                                                            // New
        {
            if (treeView1.SelectedNode != null)
            {
                sourcedir = path + @"..\" + @"..\" + treeView1.SelectedNode.FullPath.ToString() + @"\New folder";
            }
            else
            {
                MessageBox.Show("Choose folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {      
                DirectoryInfo di = new DirectoryInfo(sourcedir);
                di.Create();
            }
            catch (IOException er)
            {
                MessageBox.Show(er.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            button_refresh_Click(sender, e);
        }

        private void button_file_Click(object sender, EventArgs e)                                                                                       // Choose file
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "c:\\";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                label3.Text = openFileDialog1.FileName;
                //DirectoryInfo info = new DirectoryInfo(openFileDialog1.FileName);
            }
        }

        private void btn_delete_Click(object sender, EventArgs e)                                                                                     // Delete file
        {
            File.Delete(label3.Text);
            label3.Text = "";
        }

        private void btn_rename_Click(object sender, EventArgs e)                                                                                    // Rename File     
        {
            string sourcePath = label3.Text;
            if (File.Exists(sourcePath))
            {
                Form3 F1 = new Form3(this);
                F1.ShowDialog();
                FileInfo fi = new FileInfo(sourcePath);
                try
                {
                    fi.MoveTo(sourcePath + @"..\" + @"..\" + F1.tmp);
                }
                catch (IOException)
                {
                    MessageBox.Show("Имя пусто или содержит недопустимые знаки", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                catch(System.ArgumentException)
                {
                    MessageBox.Show("Имя содержит недопустимые знаки", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("File renamed", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Choose file", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        string dest;
        public void btn_copy_Click(object sender, EventArgs e)                                                                                // Copy File
        {
            string sourcePath = label3.Text;
            if (File.Exists(sourcePath))
            {
                FolderBrowserDialog file = new FolderBrowserDialog();
                file.RootFolder = Environment.SpecialFolder.DesktopDirectory;
                if (file.ShowDialog() == DialogResult.OK)
                {
                    dest = file.SelectedPath;
                }
                string name = Path.GetFileNameWithoutExtension(sourcePath);
                File.Copy(sourcePath, dest + @"\" + name, true);
            }
            else
            {
                MessageBox.Show("Choose file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
        }
    }
}
