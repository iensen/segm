using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace WindowsFormsApplication3
{
    class Filtering
    {
        private static void NormalizeRGB(ref int x)
        {
            if (x > 255) x = 255;
            if (x <= 0) x = 0;
        }
        private static void NormalizeRGBf(ref float x)
        {
            if (x > 1) x = 1;
            if (x < 0) x = 0;
        }
        public static void ChangeContrast(PictureBox p1, ProgressBar p2,double value)
        {
            Bitmap b = new Bitmap(p1.Image);
            ChangeContrast(b,p2, value);
            p1.Image = b;
        }
        public static void GrayScaleMedian(byte[,] InP, byte[,] OutP, int size, int w, int h, ProgressBar P1)
        {

            P1.Minimum = 0;
            P1.Maximum = w;
            P1.Value = 0;
            int[] curc = new int[size * size + 2];
            int ind, R, G, B, k1, p1, C;
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                {
                    ind = 0;
                    for (int k = i - size / 2; k <= i + size / 2; k++)
                        for (int p = j - size / 2; p <= j + size / 2; p++)
                        {
                            k1 = k;
                            p1 = p;
                            while (k < 0) k++;
                            while (k >= w) k--;
                            while (p < 0) p++;
                            while (p >= h) p--;
                            curc[ind] = InP[k, p];
                            ind++;
                            k = k1;
                            p = p1;



                        }
                    C = BitmapFilter.GetKth(curc, 0, size * size - 1, (size * size - 1) / 2, false, 256);


                    OutP[i, j] = (byte)C;
                    P1.Value = i;
                }

        }

        public static unsafe void ChangeContrast(double value,MyFloatColor[,] src, int w, int h, ProgressBar p1)
        {
            MainForm.UpdateLabel("Updating Contrast");
            p1.Maximum = (2 * w) / 10;
            p1.Value = 0;
            int Pvalue = 0;
            //Calculate Average;
            double avR = 0, avG = 0, avB = 0;
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {


                    avR += src[i, j].R;
                    avG += src[i, j].G;
                    avB += src[i, j].B;
                }
                Pvalue++;
                if (Pvalue == 10)
                {
                    Pvalue = 0;
                    p1.Value++;
                }

            };

            avB /= (w * h);
            avG /= (w * h);
            avR /= (w * h);
            float R, G, B;

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                   


                    R = (float)((src[i,j].R - avR) * value + avR);
                    G = (float)((src[i,j].G - avG) * value + avG);
                    B = (float)((src[i,j].B - avB) * value + avB);
                    NormalizeRGBf(ref B);
                    NormalizeRGBf(ref R);
                    NormalizeRGBf(ref G);

                    src[i,j].R = R;
                    src[i, j].G = G;
                    src[i, j].B = B;

                }
                Pvalue++;
                if (Pvalue == 10)
                {
                    Pvalue = 0;
                    p1.Value++;
                }
            }
          

            return;

        }
        public static unsafe void ChangeContrast(Bitmap b, ProgressBar p1,double value)
        {
            MainForm.UpdateLabel("Updating Contrast");
            p1.Maximum = (2*b.Width) / 10;
            p1.Value = 0;
            int Pvalue = 0;
            //Calculate Average;
            double avR = 0, avG = 0, avB = 0;
            Color c = new Color();
            BitmapData bData = b.LockBits(new Rectangle(0, 0,b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);

            byte bitsPerPixel = (byte)Image.GetPixelFormatSize(b.PixelFormat);
            
            
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            for (int i = 0; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {

                    byte* data = scan0 + j * bData.Stride + i * bitsPerPixel / 8;
                    //b.UnlockBits(bData);
                    //c = b.GetPixel(i, j);
                    avR += data[2];
                    avG += data[1];
                    avB += data[0];
                }
                Pvalue++;
                if (Pvalue == 10)
                {
                    Pvalue = 0;
                    p1.Value++;
                }

            };
         
            avB /= (b.Width * b.Height);
            avG /= (b.Width * b.Height);
            avR /= (b.Width * b.Height);
            int R, G, B;

            for (int i = 0; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {
                    byte* data = scan0 + j * bData.Stride + i * bitsPerPixel / 8;

                    
                    R = Convert.ToInt32((data[2] - avR) * value + avR);
                    G = Convert.ToInt32((data[1] - avG) * value + avG);
                    B = Convert.ToInt32((data[0] - avB) * value + avB);
                    NormalizeRGB(ref B);
                    NormalizeRGB(ref R);
                    NormalizeRGB(ref G);

                    data[2] = (byte)R;
                    data[1] = (byte)G;
                    data[0] = (byte)B;

                }
                Pvalue++;
                if (Pvalue == 10)
                {
                    Pvalue = 0;
                    p1.Value++;
                }
            }
            b.UnlockBits(bData);

            return;

        }

        public static unsafe void Median(Bitmap b, int size)
        {

            int[] curR = new int[size * size];
            int[] curG = new int[size * size];
            int[] curB = new int[size * size];
            byte[,] rc = new byte[b.Width, b.Height];
            byte[,] gc = new byte[b.Width, b.Height];
            byte[,] bc = new byte[b.Width, b.Height];
            ProgressBar progressBar1 = (ProgressBar)ProgressBar.FromHandle(MainForm.ProgressBr);
            progressBar1.Value = 0;
            progressBar1.Maximum = 2 * b.Width/10;
            int pvalue = 0;
            Color c = new Color();
            BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            byte bitsPerPixel = (byte)Image.GetPixelFormatSize(b.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            for (int i = 0; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {
                    byte* data = scan0 + j * bData.Stride + i * bitsPerPixel / 8;

                    rc[i, j] = data[2];
                    gc[i, j] = data[1];
                    bc[i, j] = data[0];


                }
                pvalue++;
                if(pvalue==10)
                {
                    progressBar1.Value++;
                    pvalue = 0;
                }
            }
            int ind, R, G, B, k1, p1;
            for (int i = 0; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {
                    byte* data = scan0 + j * bData.Stride + i * bitsPerPixel / 8;
                    ind = 0;
                    for (int k = i - size / 2; k <= i + size / 2; k++)
                        for (int p = j - size / 2; p <= j + size / 2; p++)
                        {
                            k1 = k;
                            p1 = p;
                            while (k < 0) k++;
                            while (k >= b.Width) k--;
                            while (p < 0) p++;
                            while (p >= b.Height) p--;
                            curR[ind] = rc[k, p];
                            curG[ind] = gc[k, p];
                            curB[ind] = bc[k, p];

                            ind++;
                            k = k1;
                            p = p1;



                        }


                    data[2] = (byte)(R = (byte)BitmapFilter.GetKth(curR, 0, size * size - 1, (size * size - 1) / 2, false, 256));
                    data[1] = (byte)(G = (byte)BitmapFilter.GetKth(curG, 0, size * size - 1, (size * size - 1) / 2, false, 256));
                    data[0] = (byte)(B = (byte)BitmapFilter.GetKth(curB, 0, size * size - 1, (size * size - 1) / 2, false, 256));
                    

                }
                pvalue++;
                if (pvalue == 10)
                {
                    progressBar1.Value++;
                    pvalue = 0;
                }
            }
            b.UnlockBits(bData);
        }
      
    }


   
}
