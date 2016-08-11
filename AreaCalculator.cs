using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Meta.Numerics.Matrices;
using Meta.Numerics;

namespace WindowsFormsApplication3
{
    class AreaCalculator
    {
	
        static public List<int> GetAreas(MyByteColor[,] src, int w, int h,ProgressBar p1,int MinSize,Rectangle r,PictureBox main,double dydx,double PatLength)
        {
			p1.Value=0;
            int pvalue = 0 ;
           
            int AreaToadd;
            List<int> answ = new List<int>();
            bool[,] Visited = new bool[w, h];
            int xst, xfin, yst, yfin;
            if (r.Bottom==-2)
            {
                xst = yst = 0;
                xfin = w;
                yfin = h;
            }
            else
            {
                xst = Math.Max(0,r.Left);
                xfin = Math.Min(w,r.Right);
                yst = Math.Max(r.Top,0);
                yfin = Math.Min(r.Bottom,h);
            }
            p1.Maximum = (xfin - xst+1) / 10;
            List<MyShortPoint> Boundary=new List<MyShortPoint>();
            for (int x = xst; x < xfin; x++)
            {
                for (int y = yst; y < yfin; y++)
                {
               
                    if (src[x, y].R ==255 && src[x, y].G==0 && src[x, y].B==0) continue;
                    if (src[x, y].R == 0 && src[x, y].G == 0 && src[x, y].B == 0) continue;
                    Boundary.Clear();
                    AreaToadd = AreaCalculator.GetArea(Visited, src, x, y, w, h,Boundary);

                    int index = 0;
                    if (AreaToadd >= MinSize)
                    {
                  //      answ.Add(AreaToadd);
                        AnyMatrix<Double>  E=ValueGetter.GetBestFitEllipse(Boundary);
                        
                        double[] a = new double[6];
                        for (int i = 0; i < 6; i++)
                        {
                            a[i] = E[i, 0];
                        }
                        a[1] /= 2;
                        a[3] /= 2;
                        a[4] /= 2;
                        if ((a[1] * a[1] - a[0] * a[2]) == 0) continue;
                        if ((a[0] - a[2]) * (a[0] - a[2]) + 4 * a[1] * a[1] < 0) continue;
                        if ((a[0] * a[4] * a[4] + a[2] * a[3] * a[3] + a[5] * a[1] * a[1] - 2 * a[1] * a[3] * a[4] - a[0] * a[2] * a[5]) / ((a[1] * a[1] - a[0] * a[2]) * (Math.Sqrt((a[0] - a[2]) * (a[0] - a[2]) + 4 * a[1] * a[1]) - (a[0] + a[2]))) < 0) continue;
                        double a1 = 2 * Math.Sqrt(2 * (a[0] * a[4] * a[4] + a[2] * a[3] * a[3] + a[5] * a[1] * a[1] - 2 * a[1] * a[3] * a[4] - a[0] * a[2] * a[5]) / ((a[1] * a[1] - a[0] * a[2]) * (Math.Sqrt((a[0] - a[2]) * (a[0] - a[2]) + 4 * a[1] * a[1]) - (a[0] + a[2]))));
                        double a2 = 2 * Math.Sqrt(2 * (a[0] * a[4] * a[4] + a[2] * a[3] * a[3] + a[5] * a[1] * a[1] - 2 * a[1] * a[3] * a[4] - a[0] * a[2] * a[5]) / ((a[1] * a[1] - a[0] * a[2]) * (-Math.Sqrt((a[0] - a[2]) * (a[0] - a[2]) + 4 * a[1] * a[1]) - (a[0] + a[2]))));
                        PictureBox p=(PictureBox)PictureBox.FromHandle(MainForm.MPicture);
                        double dx = 1.0 * p.Image.Width / p.Size.Width;
                        double dy = 1.0 * p.Image.Height / p.Size.Height;
                        a1 /= dx;
                        a2 /= dy;
                        //get Angle--------------------------
                        double fi=0;
                        if (a[1] == 0 && a[0] < a[2]) fi = 0;
                        if (a[1] == 0 && a[0] >= a[2]) fi = Math.PI / 2;
                        if (a[1] != 0 && a[0] < a[2]) fi = 1.0 / 2 * (Math.PI / 2 - Math.Atan((a[0] - a[2]) / a[1]));
                        if (a[1] != 0 && a[0] >= a[2]) fi = 1.0 / 2 * (Math.PI  - Math.Atan((a[0] - a[2]) / a[1]));
                        double a1y = a1 * Math.Sin(fi);
                        double a1x = a1 * Math.Cos(fi);
                        a1y *= dydx;
                        double a2y = a2 * Math.Sin(fi);
                        double a2x = a2 * Math.Cos(fi);
                        a2y *= dydx;
                        a1 = Math.Sqrt(a1x * a1x + a1y * a1y);
                        a2 = Math.Sqrt(a2x * a2x + a2y * a2y);
                        double div = MainForm.Nat_pat_size * PatLength;
                        a1 /= div;
                        a2 /= div;


                        double Value = AreaToadd / (div * div) * Math.Sqrt(a1 * a2) * MainForm.Density;
                        answ.Add((int)(Value*1000));
                        if (MainForm.ShowBestFitEllipses) 
                        if (main.Visible)
                            ValueGetter.DrawEllipse(a, Graphics.FromHwnd(MainForm.MPicture),dx,dy,true);
                        else
                        {
                            ValueGetter.DrawEllipse(a, Graphics.FromHwnd(MainForm.SPicture),dx,dy,true);
                        }
                      

                        
                    } 
                    


                }
                pvalue++;
                if(pvalue==10)
                {
                    pvalue = 0;
                    p1.Value++;
                }
            }
            return answ;
        }
        
        
        
