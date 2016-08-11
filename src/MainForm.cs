using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using ZedGraph;
using System.Globalization;
using System.Threading;
namespace WindowsFormsApplication3
{
    enum DistributionType:byte
    {
        Standart=2,Sieve=1
    }
    public partial class MainForm : Form
    {
        private DistributionType DType = DistributionType.Standart;
        SelectionFrame SelectFrame = new SelectionFrame();//selection frame 
        SizeSelector SizePattern = new SizeSelector();//size selector
        DistrubuteSelector Distributer = new DistrubuteSelector();
        PatternSelector PatternH = new PatternSelector();
        PatternSelector PatternV = new PatternSelector();
        static public string StartDir;//directory of the "EXE"
        static public int Binarization_W = 51;
        static public double Binarization_K = 0.1;
        static public IntPtr MPicture;
        static public IntPtr SPicture;
        static public IntPtr ProgressLabel;
        static public IntPtr ProgressBr;
        static public IntPtr QualLabel;
        static public double PatternX;
        static public double PatternY;
        static public double PatternLength;
        static public int MaxMarkers = 500;
        static public double ContrastRange = 1.4;
        static public bool DoubleFilter = false;
        static public int MaxFh=155;
        static public int MinFh=120;
        static public double MinSigma=0.65;
        static public double MaxSigma=0.85;
        static public double Density = 2200.0;
        static public double Nat_pat_size=1.0;
        static public bool FastSorting=false;
        static public bool ShowBestFitEllipses = true;
        static public List<ElipseToDraw> Ellipses = new List<ElipseToDraw>();
        string SFile = "";//chosen File
        private List<string> teMPoFiles=new List<string>();
        void updateWH()
        {
            if (MainPictureBox.Image == null) return;
            WidthLabel.Text = MainPictureBox.Image.Width.ToString() + " " + "px";
            
           HeiValue.Text = MainPictureBox.Image.Height.ToString() + " " + "px";
            
            this.Invalidate();
            
        }
        void UpdateQuality()
        {
            if (MainPictureBox.Image.Width*MainPictureBox.Image.Height>1500000)
            {
                label12.Text = "Yes";
                DoubleFilter = true;

            }
            else
            {

                label12.Text = "No";
                DoubleFilter = false;
            }
        }
        public MainForm()
        {
          
            InitializeComponent();
            StartDir = Directory.GetCurrentDirectory();
            textBox1.Text = Convert.ToString(0.75);
      
            MainPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            MPicture = this.MainPictureBox.Handle;
            SPicture = this.pictureBox1.Handle;
            ProgressLabel = label4.Handle;
            ProgressBr = progressBar1.Handle;
            pictureBox1.Visible = false;
            QualLabel = label12.Handle;
            this.WindowState = FormWindowState.Maximized;
            ResizeAll();
            UpdateMarkersCount();
            try
            {
                Image img = Image.FromFile("standartarrow.bmp");
                radioButton1.Image = img;
                radioButton1.ImageAlign = ContentAlignment.MiddleRight;
                Image img2 = Image.FromFile("scissors.bmp");
                radioButton2.Image = img2;
                radioButton2.ImageAlign = ContentAlignment.MiddleRight;
                Image img3 = Image.FromFile("patternh.bmp");
                radioButton3.Image = img3;
                radioButton3.ImageAlign = ContentAlignment.MiddleCenter;
                Image img4 = Image.FromFile("marker.bmp");
                radioButton4.Image = img4;
                radioButton4.ImageAlign = ContentAlignment.MiddleRight;
                Image img5 = Image.FromFile("chooser.bmp");
                radioButton5.Image = img5;
                radioButton5.ImageAlign = ContentAlignment.MiddleCenter;
                Image img6 = Image.FromFile("distributer.jpg");
                radioButton6.Image = img6;
                radioButton6.ImageAlign = ContentAlignment.MiddleCenter;
                Image img7 = Image.FromFile("patternv.bmp");
                radioButton7.Image = img7;
                radioButton7.ImageAlign = ContentAlignment.MiddleCenter;

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Some files were corrupted,reinstall the application");
                Application.Exit();
            }
            UpdateSelectedLabel();
       

   
      
            


        }
        private void UpdateMarkersCount()
        {
            label14.Text = Labels.Count.ToString();
            this.Invalidate();
        }
        private void UpdateSelectedLabel()
        {
            if(radioButton1.Checked)
            {
                label11.Text = "Pointer selected";
            }
            if (radioButton2.Checked)
            {
                label11.Text = "Scissors selected";
            }
            if (radioButton3.Checked)
            {
                label11.Text = "H-Pattern selected";
            }
            if (radioButton4.Checked)
            {
                label11.Text = "Marker selected";
            }
            if(radioButton5.Checked)
            {
                label11.Text = "Min. segment chooser selected";
            }
            if (radioButton6.Checked)
            {
                label11.Text = "Distributor selected";
            }
            if (radioButton7.Checked)
            {
                label11.Text = "V-Pattern selected";
            }
        }   
        public static void UpdateLabel(string s)
        {
            System.Windows.Forms.Label t =
                (System.Windows.Forms.Label)System.Windows.Forms.Label.FromHandle(MainForm.ProgressLabel);
            t.Text = s;
            t.Update();
            t.Refresh();
            Application.DoEvents();
        }

