///////////////////////////////////////////////////////
//
// Simple image to base64 - base64 to image converter
//
// Written by Ian Wigley Jan 2015
//
///////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Base64Converter
{
    public partial class Base64Converter : Form
    {
        private string m_fileExt = string.Empty;
        private string m_fileName = string.Empty;
        private System.IntPtr intptr = new IntPtr();

        public Base64Converter()
        {
            InitializeComponent();
            this.Base64.Enabled = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void loadToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Map Maker";
            openFileDialog.InitialDirectory = @"*.*";
            openFileDialog.Filter = "All files (*.*)|*.*|All files (*.png;*.jpg)|*.png;*.jpg";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.pictureBox1.Image = new Bitmap(openFileDialog.FileName);
                m_fileExt = Path.GetExtension(openFileDialog.FileName);
                m_fileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName).ToLower();
                this.Base64.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Clear();
            ImageFormat format = pictureBox1.Image.RawFormat;
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                this.pictureBox1.Image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                this.richTextBox1.Text = "<img src=\"data:image/" + m_fileExt + ";base64," + base64String + "\" alt=\"" + m_fileName + "\">" ;
            }
        }

        // Method to convert Base64 Bytes to image
        public void Base64ToImage(string base64String)
        {
            try
            {
                // Convert Base64 String to byte[]
                byte[] imageBytes = Convert.FromBase64String(base64String);
                MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);

                // Convert byte[] to Image
                ms.Write(imageBytes, 0, imageBytes.Length);
                this.pictureBox1.Image = Image.FromStream(ms, true);
                Image thumbNail = Image.FromStream(ms, true);
                pictureBox2.Image = thumbNail.GetThumbnailImage(100, 100, null, intptr);
            }
            catch
            {
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show("The base64 string still contains invalid chars, please remove them.", "Invalid String", buttons);
            }
        }

        // Method to copy the base64 text into the clipboard
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.richTextBox1.Text != string.Empty)
            {
                Clipboard.SetText(this.richTextBox1.Text);
            }
        }

        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            string[] filename = (string[])e.Data.GetData(DataFormats.FileDrop);
            string temp = filename[0];

            m_fileName = Path.GetFileNameWithoutExtension(temp).ToLower();
            m_fileExt = Path.GetExtension(temp).ToLower();

            pictureBox1.Image = Image.FromFile(filename[0]);
            this.Base64.Enabled = true;

            Image thumbNail = Image.FromFile(filename[0]);

            pictureBox2.Image = thumbNail.GetThumbnailImage(100, 100, null, intptr);
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Clear();
        }

        private void ToImageButton_Click(object sender, EventArgs e)
        {
            string base64 = this.richTextBox1.Text;

            if (base64.Contains('"'))
            {
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                var result = MessageBox.Show("The base64 string contains invalid chars, please click yes to remove them.", "Invalid String", buttons);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    // Trim the html alt tag 
                    string temp0 = base64.Remove(0, 33);

                    // Trim the html alt tag 
                    int pos = temp0.IndexOf("\" alt=\"");
                    string temp1 = temp0.Remove(pos);
                    Base64ToImage(temp1);
                }
            }
            else
            {
                Base64ToImage(this.richTextBox1.Text);
            }
        }
    }
}
