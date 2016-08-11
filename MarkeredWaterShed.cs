using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Windows.Forms;
namespace WindowsFormsApplication3
{

    public class WaterShed
    {
        private class sortint : IComparer<int>
        {
            public int Compare(int a, int b)
            {
                if (a < b) return -1;
                if (a > b) return 1;
                return 0;
            }
        }
        public class Coords
        {
            private Int16 x;
            private Int16 y;
            public Coords(int xx,int yy)
            {
                x=(short)xx;
                y=(short)yy;
            }
            public int X
            {
                get
                {
                return (int)x;
                }
                set
                {
                   x=(byte)value;
                }
            }
            public int Y
            {
                get
                {
                return (int)y;
                }
                set
                {
                    y=(byte)value;
                }
            }
            public void setC(int xx,int yy)
            {
                x=(short)xx;
                y=(short)yy;   
            }
           
        }
       static bool ValidCoords(Coords Cxy,int w,int h)
        {
            if(Cxy.X<0 || Cxy.Y<0 || Cxy.X>=w || Cxy.Y>=h)return false;
            return true;
        }
       static bool ValidCoords(int x, int y, int w, int h)
       {
           if (x < 0 || x >= w) return false;
           if (y < 0 || y >= h) return false;
           return true;
       }
       static private bool IsEnclosed(int[,] Result, int x, int y, bool[,] Visited, out int second, int w, int h)
       {
           second = -2;
           bool was = false;
           int startC = Result[x, y];
           Stack<MyShortPoint> Q = new Stack<MyShortPoint>();

           Q.Push(new MyShortPoint(x, y));
           while (Q.Count != 0)
           {

               MyShortPoint top = Q.Peek();
               if (Result[top.x, top.y] == second) continue;
              
               Q.Pop();

               if (was && Result[top.x, top.y] != startC && Result[top.x, top.y] != second)
               {
                   second = -2;
                   return false;
               }
               if (Visited[top.x, top.y] || Result[top.x, top.y] != startC) continue;
               Visited[top.x, top.y] = true;
               for (int i = -1; i <= 1; i++)
               {
                   for (int j = -1; j <= 1; j++)
                   {
                       if (i == 0 && j == 0) continue;
                       if (Math.Abs(i) + Math.Abs(j) == 2) continue;
                       if (ValidCoords(top.x + i, top.y + j, w, h))
                       {
                           if (!Visited[top.x + i, top.y + j] && Result[top.x + i, top.y + j] == startC)
                           {
                               Q.Push(new MyShortPoint(top.x + i, top.y + j));
                               continue;
                           }
                           if (!was && Result[top.x+i, top.y+j] != startC)
                           {
                               was = true;

                               second = Result[top.x+i, top.y+j];
                               continue;

                           }
                           if (was && Result[top.x+i, top.y+j] != startC && Result[top.x+i, top.y+j] != second)
                           {
                               second = -2;

                           }

                       }
                      



                   }
               }
           }

           return true;

       }
       static private void FillArea(int[,] Result, int x, int y,int ResultC,int w,int h)
       {

           Stack<MyShortPoint> Q = new Stack<MyShortPoint>();
          int startC=Result[x,y];
           Q.Push(new MyShortPoint(x, y));
           while (Q.Count != 0)
           {
               MyShortPoint top=Q.Peek();
               Result[top.x, top.y] = ResultC;
               Q.Pop();
               for (int i = -1; i <= 1; i++)
               {
                   for (int j = -1; j <= 1; j++)
                   {
                       if (i == 0 && j == 0) continue;
                       if (Math.Abs(i) + Math.Abs(j) == 2) continue;
                       if(ValidCoords(top.x+i,top.y+j,w,h) && Result[top.x+i,top.y+j]==startC)Q.Push(new MyShortPoint(top.x+i,top.y+j));


                   }
               }
           }

       }

    
       
       static private void ImproveResult(int[,] Result, int w, int h)
       { 
           ProgressBar p1=(ProgressBar)ProgressBar.FromHandle(MainForm.ProgressBr);
           int pvalue=0;
           p1.Value=0;
           p1.Maximum=w/10;
           p1.Minimum=0;
           bool [,]Visited=new bool[w,h];
           for(int i=0;i<w;i++)
           {
               for(int j=0;j<h;j++)
               {
                   if(!Visited[i,j] && Result[i,j]!=-1)
                   {
                       int second=-2;
                       bool Enc=IsEnclosed(Result,i,j,Visited,out second,w,h);
                       if(second!=-1 && second!=-2)
                       {
                           FillArea(Result,i,j,second,w,h);
                       }
                   }
               }
               pvalue++;
               if(pvalue==10)
               {
                   p1.Value++;
                   pvalue=0;
               }
           }
       }
        static public void Segmentation(
            byte [,]Pixels,                           //Gray-Scales Pixels of the image
            int w,                                    //width of the image
            int h,                                    //height of the image
            List<MarkerRelativePosition>  Markers,    //setup markers
            bool [,]Mask,                             //1 for white pixels after binarization
            int[,]Result,                             //Result: random-color segmentation
            ProgressBar P1                            //shows the progess here
            )
        {
         
         P1.Value=0;
         P1.Maximum=h+h/50;
         P1.Minimum=0;
         int Iterations=0;
         PriorityQueue<Coords, int> Q = new PriorityQueue<Coords, int>(new sortint()) ;
         bool [,]was=new bool[w,h];
         int [,]C=new int[w,h];
        //Initialization
         for(int i=0;i<w;i++)
             for(int j=0;j<h;j++)
            {
                 was[i,j]=false;
                 if(Mask[i,j]==false)
                 {
                     Result[i, j] = -1;
                 }
                 C[i,j]=Int32.MaxValue;
            }
        int CountMarkers=0;
        foreach(MarkerRelativePosition pos in Markers)
        {
            if (pos.dx == -1.0f) continue;
            int x=(int)(pos.dx*(float)w);
            int y=(int)(pos.dy*(float)h);
            C[x,y]=0;
             CountMarkers++;
            Result[x,y]=CountMarkers;
            Q.Enqueue(new Coords(x,y),0);
        }
        //Propagation
         Coords Cxy,NbrCxy=new Coords(-1,-1);
            int nx,ny;
        while(Q.Count!=0)
        {
            Cxy = Q.Dequeue().Value;
            if (was[Cxy.X, Cxy.Y]) continue;
            if(Iterations%w==0)P1.Value++;
            Iterations++;
      
            
           
            was[Cxy.X,Cxy.Y]=true;
            //iterate neighbours:
            for(int i=-1;i<=1;i++)
                for(int j=-1;j<=1;j++)
                {
                    if(i==0 && j==0)continue;
           
                    nx=Cxy.X+i;
                    ny=Cxy.Y+j;
                    if(!ValidCoords(new Coords(nx,ny),w,h))continue;
                    if (Mask[nx, ny] == false) continue;
                    if(!was[nx,ny])
                    {
                        int W=(int)Math.Abs(Pixels[Cxy.X,Cxy.Y]-Pixels[nx,ny]);
                        if(Math.Max(C[Cxy.X,Cxy.Y],W)<C[nx,ny])
                        {
                            C[nx,ny]=Math.Max(C[Cxy.X,Cxy.Y],W);
                            Result[nx,ny]=Result[Cxy.X,Cxy.Y];
                            NbrCxy.setC(nx,ny);
                      
                            Q.Enqueue(new Coords(nx,ny),-C[nx,ny]);
                        }
                    

                       
                          
                    
         
                    }
                }

        }


        GC.Collect();
        ImproveResult(Result, w, h);
        }
   
    }
}
   