        private void convolveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainPictureBox.Image == null) return;
            double sigma = 0;
            if (!Double.TryParse(textBox1.Text, out sigma))
            {
                MessageBox.Show("Input sigma for Gauss's blur");
            }
            GaussSmooth.SmoothRGBPictureBox(MainPictureBox, sigma);
        }



        private bool felzenSegment(bool GaussBlur)
        {
            int minSeg;
            double sigma;
            double c;
            if (!Int32.TryParse(textBox2.Text, out minSeg))
            {
                MessageBox.Show("input integer value for segment's min_size algo");
                return false;

            }
            if (!Double.TryParse(textBox1.Text, out sigma))
            {
                MessageBox.Show("Input sigma for gauss blur");
                return false;
            }
            if (!Double.TryParse(textBox3.Text, out c))
            {

                MessageBox.Show("Input algo paremeter");
                return false;
            }
            int w = MainPictureBox.Image.Width;
            int h = MainPictureBox.Image.Height;
            MyFloatColor[,] src = new MyFloatColor[w, h];
            ImageConverter.PictureboxToArrayRGBf(MainPictureBox, src);
            if (GaussBlur)
            {
                UpdateLabel("Gauss Filter");
                GaussSmooth.SmoothRgbArray(src, sigma, w, h);
                GC.Collect();
            }
            FelzenSegmentation.SegmentImageRGB(src, c, minSeg, w, h, progressBar1);
            GC.Collect();
            ImageConverter.PictureBoxFromImageRGBf(pictureBox1, src, w, h);
            Bitmap b = new Bitmap(pictureBox1.Image);
            SegmentVisualizer.GetClrEdges(b, progressBar1);
            pictureBox1.Image = b;
            pictureBox1.Visible = true;
            MainPictureBox.Visible = false;
            return true;

        }

        private void felzenszwalbHuttenlochersSegmentaionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainPictureBox.Image == null) return;
            if(!felzenSegment(true))return;
            Bitmap b = new Bitmap(pictureBox1.Image);
           
            SegmentVisualizer.GetClrEdges(b, progressBar1);
        
            pictureBox1.Image = b;
            MainPictureBox.Invalidate();

        }

        private void button6_Click(object sender, EventArgs e)//choosing Pattern
        {
            Form3 F = new Form3();
            F.pictureBox1.Image = MainPictureBox.Image;
            F.Show();
            F.StrechImage();
            F.Refresh();

        }


        private void makeGrayScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainPictureBox.Image == null) return;
            UpdateLabel("Gray Scaling");
            ImageConverter.ToGrayScale(MainPictureBox, progressBar1);
        }


        private void button7_Click_1(object sender, EventArgs e)
        {
            ImageLoadSave.SaveImage(MainPictureBox);

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (MainPictureBox.Image == null) return;
         

            Filtering.ChangeContrast(MainPictureBox, progressBar1,ContrastRange);
        }
        private void LoadImage()
        {
            //locked = false;
            string newf = ImageLoadSave.LoadImage(MainPictureBox);
            if (newf != string.Empty)
            {
                SelectFrame.ChangeScale(MainPictureBox);
                SizePattern.ChangeScale(MainPictureBox);
                Distributer.ChangeScale(MainPictureBox);
                PatternH.ChangeScale(MainPictureBox);
                PatternV.ChangeScale(MainPictureBox);
                SFile = newf;
            }
            else
            {
                return;
            }
            SelectFrame.Done = false;
            pictureBox1.Visible = false;
            MainPictureBox.Visible = true;
            updateWH();
            UpdateQuality();
            //label15.Text = "Undefined";
            Ellipses.Clear();
            DeleteAllMarkers();

          

        }
        private void loadImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadImage();       
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainPictureBox.Visible)
                ImageLoadSave.SaveImage(MainPictureBox);
            else
                ImageLoadSave.SaveImage(pictureBox1);
        }

        void swapBoxes()
        {
            if (pictureBox1.Image != null)
            {


                MainPictureBox.Visible = !MainPictureBox.Visible;
                pictureBox1.Visible = !pictureBox1.Visible;
            }
          
        }
        //private void pictureBox1_Click(object sender, EventArgs e)
        //{
            
          //  swapBoxes();
        //}






        private void addNewOneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainPictureBox.Image == null) return;
            Directory.SetCurrentDirectory(StartDir);
            if (MainPictureBox.Visible)
            {
                int w = MainPictureBox.Image.Width;
                int h = MainPictureBox.Image.Height;
                byte[,] Cls = new byte[w, h];
                ImageConverter.PictureBoxToImageGScale(MainPictureBox, Cls);
                ImageStaticData.Train(Cls, w, h, textBox1.Text, textBox3.Text, progressBar1);
            }
            else
            {
                int w = MainPictureBox.Image.Width;
                int h = MainPictureBox.Image.Height;
                byte[,] Cls = new byte[w, h];
                ImageConverter.PictureBoxToImageGScale(MainPictureBox, Cls);
                ImageStaticData.Train(Cls, w, h, textBox1.Text, textBox3.Text, progressBar1);
            }




        }
        private void SaveTempImage(Bitmap newBitMap)
        {
            Directory.SetCurrentDirectory(StartDir);
            string[] dataArr = DateTime.UtcNow.ToString().Split();
            string resfIlename="";
            foreach (string d in dataArr)
            {
                resfIlename += d;
            }
            string filename = "temp_" + resfIlename + ".bmp";
            filename = filename.Replace(':', '_');
            teMPoFiles.Add(filename);
      
            SFile = filename;

            newBitMap.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
            GC.Collect();

        }
        private void UpdateProgessLabel(string s)
        {
            label4.Text = s;
            label4.Update();
            label4.Refresh();
            Application.DoEvents();
        }


        private void Refresh()
        {
            MainPictureBox.Refresh();
            pictureBox1.Refresh();

        }



        private void MainPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainPictureBox.Image == null) return;
            RedrawAllMarkers();

            if (MouseButtons.Left == e.Button && radioButton4.Checked)
            {
                CreateNewMarker(true);
                int Index = Labels.Count - 1;
                Labels[Index].Location = new Point(e.X + panel1.Location.X + MainPictureBox.Location.X, e.Y + panel1.Location.Y + MainPictureBox.Location.Y);
                int MarkerPosX = e.X;
                int MarkerPosY = e.Y;
                MarkerPositions[Index].dx = ((float)MarkerPosX) / (float)MainPictureBox.Size.Width;
                MarkerPositions[Index].dy = ((float)MarkerPosY) / (float)MainPictureBox.Size.Height;
                RedrawAllMarkers();
                return;
            }
            if (MouseButtons.Right == e.Button)
            {
                swapBoxes();
                return;
            }



            Refresh();

            if (radioButton2.Checked)
            {
                SelectFrame.Exist = true;
                SelectFrame.SetStart(e.X, e.Y);
            }
            if(radioButton5.Checked)
            {
                SizePattern.Exist = true;
                SizePattern.SetStart(e.X, e.Y);
               
            }
            if (radioButton3.Checked)
            {
                PatternH.Exist = true;
                PatternH.SetStart(e.X, e.Y);
            }
            if (radioButton6.Checked)
            {
                Distributer.Exist = true;
                Distributer.SetStart(e.X, e.Y);
            }
            if (radioButton7.Checked)
            {
               PatternV.Exist=true;
                
                    PatternV.SetStart(e.X, e.Y);
                
            }

        }

        private void MainPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (radioButton2.Checked)
            {
                SelectFrame.SetFinish(e.X, e.Y);
                SelectFrame.Draw(true);
            };
            if(radioButton5.Checked)
            {
                SizePattern.SetFinish(e.X, e.Y);
                SizePattern.Draw(true);
            }
            if (radioButton3.Checked)
            {
                PatternH.SetFinish(e.X, e.Y);
                PatternH.Draw(true);
            }
            if (radioButton6.Checked)
            {
                Distributer.SetFinish(e.X, e.Y);
                Distributer.Draw(true);
            }
            if (radioButton7.Checked)
            {
                PatternV.SetFinish(e.X, e.Y);
                PatternV.Draw(true);
            }


        }
        private void UpdateSizePattern()
        {
            Rectangle r=SizePattern.GetSelectedRectangle();
            if (r.Width != 0 && r.Height != 0)
            {
                textBox2.Text = (Convert.ToInt32(Math.PI * r.Width * r.Height / 4)).ToString();
            }
            textBox2.Invalidate();
        }
        private void UpdatePattern()
        {
            Rectangle r = PatternH.GetSelectedRectangle();
            string Horiz="Nan",Vert="Nan";
            if (r.Width != 0 && r.Height != 0)
            {
               Horiz=((int)(Math.Sqrt(1.0 * r.Width * r.Width + 1.0 * r.Height * r.Height))).ToString();
            }
            r = PatternV.GetSelectedRectangle();
            if (r.Width != 0 && r.Height != 0)
            {
                Vert= ((int)(Math.Sqrt(1.0 * r.Width * r.Width + 1.0 * r.Height * r.Height))).ToString();
            }
            label8.Text = Horiz + "X" + Vert;
            label8.Invalidate();
        }
        private void MainPictureBox_MouseMove(object sender, MouseEventArgs e)
        {

         
            if (SelectFrame.Exist)
            {
                Refresh();
                SelectFrame.SetFinish(e.X, e.Y);
                SelectFrame.Draw(false);
            }
            if(SizePattern.Exist)
            {
                Refresh();
                SizePattern.SetFinish(e.X, e.Y);
                SizePattern.Draw(false);
                UpdateSizePattern();
            }

            if (PatternH.Exist)
            {
                Refresh();
                PatternH.SetFinish(e.X, e.Y);
                PatternH.Draw(false);
                UpdatePattern();
            }
            if (Distributer.Exist)
            {
                Refresh();
                Distributer.SetFinish(e.X, e.Y);
                Distributer.Draw(false);
                
            }
            if (PatternV.Exist)
            {
                Refresh();
                PatternV.SetFinish(e.X, e.Y);
                PatternV.Draw(false);
                UpdatePattern();
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {

           

        }

        private void GetOptParameters(PictureBox p)
        {
            int w = p.Image.Width;
            int h = p.Image.Height;
            byte[,] src = new byte[w, h];
            ImageConverter.PictureBoxToImageGScale(p, src);
            GetOptParameters(src, w, h);

        }
        private void GetOptParameters(byte[,] src, int w, int h)
        {
            UpdateLabel("Getting optimal parameters");
            double sigma = 0;
            int Algo = 0;
            ImageStaticData.GetOptimalParams(src, w, h, ref sigma, ref Algo, progressBar1);
            if (Algo < 120) Algo = 120;
            if (Algo > 150) Algo = 150;
            textBox1.Text = sigma.ToString();
            textBox3.Text = Algo.ToString();
        }
        private void button7_Click_2(object sender, EventArgs e)
        {
            if (MainPictureBox.Visible)
            {
                GetOptParameters(MainPictureBox);
            }
            else
            {
                GetOptParameters(pictureBox1);
            }
        }

        private void distributionToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private bool ParseAngle(string s, out double dydx,out double patLength)
        {
            int index = s.IndexOf('X');
            double dy=0,dx=0;
            patLength = 0;
            dydx = -1;
            string s1 = s.Substring(0, index );
            string s2 = s.Substring(index + 1);
            if (!Double.TryParse(s1, out dx)) return false;
            if (!Double.TryParse(s2, out dy)) return false;
            patLength = dx;
            dydx = dy / dx;
            return true; 

        }
        private void Distribute(int type,Rectangle r)
        {
            MainForm.Ellipses.Clear();
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("You didn't complete segmentation yet");
                return;
            }
            double dydx,PatLength;
            if (!ParseAngle(label8.Text, out dydx, out PatLength))
            {
                MessageBox.Show("Choose Horizontal and vertical pattern");
                return;
            }
            int w = pictureBox1.Image.Width;
            int h = pictureBox1.Image.Height;

            MyByteColor[,] src = new MyByteColor[w, h];
            ImageConverter.PictureboxToArrayRGB(pictureBox1, src);
            int Minvalue;
            if (!Int32.TryParse(textBox2.Text, out Minvalue))
            {
                MessageBox.Show("Choose min Stone size the program will account");
                return;
            }
            List<int> Areas = null;



            Areas = AreaCalculator.GetAreas(src, w, h, progressBar1, Minvalue, r, MainPictureBox, dydx, PatLength);
           
            List<int> Counts = new List<int>();
            List<int> Silves = new List<int>(100);
            Distribution.getDistribution(Areas, Silves, Counts, type);
            Distribution f = new Distribution(Counts, Silves, 1);
            f.Show();

            GC.Collect();
        }
        private void sieveDistriButionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DType == DistributionType.Standart)
            {
                DType = DistributionType.Sieve;
                sieveDistriButionToolStripMenuItem.Text = "Switch to standart distribution";
            }
            else
            {
                DType = DistributionType.Standart;
                sieveDistriButionToolStripMenuItem.Text = "Switch to sieve distribution";
            }

         

        }

        private void standartDistributionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Distribute((int)DType,new Rectangle(-1,-1,-1,-1));
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {



        }

        private void ResizeAll()
        {

            panel1.Size = new Size(this.ClientRectangle.Width - 260, this.ClientRectangle.Height - 170);
            panel2.Size = new Size(panel1.Location.X, this.ClientRectangle.Width-panel1.Location.Y-20);
            
           
            MainPictureBox.Size = new Size(panel1.Size.Width - 10, panel1.Size.Height - 10);
            pictureBox1.Size = new Size(panel1.Size.Width - 8, panel1.Size.Height - 5);
      
            progressBar1.Location = new Point(panel2.Width+(this.ClientRectangle.Width-panel2.Width-progressBar1.Width)/2, this.ClientRectangle.Height - 90);
            label1.Location = new Point(progressBar1.Location.X + 40, progressBar1.Location.Y + 30);
            label23.Location = new Point(label1.Location.X + 20+label1.Width,label1.Location.Y);
            label24.Location = new Point(label23.Location.X + 20+label23.Width, label23.Location.Y);
            label4.Location = new Point(progressBar1.Location.X, progressBar1.Location.Y - 10);
         }

        private void Form1_Resize(object sender, EventArgs e)
        {

            // Leave a small margin around the outside of the control
            ResizeAll();
        }

        private void medianFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainPictureBox.Image == null) return;
            int w = MainPictureBox.Image.Width;
            int h = MainPictureBox.Image.Height;
            Bitmap b = new Bitmap(MainPictureBox.Image);
            Filtering.Median(b, 5);
            MainPictureBox.Image = b;
        }


        private void binarizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainPictureBox.Image == null) return;
            Bitmap b = new Bitmap(MainPictureBox.Image);
            Binarization.Binarize(b, false, progressBar1, pictureBox1);
            MainPictureBox.Visible = false;
            pictureBox1.Visible = true;
        }


        private void label1_MouseLeave(object sender, EventArgs e)
        {

        }




        private void menuStrip1_KeyDown(object sender, KeyEventArgs e)
        {

        }
        


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (MouseButtons.Right == e.Button)
            {
                swapBoxes();
                return;
            }

        }



        private void ClearSmallErrors()
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("You should complete binarization first");
                return;
            }
            bool[,] src = new bool[pictureBox1.Image.Width, pictureBox1.Image.Height];
            int w = pictureBox1.Image.Width;
            int h = pictureBox1.Image.Height;
            int minsize;
            if (!Int32.TryParse(textBox2.Text, out minsize))
            {
                MessageBox.Show("Choose Min element size");
                return;
            }


            Bitmap b = new Bitmap(pictureBox1.Image);
            Binarization.DeleteInnerEffects(ref b, minsize, progressBar1);
            pictureBox1.Image = b;


        }
        private void deleteInnerDefectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearSmallErrors();
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            this.Refresh();
        }
        private void GrayScaleMedianFilter()
        {
            Bitmap b = (Bitmap)MainPictureBox.Image;
            int w = b.Width;
            int h = b.Height;
            byte[,] InImage = new Byte[w, h];
            byte[,] OutImage = new Byte[w, h];
            BitmapFilter.G_PixelsFrombitmap(b, InImage);
            Application.DoEvents();

            Filtering.GrayScaleMedian(InImage, OutImage, 5, w, h, progressBar1);
            BitmapFilter.G_BitMapFromPixels(b, OutImage);
            MainPictureBox.Image = b;
            GC.Collect();
        }
        private void greyScaleMedianFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainPictureBox.Image == null) return;
            GrayScaleMedianFilter();
        }

        private void gradientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap b = new Bitmap(MainPictureBox.Image);
            int w = b.Width;
            int h = b.Height;
            byte[,] Pixels = new byte[w, h];
            BitmapFilter.G_PixelsFrombitmap(b, Pixels);
            float[,] Grad = new float[w, h];
            BitmapFilter.Gradient(Pixels, Grad, w, h);
        }


        private void setUpMarkers(float[,] src, bool[,] Mask, int w, int h, double gauss, int Algo)
        {
            UpdateLabel("SettingMarkers");
            int minSeg = 0;

            if (!int.TryParse(textBox2.Text, out minSeg))
            {
                MessageBox.Show("Choose minimal segment size");
                return;
            }
         
            progressBar1.Value = 0;
            label4.Visible = true;

            GaussSmooth.SmoothGrayScaleArray(src, 0.75, w, h);
            MyFloatColor[,] src2 = new MyFloatColor[w, h];
            ImageConverter.FloatGStoFloatRGB(src, src2, w, h, progressBar1);
            src = null;
            GC.Collect();
            FelzenSegmentation.SegmentImageRGB(src2, Algo, minSeg, w, h, progressBar1);

            GC.Collect();
            //   List < MyShortPoint >= new List<MyShortPoint>();
            List<MyShortPoint> Markers = new List<MyShortPoint>();
            bool[,] Visited = new bool[w, h];


            List<int> Areas = new List<int>();
            int maxArea = -1;
            int MarkerX = 0;
            int MarkerY = 0;
            List<MyShortPoint> Lt = new List<MyShortPoint>();
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {


                    if (!Visited[i, j])
                    {
                        int black = 0;
                        Lt.Clear();
                        int Area = AreaCalculator.GetAreaW(Lt, Mask, Visited, src2, i, j, ref black, w, h);
                        if (black >= Area / 2) continue;
                        Int64 addx = 0;
                        Int64 addy = 0;
                        foreach (MyShortPoint pt in Lt)
                        {
                            addx += pt.x;
                            addy += pt.y;
                        }
                        Int16 adxi = (short)(addx / Lt.Count);
                        Int16 adyi = (short)(addy / Lt.Count);
                        int currentDif = 1000000000;
                        foreach (MyShortPoint pt in Lt)
                        {
                            if ((int)(pt.x - adxi) * (int)(pt.x - adxi) + (int)(pt.y - adyi) * (int)(pt.y - adyi) < currentDif)
                            {
                                currentDif = (int)(pt.x - adxi) * (int)(pt.x - adxi) + (int)(pt.y - adyi) * (int)(pt.y - adyi);
                                MarkerX = pt.x;
                                MarkerY = pt.y;
                            }
                        }
                        Markers.Add(new MyShortPoint(MarkerX, MarkerY));
                        Areas.Add(Area);
                        if (Area > maxArea)
                        {
                            maxArea = Area;
                        }

                    }

                }
            }
            int midt = BitmapFilter.GetKth(Areas.ToArray(), 0, Areas.Count - 1, 2 * (Areas.Count - 1) / 5, true, maxArea);
            
            for (int i = 0; i < Areas.Count; i++)
            {
                if (Areas[i] < midt)
                {
                    Areas.RemoveAt(i);
                    Markers.RemoveAt(i);
                }
            }
            
            //MarkerPositions.Clear();
            int ct = Markers.Count;
            bool VisibleM;
            if (ct > MaxMarkers)
            {
                VisibleM = false;
            }
            else
            {
                VisibleM = true;
            }
            for (int i = 0; i < ct; i++)
            {
                CreateNewMarker(VisibleM);
                MarkerPositions[MarkerPositions.Count - 1].dx = (float)Markers[i].x / MainPictureBox.Image.Width;
                MarkerPositions[MarkerPositions.Count - 1].dy = (float)Markers[i].y / MainPictureBox.Image.Height;
                Invalidate();
                Refresh();

            }
            RedrawAllMarkers();
            Invalidate();
            Refresh();
            Application.DoEvents();

        }

        private int getGray(MyFloatColor c)
        {
            float c1 = 0.3f * c.R + 0.59f * c.G + 0.11f * c.B;
            return (int)(c1 * 255);
        }




        private void button12_Click(object sender, EventArgs e)
        {


        }

        private void fullSegmentationToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void mToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void ClearProggressLabels()
        {
            label1.ForeColor = Color.Gray;
            label23.ForeColor = Color.Gray;
            label24.ForeColor = Color.Gray;
        }
        private void DoIt(bool SetUpMarkers)
        {
            float[,] f = null;
            int minsize;
            if (!Int32.TryParse(textBox2.Text, out minsize))
            {
                MessageBox.Show("Input min. segment size");
                return;
            }
            FilterAndSetupMarkers(ref f, SetUpMarkers);
            label24.ForeColor = Color.FromArgb(0, 255, 0);
            if (f == null)
            {
                return;
            }

            int w = MainPictureBox.Image.Width;
            int h = MainPictureBox.Image.Height;
            bool[,] Mask = new bool[w, h];
            byte[,] src = new byte[w, h];
            ImageConverter.FloatGsToByteGs(f, src, w, h, progressBar1);
            f = null;
            GC.Collect();
            UpdateLabel("getting Mask");

            Binarization.GetMaskLT(src, Mask, w, h, progressBar1,Binarization_W,Binarization_K);
            UpdateLabel("Clearing Mask");
            Binarization.DeleteInnerEffects(Mask, minsize, w, h, progressBar1);

            //waterShed
            int[,] Result = new int[w, h];
            UpdateLabel("WaterSHed Segmentation");
            WaterShed.Segmentation(src, w, h, MarkerPositions, Mask, Result, progressBar1);
            src = null;
            Mask = null;
            GC.Collect();
            Bitmap b2 = null;
            UpdateLabel("Segments Visualizing");
            SegmentVisualizer.ColorBitmap(ref b2, Result, w, h, Labels.Count, progressBar1);
            SegmentVisualizer.GetClrEdges(b2, progressBar1);
            
			pictureBox1.Image = b2;
            pictureBox1.Visible = true;
            MainPictureBox.Visible = false;
			this.Invalidate ();
            ClearProggressLabels();



        }
        private void automaticToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int minsize;
            if (!Int32.TryParse(textBox2.Text, out minsize))
            {
                MessageBox.Show("Choose min. segment size");
                return;
            }
            DeleteAllMarkers();

            DoIt(true);
        }

        private void FilterAndSetupMarkers(ref float[,] res,bool setupMarkers)
        {
            double sigma;
            int MinSeg;
            if (!Double.TryParse(textBox1.Text, out sigma))
            {
                MessageBox.Show("Input sigma for gauss smooth");
                return;
            }
            if (!Int32.TryParse(textBox2.Text, out MinSeg))
            {
                MessageBox.Show("Input minimum segment to count size");
                return;
            }
            label1.ForeColor = Color.FromArgb(0, 255, 0);
            MyFloatColor[,] src = new MyFloatColor[MainPictureBox.Image.Width, MainPictureBox.Image.Height];

            ImageConverter.PictureboxToArrayRGBf(MainPictureBox, src);
            GC.Collect();
            int w = MainPictureBox.Image.Width;
            int h = MainPictureBox.Image.Height;
            GaussSmooth.SmoothRgbArray(src, sigma, w, h);
            Filtering.ChangeContrast(ContrastRange, src, w, h, progressBar1);
            byte[,] gssrc = new byte[w, h];
            byte[,] dstFt = new byte[w, h];
            ImageConverter.FloatToByteGs(src, gssrc, w, h, progressBar1);
            src = null;
            GC.Collect();
           
            bool[,] Mask = new bool[w, h];

            int algo = 0;

            if (DoubleFilter)
            {
                Filtering.GrayScaleMedian(gssrc, dstFt, 3, w, h, progressBar1);
                Filtering.GrayScaleMedian(dstFt, gssrc, 5, w, h, progressBar1);
                dstFt = null;
                GC.Collect();
                if (setupMarkers)
                {
                    Binarization.GetMaskLT(gssrc, Mask, w, h, progressBar1,Binarization_W,Binarization_K);//was true!
                    ImageStaticData.GetOptimalParams(gssrc, w, h, ref sigma, ref algo, progressBar1);
                    float[,] fsrc = new float[w, h];
                    ImageConverter.ByteGsToFloatGs(gssrc, fsrc, w, h, progressBar1);
                    gssrc = null;
                    GC.Collect();
                    ClearProggressLabels();
                    label23.ForeColor = Color.FromArgb(0, 255, 0);
                    setUpMarkers(fsrc, Mask, w, h, sigma, algo);
                    ClearProggressLabels();
                    res = fsrc;
                }
                else
                {
                    float[,] fsrc = new float[w, h];
                    ImageConverter.ByteGsToFloatGs(gssrc, fsrc, w, h, progressBar1);
                    res = fsrc;
                }

               
            }
            else
            {
                //gssrc = null;
                GC.Collect();
                if (setupMarkers)
                {

                    Binarization.GetMaskLT(gssrc, Mask, w, h, progressBar1,Binarization_W,Binarization_K);//was true!
                    ImageStaticData.GetOptimalParams(dstFt, w, h, ref sigma, ref algo, progressBar1);
                    float[,] fsrc = new float[w, h];
                    ImageConverter.ByteGsToFloatGs(gssrc, fsrc, w, h, progressBar1);
                    dstFt = null;
                    label23.ForeColor = Color.FromArgb(0, 255, 0);
                    setUpMarkers(fsrc, Mask, w, h, sigma, algo);
                    ClearProggressLabels();
                    res = fsrc;
                }
                else
                {
                    float[,] fsrc = new float[w, h];
                    ImageConverter.ByteGsToFloatGs(dstFt, fsrc, w, h, progressBar1);
                    res = fsrc;
                }

            }


        }
        private void setupmarkersToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            float[,] f = null;
            FilterAndSetupMarkers(ref f,true);

        }

        private void segmentationOnMarkersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoIt(false);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap MyBmp = new Bitmap(MainPictureBox.Image);
            if (!SelectFrame.Done)
            {
                MessageBox.Show("Choose rectangle using your mouse");
                return;
            }

            UpdateProgessLabel("Cutting Image");
            ImageConverter.Cut(MyBmp, SelectFrame.GetSelectedRectangle(), progressBar1, MainPictureBox);
            Ellipses.Clear();
            DeleteAllMarkers();
            SaveTempImage((Bitmap)MainPictureBox.Image);
            SelectFrame.Done = false;
            updateWH();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ResizeAll();
        }

        private void grayScaleMedianFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainPictureBox.Image == null) return;
            int w = MainPictureBox.Image.Width;
            int h = MainPictureBox.Image.Height;
            Bitmap b = new Bitmap(MainPictureBox.Image);
            Byte[,] src = new Byte[MainPictureBox.Image.Width, MainPictureBox.Image.Height];
            Byte[,] dst = new Byte[MainPictureBox.Image.Width, MainPictureBox.Image.Height];
            ImageConverter.BitmapToImageGScale(b, src);
            Filtering.GrayScaleMedian(src, dst, 5, w, h, progressBar1);
            //ImageConverter.
            src = null;
            GC.Collect();
            ImageConverter.BitmapFromByteGs(ref b, dst, w, h);
            MainPictureBox.Image = b;

        }

        private void getReWorkedImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainPictureBox.Image == null) return;
            double sigma;
            int MinSeg;
            if (!Double.TryParse(textBox1.Text, out sigma))
            {
                MessageBox.Show("Input sigma for gauss smooth");
                return;
            }
          
            MyFloatColor[,] src = new MyFloatColor[MainPictureBox.Image.Width, MainPictureBox.Image.Height];

            ImageConverter.PictureboxToArrayRGBf(MainPictureBox, src);
            GC.Collect();
            int w = MainPictureBox.Image.Width;
            int h = MainPictureBox.Image.Height;
            GaussSmooth.SmoothRgbArray(src, sigma, w, h);
            Filtering.ChangeContrast(ContrastRange, src, w, h, progressBar1);
            byte[,] gssrc = new byte[w, h];
            byte[,] dstFt = new byte[w, h];
            ImageConverter.FloatToByteGs(src, gssrc, w, h, progressBar1);
            src = null;
            GC.Collect();
          
    
            int algo = 0;

            if (DoubleFilter)
            {
                Filtering.GrayScaleMedian(gssrc, dstFt, 3, w, h, progressBar1);
                Filtering.GrayScaleMedian(dstFt, gssrc, 5, w, h, progressBar1);
                dstFt = null;
                GC.Collect();
                Bitmap b = new Bitmap(w, h);
                ImageConverter.BitmapFromByteGs(ref b, gssrc, w, h);
                MainPictureBox.Image = b;

            }
            else
            {
                Bitmap b = new Bitmap(w, h);
                ImageConverter.BitmapFromByteGs(ref b, gssrc, w, h);
                MainPictureBox.Image = b;


            }

            GC.Collect();

        }

        private void getParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainPictureBox.Image == null) return;
            Bitmap b=new Bitmap(MainPictureBox.Image);
            byte [,]src =new byte[b.Width,b.Height];
            ImageConverter.BitmapToImageGScale(b,src);
            double sigma = 0 ;
            int algo = 0;
            ImageStaticData.GetOptimalParams(src, b.Width, b.Height, ref sigma, ref algo, progressBar1);
            textBox1.Text = sigma.ToString();
            textBox3.Text = algo.ToString();
        }

        private void setupMarkersOnReworkedImageUsingParamsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainPictureBox.Image == null) return;
            double sigma;
            int algo;
            if(!Double.TryParse(textBox1.Text,out sigma))
            {
                MessageBox.Show("Input or generate sigma");
                return;
            }
            if(!Int32.TryParse(textBox3.Text,out algo))
            {
                MessageBox.Show("Input or generate algo param");
                return;
            }
            int w=MainPictureBox.Image.Width;
            int h=MainPictureBox.Image.Height;
            float [,]src=new float[w,h];
            byte [,]bsrc=new byte[w,h];
            bool [,]Mask=new bool[w,h];
            Bitmap b=new Bitmap(MainPictureBox.Image);
            ImageConverter.BitmapToImageGScalef(b,src);
            ImageConverter.FloatGsToByteGs(src ,bsrc, w, h, progressBar1);
       
            UpdateLabel("getting Mask");

            Binarization.GetMaskLT(bsrc, Mask, w, h, progressBar1,Binarization_W,Binarization_K);
            setUpMarkers(src, Mask, w, h, sigma, algo);

        }

        private void resultSegmentedImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainPictureBox.Image == null) return;
            int minseg;
            if (!Int32.TryParse(textBox2.Text, out minseg))
            {
                MessageBox.Show("Choose minimal segment");
                return;
            }
            int w=MainPictureBox.Image.Width;
            int h=MainPictureBox.Image.Height;
            float[,] f = new float[w, h];
            byte[,] src = new byte[w, h];
            bool[,] Mask = new bool[w, h];
            Bitmap b = new Bitmap(MainPictureBox.Image);
            ImageConverter.BitmapToImageGScalef(b, f);
            ImageConverter.FloatGsToByteGs(f, src, w, h, progressBar1);
            f = null;
            GC.Collect();
            UpdateLabel("getting Mask");

            Binarization.GetMaskLT(src, Mask, w, h, progressBar1,Binarization_W,Binarization_K);
            UpdateLabel("Clearing Mask");
            Binarization.DeleteInnerEffects(Mask, minseg, w, h, progressBar1);

            //waterShed
            int[,] Result = new int[w, h];
            UpdateLabel("WaterSHed Segmentation");
            WaterShed.Segmentation(src, w, h, MarkerPositions, Mask, Result, progressBar1);

            src = null;
            Mask = null;
            GC.Collect();
            Bitmap b2 = null;
            UpdateLabel("Segments Visualizing");
            SegmentVisualizer.ColorBitmap(ref b2, Result, w, h, Labels.Count, progressBar1);
            pictureBox1.Image = b2;
            pictureBox1.Visible = true;
            MainPictureBox.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            LoadImage();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSelectedLabel();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSelectedLabel();
            SizePattern.ChangeScale(MainPictureBox);
           
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSelectedLabel();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSelectedLabel();
        }

        private void optioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Options f = new Options();
            f.Show();
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            if (!radioButton2.Checked) return;
            if (MainPictureBox.Image == null) return;
            Bitmap MyBmp = new Bitmap(MainPictureBox.Image);
            if (!SelectFrame.Done)
            {
                //MessageBox.Show("Choose rectangle using your mouse");
                return;
            }

            UpdateProgessLabel("Cutting Image");
            Rectangle r = SelectFrame.GetSelectedRectangle();
            if(r.Width ==0 || r.Height==0)
            {
                MessageBox.Show("Choose rectangle using mouse");
                return;
            }
            ImageConverter.Cut(MyBmp, SelectFrame.GetSelectedRectangle(), progressBar1, MainPictureBox);
            Ellipses.Clear();
            DeleteAllMarkers();
            SaveTempImage((Bitmap)MainPictureBox.Image);
            SelectFrame.Done = false;
            updateWH();
            PatternH.ChangeScale(MainPictureBox);
            PatternV.ChangeScale(MainPictureBox);
            SizePattern.ChangeScale(MainPictureBox);
            SelectFrame.ChangeScale(MainPictureBox);
            Distributer.ChangeScale(MainPictureBox);
        }

        private void HeightLabel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            Directory.SetCurrentDirectory(StartDir);
            MainPictureBox.Load(SFile);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            string Message = "this files were created by program:" + Environment.NewLine ;
            if (teMPoFiles.Count == 0) return;
            foreach(string s in teMPoFiles)
            {
                Message += s + Environment.NewLine;
            }
            Message += "Do you wand to delete them?";

            if (MessageBox.Show(Message, "!", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Directory.SetCurrentDirectory(StartDir);
                foreach (string s in teMPoFiles)
                {
                    File.Delete(s);
                }
            }
        }

        private void binarizationWithLocalThresHoldingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainPictureBox.Image == null) return;
            Bitmap b=(Bitmap)MainPictureBox.Image;
            int w = b.Width;
            int h = b.Height;
            byte[,] src = new byte[w, h];
            bool[,] Mask = new bool[w, h];
            ImageConverter.BitmapToImageGScale(b, src);
            Binarization.GetMaskLT(src, Mask, w, h, progressBar1,Binarization_W,Binarization_K);
            ImageConverter.BitmapFromBool(ref b, Mask, w, h);
            pictureBox1.Image = b;
            pictureBox1.Visible = true;
            MainPictureBox.Visible = false;

        }
        
        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            Distributer.ChangeScale(MainPictureBox);
            UpdateSelectedLabel();
        }

        private void radioButton6_Click(object sender, EventArgs e)
        {
            if (!radioButton6.Checked) return;
            if (MainPictureBox.Image == null) return;
         
            Bitmap MyBmp = new Bitmap(MainPictureBox.Image);

            Rectangle r = Distributer.GetSelectedRectangle();
            if (r.Width == 0 || r.Height == 0)
            {
                //MessageBox.Show("Choose rectangle using mouse");
                Distribute((int)(DType),new Rectangle(0,0,MainPictureBox.Image.Width,MainPictureBox.Image.Height));
                return;
            }
            Distribute((int)DType, r);
           
        }

        private void MainPictureBox_LoadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            MessageBox.Show("LOL");
            if (radioButton6.Checked && Distributer.GetSelectedRectangle().Width > 0 && Distributer.GetSelectedRectangle().Height > 0)
            {
                Distributer.Draw(false);
            }
        }

        private void MainPictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (MainPictureBox.Image == null) return;
            if (radioButton6.Checked && Distributer.GetSelectedRectangle().Width > 0 && Distributer.GetSelectedRectangle().Height > 0)
            {
                Distributer.Draw(false);
            }
            if (ShowBestFitEllipses)
            {

                double dx = 1.0 * MainPictureBox.Image.Width / MainPictureBox.Size.Width;
                double dy = 1.0 * MainPictureBox.Image.Height / MainPictureBox.Size.Height;
                Graphics g = Graphics.FromHwnd(MPicture);
            
                foreach (ElipseToDraw E in Ellipses)
                {
                
                    g.DrawEllipse(new Pen(Color.YellowGreen), (float)(E.x - E.w / 2), (float)(E.y - E.h / 2), (float)E.w, (float)E.h);
                }
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (pictureBox1.Image == null) return;
            if (radioButton6.Checked && Distributer.GetSelectedRectangle().Width > 0 && Distributer.GetSelectedRectangle().Height > 0)
            {
                Distributer.DrawToPicture2(false);
            }
            if (ShowBestFitEllipses)
            {

                double dx = 1.0 * pictureBox1.Image.Width / pictureBox1.Size.Width;
                double dy = 1.0 * pictureBox1.Image.Height / pictureBox1.Size.Height;
                Graphics g = Graphics.FromHwnd(SPicture);
                foreach (ElipseToDraw E in Ellipses)
                {
                    if (E.x == -2147483648) continue;
                        g.DrawEllipse(new Pen(Color.YellowGreen), (float)(E.x - E.w / 2), (float)(E.y - E.h / 2), (float)E.w, (float)E.h);
                  
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Distributer.Exist)
            {
                Refresh();
                Distributer.SetFinish(e.X, e.Y);
                Distributer.DrawToPicture2(false);

            }

        }

        private void pictureBox1_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (MainPictureBox.Image == null) return;
            RedrawAllMarkers();
            if (e.Button == MouseButtons.Right)
            {
                swapBoxes();
                return;
            }
            Refresh();
            if (radioButton6.Checked )
            {
                Distributer.Exist = true;
                Distributer.SetStart(e.X, e.Y);
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
           
            if (radioButton6.Checked)
            {
                Distributer.SetFinish(e.X, e.Y);
                Distributer.Draw(true);
            }
        }

        private void getCurrentImageMidValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainPictureBox.Image == null) return;
            int w=MainPictureBox.Image.Width;
            int h=MainPictureBox.Image.Height;
            byte[,] Img = new byte[w, h];
            Bitmap b=(Bitmap)MainPictureBox.Image;
            ImageConverter.BitmapToImageGScale(b, Img);
            MessageBox.Show(ImageStaticData.getCurrentImageDisper(Img,w,h,progressBar1).ToString());
        }

        private void showSigmaGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GraphForm G = new GraphForm("Sigma");
            List<double> y=new List<double>();
            for (int x = 0; x <= 200; x++)
            {
                y.Add(ImageStaticData.getSigma(x));
            }
            G.DrawGraph(y);
            G.Show();
        }

        private void showAlgoParametGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GraphForm G = new GraphForm("Algo Parameter");
            List<double> y = new List<double>();
            for (int x = 0; x <= 200; x++)
            {
                y.Add(ImageStaticData.GetalgoParam(x));
            }
            G.DrawGraph(y);
            G.Show();
        }

        private void setUpMarkersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float[,] f = null;
            FilterAndSetupMarkers(ref f, true);
        }
        private Pcnn PN;
        private void segmentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int W=MainPictureBox.Image.Width;
            int H=MainPictureBox.Image.Height;
            byte[,] buf = new byte[W, H];
            ImageConverter.BitmapToImageGScale((Bitmap)MainPictureBox.Image,buf);
            PN = new Pcnn(W, H, buf, 1,progressBar1.Handle);
            PN.NextSegmentIteration(this.progressBar1.Handle);
            PN.LoadToBitmap(SPicture);
            swapBoxes();
        }

        private void nextIterationNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PN.NextSegmentIteration(this.progressBar1.Handle);
            PN.LoadToBitmap(SPicture);
            swapBoxes();
        }

        private void generateAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            
            int W = MainPictureBox.Image.Width;
            int H = MainPictureBox.Image.Height;
            byte[,] buf = new byte[W, H];
            ImageConverter.BitmapToImageGScale((Bitmap)MainPictureBox.Image, buf);
            PN = new Pcnn(W, H, buf, 0, this.progressBar1.Handle);
            Chooser ch = new Chooser(10,PN);
            ch.Show();
            PN.NextSegmentIteration(this.progressBar1.Handle);
            //PN.LoadToBitmap(SPicture);
            for (int i = 0; i < 10; i++)
            {
                PN.NextSegmentIteration(ch.GetProgressBarHandle(i));
                PN.LoadToBitmap(ch.GetPictureBoxHandle(i));
            }

           // PN.ClearAll();

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }





    }

}

