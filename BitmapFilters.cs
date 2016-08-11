using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Windows.Forms;
namespace WindowsFormsApplication3
{
    public class ConvMatrix
    {
        public int TopLeft = 0, TopMid = 0, TopRight = 0;
        public int MidLeft = 0, Pixel = 1, MidRight = 0;
        public int BottomLeft = 0, BottomMid = 0, BottomRight = 0;
        public int Factor = 1;
        public int Offset = 0;
        public void SetAll(int nVal)
        {
            TopLeft = TopMid = TopRight = MidLeft = Pixel = MidRight = BottomLeft = BottomMid = BottomRight = nVal;
        }
    }
    struct pair
    {
        public int x;
        public int y;
        public pair(int x1, int y1)
        {
            x = x1;
            y = y1;
        }
    }

    public class BitmapFilter
    {
        
       
        public static void binarize(byte[,] b, int thres, ProgressBar p1,int w,int h)
        {
            p1.Minimum = 0;
            p1.Maximum = w;
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                  
                    if (b[i,j] > thres)
                    {
                        b[i, j] = 1;
                    }
                    else
                    {
                        b[i, j] = 0;
                    }
                }
                p1.Value = i;
            }
        }
       public static  void normalize(List<float> mask)
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
       public static void GetImage(PictureBox pictureBox, MyFloatColor[,] Cls)
       {

           Bitmap newbmp1 = new Bitmap(pictureBox.Image);
           int w = newbmp1.Width;
           int h = newbmp1.Height;
           Color c;
           for (int i = 0; i < w; i++)
               for (int j = 0; j < h; j++)
               {
                   c = newbmp1.GetPixel(i, j);
                   Cls[i, j].R = c.R * 1.0f / 255;
                   Cls[i, j].G = c.G * 1.0f / 255;
                   Cls[i, j].B = c.B * 1.0f / 255;

               }


       }
       public static void smooth(MyFloatColor[,] src, MyFloatColor[,] dst, double sigma, int width, int height,ProgressBar P1)
       {
           List<float> mask = new List<float>(MakeGaussMask(sigma));
           normalize(mask);
           MyFloatColor[,] tmp = new MyFloatColor[width, height];
           convolve_SumV(src, tmp, mask, width, height,P1);
           convolve_SumH(tmp, dst, mask, width, height,P1);

       }
       public static void FillPictureBox(PictureBox p, MyFloatColor[,] tbl, int w, int h)
       {

           Bitmap newbmp = new Bitmap(w, h);
           Color c = new Color();

           for (int i = 0; i < w; i++)
               for (int j = 0; j < h; j++)
               {
                   c = Color.FromArgb(255, Convert.ToInt32(tbl[i, j].R * 255),
                                    Convert.ToInt32(tbl[i, j].G * 255),
                                    Convert.ToInt32(tbl[i, j].B * 255));
                   newbmp.SetPixel(i, j, c);
               }
           p.Image = newbmp;
       }
      public  static int GetKth(int[] w, int l, int r, int k, bool DeleteEq,int maxEl)
       {
           int C;
           int l1, r1;
           int x;
           int buf;
           int[] w2 = new int[r - l + 1];
           int curind = 0;
           bool[] was = new bool[maxEl+1];
           if (DeleteEq)
           {
               for (int i = l; i <= r; i++)
               {
                   if (!was[w[i]])
                   {
                       was[w[i]] = true;
                       w2[curind++] = w[i];
                   }
               }
               r = l + curind - 1;
               for (int i = l; i <= r; i++)
               {
                   w[i] = w2[i];
               }
           }
           int temp;
           while (true)
           {
               if (r <= l + 1)
               {
                   // текущая часть состоит из 1 или 2 элементов -
                   //	 легко можем найти ответ
                   if (r == l + 1 && w[r] < w[l])
                   {
                       temp = w[l];
                       w[l] = w[r];
                       w[r] = temp;

                   }

                   return w[k];
               }

               // упорядочиваем w[l], w[l+1], w[r]
               int mid = (l + r) >> 1;
               temp = w[mid];
               w[mid] = w[l + 1];
               w[l + 1] = temp;

               if (w[l] > w[r])
               {
                   temp = w[l];
                   w[l] = w[r];
                   w[r] = temp;
               }
               if (w[l + 1] > w[r])
               {
                   temp = w[l + 1];
                   w[l + 1] = w[r];
                   w[r] = temp;
               }
               if (w[l] > w[l + 1])
               {
                   temp = w[l + 1];
                   w[l + 1] = w[l];
                   w[l] = temp;
               }

               // выполняем разделение
               // барьером является a[l+1], т.е. медиана среди a[l], a[l+1], a[r]
               int
                   i = l + 1,
                   j = r;
               int
                   cur = w[l + 1];
               for (; ; )
               {
                   while (w[++i] < cur) ;
                   while (w[--j] > cur) ;
                   if (i > j)
                       break;

                   temp = w[i];
                   w[i] = w[j];
                   w[j] = temp;
               }

               // вставляем барьер
               w[l + 1] = w[j];
               w[j] = cur;

               // продолжаем работать в той части,
               //	 которая должна содержать искомый элемент
               if (j >= k)
                   r = j - 1;
               if (j <= k)
                   l = i;

           }
       }
      public static void G_BitMapFromPixels(Bitmap b, byte[,] InP)
      {
          int w = b.Width;
          int h = b.Height;
          for(int i=0;i<w;i++)
              for(int j=0;j<h;j++)
              {
                  b.SetPixel(i, j, Color.FromArgb(255, (int)InP[i, j], (int)InP[i, j], (int)InP[i, j]));
              }
      }
      public static void G_PixelsFrombitmap(Bitmap b, byte[,] InP)
      {
          int w = b.Width;
          int h = b.Height;
          for (int i = 0; i < w; i++)
              for (int j = 0; j < h; j++)
              {
                  Color c = b.GetPixel(i, j);
                  InP[i, j] = (byte)c.G;
              }
      }

      
       public static  void Smooth(MyFloatColor[,] src, int w, int h, double sigma,ProgressBar P1,PictureBox Pct1)
       {
           GetImage(Pct1, src);
           MyFloatColor[,] bufdst = new MyFloatColor[w, h];

     
           smooth(src, bufdst, sigma, w, h,P1);
           for (int i = 0; i < w; i++)
               for (int j = 0; j < h; j++)
               {
                   src[i, j] = bufdst[i, j];
               }
           bufdst = null;
       }

       public static  float GaussFun(float sigma, int i)
        {

            return (float)((1 / (Math.Sqrt(2 * Math.PI) * sigma)) * Math.Exp(-(i * i) / (2 * sigma * sigma)));
        }
        public static void convolve_SumH(MyFloatColor[,] src, MyFloatColor[,] dst,
                   List<float> mask, int width, int height,ProgressBar P1)
        {
            int len = mask.Count;

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
                P1.Value = height + y;

            }
        }

        public static void convolve_SumV(MyFloatColor[,] src, MyFloatColor[,] dst,
           List<float> mask, int width, int height,ProgressBar P1)
        {
            int len = mask.Count;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float sumR = mask[0] * src[x, y].R;
                    float sumG = mask[0] * src[x, y].G;
                    float sumB = mask[0] * src[x, y].B;


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
               P1.Value = y;
            }
        }




        public static List<float> MakeGaussMask(double sigma)
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
        public const short EDGE_DETECT_KIRSH = 1;
        public const short EDGE_DETECT_PREWITT = 2;
        public const short EDGE_DETECT_SOBEL = 3;


        public static bool isRed(Color c)
        {
            if (c.R == 255 && c.B == 0 && c.G == 0) return true;
            else return false;
        }


        

        public static bool Conv3x3(Bitmap b, ConvMatrix m)
        {
            // Avoid divide by zero errors
            if (0 == m.Factor) return false;

            Bitmap bSrc = (Bitmap)b.Clone();

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            int stride2 = stride * 2;
            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr SrcScan0 = bmSrc.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* pSrc = (byte*)(void*)SrcScan0;

                int nOffset = stride - b.Width * 3;
                int nWidth = b.Width - 2;
                int nHeight = b.Height - 2;

                int nPixel;

                for (int y = 0; y < nHeight; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {
                        nPixel = ((((pSrc[2] * m.TopLeft) + (pSrc[5] * m.TopMid) + (pSrc[8] * m.TopRight) +
                            (pSrc[2 + stride] * m.MidLeft) + (pSrc[5 + stride] * m.Pixel) + (pSrc[8 + stride] * m.MidRight) +
                            (pSrc[2 + stride2] * m.BottomLeft) + (pSrc[5 + stride2] * m.BottomMid) + (pSrc[8 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[5 + stride] = (byte)nPixel;

                        nPixel = ((((pSrc[1] * m.TopLeft) + (pSrc[4] * m.TopMid) + (pSrc[7] * m.TopRight) +
                            (pSrc[1 + stride] * m.MidLeft) + (pSrc[4 + stride] * m.Pixel) + (pSrc[7 + stride] * m.MidRight) +
                            (pSrc[1 + stride2] * m.BottomLeft) + (pSrc[4 + stride2] * m.BottomMid) + (pSrc[7 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[4 + stride] = (byte)nPixel;

                        nPixel = ((((pSrc[0] * m.TopLeft) + (pSrc[3] * m.TopMid) + (pSrc[6] * m.TopRight) +
                            (pSrc[0 + stride] * m.MidLeft) + (pSrc[3 + stride] * m.Pixel) + (pSrc[6 + stride] * m.MidRight) +
                            (pSrc[0 + stride2] * m.BottomLeft) + (pSrc[3 + stride2] * m.BottomMid) + (pSrc[6 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[3 + stride] = (byte)nPixel;

                        p += 3;
                        pSrc += 3;
                    }
                    p += nOffset;
                    pSrc += nOffset;
                }
            }

            b.UnlockBits(bmData);
            bSrc.UnlockBits(bmSrc);

            return true;
        }





        public static bool EmbossLaplacian(Bitmap b)
        {
            ConvMatrix m = new ConvMatrix();
            m.SetAll(-1);
            m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = 0;
            m.Pixel = 4;
            m.Offset = 127;

            return BitmapFilter.Conv3x3(b, m);
        }
        public static bool EdgeDetectQuick(Bitmap b)
        {
            ConvMatrix m = new ConvMatrix();
            m.TopLeft = m.TopMid = m.TopRight = -1;
            m.MidLeft = m.Pixel = m.MidRight = 0;
            m.BottomLeft = m.BottomMid = m.BottomRight = 1;

            m.Offset = 127;

            return BitmapFilter.Conv3x3(b, m);
        }
              public static void Gradient(byte[,] Pixels,float [,] Grad,int w,int h)
        {
        
            int [,]gx=new int [3,3];
            int [,]gy=new int[3,3];
            int[,] DX = new int[w, h];
            int[,] DY = new int[w, h];

            
            gx[0,0] = -1; gx[0,1] = 0; gx[0,2] = 1;
            gx[1,0] = 0; gx[1,1] = 0; gx[1,2] = 0;
            gx[2,0] = 0; gx[2,1] = 0; gx[2,2] = 0;

            gy[0,0] = -1; gy[0,1] = 0; gy[0,2] = 0;
            gy[1,0] = 0; gy[1,1] = 0; gy[1,2] = 0;
            gy[2,0] = 1; gy[2,1] = 0; gy[2,2] = 0;

            int sum = 0;
            int i, j, m, n;
            int X = 0, Y = 0;
            for (i = 1; i <h - 1; i++)
            {
                for (j = 1; j < w - 1; j++)
                {
                    sum = 0;
                    for (m = -1; m <= 1; m++)
                    {
                        for (n = -1; n <= 1; n++)
                        {
                            sum += (int)Pixels[i + m,j + n] * gx[m + 1,n + 1];
                        }
                    }

                    DX[X,Y] = sum;
                    Y++;
                }
                X++; Y = 0;
            }
           

            // CALCULATING Y GRADIENT 

            X = 0; Y = 0;

            for (i = 1; i < h - 1; i++)
            {
                for (j = 1; j < w - 1; j++)
                {
                    sum = 0;
                    for (m = -1; m <= 1; m++)
                    {
                        for (n = -1; n <= 1; n++)
                        {
                            sum +=Pixels[i + m,j + n] * gy[m + 1,n + 1];
                        }
                    }

                    DY[X,Y] = sum;
                    Y++;
                }
               X++; Y = 0;
            }

            for (i = 0; i < h; i++)
            {
                for (j = 0; j < w; j++)
                {
                    Grad[i, j] = (float)Math.Sqrt(DX[i, j] * DX[i, j] + DY[i, j]*DY[i,j]);
                }
            }
        }




        public static bool EdgeDetectHorizontal(Bitmap b)
        {
            Bitmap bmTemp = (Bitmap)b.Clone();

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmData2 = bmTemp.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr Scan02 = bmData2.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* p2 = (byte*)(void*)Scan02;

                int nOffset = stride - b.Width * 3;
                int nWidth = b.Width * 3;

                int nPixel = 0;

                p += stride;
                p2 += stride;

                for (int y = 1; y < b.Height - 1; ++y)
                {
                    p += 9;
                    p2 += 9;

                    for (int x = 9; x < nWidth - 9; ++x)
                    {
                        nPixel = ((p2 + stride - 9)[0] +
                            (p2 + stride - 6)[0] +
                            (p2 + stride - 3)[0] +
                            (p2 + stride)[0] +
                            (p2 + stride + 3)[0] +
                            (p2 + stride + 6)[0] +
                            (p2 + stride + 9)[0] -
                            (p2 - stride - 9)[0] -
                            (p2 - stride - 6)[0] -
                            (p2 - stride - 3)[0] -
                            (p2 - stride)[0] -
                            (p2 - stride + 3)[0] -
                            (p2 - stride + 6)[0] -
                            (p2 - stride + 9)[0]);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        (p + stride)[0] = (byte)nPixel;

                        ++p;
                        ++p2;
                    }

                    p += 9 + nOffset;
                    p2 += 9 + nOffset;
                }
            }

            b.UnlockBits(bmData);
            bmTemp.UnlockBits(bmData2);

            return true;
        }

        public static bool EdgeDetectVertical(Bitmap b)
        {
            Bitmap bmTemp = (Bitmap)b.Clone();

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmData2 = bmTemp.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr Scan02 = bmData2.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* p2 = (byte*)(void*)Scan02;

                int nOffset = stride - b.Width * 3;
                int nWidth = b.Width * 3;

                int nPixel = 0;

                int nStride2 = stride * 2;
                int nStride3 = stride * 3;

                p += nStride3;
                p2 += nStride3;

                for (int y = 3; y < b.Height - 3; ++y)
                {
                    p += 3;
                    p2 += 3;

                    for (int x = 3; x < nWidth - 3; ++x)
                    {
                        nPixel = ((p2 + nStride3 + 3)[0] +
                            (p2 + nStride2 + 3)[0] +
                            (p2 + stride + 3)[0] +
                            (p2 + 3)[0] +
                            (p2 - stride + 3)[0] +
                            (p2 - nStride2 + 3)[0] +
                            (p2 - nStride3 + 3)[0] -
                            (p2 + nStride3 - 3)[0] -
                            (p2 + nStride2 - 3)[0] -
                            (p2 + stride - 3)[0] -
                            (p2 - 3)[0] -
                            (p2 - stride - 3)[0] -
                            (p2 - nStride2 - 3)[0] -
                            (p2 - nStride3 - 3)[0]);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[0] = (byte)nPixel;

                        ++p;
                        ++p2;
                    }

                    p += 3 + nOffset;
                    p2 += 3 + nOffset;
                }
            }

            b.UnlockBits(bmData);
            bmTemp.UnlockBits(bmData2);

            return true;
        }
                public static bool EdgeDetectHomogenity(Bitmap b, byte nThreshold)
        {
            // This one works by working out the greatest difference between a pixel and it's eight neighbours.
            // The threshold allows softer edges to be forced down to black, use 0 to negate it's effect.
            Bitmap b2 = (Bitmap)b.Clone();

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmData2 = b2.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr Scan02 = bmData2.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* p2 = (byte*)(void*)Scan02;

                int nOffset = stride - b.Width * 3;
                int nWidth = b.Width * 3;

                int nPixel = 0, nPixelMax = 0;

                p += stride;
                p2 += stride;

                for (int y = 1; y < b.Height - 1; ++y)
                {
                    p += 3;
                    p2 += 3;

                    for (int x = 3; x < nWidth - 3; ++x)
                    {
                        nPixelMax = Math.Abs(p2[0] - (p2 + stride - 3)[0]);
                        nPixel = Math.Abs(p2[0] - (p2 + stride)[0]);
                        if (nPixel > nPixelMax) nPixelMax = nPixel;

                        nPixel = Math.Abs(p2[0] - (p2 + stride + 3)[0]);
                        if (nPixel > nPixelMax) nPixelMax = nPixel;

                        nPixel = Math.Abs(p2[0] - (p2 - stride)[0]);
                        if (nPixel > nPixelMax) nPixelMax = nPixel;

                        nPixel = Math.Abs(p2[0] - (p2 + stride)[0]);
                        if (nPixel > nPixelMax) nPixelMax = nPixel;

                        nPixel = Math.Abs(p2[0] - (p2 - stride - 3)[0]);
                        if (nPixel > nPixelMax) nPixelMax = nPixel;

                        nPixel = Math.Abs(p2[0] - (p2 - stride)[0]);
                        if (nPixel > nPixelMax) nPixelMax = nPixel;

                        nPixel = Math.Abs(p2[0] - (p2 - stride + 3)[0]);
                        if (nPixel > nPixelMax) nPixelMax = nPixel;

                        if (nPixelMax < nThreshold) nPixelMax = 0;

                        p[0] = (byte)nPixelMax;

                        ++p;
                        ++p2;
                    }

                    p += 3 + nOffset;
                    p2 += 3 + nOffset;
                }
            }

            b.UnlockBits(bmData);
            b2.UnlockBits(bmData2);

            return true;

        }
        public static bool EdgeDetectDifference(Bitmap b, byte nThreshold)
        {
            // This one works by working out the greatest difference between a pixel and it's eight neighbours.
            // The threshold allows softer edges to be forced down to black, use 0 to negate it's effect.
            Bitmap b2 = (Bitmap)b.Clone();

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmData2 = b2.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr Scan02 = bmData2.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* p2 = (byte*)(void*)Scan02;

                int nOffset = stride - b.Width * 3;
                int nWidth = b.Width * 3;

                int nPixel = 0, nPixelMax = 0;

                p += stride;
                p2 += stride;

                for (int y = 1; y < b.Height - 1; ++y)
                {
                    p += 3;
                    p2 += 3;

                    for (int x = 3; x < nWidth - 3; ++x)
                    {
                        nPixelMax = Math.Abs((p2 - stride + 3)[0] - (p2 + stride - 3)[0]);
                        nPixel = Math.Abs((p2 + stride + 3)[0] - (p2 - stride - 3)[0]);
                        if (nPixel > nPixelMax) nPixelMax = nPixel;

                        nPixel = Math.Abs((p2 - stride)[0] - (p2 + stride)[0]);
                        if (nPixel > nPixelMax) nPixelMax = nPixel;

                        nPixel = Math.Abs((p2 + 3)[0] - (p2 - 3)[0]);
                        if (nPixel > nPixelMax) nPixelMax = nPixel;

                        if (nPixelMax < nThreshold) nPixelMax = 0;

                        p[0] = (byte)nPixelMax;

                        ++p;
                        ++p2;
                    }

                    p += 3 + nOffset;
                    p2 += 3 + nOffset;
                }
            }

            b.UnlockBits(bmData);
            b2.UnlockBits(bmData2);

            return true;

        }

        public static bool EdgeEnhance(Bitmap b, byte nThreshold)
        {
            // This one works by working out the greatest difference between a nPixel and it's eight neighbours.
            // The threshold allows softer edges to be forced down to black, use 0 to negate it's effect.
            Bitmap b2 = (Bitmap)b.Clone();

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmData2 = b2.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr Scan02 = bmData2.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* p2 = (byte*)(void*)Scan02;

                int nOffset = stride - b.Width * 3;
                int nWidth = b.Width * 3;

                int nPixel = 0, nPixelMax = 0;

                p += stride;
                p2 += stride;

                for (int y = 1; y < b.Height - 1; ++y)
                {
                    p += 3;
                    p2 += 3;

                    for (int x = 3; x < nWidth - 3; ++x)
                    {
                        nPixelMax = Math.Abs((p2 - stride + 3)[0] - (p2 + stride - 3)[0]);

                        nPixel = Math.Abs((p2 + stride + 3)[0] - (p2 - stride - 3)[0]);

                        if (nPixel > nPixelMax) nPixelMax = nPixel;

                        nPixel = Math.Abs((p2 - stride)[0] - (p2 + stride)[0]);

                        if (nPixel > nPixelMax) nPixelMax = nPixel;

                        nPixel = Math.Abs((p2 + 3)[0] - (p2 - 3)[0]);

                        if (nPixel > nPixelMax) nPixelMax = nPixel;

                        if (nPixelMax > nThreshold && nPixelMax > p[0])
                            p[0] = (byte)Math.Max(p[0], nPixelMax);

                        ++p;
                        ++p2;
                    }

                    p += nOffset + 3;
                    p2 += nOffset + 3;
                }
            }

            b.UnlockBits(bmData);
            b2.UnlockBits(bmData2);

            return true;
        }
        public static bool checkBounds(int x, int y,int w,int h)
        {
            if (x < 0 || x >= w) return false;
            if (y < 0 || y >= h) return false;
            return true;
        }
        public static void GetBitArrayFromBitmap(Bitmap b, bool[,] Pixels)
      {
        Color c;
         for(int i=0;i<b.Width;i++)
        for(int j=0;j<b.Height;j++)
         {
             Pixels[i, j] = (b.GetPixel(i, j).R == 255);
        }
       }

     public static int GetBinaryArea(bool[,] Pixels, int x1, int y1, int w, int h,bool [,]Visited)
        {
      
             bool startColor=Pixels[x1,y1];
      
             int result = 0;
             if (Visited[x1, y1]) return Int32.MaxValue;
     
            Stack<pair> s = new Stack<pair>();
            s.Push(new pair(x1,y1));

            while (s.Count !=0)
            {
                pair top = s.Peek();
                s.Pop();


                if (top.x < 0 || top.x >= w) continue;
                if (top.y < 0 || top.y >=h) continue;
                if (Pixels[top.x,top.y]!=startColor)continue;
                if(Visited[top.x,top.y])continue;
                Visited[top.x,top.y] = true; 
                result++;
                if(checkBounds(top.x+1,top.y,w,h))
                    if (Pixels[top.x + 1, top.y] == startColor)
                s.Push(new pair(top.x + 1, top.y));
                if (checkBounds(top.x - 1, top.y, w, h))
                    if (Pixels[top.x - 1, top.y] == startColor)
                s.Push(new pair(top.x - 1, top.y));
                if (checkBounds(top.x , top.y+1, w, h))
                    if (Pixels[top.x, top.y + 1] == startColor)
                s.Push(new pair(top.x, top.y + 1));
                if (checkBounds(top.x, top.y - 1, w, h))
                    if (Pixels[top.x, top.y - 1] == startColor)
                s.Push(new pair(top.x, top.y - 1));
            }
       
            return result;
        }
     public static void FillBinaryArea(bool[,] Pixels, int x1, int y1, int w, int h, bool bit)
        {
          

        

            Stack<pair> s = new Stack<pair>();
            s.Push(new pair(x1, y1));
         
            while (s.Count != 0)
            {
                pair top = s.Peek();
                s.Pop();


                if (top.x < 0 || top.x >= w) continue;
                if (top.y < 0 || top.y >= h) continue;
                if (Pixels[top.x, top.y] == bit) continue;
      
                Pixels[top.x,top.y]=bit;
         
                if (checkBounds(top.x + 1, top.y, w, h))
                    if (Pixels[top.x + 1, top.y] == !bit)
                        s.Push(new pair(top.x + 1, top.y));
                if (checkBounds(top.x - 1, top.y, w, h))
                    if (Pixels[top.x - 1, top.y] == !bit)
                        s.Push(new pair(top.x - 1, top.y));
                if (checkBounds(top.x, top.y + 1, w, h))
                    if (Pixels[top.x, top.y + 1] == !bit)
                        s.Push(new pair(top.x, top.y + 1));
                if (checkBounds(top.x, top.y - 1, w, h))
                    if (Pixels[top.x, top.y - 1] == !bit)
                        s.Push(new pair(top.x, top.y - 1));
            }
        }
    }
}