        static public bool checkBounds(int x, int y, int w, int h)
        {
            if (x < 0 || x >= w) return false;
            if (y < 0 || y >= h) return false;
            return true;
        }
        
        static private bool eqf(float x, float y)
        {
            if (Math.Abs(x - y) < 0.000001) return true; else return false;
        }
        static public int GetArea(bool[,] Visited, MyByteColor[,] src, int x, int y,int w,int h,List<MyShortPoint>Boundary)
            //returns the number of pixels of the connecting one-colored region starting from x,y
            //Visited-Pixels are not enqueued
        {

            MyByteColor startColor = src[x, y];
      
            int result = 0;


            Stack<pair> s = new Stack<pair>();
            s.Push(new pair(x, y));

            while (s.Count != 0)
            {
                pair top = s.Peek();
                s.Pop();


                if (top.x < 0 || top.x >= w) continue;
                if (top.y < 0 || top.y >= h) continue;
                if (( src[top.x, top.y].R==255) && (src[top.x, top.y].G==0) &&  src[top.x, top.y].B== 0) continue;
                if ((src[top.x, top.y].R == 0) && (src[top.x, top.y].G == 0) && src[top.x, top.y].B == 0) continue;
                if (Visited[top.x, top.y]) continue;

                Visited[top.x, top.y] = true;
                result++;
                if (checkBounds(top.x + 1, top.y, w, h))
                    if (src[top.x + 1, top.y].G == startColor.G && src[top.x + 1, top.y].R == startColor.R && src[top.x + 1, top.y].B == startColor.B)
                        s.Push(new pair(top.x + 1, top.y));
                    else
                    {
                        Boundary.Add(new MyShortPoint(top.x+1,top.y));
                    }
                if (checkBounds(top.x - 1, top.y, w, h))
                    if (src[top.x - 1, top.y].B == startColor.B && src[top.x - 1, top.y].G == startColor.G && src[top.x - 1, top.y].R == startColor.R)
                        s.Push(new pair(top.x - 1, top.y));
                    else
                    {
                        Boundary.Add(new MyShortPoint(top.x - 1, top.y));
                    }
                if (checkBounds(top.x, top.y + 1, w, h))
                    if (src[top.x, top.y + 1].R == startColor.R && src[top.x, top.y + 1].G == startColor.G && src[top.x, top.y + 1].B == startColor.B)
                        s.Push(new pair(top.x, top.y + 1));
                    else
                    {
                        Boundary.Add(new MyShortPoint(top.x, top.y + 1));
                    }
                if (checkBounds(top.x, top.y - 1, w, h))
                    if (src[top.x, top.y - 1].R == startColor.R && src[top.x, top.y - 1].G == startColor.G && src[top.x, top.y - 1].B == startColor.B)
                        s.Push(new pair(top.x, top.y - 1));
                    else
                    {
                        Boundary.Add(new MyShortPoint(top.x, top.y - 1));
                    }
            }

            return result;
        }


