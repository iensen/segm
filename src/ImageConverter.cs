/*
 * class for working and converting bitmap to Arrays of different types 
 * and for copy a part of bitmap to another bitmap
 */
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Drawing.Imaging;

namespace WindowsFormsApplication3
{
    public class ImageConverter
    {
        
        
        public static unsafe void Cut(Bitmap FilledBmb,  Rectangle R, ProgressBar pR, PictureBox P1)//it cuts a part of Picture to anoter picture
        {
            Bitmap newBmp = new Bitmap(R.Width, R.Height);
            int Pvalue = 0;
            pR.Value = 0;
            pR.Maximum = (R.Right - R.Left) / 10;

            BitmapData FbData = FilledBmb.LockBits(new Rectangle(0, 0, FilledBmb.Width, FilledBmb.Height), ImageLockMode.ReadWrite, FilledBmb.PixelFormat);
            BitmapData NbData = newBmp.LockBits(new Rectangle(0, 0, newBmp.Width, newBmp.Height), ImageLockMode.ReadWrite, FilledBmb.PixelFormat);
            byte bitsPerPixel = (byte)Image.GetPixelFormatSize(FilledBmb.PixelFormat);
            byte* scan0 = (byte*)FbData.Scan0.ToPointer();
            byte* scan1 = (byte*)NbData.Scan0.ToPointer();
           
    
            try
            {
                for (int i = R.Left; i < R.Right; i++)
                {
                    for (int j = R.Top; j < R.Bottom; j++)
                    {
                        byte* data1 = scan0 + j * FbData.Stride + i * bitsPerPixel / 8;
                        byte* data2 = scan1 + (j - R.Top) * NbData.Stride + (i - R.Left) * bitsPerPixel / 8;
                        data2[2] = data1[2];
                        data2[1] = data1[1];
                        data2[0] = data1[0];
                        data2[3] = data1[3];
              

                    }
                    Pvalue++;
                    if (Pvalue == 10)
                    {
                        Pvalue = 0;

                        pR.Value++;
                        Application.DoEvents();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Your selection was incorrect");
                return;
            }
           
             
                FilledBmb.UnlockBits(FbData);
                newBmp.UnlockBits(NbData);
              
            
            P1.Image = newBmp;
            P1.Invalidate();
   
        }

        public static void PictureboxToArrayRGBf(PictureBox pictureBox, MyFloatColor[,] Cls)
        {

            Bitmap newbmp1 = new Bitmap(pictureBox.Image);
            BitmapToArrayRGBf(newbmp1, Cls);

        }
        public static unsafe void BitmapToArrayRGBf(Bitmap b, MyFloatColor[,] Cls)
        {
            ProgressBar p = (ProgressBar)ProgressBar.FromHandle(MainForm.ProgressBr);
            MainForm.UpdateLabel("Getting Information From PictureBox"); 
            int w = b.Width;
            int h = b.Height;
            p.Maximum = w / 10;
            Color c;
            p.Value = 0;
            int CurPvalue = 0;
            BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            byte bitsPerPixel = (byte)Image.GetPixelFormatSize(b.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    byte* data = scan0 + j * bData.Stride + i * bitsPerPixel / 8;
                    Cls[i, j].R = data[2] * 1.0f / 255;
                    Cls[i, j].G = data[1] * 1.0f / 255;
                    Cls[i, j].B = data[0] * 1.0f / 255;

                }
                CurPvalue++;
                if (CurPvalue == 10)
                {
                    p.Value++;
                    CurPvalue = 0;
                }
            }

            b.UnlockBits(bData);
        }
        public static void PictureboxToArrayRGB(PictureBox pictureBox, MyByteColor[,] Cls)
        {

            Bitmap newbmp1 = new Bitmap(pictureBox.Image);
            BitmapToArrayRGB(newbmp1, Cls);

        }

        public static unsafe void BitmapToArrayRGB(Bitmap b, MyByteColor[,] Cls)
        {
            ProgressBar p = (ProgressBar)ProgressBar.FromHandle(MainForm.ProgressBr);
            MainForm.UpdateLabel("Getting Information From PictureBox");
            int w = b.Width;
            int h = b.Height;
            p.Maximum = w / 10;
            Color c;
            p.Value = 0;
            int CurPvalue = 0;
            BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            byte bitsPerPixel = (byte)Image.GetPixelFormatSize(b.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    byte* data = scan0 + j * bData.Stride + i * bitsPerPixel / 8;
                    Cls[i, j].R = data[2];
                    Cls[i, j].G = data[1];
                    Cls[i, j].B = data[0];

                }
                CurPvalue++;
                if (CurPvalue == 10)
                {
                    p.Value++;
                    CurPvalue = 0;
                }
            }

            b.UnlockBits(bData);
        }
        public static void PictureBoxToImageGScalef(PictureBox pictureBox, float[,] Cls)
        {

            Bitmap newbmp1 = new Bitmap(pictureBox.Image);
            BitmapToImageGScalef(newbmp1,Cls);
            

        }
        public static void PictureBoxToImageGScale(PictureBox pictureBox, byte[,] Cls)
        {

            Bitmap newbmp1 = new Bitmap(pictureBox.Image);
            BitmapToImageGScale(newbmp1, Cls);


        }
        public static unsafe void BitmapToImageGScalef(Bitmap b, float [,]Cls)
        {
            MainForm.UpdateLabel("Getting Information From PictureBox");
            ProgressBar p = (ProgressBar)ProgressBar.FromHandle(MainForm.ProgressBr);
            int w = b.Width;
            int h = b.Height;
            p.Maximum = w / 10;
            int Pvalue = 0;
            p.Value = 0;
           
            BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            byte bitsPerPixel = (byte)Image.GetPixelFormatSize(b.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    byte* data = scan0 + j * bData.Stride + i * bitsPerPixel / 8;
                    Cls[i, j] = data[2] * 1.0f / 255;

                }
                Pvalue++;
                if (Pvalue == 10)
                {
                    p.Value++;
                    Pvalue = 0;
                }
            }
            b.UnlockBits(bData);

        }
        public static void FloatGStoFloatRGB(float[,] src, MyFloatColor[,] dst, int w, int h, ProgressBar p1)
        {
            MainForm.UpdateLabel("Image Converting");
            p1.Value = 0;
            int pvalue = 0;
            p1.Minimum = 0;
            p1.Maximum = w / 10;
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    dst[i, j].R = dst[i, j].G = dst[i, j].B = src[i, j];
                }
                pvalue++;
                if (pvalue == 10)
                {
                    p1.Value++;
                    pvalue = 0;
                }
            }
        }
        public static void ByteGsToFloatGs(byte[,]src,float[,]dst,int w,int h,ProgressBar p1)
        {
            MainForm.UpdateLabel("Image Converting");
            p1.Value = 0;
            int pvalue = 0;
            p1.Minimum = 0;
            p1.Maximum = w / 10;
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    dst[i, j] = src[i, j] * 1.0f / 255;
                }
                pvalue++;
                if (pvalue == 10)
                {
                    p1.Value++;
                    pvalue = 0;
                }
            }
        }
        public static void FloatGsToByteGs(float[,] src, byte[,] dst, int w, int h, ProgressBar p1)
        {
            MainForm.UpdateLabel("Image Converting");
            p1.Value = 0;
            int pvalue = 0;
            p1.Minimum = 0;
            p1.Maximum = w / 10;
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    dst[i, j] = (byte)(src[i, j] *255);
                }
                pvalue++;
                if (pvalue == 10)
                {
                    p1.Value++;
                    pvalue = 0;
                }
            }
        }
        
        public static void FloatToByteGs(MyFloatColor[,] src, byte[,] Dst,int w,int h,ProgressBar p1)
        {
            MainForm.UpdateLabel("Image Converting to Grayscale");
            p1.Value = 0;
            int pvalue = 0;
            p1.Minimum = 0;
            p1.Maximum = w / 10;
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    double c1 = 0.3 * src[i, j].R + 0.59 * src[i, j].G + 0.11 * src[i, j].B;
                    Dst[i, j] = (byte)(c1 * 255);
                }
                pvalue++;
                if (pvalue == 10)
                {
                    p1.Value++;
                    pvalue = 0;
                }
            }
        }
        public static unsafe void BitmapToImageGScale(Bitmap b, byte[,] Cls)
        {
            MainForm.UpdateLabel("Getting Information From PictureBox");
            ProgressBar p = (ProgressBar)ProgressBar.FromHandle(MainForm.ProgressBr);
            int w = b.Width;
            int h = b.Height;
            p.Maximum = w / 10;
            int Pvalue = 0;
            p.Value = 0;

            BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            byte bitsPerPixel = (byte)Image.GetPixelFormatSize(b.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    byte* data = scan0 + j * bData.Stride + i * bitsPerPixel / 8;
                    Cls[i, j] = data[2];

                }
                Pvalue++;
                if (Pvalue == 10)
                {
                    p.Value++;
                    Pvalue = 0;
                }
            }
            b.UnlockBits(bData);

        }
        public static void PictureBoxToImageBinary(PictureBox pictureBox, bool[,] Cls)
        {

            Bitmap newbmp1 = new Bitmap(pictureBox.Image);
            BitmapToImageBinary(newbmp1, Cls);


        }
        public static unsafe void BitmapToImageBinary(Bitmap b, bool[,] Cls)
        {
            MainForm.UpdateLabel("Getting Information From PictureBox");
            ProgressBar p = (ProgressBar)ProgressBar.FromHandle(MainForm.ProgressBr);
            int w = b.Width;
            int h = b.Height;
            p.Maximum = w / 10;
            int Pvalue = 0;
            Color c;
            p.Value = 0;
            BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            byte bitsPerPixel = (byte)Image.GetPixelFormatSize(b.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    byte* data = scan0 + j * bData.Stride + i * bitsPerPixel / 8;
                    Cls[i, j] = (data[0]==255)?true:false ;

                }
                Pvalue++;
                if (Pvalue == 10)
                {
                    p.Value++;
                    Pvalue = 0;
                }
            }
            b.UnlockBits(bData);
        }

        public static void PictureBoxFromImageRGBf(PictureBox p, MyFloatColor[,] tbl, int w, int h)
        {
            Bitmap newbmp = new Bitmap(w, h);
            BitmapFromImageRGBf(ref newbmp, tbl);
            p.Image = newbmp;
        }
        public static unsafe void BitmapFromImageRGBf(ref Bitmap b, MyFloatColor[,] tbl)
        {
            MainForm.UpdateLabel("Loading information to PictureBox");
            ProgressBar pr = (ProgressBar)ProgressBar.FromHandle(MainForm.ProgressBr);
            int w = b.Width;
            int h = b.Height;
            pr.Maximum = w / 10;
            int Pvalue = 0;
            pr.Value = 0;
            Color c;
            BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            byte bitsPerPixel = (byte)Image.GetPixelFormatSize(b.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    byte* data = scan0 + j * bData.Stride + i * bitsPerPixel / 8;
                    data[2] = (byte)(tbl[i, j].R * 255);
                    data[1] = (byte)(tbl[i, j].G * 255);
                    data[0] = (byte)(tbl[i, j].B * 255);
                    data[3] = 255;
                  
                }
                Pvalue++;
                if (Pvalue == 10)
                {
                    pr.Value++;
                    Pvalue = 0;
                }
            }
            b.UnlockBits(bData);
        }
        public static unsafe void BitmapFromByteGs(ref Bitmap b, byte[,] tbl, int w, int h)
        {
            MainForm.UpdateLabel("Loading information to PictureBox");
            ProgressBar pr = (ProgressBar)ProgressBar.FromHandle(MainForm.ProgressBr);
            b = new Bitmap(w, h);
            pr.Maximum = w / 10;
            int Pvalue = 0;
            pr.Value = 0;
            Color c;
            BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            byte bitsPerPixel = (byte)Image.GetPixelFormatSize(b.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    byte* data = scan0 + j * bData.Stride + i * bitsPerPixel / 8;
                    data[2] = tbl[i, j];
                    data[1] = tbl[i, j];
                    data[0] = tbl[i, j];
                    data[3] = 255;

                }
                Pvalue++;
                if (Pvalue == 10)
                {
                    pr.Value++;
                    Pvalue = 0;
                }
            }
            b.UnlockBits(bData);
        }
       
        public static unsafe void BitmapFromBool(ref Bitmap b,bool[,] tbl,int w,int h)
        {
            MainForm.UpdateLabel("Loading information to PictureBox");
            ProgressBar pr = (ProgressBar)ProgressBar.FromHandle(MainForm.ProgressBr);
            b = new Bitmap(w, h);
            pr.Maximum = w / 10;
            int Pvalue = 0;
            pr.Value = 0;
            Color c;
            BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            byte bitsPerPixel = (byte)Image.GetPixelFormatSize(b.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    byte* data = scan0 + j * bData.Stride + i * bitsPerPixel / 8;
                    data[2] = tbl[i, j] ? (byte)(255):(byte)0;
                    data[1] = tbl[i, j] ? (byte)(255) : (byte)0;
                    data[0] = tbl[i, j] ? (byte)(255) : (byte)0;
                    data[3] = 255;

                }
                Pvalue++;
                if (Pvalue == 10)
                {
                    pr.Value++;
                    Pvalue = 0;
                }
            }
            b.UnlockBits(bData);
        }
        public static unsafe void ApplyMask( Bitmap b, bool[,] bits, int w, int h)
        {
            MainForm.UpdateLabel("Loading information to PictureBox");
            ProgressBar pr = (ProgressBar)ProgressBar.FromHandle(MainForm.ProgressBr);
           // b = new Bitmap(w, h);
            pr.Maximum = w / 10;
            int Pvalue = 0;
            pr.Value = 0;
            Color c;
            BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            byte bitsPerPixel = (byte)Image.GetPixelFormatSize(b.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    if (bits[i, j])
                    {
                       // for (int k = i - 1; k <= i + 1; k++)
                        //{
                          //  for (int t = j - 1; t <= j + 1; t++)
                            //{
                              //  if (k < w && k > 0 && t < h && t > 0)
                                //{
                                    byte* data = scan0 + j * bData.Stride + i * bitsPerPixel / 8;
                                    data[2] = (byte)0;
                                    data[1] = (byte)0;
                                    data[0] = (byte)0;
                                    data[3] = 255;
                                //}

                            //}
                        //}
                       
                    }
                
                }
                Pvalue++;
                if (Pvalue == 10)
                {
                    pr.Value++;
                    Pvalue = 0;
                }
            }
            b.UnlockBits(bData);
        }
        public static void PictureBoxFromImageGscalef(PictureBox p,float[,] tbl, int w, int h)
        {
            Bitmap newbmp = new Bitmap(w, h);
            BitmapFromImageGScalef(ref newbmp, tbl);
            p.Image = newbmp;
        }
        public static unsafe void BitmapFromImageGScalef(ref Bitmap b,float[,] tbl)
        {
            MainForm.UpdateLabel("Loading information to PictureBox");
            ProgressBar pr = (ProgressBar)ProgressBar.FromHandle(MainForm.ProgressBr);
            int w = b.Width;
            int h = b.Height;
            pr.Maximum = w / 10;
            int Pvalue = 0;
            pr.Value = 0;
            Color c;
            BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            byte bitsPerPixel = (byte)Image.GetPixelFormatSize(b.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    byte* data = scan0 + j * bData.Stride + i * bitsPerPixel / 8;
               
                    data[2] = data[1]=data[0]=(byte)(tbl[i,j]*255);
                    
                }
                Pvalue++;
                if (Pvalue == 10)
                {
                    pr.Value++;
                    Pvalue = 0;
                }
            }
            b.UnlockBits(bData);
        }


        public static unsafe void ToGrayScale(Bitmap b, ProgressBar P1)
        {
            if (b == null) return;
            Color c = new Color();
            P1.Value = 0;
            P1.Minimum = 0;
            P1.Maximum = b.Width;
            BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            byte bitsPerPixel = (byte)Image.GetPixelFormatSize(b.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            for (int i = 0; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {
                    byte* data = scan0 + j * bData.Stride + i * bitsPerPixel / 8;
                    double c1 = 0.3 * data[2] + 0.59 * data[1] + 0.11 * data[0];
                    data[1]=data[0]=data[2] = (byte)c1;
 
                }
                P1.Value = i;
            }
            b.UnlockBits(bData);
          
        }
        public static void ToGrayScale(PictureBox pict, ProgressBar p1)
        {
            Bitmap b = new Bitmap(pict.Image);
            ToGrayScale(b, p1);
            pict.Image = b;
        }
       

    }


    
}
