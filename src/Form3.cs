using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class Form3 : Form
    {

        Bitmap myBMP;
        Graphics g;
        private double _curscale = 1;
        private int _dx = 0;
        private int _dy = 0;
        private bool checkedfirst = false;
        private int _x1, _y1, _x2, _y2;

        public Form3()
        {
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            StrechImage();
            _curscale = 1;
            _dx = _dy = 0;
          
           
           
          
        }
         
        private bool pressed = false;
        public void Refresh()
        {
            myBMP = new Bitmap(pictureBox1.Image);
            g = Graphics.FromImage(myBMP);
            pictureBox1.Image = myBMP;
        }
        private void RefreshPicture()
        {
            PictureBox pt = (PictureBox) PictureBox.FromHandle(MainForm.MPicture);
            pictureBox1.Image = pt.Image;
        }
        public  void StrechImage()
        {
            Size t = new Size(this.Size.Width - 50 - panel1.Location.X, this.Size.Height - 60 - panel1.Location.Y);
  
            panel1.Size = t;

        }

        private void OK_Click(object sender, EventArgs e)
        {
            if (!Double.TryParse(Length.Text,out MainForm.PatternLength))
            {
                MessageBox.Show("Не выбраны параметры образца");
                return;
            }
            if (!Double.TryParse(Xtext.Text,out MainForm.PatternX))
            {
                MessageBox.Show("Не выбраны параметры образца");
                return;
            }
            if (!Double.TryParse(Ytext.Text,out MainForm.PatternY))
            {
                MessageBox.Show("Не выбраны параметры образца");
                return;
            }
            this.Dispose();

        }

        private void Form3_Resize(object sender, EventArgs e)
        {
            StrechImage();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StrechImage();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
          
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
           // MessageBox.Show(e.X.ToString() + " " + e.Y.ToString());
            if(radioButton1.Checked)
            {

                if (pictureBox1.Image.Size.Width>1500 || pictureBox1.Image.Size.Height>1500)
                {
                    MessageBox.Show("Size is too big");
                    return ;
                }
                
                _dx += (e.X/2)/Convert.ToInt32(_curscale);
                _dy += (e.Y / 2) / Convert.ToInt32(_curscale); 
                _curscale *= 2;
                RefreshPicture();
                pressed = true;
                Bitmap nmbp=new Bitmap(pictureBox1.Image);
                int w = pictureBox1.Image.Width;
                int h = pictureBox1.Image.Height;
                
                int x = e.X;
                int y = e.Y;
                ScaleImage(_curscale);
                Bitmap scaledbmp=new Bitmap(pictureBox1.Image);
                int xl = x*2;
                int yl =y*2;

            
                for(int i=x;i>=0;i--)
                    for(int j=y;j>=0;j--)
                    {
                     nmbp.SetPixel(i,j,scaledbmp.GetPixel(xl-(x-i) ,yl-y+j));
                     //nmbp.SetPixel(x-(i-x), j, scaledbmp.GetPixel(xl - (x - i), yl + y - j));

                    }
                int j2;
                for (int i = x+1; i <w; i++)//цикло по строкам
                    for (int j = y ; j < h;j++ )
                    {
                      
                        nmbp.SetPixel(i, j, scaledbmp.GetPixel(xl+i-x,yl+j-y));   
                      
                       
                      
                       //nmbp.SetPixel(i,y-(j-y),scaledbmp.GetPixel(xl+i-x,yl+y-j));

                        
                    }
                for (int i = x + 1; i < w; i++)
                    for (int j = y ; j >= 0;j-- )
                    {
                        nmbp.SetPixel(i,j,scaledbmp.GetPixel(xl+i-x,yl-(y-j)));
                    }
                for (int i = 0; i <=x; i++)
                    for (int j = y ; j < h;j++ )
                        nmbp.SetPixel(i,j,scaledbmp.GetPixel(xl-(x-i),yl+(j-y)));


                        pictureBox1.Image = nmbp;



            }
               
            else
            {
                if(!checkedfirst)
                {
                    _x1 = e.X;
                    _y1 = e.Y;
                    myBMP = (Bitmap) pictureBox1.Image;
                    checkedfirst = true;
                    g = Graphics.FromImage(myBMP);
                    pictureBox1.Image = myBMP;
                    g.DrawEllipse(Pens.Red,e.X,e.Y,2,2);
                    label3.Text = "Choose second point of  the  diameter";
                }
                else
                {
                    _x2 = e.X;
                    _y2 = e.Y;
                    g.DrawEllipse(Pens.Red, e.X, e.Y, 2, 2);
                    g.DrawLine(Pens.Red,_x1,_y1,_x2,_y2);
                    pictureBox1.Image = myBMP;
                    label3.Text = "Chosen!";
                    checkedfirst = false;
                    Length.Text =String.Format("{0:f10}",(Math.Sqrt((_x1 - _x2) * (_x1 - _x2) + (_y1 - _y2) * (_y1 - _y2)) / _curscale));
                    Xtext.Text = (_dx + (_x1 + _x2)/(2*_curscale)).ToString();
                    Ytext.Text = (_dy + (_y1 + _y2)/(2*_curscale)).ToString();


                  

                }
            }
            
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            pressed = false;
        }

        private void ScaleImage(double scale)
        {
           
       
            int nw = Convert.ToInt32(Convert.ToDouble(pictureBox1.Image.Width)*scale);
            int nh = Convert.ToInt32(Convert.ToDouble(pictureBox1.Image.Height) * scale);

            Size nSize = new Size(nw, nh);
            Image gdi = new Bitmap(nSize.Width, nSize.Height);
            Graphics ZoomInGraphics = Graphics.FromImage(gdi);
            ZoomInGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            ZoomInGraphics.DrawImage(pictureBox1.Image, new Rectangle(new Point(0, 0), nSize), new Rectangle(new Point(0, 0), pictureBox1.Image.Size), GraphicsUnit.Pixel);
            ZoomInGraphics.Dispose();
            pictureBox1.Image = gdi;
            pictureBox1.Size = gdi.Size;
     
            StrechImage();
            return;
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            pressed = true;

           
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (_curscale < 2)
            {
                MessageBox.Show("Image has it's original size");
                return;
            }
            _curscale /= 2;

            ScaleImage(0.5);
            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
           label3.Visible = false;
            checkedfirst = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            label3.Visible = true;
            checkedfirst = false;
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            RefreshPicture();
            _curscale = 1.0;
            _dy = 0;
            _dx = 0;

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

    }
}
