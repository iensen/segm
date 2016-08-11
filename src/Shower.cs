using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class Shower : Form
    {
        public Shower(Bitmap b)
        {
            InitializeComponent();
            pictureBox1.Image=b;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {

        }

        private void Shower_SizeChanged(object sender, EventArgs e)
        {
            pictureBox1.Width = this.ClientRectangle.Width;
            pictureBox1.Height = this.ClientRectangle.Height;
            pictureBox1.Invalidate();
            pictureBox1.Refresh();
        }
    }
}
