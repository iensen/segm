 
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Windows.Forms;
namespace WindowsFormsApplication3
{


    public class FelzenSegmentation
    {
        //static edge[] edges;
        static ImprovedEdge[] edges;
        private static bool GetBit(int i, int bitCount)
        {

            return ((i & (1 << bitCount)) > 0) ? true : false;

        }
       // private static void QSort(edge[] a, int left, int right)
         private static void QSort(ImprovedEdge[] a, int left, int right)
        {
          float key = a[(left + right) / 2].w;
            int i = left;
            int j = right;
            do
            {
                while (a[i].w < key) ++i;
                while (key < a[j].w) --j;
                if (i <= j)
                {
                    ImprovedEdge t = a[i];
                    a[i] = a[j];
                    a[j] = t;
                    ++i;
                    --j;
                }
            } while (i <= j);
            if (left < j) QSort(a, left, j);
            if (i < right) QSort(a, i, right);
        }
        private static float diff(MyFloatColor c1, MyFloatColor c2)
        {

            double c1R = Convert.ToDouble(c1.R) * 255;
            double c1G = Convert.ToDouble(c1.G) * 255;
            double c1B = Convert.ToDouble(c1.B) * 255;
            double c2R = Convert.ToDouble(c2.R) * 255;
            double c2G = Convert.ToDouble(c2.G) * 255;
            double c2B = Convert.ToDouble(c2.B) * 255;

            return (float)(Math.Sqrt((c1B - c2B) * (c1B - c2B) + (c1R - c2R) * (c1R - c2R) + (c1G - c2G) * (c1G - c2G)));

        }
        private static float diff(float c1,float c2)
        {



            return (float)Math.Sqrt(3 * (c1 - c2) * (c1 - c2));

        }
        static Color GetRandomColor()
        {
            Random r=new Random();
            Color c = new Color();
            c = Color.FromArgb(255, r.Next() % 256, r.Next() % 256, r.Next() % 256);
            return c;
        }

        public static void SegmentImageRGB(MyFloatColor[,] src, double c, int minSeg,int w,int h,ProgressBar p1)
        {
            edges = new ImprovedEdge[2 * w * h + 6];
            int ct = 0;
            MyFloatColor c1, c2;
            MainForm.UpdateLabel("Felzen.. Segmentation");
            //building graph
            p1.Value = 0;
            int pvalue = 0;
            p1.Maximum = 2 * h / 10;
            /*
            
             * right-1
             * down-0
             
             */
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    c1 = src[x, y];
                    if (x + 1 < w)
                    {
                        c2 = src[x + 1, y];

                        //edges[ct].a = y * w + x;
                        edges[ct].a = ((y * w + x)<<1)+1;
                        //edges[ct].b = y * w + x + 1;//right

                        edges[ct].w = diff(c1, c2);
                        ct++;
                    }
                    if (y + 1 < h)
                    {
                        c2 = src[x, y + 1];

                        edges[ct].a = (y * w + x)<<1;
                        //edges[ct].b = (y + 1) * w + x;//down
                        edges[ct].w = diff(c1, c2);
                        ct++;
                    }
                }
                pvalue++;
                if (pvalue == 10)
                {
                    p1.Value++;
                    pvalue = 0;
                    Application.DoEvents();
                }
            }
            int[] Disset = new int[w * h];
            int[] Size = new int[w * h];
            int NumC = 0;
            SegmentVert(Disset, edges, ct, w * h, c, Size, ref NumC,w);
            GC.Collect();
            for (int i = 0; i < ct; i++)
            {
                int start = (edges[i].a) >> 1;
                int fin=0;
                if ((edges[i].a) % 2 == 0) fin = start + w;
                else fin = start + 1;

                int a = Djs.findset(start, Disset);
                int b = Djs.findset(fin, Disset);
                if ((a != b) && ((Size[a] < minSeg) || (Size[b] < minSeg)))
                {

                    Djs.union1(a, b, Disset, Size);
                    NumC--;

                }

            }
     
             byte[] R=new byte[w*h];
             byte[] G=new byte[w*h];
            byte []B=new byte[w*h];
            Random r = new Random();
            r.NextBytes(R);
            r.NextBytes(G);
            r.NextBytes(B);
            //DrawPictureBox
            edges = null;

