using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
namespace WindowsFormsApplication3
{
    public class Pcnn
    {
        private float [,]F,L,U,B,Wc;
        private bool [,]res,Eres;
        private int W,H;
        private float Af,Al;
        private float Vl;
        private int k=1,l=1;
        private byte[,]LinktoG;
        private ProgressBar pb;
        bool Bcalced,Wcalced;
        float[,] T;
        int EdgeDetection;
        private  static byte GetKth(byte[] w, int l, int r, int k)
       {
           
           int C;
           int l1, r1;
           int x;
           int buf;

           int curind = 0;
      
         
           byte temp;
           while (true)
           {
               if (r <= l + 1)
               {
            
                   if (r == l + 1 && w[r] < w[l])
                   {
                       temp = w[l];
                       w[l] = w[r];
                       w[r] = temp;

                   }

                   return w[k];
               }

        
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

               int
                   i = l + 1,
                   j = r;
               
                   byte cur = w[l + 1];
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

          
               w[l + 1] = w[j];
               w[j] = cur;

        
               if (j >= k)
                   r = j - 1;
               if (j <= k)
                   l = i;

           }
       }
        
        public Pcnn(int w,int h,byte[,] G,int e,IntPtr Pb)
        {
            W=w;
            H=h;
            EdgeDetection=e;
            k = 1;
            l = 1;
           
            L=new float [w,h];
            U = new float[w, h];
            res=new bool[w,h];
            T = new float[w, h];
            B = new float[w, h];
            Wc = new float[w, h];
          bool[,] Eres = new bool[w, h];
            LinktoG=G;
            for(int i=0;i<W;i++)for(int j=0;j<H;j++)
            {
          
                T[i, j] = (e == 1) ? 1.0f : 0.15f;
                
            }
            pb = (ProgressBar)ProgressBar.FromHandle(Pb);

            
        }
        private float calcB(int i,int j,int k,int l)
        {
            if(EdgeDetection==1)return 1;
            if (Bcalced) return B[i, j];
            int count = 0;
            byte[] getM = new byte[9];
            for (int ii = i - k; ii <= i + k; ii++) for (int jj = j - k; jj <= j + k; jj++) if (check(ii, jj))
                    {
                        getM[count] = LinktoG[ii, jj];
                        count++;
                    }
            byte Mean = GetKth(getM, 0, count - 1, count / 2);
            double Mean2 = (double)(Mean * 1.0 / 255);
            double Var=0;
            for (int ii = i - k; ii <= i + k; ii++) for (int jj = j - k; jj <= j + k; jj++) if (check(ii, jj))
            {
                Var += (LinktoG[ii, jj] * 1.0 / 255 - Mean2) * (LinktoG[ii, jj] * 1.0 / 255 - Mean2);
        
            }
            return B[i,j]=(float)(Math.Sqrt(Var)/Mean2);
        }
        private bool check(int i,int j)
        {
            return i>=0 && j>=0 && i<W && j<H;
        }


