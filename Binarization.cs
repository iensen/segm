using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;

using System.Windows.Forms;
using System.IO;
using ZedGraph;
using System.Globalization;
using System.Threading;
namespace WindowsFormsApplication3
{
    public class Binarization
    {
        private static int GetOptimalTreshold(int[] P)
        {
            int[] sumS = new int[256];
            float[] NormalSum = new float[256];
            int Allsum = 0;
            foreach (int s in P)
            {
                Allsum += s;
            }
            float ui = 0;
            for (int i = 0; i < 256; i++)
            {
                NormalSum[i] = (float)P[i] / (float)Allsum;
                ui += NormalSum[i] * i;

            }
            float mindisp = -1;
            int FoundT = 0;
            for (int T = 0; T < 256; T++)//Optim
            {
                float q0 = 0, qf = 0, iq0 = 0, iqf = 0;
                for (int i = 0; i < T; i++)
                {
                    q0 += NormalSum[i];
                    iq0 += i * NormalSum[i];
                }
                for (int i = T + 1; i <= 255; i++)
                {
                    qf += NormalSum[i];
                    iqf += i * NormalSum[i];
                }

                float u0 = iq0 / q0;
                float uf = iqf / qf;
                float disp = (uf - u0) * (uf - u0) * (q0 * qf);
                if (disp > mindisp)
                {
                    mindisp = disp;
                    FoundT = T;

                }


            }

            return FoundT;//it was 3/4 before

        }
        public static void GetMask(byte[,] src, bool[,] Mask, int w, int h, ProgressBar p1, bool less)
        {
            MainForm.UpdateLabel("Getting Mask");
            p1.Minimum = 0;
            p1.Value = 0;
            p1.Maximum = 2 * w / 10;
            int pvalue = 0;
            int[] P = new int[256];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    P[src[i, j]]++;
                }

            }
            int thres;
            if (less) thres = (int)(3 * GetOptimalTreshold(P) / 4);
            else
            {
                thres = (int)(15 * GetOptimalTreshold(P) / 16);
            }
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {

                    if (src[i, j] > thres)
                    {

                        Mask[i, j] = true;
                    }
                    else
                    {
                        Mask[i, j] = false;
                    }
                }
                pvalue++;
                if (pvalue == 10)
                {
                    p1.Value++;
                    pvalue = 0;
                }
            }

        }

        private static void CalculateGS(byte[,]src,int [,]G,Int64[,] S,int w,int h,ProgressBar p1)
        {
            MainForm.UpdateLabel("Calculating integral images");
            G[0, 0] = src[0, 0];
            S[0, 0] = src[0, 0] * src[0, 0];
            byte pvalue = 0;
            p1.Value = 0;
            p1.Minimum = 0;
            p1.Maximum = w / 10;
            for (int i = 0; i < w; i++)//i-the number of COLUMN,i.e X
            {
                for (int j = 0; j < h; j++)
                {
                    if (i == 0)
                    {
                        if (j == 0) continue;
                        G[i, j] = G[i, j - 1] + src[i, j];
                        S[i, j] = S[i, j - 1] + (int)src[i, j ] * src[i, j ];
                    }
                    else if (j == 0)
                    {
                        if (i == 0) continue;
                        G[i, j] = G[i - 1, j] + src[i, j];
                        S[i, j] = S[i - 1, j] + (int)src[i, j] * src[i, j];
                    }
                    else
                    {
                        G[i,j] = G[i - 1, j] + G[i, j - 1] - G[i - 1, j - 1] + src[i, j];
                        S[i,j] = S[i - 1, j] + S[i, j - 1] - S[i - 1, j - 1] + (int)src[i, j]*src[i,j];
                    }

                }
                pvalue++;
                if (pvalue == 10)
                {
                    pvalue = 0;
                    p1.Value++;
                }
            }
        }
        public static double GetLocalVariance(int[,] G, long[,] Gs, int w, int h,int WindowSz,int x,int y,double k)
        {
            int MinX = Math.Max(0, x - WindowSz / 2 - 1);
            int MaxX = Math.Min(x + WindowSz / 2, w - 1);
            int MinY = Math.Max(0, y - WindowSz / 2 - 1);
            int MaxY = Math.Min(y + WindowSz / 2, h - 1);
            int MinXa = Math.Max(0, x - WindowSz / 2);
            int MinYa = Math.Max(0, y - WindowSz / 2);
            int Area = (MaxX - MinXa + 1) * (MaxY - MinYa + 1);
            Int64 answ = (long)Gs[MaxX, MaxY] + Gs[MinX, MinY] - Gs[MaxX, MinY] - Gs[MinX, MaxY];
            long Im = (long)(G[MaxX, MaxY] + G[MinX, MinY] - G[MaxX, MinY] - G[MinX, MaxY]);
            double a = (double)answ/Area-(double)Im/Area/Area*Im;
            a /= Area;
           double s= Math.Sqrt(a);
            return (double)Im /(Area)* (1 + k * (s / 128.0 - 1));

        }
   
        public static void GetMaskLT(byte[,] src, bool[,] Mask, int w, int h, ProgressBar p1, int W,double k)
        {
            int[,] G = new int[w, h];
            long[,] GS = new long[w, h];
            CalculateGS(src, G, GS, w, h, p1);
            MainForm.UpdateLabel("Getting Mask");
            p1.Minimum = 0;
            p1.Value = 0;
            p1.Maximum = 2 * w / 10;
            int pvalue = 0;
            int[] P = new int[256];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    P[src[i, j]]++;
                }

            }
           
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    int thres = (int)GetLocalVariance(G, GS, w, h, W, i, j, k);
                    if (src[i, j] > thres)
                    {

                        Mask[i, j] = true;
                    }
                    else
                    {
                        Mask[i, j] = false;
                    }
                }
                pvalue++;
                if (pvalue == 10)
                {
                    p1.Value++;
                    pvalue = 0;
                }
            }
        }
        public static unsafe void Binarize(Bitmap b, bool LessTreshold, ProgressBar p1, PictureBox pictureBox1)
        {
            MainForm.UpdateLabel("Binarization");
            // MainPictureBox.Image=b;
            int w = b.Width;
            int h = b.Height;
            int[] P = new int[256];
            BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            byte bitsPerPixel = (byte)Image.GetPixelFormatSize(b.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                {
                    byte* data = scan0 + j * bData.Stride + i * bitsPerPixel / 8;
                    P[data[0]]++;
                }
            int threshold = GetOptimalTreshold(P);
            if (LessTreshold) threshold = threshold * 3 / 4; else threshold = threshold * 5 * 3 / 16;


            p1.Minimum = 0;
            p1.Maximum = b.Width / 10;
            p1.Value = 0;
            int pvalue = 0;
            for (int i = 0; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {
                    byte* data = scan0 + j * bData.Stride + i * bitsPerPixel / 8;
                    byte c = data[0];
                    if (c > threshold)
                    {

                        data[0] = data[1] = data[2] = 255;
                    }
                    else
                    {
                        data[0] = data[1] = data[2] = 0;
                    }
                }
                pvalue++;
                if (pvalue == 10)
                {
                    p1.Value++;
                    pvalue = 0;
                }
            }
            b.UnlockBits(bData);
            pictureBox1.Image = b;

        }
        static public void DeleteInnerEffects(ref Bitmap b, int minsize, ProgressBar p1)
        {
            int w = b.Width;
            int h = b.Height;
            bool[,] Visited = new bool[w, h];
            bool[,] Pixels = new bool[w, h];
            BitmapFilter.GetBitArrayFromBitmap(b, Pixels);
            DeleteInnerEffects(Pixels, minsize, w, h, p1);
            ImageConverter.BitmapFromBool(ref b, Pixels, w, h);

        }
        static public void DeleteInnerEffects(bool[,] Pixels, int minsize,int w,int h, ProgressBar p1)
        {
            MainForm.UpdateLabel("Improving binarization");
            int pvalue = 0;
            p1.Value = 0;
            p1.Minimum = 0;
            p1.Maximum = w / 10;
         
            bool[,] Visited = new bool[w, h];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    if (Pixels[i, j] == true) continue;
                    if (Visited[i, j]) continue;
                    int GetA = (BitmapFilter.GetBinaryArea(Pixels, i, j, w, h, Visited));
                    if (GetA < minsize)
                    {
                        BitmapFilter.FillBinaryArea(Pixels, i, j, w, h, true);
                    }
                }
                pvalue++;
                if (pvalue == 10)
                {
                    p1.Value++;
                    pvalue = 0; 
                }
            }

        }


    }
}

