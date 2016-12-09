using LibSubWCRev;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SvnCopy
{
    public partial class Form1 : Form
    {
        private const string svnCopyFilePath = @"C:\SvnCopySetting.txt";


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = Application.StartupPath;
            var dict = LoadSetting();
            if (dict.ContainsKey(this.textBox1.Text))
            {
                this.textBox2.Text = dict[this.textBox1.Text];
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.textBox2.Text = fbd.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.textBox2.Text == "")
            {
                MessageBox.Show("请选择目标文件夹。");
                return;
            }
            var srcFolderPath = this.textBox1.Text;
            var tarFolderPath = this.textBox2.Text;
            SaveSetting();
            TryCopyFolder(srcFolderPath, srcFolderPath, tarFolderPath);
            this.Close();
        }

        private Dictionary<string, string> LoadSetting()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (File.Exists(svnCopyFilePath))
            {
                var texts = File.ReadAllLines(svnCopyFilePath);
                foreach (var text in texts)
                {
                    var sp=text.Trim().Split('|');
                    dict[sp[0]] = sp[1];
                }
            }
            return dict;
        }

        private void SaveSetting()
        {
            var srcFolderPath = this.textBox1.Text;
            var tarFolderPath = this.textBox2.Text;
            var dict = LoadSetting();
            dict[srcFolderPath] = tarFolderPath;
            StringBuilder sb = new StringBuilder();
            foreach (var item in dict)
            {
                sb.Append(item.Key);
                sb.Append("|");
                sb.Append(item.Value);
                sb.Append("\r\n");
            }
            File.WriteAllText(svnCopyFilePath, sb.ToString());
        }
        public void TryCopyFolder(string folderPath, string srcFolderPath, string tarFolderPath)
        {
            var folders = Directory.GetDirectories(folderPath);
            foreach (var folder in folders)
            {
                TryCopyFolder(folder, srcFolderPath, tarFolderPath);
            }
            var files = Directory.GetFiles(folderPath);
            foreach (var file in files)
            {
                TryCopy(file, srcFolderPath, tarFolderPath);
            }
        }

        public void TryCopy(string filePath, string srcFolderPath, string tarFolderPath)
        {
            //if (Path.GetFileName(filePath)=="SvnCopy.exe")
            //{
            //    return;
            //}
            if (CanCopy(filePath))
            {
                var tarFilePath = tarFolderPath + filePath.Substring(srcFolderPath.Length);
                Directory.CreateDirectory(Path.GetDirectoryName(tarFilePath));
                File.Copy(filePath, tarFilePath, true);
            }
        }


        public bool CanCopy(string filePath)
        {
            SubWCRev sub = new SubWCRev();
            sub.GetWCInfo(filePath, true, true);
            if (sub.HasUnversioned)
            {
                return false;
            }
            return sub.HasModifications;// || sub.HasUnversioned;
        }




    }
}