        private float CalcW(int i,int j,int k,int l,byte [,]G)
        {
            if (EdgeDetection == 1) return 1;
            if (Wcalced) return Wc[i, j];
           float ds;
           float znam=0;
           if(!check(i+k,j+l))return -1;
           float chisl=(float)(1.0/(Math.Sqrt(k*k+l*l)));
           for(int ii=i-k;ii<=i+k;ii++)for(int jj=j-k;jj<=j+k;jj++)if(check(ii,jj))
           {
              ds=(float)Math.Sqrt((ii-i)*(ii-i)+(jj-j)*(jj-j));
              if (Math.Abs(ds * (float)(G[ii, jj] - G[i, j]) + 1)==0.0) continue;
              znam+=1/Math.Abs(ds*(float)(G[ii,jj]-G[i,j])+1);
           }
           return chisl / znam;

           
        }
        public void LoadToBitmap(IntPtr P)
        {
            PictureBox P2=(PictureBox)PictureBox.FromHandle(P);
            Bitmap b = null ;
            if (EdgeDetection == 0)
                ImageConverter.BitmapFromBool(ref b, res, W, H);
            else
            {
                ImageConverter.BitmapFromBool(ref b, Eres, W, H);
            }
            P2.Image = b;
        }
        public float CalcL(int i,int j)
        {
            double result=0;
            double gW;
            for(int ii=i-k;ii<=i+k;ii++)for(int jj=j-k;jj<=j+k;jj++)if(check (ii,jj) && (i!=ii || j!=jj))
            {
                
                result += ((res[ii, jj]) ? 1 : 0) * ((EdgeDetection == 0) ? CalcW(i, j, Math.Abs(ii - i), Math.Abs(jj - j), LinktoG) : 1);
            }
            return (float)result;
        }
        private bool Step(float a,float b)
        {
            return (a > b) ? true : false;
        }
        public void InitSegmentation()
        {
            pb.Minimum = 0;
            pb.Maximum=W;
            pb.Value = 0;
            for (int i = 0; i < W; i++) 
            {
                for (int j = 0; j < H; j++)
                {
                    L[i, j] = CalcL(i, j);
                    U[i, j] = (float)(LinktoG[i, j] * 1.0 / 255) * (1 + calcB(i, j, k, l) * CalcL(i, j));
                    res[i, j] = Step(U[i, j], T[i, j]);
                    T[i, j] = (EdgeDetection == 1) ? 1.0f : 0.15f;
                }
                pb.Value++;
            }
            Bcalced = true;
        }


        
        public void InitEdgeDetection(bool [,]Bin)
        {
            pb.Value = 0;
            pb.Maximum = W;
            
            Wc = null;
            B = null;
            Wcalced = false;
            Bcalced = false;
            GC.Collect();
            Eres = new bool[W, H];
            EdgeDetection = 1;
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    if (Bin[i, j] == true) LinktoG[i, j] = 255; else LinktoG[i, j] = 26;
                    L[i, j] = CalcL(i, j);
                    U[i, j] = (!Bin[i, j]) ? 0.1f : 1;
                    res[i, j] = Step(U[i, j], T[i, j]);
                    T[i, j] = (EdgeDetection == 0) ? 1.0f : 0.15f;
                }
                pb.Value++;
            }
            
        }

        public void ClearAll()
        {
            F=L=U=B=Wc=null;
            GC.Collect();
        }
        public int DestroySegmentStuff()
        {
            return 0;
        }
        public void NextEdgeIteration(IntPtr p)
        {

        }
      
        public void NextSegmentIteration(IntPtr p)
        {
            ProgressBar pb = (ProgressBar)(ProgressBar.FromHandle(p));
           pb.Minimum = 0;
            pb.Maximum=W;
            pb.Value = 0;
          
            for(int i=0;i<W;i++)
            {
                Application.DoEvents();
              for(int j=0;j<H;j++)
              {
                L[i,j]=CalcL(i,j);
                U[i, j] = (float)(LinktoG[i, j]*1.0/255)*(1 +   CalcL(i, j));//5!
                res[i,j] = Step(U[i,j], T[i,j]);          
                 bool Tem =!res[i, j];
                while (Tem != res[i, j])//6
                {

                    Tem = res[i, j];
                    U[i, j] = (float)(LinktoG[i, j]*1.0/255) * (1 + calcB(i, j, k, l) * CalcL(i, j));
                    res[i, j] = Step(U[i, j], T[i, j]);
                    if (res[i, j]!=Tem)//7
                    {
                        L[i, j] = CalcL(i, j);
                       
                    }
                    if (EdgeDetection==1)
                    {
                        Eres[i, j] = res[i,j];
                    }
                   
                
                }
                double g = (EdgeDetection == 1) ? 0.15f : 1.0f;
                  double r = (EdgeDetection==0)?1 / 10.0:0;
                  double fu = 100;
                  if (EdgeDetection == 1) fu = 100;
                  T[i, j] = (float)(T[i,j] - r+ 1.4*fu/ 255.0 * ((res[i, j]) ? 1.0 : 0.0));
               // T[i, j] += (float)( - r + 100.0 / 255 * ((res[i, j]) ? 1.0 : 0.0));
                //if (T[i, j] < 0) T[i, j] = 0;
               // if (T[i, j] > 1.0) T[i, j] = 1;

              }
               ++pb.Value;
            }
            Bcalced = true;
            Wcalced = true;
            
        }
    }
}
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
