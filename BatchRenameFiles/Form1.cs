using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace BatchRenameFiles
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Text = Environment.CurrentDirectory;
            dlg = new FolderBrowserDialog();
            dlg.SelectedPath = Environment.CurrentDirectory;
            dlg.ShowNewFolderButton = true;
            button2.Enabled = false;

            showCurDir();
        }
        FolderBrowserDialog dlg;
        Dictionary<string, string> dirFiles = new Dictionary<string, string>();

        
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.dirview.Height = this.ClientSize.Height - 82;
        }

        void showCurDir()
        {
            this.Text = dlg.SelectedPath;

            var files = Directory.GetFiles(dlg.SelectedPath);
            dirFiles.Clear();
            this.dirview.Clear();
            this.dirview.BeginUpdate();
            foreach (var file in files)
            {
                dirFiles.Add(Path.GetFileName(file), file);
                this.dirview.Items.Add(Path.GetFileName(file));
            }
            this.dirview.EndUpdate();

        }
        private void button1_Click(object sender, EventArgs e)
        {
            //打开目录

            var ret = dlg.ShowDialog();
            if (ret == DialogResult.OK)
            {
                showCurDir();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //必须有查找字符串
            if (string.IsNullOrEmpty(txtFind.Text))
            {
                MessageBox.Show("输入您需要替换的文字！");
                return;
            }

            //去重
            if(txtFind.Text== txtReplace.Text)
            {
                MessageBox.Show("为什么查找和替换的文字一样。。");
                return;
            }

            List<string> reps = new List<string>();
            foreach (var file in dirFiles.Keys)
            {
                if (file.IndexOf(txtFind.Text) >= 0)
                {
                    reps.Add(file);
                }
            }

            //替换
            dirview.Items.Clear();
            dirview.BeginUpdate();

            foreach(var file in reps)
            {
                string newfilename = file.Replace(txtFind.Text, txtReplace.Text);
                dirview.Items.Add(newfilename);

                string oldfilepath = dirFiles[file];
                string newfilepath = Path.GetDirectoryName(oldfilepath)+"\\" + newfilename;
                dirFiles.Add(newfilename, newfilepath);
                dirFiles.Remove(file);

                //改名
                File.Move(oldfilepath, newfilepath);
            }

            dirview.EndUpdate();
            
            lblret.Text = string.Format("成功替换了{0}个文件名", reps.Count);
            lblret.ForeColor = Color.Red;

            txtFind.Text = "";
            
        }

        private void txtFind_TextChanged(object sender, EventArgs e)
        {
            if (txtFind.Text.Length > 0)
            {
                button2.Enabled = true;
            }
            else
            {
                button2.Enabled = false;
            }
            dirview.Items.Clear();
            dirview.BeginUpdate();
            //根据查找内容过滤文件名
            foreach(var file in dirFiles.Keys)
            {
                if (file.IndexOf(txtFind.Text) >= 0 || txtFind.Text=="")
                {
                    dirview.Items.Add(file);
                }
            }
            dirview.EndUpdate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var s = txtFind.Text;
            txtFind.Text = txtReplace.Text;
            txtReplace.Text = s;
        }
    }
}