        static public int GetAreaW(List<MyShortPoint> Lt, bool[,] Mask, bool[,] Visited, MyFloatColor[,] src, double x1, double y1,  ref int black,int w,int h)
        {
            /*More Complex Area calculator
             * Save each found pixel in Lt,
             * Mask-if Mask[i,j]==0 pixel is not calculated
             * result number of pixel
             */
            int x = Convert.ToInt32(Math.Round(x1));
            int y = Convert.ToInt32(Math.Round(y1));
            MyFloatColor startColor = src[x, y];
            int result = 0;
            

            Stack<pair> s = new Stack<pair>();
            s.Push(new pair(x, y));
            
            while (s.Count != 0)
            {
                
                pair top = s.Peek();
                s.Pop();


                if (top.x < 0 || top.x >= w) continue;
                if (top.y < 0 || top.y >= h) continue;



                if (eqf(src[top.x, top.y].R, -1.0f) || eqf(src[top.x, top.y].R, 1.0f) && eqf(src[top.x, top.y].G, 0.0f) && eqf(src[top.x, top.y].B, 0.0f)) continue;
                if (Visited[top.x, top.y]) continue;
                Lt.Add(new MyShortPoint(top.x, top.y));
                result++;
                if (Mask[top.x, top.y] == false) black++;
               

                //src[top.x, top.y].R = -1.0f;
                Visited[top.x, top.y] = true;
                result++;
               
                if (checkBounds(top.x + 1, top.y, w, h))
                    if (src[top.x + 1, top.y].G == startColor.G && src[top.x + 1, top.y].R == startColor.R && src[top.x + 1, top.y].B == startColor.B)
                    {
                        
                        if (!Visited[top.x + 1, top.y])
                            s.Push(new pair(top.x + 1, top.y));
                    }
                    
                if (checkBounds(top.x - 1, top.y, w, h))
                    if (src[top.x - 1, top.y].B == startColor.B && src[top.x - 1, top.y].G == startColor.G && src[top.x - 1, top.y].R == startColor.R)
                    {
                         if (!Visited[top.x - 1, top.y])
                            s.Push(new pair(top.x - 1, top.y));
                    }
                   
                if (checkBounds(top.x, top.y + 1, w, h))
                    if (src[top.x, top.y + 1].R == startColor.R && src[top.x, top.y + 1].G == startColor.G && src[top.x, top.y + 1].B == startColor.B)
                    {
                        
                        if (!Visited[top.x, top.y + 1]) s.Push(new pair(top.x, top.y + 1));
                    }
                    
                if (checkBounds(top.x, top.y - 1, w, h))
                    if (src[top.x, top.y - 1].R == startColor.R && src[top.x, top.y - 1].G == startColor.G && src[top.x, top.y - 1].B == startColor.B)
                    {
                        if (!Visited[top.x, top.y - 1]) s.Push(new pair(top.x, top.y - 1));
                    }
                    
                
            }

            return result;
        }



    }
}
