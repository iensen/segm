using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace WindowsFormsApplication3
{
    public partial class Chooser : Form
    {

        private Button[] SnowButton,ApplyButton;
        private  PictureBox[] Ps;
        private ProgressBar[]Pgbs;
        public IntPtr GetPictureBoxHandle(int index)
        {
            try
            {
                return Ps[index].Handle;
            }
            catch
            {
                MessageBox.Show("Incorrect Index");

                return this.Handle ;
            }
         
        }
        public IntPtr GetProgressBarHandle(int index)
        {
            try
            {
                return Pgbs[index].Handle;
            }
            catch
            {
                MessageBox.Show("Incorrect Index");
                return this.Handle;
            }
        }

        Pcnn PN;
        public Chooser(int n,Pcnn PN)
        {
            this.PN = PN;
            InitializeComponent();
            Rectangle rec = Screen.PrimaryScreen.Bounds;
           
              this.Width = rec.Size.Width;
              this.Height = rec.Size.Height;
              this.Top = rec.Top;
              this.Left = rec.Left;
              Application.DoEvents();
      
          
            //сделать для каждого  N!
            if(n==10)
            {
                
                int w = this.Width;
                int h = this.Height;
                w -= 100;
                h -= 100;
                int interval = 30;
                int w1=(this.Width-(n-1)*interval)/(n/2);
                int h1 = (this.Height - 100) / 2-100;
                Ps = new PictureBox[n];
                Pgbs = new ProgressBar[n];
               SnowButton = new Button[n];
               ApplyButton = new Button[n];
               int dy = 20;
                for(int i=0;i<10;i++)
                {
                    int x=(i%5)*w1+interval+(i%5+1)*interval;
                    int y = (i >= 5) ? 50 : (this.Height / 2);
                    x=(i%5)*w1+interval+(i%5+1)*interval;
                    y = (i >= 5) ? 50 : (this.Height / 2 + 50);
                   // MessageBox.Show(((i % 5) * w1 + interval + (i % 5 + 1) * interval).ToString());
                   // MessageBox.Show(x.ToString());
                    Ps[i]=new PictureBox();
                    Ps[i].Width=w1;
                    Ps[i].Height = h1 - dy; 
                    Ps[i].SizeMode = PictureBoxSizeMode.StretchImage;
                    Ps[i].Location = new Point(x, y);
                    Ps[i].DoubleClick += new EventHandler(Chooser_DoubleClick);
                    Pgbs[i]=new ProgressBar();
                    Pgbs[i].Location=new Point(Ps[i].Location.X,Ps[i].Location.Y+Ps[i].Height+20-dy);
                    Pgbs[i].Width=Ps[i].Width;
                    Pgbs[i].Height=20;
                    SnowButton[i] = new Button();
                    SnowButton[i].Location = new Point(Pgbs[i].Location.X, Pgbs[i].Location.Y + Pgbs[i].Height + 10-dy);
                    SnowButton[i].Width = Ps[i].Width;
                    SnowButton[i].Height = 24;
                    SnowButton[i].Text = "Delete Snow";
                    SnowButton[i].Click += new EventHandler(Chooser_Chosen);
                    ApplyButton[i] = new Button();
                    ApplyButton[i].Location = new Point(Pgbs[i].Location.X, Pgbs[i].Location.Y + Pgbs[i].Height + 10 - dy+30);
                    ApplyButton[i].Width = Ps[i].Width;
                    ApplyButton[i].Height = 24;
                    ApplyButton[i].Text = "Apply";
                 
                    ApplyButton[i].Click += new EventHandler(Apply_Chosen);
                    
                    this.Controls.Add(Ps[i]);
                    this.Controls.Add(ApplyButton[i]);
                    this.Controls.Add(SnowButton[i]);
                    this.Controls.Add(Pgbs[i]);
                }
                

            }
      
        }
        bool check(int x,int y,int w,int h)
        {
            if(x>=0 && x<w && y>=0 && y<h)return true;else return false;
        }
        void DeterMine(bool[,] todel, bool[,] Pixel, int w, int h)
        {
            for (int i = 0; i < w; i++)
            {
               
                for (int j = 0; j < h; j++)
                {
                    int ct = 0;
                    for (int k = -1; k <= 1; k++)
                    {
                        for (int t = -1; t <= 1; t++)
                        {
                            if (k == 0 && t == 0) continue;
                            if (check(i + k, j + t,w,h) && Pixel[i + k, j + t] == true) ct++;

                        }
                    }
                    if (ct >= 6 || ct<=1) todel[i, j] = true;
                }
            }
        }
        void del(bool[,] Pixel, bool[,] todel, int w, int h)
        {
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    if (todel[i, j] == true)
                    {
                        Pixel[i, j] = false;
                    }
                }
            }

        }
        void Chooser_Chosen(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                if (sender.Equals(SnowButton[i]))
                {
                    bool[,] Bin = new bool[Ps[0].Image.Width, Ps[i].Image.Height];
                    ImageConverter.PictureBoxToImageBinary(Ps[i], Bin);
                    bool[,] Todel = new bool[Ps[i].Image.Width, Ps[i].Image.Height];
                    DeterMine(Todel, Bin, Ps[i].Image.Width, Ps[i].Image.Height);
                    del(Bin, Todel, Ps[i].Image.Width, Ps[i].Image.Height);
                    Bitmap b = new Bitmap(Ps[i].Image.Width, Ps[i].Image.Height);
                    ImageConverter.BitmapFromBool(ref b, Bin, Ps[i].Image.Width, Ps[i].Image.Height);
                    Ps[i].Image = b;
                    PN.ClearAll();
                }


            }
        }


        void Apply_Chosen(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                if (sender.Equals(ApplyButton[i]))
                {
                    
                    bool [,]Mask=new bool[Ps[i].Image.Width, Ps[i].Image.Height];
                    ImageConverter.PictureBoxToImageBinary(Ps[i], Mask);
                    PictureBox Main = (PictureBox)PictureBox.FromHandle(MainForm.MPicture);
                    Bitmap b = (Bitmap)Main.Image;
                    ImageConverter.ApplyMask( b, Mask, Ps[i].Image.Width, Ps[i].Image.Height);
          
                    Main.Image = b;
                    Main.Visible = true;
                }


            }
        }


        void Chooser_DoubleClick(object sender, EventArgs e)
        {

            Shower p = new Shower((Bitmap)((PictureBox)(sender)).Image);
            p.Show();
        }
    }
}
