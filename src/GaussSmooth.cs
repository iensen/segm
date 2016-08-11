using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Threading;

namespace WindowsFormsApplication3
{
    class GaussSmooth
    {
        private static void normalize(List<float> mask)
        {
            int len = mask.Count;
            float sum = 0;
            for (int i = 1; i < len; i++)
            {
                sum += Math.Abs(mask[i]);
            }
            sum = 2 * sum + Math.Abs(mask[0]);
            for (int i = 0; i < len; i++)
            {
                mask[i] /= sum;
            }
        }


        private static float GaussFun(float sigma, int i)
        {

            return (float)((1 / (Math.Sqrt(2 * Math.PI) * sigma)) * Math.Exp(-(i * i) / (2 * sigma * sigma)));
        }
        private static  void convolve_SumH(MyFloatColor[,] src, MyFloatColor[,] dst,
                   List<float> mask, int width, int height)
        {
            ProgressBar progressBar1 = (ProgressBar)ProgressBar.FromHandle(MainForm.ProgressBr);
            int len = mask.Count;
            int pval = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
               
                    float sumR = mask[0] * src[x, y].R;
                    float sumG = mask[0] * src[x, y].G;
                    float sumB = mask[0] * src[x, y].B;


                    for (int i = 1; i < len; i++)
                    {
                      
                        sumR += mask[i] * (src[Math.Max(x - i, 0), y].R + src[Math.Min(x + i, width - 1), y].R);
                        sumG += mask[i] * (src[Math.Max(x - i, 0), y].G + src[Math.Min(x + i, width - 1), y].G);
                        sumB += mask[i] * (src[Math.Max(x - i, 0), y].B + src[Math.Min(x + i, width - 1), y].B);
                    }
                    dst[x, y].R = sumR;
                    dst[x, y].G = sumG;
                    dst[x, y].B = sumB;


                }
             
