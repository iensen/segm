using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using ZedGraph;
using System.Globalization;
using System.Threading;
using System.Drawing.Imaging;

namespace WindowsFormsApplication3
{
    class SegmentVisualizer
    {
        public static unsafe void ColorBitmap(ref Bitmap b, int[,] Result, int w, int h,int count, ProgressBar p1)
        {
            MainForm.UpdateLabel("Segmented Image Creating");
            p1.Value = 0;
             Color[] RanDcolors = new Color[count+1];
            Random r = new Random();
            for (int i = 0; i <= count; i++)
            {
                RanDcolors[i] = Color.FromArgb(255, r.Next(0, 255), r.Next(0, 255), r.Next(0, 255));
            }
            p1.Minimum = 0;
            p1.Maximum = w/10;
            b = new Bitmap(w, h);
            int pvalue = 0;
          
            BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            byte bitsPerPixel = (byte)Image.GetPixelFormatSize(b.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    byte* data = scan0 + j * bData.Stride + i * bitsPerPixel / 8;
                  
                    if (Result[i, j] == -1)
                    {
                        data[0] = data[1] = data[2] = 0;
                        data[3] = 255;
                    }
                    else
                    {
                        data[2] = RanDcolors[Result[i, j]].R;
                        data[1] = RanDcolors[Result[i, j]].G;
                        data[0] = RanDcolors[Result[i, j]].B;
                        data[3] = 255;
                  
                    }
                }
                pvalue++;
                if (pvalue == 10)
                {
                    pvalue = 0;
                    p1.Value++;
                    
                }

            }
            b.UnlockBits(bData);
        }
        private static unsafe bool equals(byte* c1, byte* c2)
        {
            return (c1[0] == c2[0] && c1[1] == c2[1] && c2[2] == c1[2]);
        }
        public static unsafe void GetClrEdges(Bitmap b, ProgressBar P1, PictureBox Second)
        {
            Bitmap b2 = new Bitmap(Second.Image);
            BitmapData FbData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            BitmapData NbData = b2.LockBits(new Rectangle(0, 0, b2.Width, b2.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            byte bitsPerPixel = (byte)Image.GetPixelFormatSize(b.PixelFormat);
            byte* scan0 = (byte*)FbData.Scan0.ToPointer();
            byte* scan1 = (byte*)NbData.Scan0.ToPointer();
  
            P1.Minimum = 0;
            P1.Maximum = b.Width;
            P1.Value = 0;
            for (int i = 0; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {
                    byte* data = scan0 + j * FbData.Stride + i * bitsPerPixel / 8;
                    byte* datas = scan1 + j * NbData.Stride + i * bitsPerPixel / 8;
                    //HoRizoNtal
                
                    if (i + 1 != b.Width)
                    {
                        byte* data2 = scan0 + j * FbData.Stride + (i+1) * bitsPerPixel / 8;
                        byte* datas2 = scan1 + j * NbData.Stride + (i+1) * bitsPerPixel / 8;
                        //c2 = b.GetPixel(i + 1, j)
                        if (data[2]!=255 && data2[2]!=255 && !equals(data,data2))
                        {
                            datas2[0]=datas[0]=data2[0]=data[0] = 0;
                            datas2[1]=datas[1]=data2[1]=data[1] = 0;
                            datas2[2]=datas[2]=data2[2] = data[2] = 255;
                            datas2[3]=datas[3]=data2[3] = data[3] = 255;
                            
                           
                        }
                    }
                    if (j + 1 != b.Height)
                    {
                        byte* data2 = scan0 + (j+1) * FbData.Stride + (i) * bitsPerPixel / 8;
                        byte* datas2 = scan1 + (j+1) * NbData.Stride + (i) * bitsPerPixel / 8;
                        if (data[2] != 255 && data2[2]!=255 && !equals(data, data2))
                        {
                            datas2[0] = datas[0] = data2[0] = data[0] = 0;
                            datas2[1] = datas[1] = data2[1] = data[1] = 0;
                            datas2[2] = datas[2] = data2[2] = data[2] = 255;
                            datas2[3] = datas[3] = data2[3] = data[3] = 255;


                        }
                    }

                }
                P1.Value = i;
            }
            b.UnlockBits(FbData);
            b2.UnlockBits(NbData);
            Second.Image = (Image)b2.Clone();
        }

          public static unsafe void GetClrEdges(Bitmap b, ProgressBar P1)
        {
         
            BitmapData FbData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);

            byte bitsPerPixel = (byte)Image.GetPixelFormatSize(b.PixelFormat);
            byte* scan0 = (byte*)FbData.Scan0.ToPointer();
     
  
            P1.Minimum = 0;
            P1.Maximum = b.Width;
            P1.Value = 0;
            for (int i = 0; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {
                    byte* data = scan0 + j * FbData.Stride + i * bitsPerPixel / 8;
                   
                    //HoRizoNtal
                
                    if (i + 1 != b.Width)
                    {
                        byte* data2 = scan0 + j * FbData.Stride + (i+1) * bitsPerPixel / 8;
                    
                        //c2 = b.GetPixel(i + 1, j)
                        if (data[2]!=255 && data2[2]!=255 && !equals(data,data2))
                        {
                        data2[0]=data[0] = 0;
                     data2[1]=data[1] = 0;
                       data2[2] = data[2] = 255;
                    data2[3] = data[3] = 255;
                            
                           
                        }
                    }
                    if (j + 1 != b.Height)
                    {
                        byte* data2 = scan0 + (j+1) * FbData.Stride + (i) * bitsPerPixel / 8;
                      
                        if (data[2] != 255 && data2[2]!=255 && !equals(data, data2))
                        {
                             data2[0] = data[0] = 0;
                            data2[1] = data[1] = 0;
                        data2[2] = data[2] = 255;
                         data2[3] = data[3] = 255;


                        }
                    }

                }
                P1.Value = i;
            }
            b.UnlockBits(FbData);
     
        }
    
    }
}
