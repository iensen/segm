using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
namespace WindowsFormsApplication3
{
    public partial class Distribution : Form
    {
        public Distribution(List<int> Counts,List<int> X,int type)
        {
            InitializeComponent();
            if (type == 1) CreateGraph1(zgc, X, Counts);
        }
        public static void getDistribution(List<int> Areas, List<int> Sieves, List<int> Counts, int type)
        {
            int maxElement = -1;
            const int CountSieves = 100;
            for (int i = 0; i < 100; i++)
            {
                Counts.Add(0);
            }
            foreach (int Area in Areas)
            {
                if (Area > maxElement)
                {
                    maxElement = Area;
                }
            }
            for (int i = 0; i < 100; i++)
            {


                if (i == 99)
                {
                    Sieves.Add(maxElement);
                }
                else
                {
                    Sieves.Add(Convert.ToInt32((i + 1) * 1.0 * maxElement / 100));
                }
            }



            int index = 99;
            foreach (int Area in Areas)
            {
                if (type == 1)
                {


                    index = 99;
                    while (index >= 0 && Sieves[index] >= Area)
                    {
                        Counts[index]++;
                        index--;
                    }
                }
                else
                {
                    index = 99;
                    while (index >= 0 && Sieves[index] > Area) index--;
                    if (index > 0) 
                    Counts[index]++;
                }
            }



        }

        private void CreateGraph1(ZedGraphControl zgc,List<int> X,List<int>Counts)
        {
            GraphPane myPane = zgc.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "Values(in gramms)";
            myPane.XAxis.Title.Text = "Sieve size ";
            myPane.YAxis.Title.Text = "Particle's count";
            
            // Make up some data points from the Sine function
            PointPairList list = new PointPairList();
            for (int x = 1; x <= 100; x++)
            {


                list.Add(Convert.ToDouble(X[x-1]), Convert.ToDouble(Counts[x-1]));
            }
            /*
            // Generate a blue curve with circle symbols, and "My Curve 2" in the legend
            LineItem myCurve = myPane.AddCurve("", list, Color.Blue,
                                    SymbolType.Circle);
            // Fill the area under the curve with a white-red gradient at 45 degrees
            myCurve.Line.Fill = new Fill(Color.White, Color.Blue, 45F);
            // Make the symbols opaque by filling them with white
            myCurve.Symbol.Type = SymbolType.Plus;
            myCurve.Symbol.Fill = new Fill(Color.White);

            // Fill the axis background with a color gradient
            myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45F);

            // Fill the pane background with a color gradient
            myPane.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45F);

            // Calculate the Axis Scale Ranges
            zgc.AxisChange();
             */
            // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
            myPane.CurveList.Clear();

     

            Random rnd = new Random();

            // Высота столбиков
            double[] values = new double[100];

            // Заполним данные
            double[] XX = new double[100];
            for (int i = 0; i < 100; i++)
            {
                values[i] = Convert.ToDouble(Counts[i]);
                XX[i] = Convert.ToDouble(X[i]);
            }

            // Создадим кривую-гистограмму
            // Первый параметр - название кривой для легенды
            // Второй параметр - значения для оси X
            // Третий параметр - значения для оси Y
            // Четвертый параметр - цвет
            BarItem bar = myPane.AddBar("Weight", XX, values, Color.Blue);

            // !!! Расстояния между кластерами (группами столбиков) гистограммы = 0.0
            // У нас в кластере только один столбик.
            myPane.BarSettings.MinClusterGap = 0.0f;

            // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
            zgc.AxisChange();

            // Обновляем график
            zgc.Invalidate();
        }

        private void zedGraphControl1_Resize(object sender, EventArgs e)
        {

        }

  
        private void Distribution_Resize(object sender, EventArgs e)
        {
            SetSize();
        }

        private void SetSize()
        {
            zgc.Location = new Point(10, 10);
            // Leave a small margin around the outside of the control
            zgc.Size = new Size(this.ClientRectangle.Width - 20, this.ClientRectangle.Height - 20);
        }

        private void Distribution_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainForm.Ellipses.Clear();
            PictureBox p = (PictureBox)PictureBox.FromHandle(MainForm.MPicture);
            p.Refresh();
            PictureBox p2 = (PictureBox)PictureBox.FromHandle(MainForm.SPicture);
            p2.Refresh();
        }

    }
}
