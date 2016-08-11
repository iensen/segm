using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.IO;
namespace WindowsFormsApplication3
{
    class ImageStaticData
    {
        private static double diff(byte c1,byte c2)
        {

            double c11 = Convert.ToDouble(c1);
            double c22 = Convert.ToDouble(c2);
            return Math.Abs(c1 - c2); ;

        }
       private static  void GetParam(string File, List<float> x, List<float> y)
        {
            FileStream fs = new FileStream(File, FileMode.Open,
             FileAccess.Read);
            StreamReader F = new StreamReader(fs, System.Text.Encoding.Unicode);
            string s;

            while ((s = F.ReadLine()) != null)
            {
                if (s == string.Empty) continue;
                string[] sspl = s.Split();
                float xtoAdd, ytoAdd;
                float.TryParse(sspl[0], out xtoAdd);
                float.TryParse(sspl[1], out ytoAdd);
                x.Add(xtoAdd);
                y.Add(ytoAdd);

            }
        }
       
        public static void Train(byte[,] src, int w, int h,string gauss,string algo,ProgressBar p1)
        {
            double disp = getCurrentImageDisper(src, w, h,p1);
            WriteTOfile("data/Gauss.txt", disp.ToString() + " " + gauss);
            WriteTOfile("data/AlgoParam.txt", disp.ToString() + " " + algo);
        }
        private static void WriteTOfile(string File, string WhatToWrite)
        {
            FileStream fs = new FileStream(File, FileMode.Append,
             FileAccess.Write);
            StreamWriter F = new StreamWriter(fs, System.Text.Encoding.Unicode);
            F.WriteLine(WhatToWrite);
            F.Close();

        }
        private static float LagRange(float xp, int n, int i, List<float> x)
        {


            // числитель и знаменатель 
            float Chesl;
            float Znam;

            Chesl = 1; Znam = 1;

            int k;
            // вычисление числителя
            for (k = 0; k != n; k++)
            {

                if (k == i) continue;
                // убираем множитель x - x(i)
                Chesl *= xp - x[k];
            }
            // вычисление знаменателя
            for (k = 0; k != n; k++)
            {

                if (x[i] == x[k]) continue;
                // убираем, а то ноль в знаменателе

                Znam *= x[i] - x[k];
            }
            return Chesl / Znam;
        }
     
      public static float getSigma(float cur)
        {
            List<float> x = new List<float>();
            List<float> y = new List<float>();
            int n = 0;
            float R = 0;
            Directory.SetCurrentDirectory(MainForm.StartDir);
            GetParam("data/Gauss.txt", x, y);
            n = x.Count;
            for (int i = 0; i != n; i++)
            {

                R += y[i] * LagRange(cur, n, i, x);
            }
            return R;



        }
       public static int GetalgoParam(float cur)
        {
            List<float> x=new List<float>();
            List<float> y=new List<float>();
            int n = 0;
            float R = 0;
            Directory.SetCurrentDirectory(MainForm.StartDir);
            GetParam("data/AlgoParam.txt", x, y);
            n = x.Count;
            for (int i = 0; i != n; i++)
            {

                R += y[i] * LagRange(cur, n, i, x);
            }
            if (R < MainForm.MinFh) R = MainForm.MinFh;
            if (R > MainForm.MaxFh) R = MainForm.MaxFh;
            return Convert.ToInt32(R);

        }
        public static void GetOptimalParams(byte[,] src, int w, int h, ref double sigma, ref int Algo,ProgressBar p1)
        {
            float CurData = (float)getCurrentImageDisper(src, w, h, p1);
            sigma = getSigma(CurData);
            Algo = GetalgoParam(CurData);
            if (sigma <MainForm.MinSigma) sigma =MainForm.MinSigma;
            if (sigma > MainForm.MaxSigma) sigma = MainForm.MaxSigma;
        }
        public static double getCurrentImageDisper(byte[,] src,int w,int h,ProgressBar progressBar1)
        {
            MainForm.UpdateLabel("getting Image Static Data");
            
            double answ = 0;
            int ct = 0;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = h;
            byte c1, c2;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    c1 = src[x, y];
                    if (x + 1 < w)
                    {
                       c2=src[x+1,y];

                        answ += diff(c1, c2);
                        ct++;
                    }
                    if (y + 1 < h)
                    {
                        c2 = src[x, y + 1];
                        answ += diff(c1, c2);
                        ct++;
                    }
                }
                progressBar1.Value = y;
            }
            return (int)(answ*10 / ct);
            /*
            double mid = answ / ct;
            answ = 0;
            ct = 0;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    c1 = src[x, y];
                    if (x + 1 < w)
                    {
                        c2 = src[x + 1, y];

                        answ += (mid - diff(c1, c2)) * (mid - diff(c1, c2));
                        ct++;
                    }
                    if (y + 1 < h)
                    {
                        c2 = src[x, y + 1];
                        answ += (mid - diff(c1, c2)) * (mid - diff(c1, c2));
                        ct++;
                    }
                }
                progressBar1.Value = y + h;
            }
            return Math.Sqrt(answ / ct);
             */

        }
    }
}