                pval++;
                if (pval == 10)
                {
                    progressBar1.Value++;
                    pval = 0;
                }
              

            }
        }
        private static void convolve_SumH(float[,] src, float[,] dst,
                   List<float> mask, int width, int height)
        {
            ProgressBar progressBar1 = (ProgressBar)ProgressBar.FromHandle(MainForm.ProgressBr);
            int len = mask.Count;
            int pval = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                 
                    float sumR = mask[0] * src[x, y];

                    for (int i = 1; i < len; i++)
                    {
                        sumR += mask[i] * (src[Math.Max(x - i, 0), y] + src[Math.Min(x + i, width - 1), y]);

                    }
                    dst[x, y] = sumR;


                }
                pval++;
                if (pval == 10)
                {
                    progressBar1.Value++;
                    pval = 0;
                }
              

            }
        }

        private static void convolve_SumV(MyFloatColor[,] src, MyFloatColor[,] dst,
           List<float> mask, int width, int height)
        {
            ProgressBar progressBar1 = (ProgressBar)ProgressBar.FromHandle(MainForm.ProgressBr);
            int len = mask.Count;
            int pval = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float sumR = mask[0] * src[x, y].R;
                    float sumG = mask[0] * src[x, y].G;
                    float sumB = mask[0] * src[x, y].B;
                    //
                  
                    for (int i = 1; i < len; i++)
                    {
                        sumR += mask[i] * (src[x, Math.Max(y - i, 0)].R + src[x, Math.Min(y + i, height - 1)].R);
                        sumG += mask[i] * (src[x, Math.Max(y - i, 0)].G + src[x, Math.Min(y + i, height - 1)].G);
                        sumB += mask[i] * (src[x, Math.Max(y - i, 0)].B + src[x, Math.Min(y + i, height - 1)].B);
                    }
                    dst[x, y].R = sumR;
                    dst[x, y].G = sumG;
                    dst[x, y].B = sumB;
                   

                }
                pval++;
                if (pval == 10)
                {
                    progressBar1.Value++;
                    pval = 0;
                    Application.DoEvents();
                }
                
            }
        }
        private static  void convolve_SumV(float[,] src, float[,] dst,
           List<float> mask, int width, int height)
        {
            ProgressBar progressBar1 = (ProgressBar)ProgressBar.FromHandle(MainForm.ProgressBr);
            int len = mask.Count;
            int pval = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float sumR = mask[0] * src[x, y];

                    //
           
                    for (int i = 1; i < len; i++)
                    {
                        sumR += mask[i] * (src[x, Math.Max(y - i, 0)] + src[x, Math.Min(y + i, height - 1)]);

                    }
                    dst[x, y] = sumR;

               

                }
                pval++;
                if (pval == 10)
                {
                    progressBar1.Value++;
                    pval = 0;
                }
                
            }
        }



        private static List<float> MakeGaussMask(double sigma)
        {
            double WIDTH = (Math.Ceiling(3 * sigma));

            sigma = Math.Max(sigma, 0.01F);
            int len = Convert.ToInt32(Math.Round(sigma * WIDTH)) + 1;
            if (sigma * WIDTH - Math.Round(sigma * WIDTH) > 0.00001) len++;
            List<float> mask = new List<float>();
            for (int i = 0; i < len; i++)
            {
                mask.Add(GaussFun((float)sigma, i));
            }
            return mask;
        }

         public static  void SmoothRgbArray(MyFloatColor[,] src, double sigma, int width, int height)
        {
            List<float> mask = new List<float>(MakeGaussMask(sigma));
            normalize(mask);
            MyFloatColor[,] tmp = new MyFloatColor[width, height];
            ProgressBar progressBar1 = (ProgressBar)ProgressBar.FromHandle(MainForm.ProgressBr);
            progressBar1.Maximum = 2 * height/10;
            progressBar1.Value = 0;
            progressBar1.Minimum = 0;
            convolve_SumV(src, tmp, mask, width, height);
            convolve_SumH(tmp, src, mask, width, height);

        }

        public static void SmoothGrayScaleArray(float[,] src, double sigma, int width, int height)
        {
            MainForm.UpdateLabel("Gauss filter");
            ProgressBar progressBar1 = (ProgressBar)ProgressBar.FromHandle(MainForm.ProgressBr);
            List<float> mask = new List<float>(MakeGaussMask(sigma));
            normalize(mask);
            float[,] tmp = new float[width, height];
            progressBar1.Maximum = 2 * height;
            progressBar1.Value = 0;
            progressBar1.Minimum = 0;
            convolve_SumV(src, tmp, mask, width, height);
            convolve_SumH(tmp,src, mask, width, height);

        }
        public static void SmoothRgbBitmap(ref Bitmap b,double sigma)
        {
           MyFloatColor [,]src=new MyFloatColor[b.Width,b.Height];
           ImageConverter.BitmapToArrayRGBf(b, src);
           int w=b.Width;
           int h=b.Height;
           b = null;
           GC.Collect();
           SmoothRgbArray(src, sigma, w, h);
           b=new Bitmap(w,h);
           ImageConverter.BitmapFromImageRGBf(ref b, src);
        }
        public static void SmoothGScaleBitmap(ref Bitmap b, double sigma)
        {
            float [,]src = new float[b.Width, b.Height];
            int w = b.Width;
            int h = b.Height;
            b = null;
            GC.Collect();
            SmoothGrayScaleArray(src, sigma, w, h);
            b = new Bitmap(w, h);
            ImageConverter.BitmapFromImageGScalef(ref b, src);

        }
        public static void SmoothGScalePictureBox(PictureBox p,double sigma)
        {
            Bitmap b = new Bitmap(p.Image);
           // p.Image = null;
            GC.Collect();
            SmoothGScaleBitmap(ref b, sigma);
            p.Image = b;

        }
        public static void SmoothRGBPictureBox(PictureBox p, double sigma)
        {
            Bitmap b = new Bitmap(p.Image);
           // p.Image = null;
            GC.Collect();
            SmoothRgbBitmap(ref b, sigma);
            p.Image = (Bitmap)b.Clone();
        }



    }
}
