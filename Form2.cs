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

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 Form = new Form1();
            Form.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public string path;
        private void PopulateTreeView()
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

        private void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo)
        {
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
                    textBox1.Text = "";
                }
            }
        }

        void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode newSelected = e.Node;
            listView1.Items.Clear();
            DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;

            foreach (DirectoryInfo dir in nodeDirInfo.GetDirectories())
            {
                item = new ListViewItem(dir.Name, 0);
                subItems = new ListViewItem.ListViewSubItem[]
                    {new ListViewItem.ListViewSubItem(item, "Directory"),
                     new ListViewItem.ListViewSubItem(item, dir.LastAccessTime.ToShortDateString())};
                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }
            foreach (FileInfo file in nodeDirInfo.GetFiles())
            {
                item = new ListViewItem(file.Name, 1);
                subItems = new ListViewItem.ListViewSubItem[]
                    { new ListViewItem.ListViewSubItem(item, "File"),
                      new ListViewItem.ListViewSubItem(item, file.LastAccessTime.ToShortDateString())};
                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void button_set_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                DirectoryInfo info = new DirectoryInfo(textBox1.Text);
                path = textBox1.Text;
                if (info.Exists)
                {
                    treeView1.Nodes.Clear();
                    TreeNode rootNode;
                    rootNode = new TreeNode(info.Name);
                    rootNode.Tag = info;
                    this.Cursor = Cursors.WaitCursor;
                    GetDirectories(info.GetDirectories(), rootNode);
                    treeView1.Nodes.Add(rootNode);
                    this.Cursor = Cursors.Default;
                }
                else
                {
                    MessageBox.Show("Path: Invalid Syntax", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Path: Invalid Syntax", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string selectedNodeText;
        public string sourcedir;
        private void button_rename_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode as TreeNode;
            selectedNodeText = node.Text;
            this.treeView1.LabelEdit = true;
            node.BeginEdit();
        }

        private void treeView1_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            sourcedir = path + @"..\" + @"..\" + treeView1.SelectedNode.FullPath.ToString();
        }

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode as TreeNode;
            this.treeView1.LabelEdit = false;

            if (e.Label.IndexOfAny(new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' }) != -1)
            {
                MessageBox.Show("Invalid Name.\n" + "The step Name must not contain " + "following characters:\n \\ / : * ? \" < > |", "Edit Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (string.IsNullOrWhiteSpace(e.Label))
            {
                MessageBox.Show("Invalid name", "Error");
                e.CancelEdit = true;
                return;
            }
            else
            {
                string label = (!string.IsNullOrEmpty(e.Label) ? e.Label : selectedNodeText);
                if (null != e.Label)
                {
                    try
                    {
                        MessageBox.Show(sourcedir, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        DirectoryInfo di = new DirectoryInfo(sourcedir);
                        //string sourcedir2 = path + @"..\" + @"..\" + treeView1.SelectedNode.FullPath.ToString() + @"..\" + @"..\" + e.Label; 
                        //DirectoryInfo dis = new DirectoryInfo(sourcedir2);
                        //MessageBox.Show(sourcedir2 +"\n\n" + e.Label, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //Directory.CreateDirectory(sourcedir2);
                        //Directory.Move(sourcedir, sourcedir2);
                        di.MoveTo(di.Parent + "\\" + e.Label);
                    }
                    catch (IOException er)
                    {
                        MessageBox.Show(er.Message);
                    }
                }
            }
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            string sourcedir = path + @"..\" + @"..\" + treeView1.SelectedNode.FullPath.ToString();
            try
            {
                Directory.Delete(sourcedir, true);
                DirectoryInfo info = new DirectoryInfo(path);
                treeView1.Nodes.Clear();
                TreeNode rootNode;
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info.GetDirectories(), rootNode);
                treeView1.Nodes.Add(rootNode);
            }
            catch (System.IO.IOException)
            {
                MessageBox.Show("Deleting Error: Incorrect path or access denied", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button_copy_Click(object sender, EventArgs e)
        {
            string sourcedir = path + @"..\" + @"..\" + treeView1.SelectedNode.FullPath.ToString();
            Form3 f = new Form3(this);
            f.ShowDialog();
            MessageBox.Show("Source path:\n\n" + sourcedir + "\n\nDestination path:\n\n" + f.tmp, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DirectoryInfo info = new DirectoryInfo(path);
            try
            {
                Directory.Move(sourcedir, f.tmp);
            }
            catch(System.IO.IOException)
            {
                MessageBox.Show("Unknown Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (System.ArgumentException)
            {
                MessageBox.Show("Invalid Syntax" + f.tmp, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            treeView1.Nodes.Clear();
            TreeNode rootNode;
            rootNode = new TreeNode(info.Name);
            rootNode.Tag = info;
            GetDirectories(info.GetDirectories(), rootNode);
            treeView1.Nodes.Add(rootNode);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            treeView1.Nodes.Clear();
            TreeNode rootNode;
            rootNode = new TreeNode(info.Name);
            rootNode.Tag = info;
            GetDirectories(info.GetDirectories(), rootNode);
            treeView1.Nodes.Add(rootNode);
        }

        private void button_file_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "c:\\";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                label3.Text = openFileDialog1.FileName;
                //DirectoryInfo info = new DirectoryInfo(openFileDialog1.FileName);
            }
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            File.Delete(label3.Text);
            label3.Text = "";
        }

        private void btn_rename_Click(object sender, EventArgs e)
        {
            string sourcePath = label3.Text;
            string destinationPath = label3.Text;
            if (File.Exists(sourcePath))
            {
                FileInfo fi = new FileInfo(sourcePath);

                fi.MoveTo(destinationPath);
                MessageBox.Show("File renamed");
            }
            else if (Directory.Exists(sourcePath))
            {
                DirectoryInfo di = new DirectoryInfo(sourcePath);

                di.MoveTo(destinationPath);
                MessageBox.Show("Directory renamed");
            }
            this.Close();
        }
    }
}
