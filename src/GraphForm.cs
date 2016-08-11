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
    public partial class GraphForm : Form
    {
        static string Title;
        public GraphForm()
        {
            InitializeComponent();
        }
        public GraphForm(string s)
        {
            Title = s;
            InitializeComponent();


        }
        public void DrawGraph(List<double>Values)//from 0 to 200!
        {
            
            GraphPane pane = zedGraph.GraphPane;
            pane.Title.Text = Title;
            // Создадим список точек
            PointPairList list = new PointPairList();

            int xmin = 0;
            int xmax = 200;
            for (int x = xmin; x <= xmax; x++)
            {

                // Случайная координата по Y
                double y = Values[x];

                // добавим в список точку
                list.Add(x, y);
            }
            Color curveColor = Color.BlueViolet;
            LineItem myCurve = pane.AddCurve("", list, curveColor, SymbolType.None);

            // Включим сглаживание
            myCurve.Line.IsSmooth = true;

            // Обновим график
            zedGraph.AxisChange();
            zedGraph.Invalidate();

            
        }
    }
}