            int normIndex;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    normIndex = y * w + x;
                    int set = Djs.findset(normIndex, Disset);
                    src[x, y].G = ((float)1.0 * G[set]) / 255; ;
                    src[x, y].R = ((float)(1.0*R[set])) / 255; ;
                    src[x, y].B = ((float)(1.0 * B[set])) / 255; ;
                }
                pvalue++;
                if (pvalue == 10)
                {
                    pvalue = 0;
                    p1.Value++;
                    Application.DoEvents();
                }
            }
        }


        public static void SegmentImageRGBgs(float[,] src, double c, int minSeg, int w, int h, ProgressBar p1)
        {
            edges = new ImprovedEdge[2 * w * h + 6];
            int ct = 0;
            float c1, c2;
            MainForm.UpdateLabel("Felzen.. Segmentation");
            //building graph
            p1.Value = 0;
            int pvalue = 0;
            p1.Maximum = 2 * h / 10;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    c1 = src[x, y];
                    if (x + 1 < w)
                    {
                        c2 = src[x + 1, y];

                        //edges[ct].a = y * w + x;
                      //  edges[ct].b = y * w + x + 1;
                        edges[ct].a = ((y * w + x) << 1) + 1;
                        edges[ct].w = diff(c1, c2);
                        ct++;
                    }
                    if (y + 1 < h)
                    {
                        c2 = src[x, y + 1];

                       // edges[ct].a = y * w + x;
                        //edges[ct].b = (y + 1) * w + x;
                        edges[ct].a = ((y * w + x) << 1) + 1;
                        edges[ct].w = diff(c1, c2);
                        ct++;
                    }
                }
                pvalue++;
                if (pvalue == 10)
                {
                    p1.Value++;
                    pvalue = 0;
                    Application.DoEvents();
                }
            }
            int[] Disset = new int[w * h];
            int[] Size = new int[w * h];
            int NumC = 0;
            SegmentVert(Disset, edges, ct, w * h, c, Size, ref NumC,w);
            GC.Collect();
            for (int i = 0; i < ct; i++)
            {
                int start = (edges[i].a) >> 1;
                int fin = 0;
                if ((edges[i].a) % 2 == 0) fin = start + w;
                else fin = start + 1;
                int a = Djs.findset(start, Disset);
                int b = Djs.findset(fin, Disset);
                if ((a != b) && ((Size[a] < minSeg) || (Size[b] < minSeg)))
                {

                    Djs.union1(a, b, Disset, Size);
                    NumC--;

                }

            }
            Color[] Colors = new Color[w * h];
            for (int i = 0; i < w * h; i++)
            {
                Colors[i] = GetRandomColor();
            }
            //DrawPictureBox
            edges = null;

            int normIndex;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    normIndex = y * w + x;
                    src[x, y] = (float)1.0 * (Colors[Djs.findset(normIndex, Disset)].G) / 255; ;
                    src[x, y] = (float)1.0 * (Colors[Djs.findset(normIndex, Disset)].R) / 255; ;
                    src[x, y] = (float)1.0 * (Colors[Djs.findset(normIndex, Disset)].B) / 255; ;
                }
                pvalue++;
                if (pvalue == 10)
                {
                    pvalue = 0;
                    p1.Value++;
                    Application.DoEvents();
                }
            }
        }
        
        //private static void SegmentVert(int[] Disset, edge[] edges, int EdgeCount, int VertCount, double c, int[] Size, ref int NumC)
          private static void SegmentVert(int[] Disset, ImprovedEdge[] edges, int EdgeCount, int VertCount, double c, int[] Size, ref int NumC,int w)
        {
            NumC = VertCount;
            QSort(edges, 0, EdgeCount - 1);
            //Array.Sort(edges, new Cmysort());
     
            int[] UnionSize = new int[VertCount];
            Djs.init(Disset, VertCount);
            double[] CritFunc = new double[VertCount];
            for (int i = 0; i < VertCount; i++)
            {
                CritFunc[i] = c;
                Size[i] = 1;

            }
            for (int i = 0; i < EdgeCount; i++)
            {
                int start = (edges[i].a) >> 1;
                int fin = 0;
                if ((edges[i].a) % 2 == 0) fin = start + w;
                else fin = start + 1;
                int SetA = Djs.findset(start, Disset);
                int SetB = Djs.findset(fin, Disset);
                if (SetA != SetB)
                {
                    if (edges[i].w < CritFunc[SetA] && edges[i].w < CritFunc[SetB])
                    {
                        NumC--;
                        Djs.union1(SetA, SetB, Disset, Size);
                        SetA = Djs.findset(SetA, Disset);
                        CritFunc[SetA] = edges[i].w + c / Size[SetA];


                    }

                }



            }




        }

    }
    public class Cmysort : IComparer<edge>
    {
	
        public int Compare(edge x, edge y)
        {
            if (x.w > y.w) return 1;
            if (x.w == y.w) return 0;
            if (x.w < y.w) return -1;
            return 0;
        }
    }
}