using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
            if(MainForm.DoubleFilter)
            {
                comboBox1.Text = "Yes";
            }
            else
            {
                comboBox1.Text = "No";
            }
            if (MainForm.ShowBestFitEllipses)
            {
                comboBox2.Text = "Yes";

            }
            else
            {
                comboBox2.Text = "No";
            }
            textBox2.Text = MainForm.MaxMarkers.ToString();
            textBox3.Text=MainForm.MaxSigma.ToString();
            textBox4.Text = MainForm.MinSigma.ToString();
            textBox5.Text = MainForm.MaxFh.ToString();
            textBox6.Text = MainForm.MinFh.ToString();
            textBox1.Text = MainForm.ContrastRange.ToString();
            textBox7.Text = MainForm.Binarization_K.ToString();
            textBox8.Text = MainForm.Binarization_W.ToString();
            textBox9.Text = MainForm.Density.ToString();
            textBox10.Text = MainForm.Nat_pat_size.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(comboBox1.Text=="Yes")
            {
                MainForm.DoubleFilter = true;
            }
            else
            {
                MainForm.DoubleFilter = false;
            }
            if (comboBox2.Text == "Yes")
            {
                MainForm.ShowBestFitEllipses = true;
            }
            else
            {
                MainForm.ShowBestFitEllipses = false;
            }
            Label t =(Label) Label.FromHandle(MainForm.QualLabel);
            t.Text = (MainForm.DoubleFilter) ? "Yes" : "No";
            int Markers;
            if(!Int32.TryParse(textBox2.Text,out Markers))
            {
                MessageBox.Show("Value in \"Maximum markers to show\" fieid is incorrect");
                return;
            }
            else
            {
                MainForm.MaxMarkers = Markers;
            }
            //===========================================
            double ContrastParameter;
            if (!Double.TryParse(textBox1.Text, out ContrastParameter))
            {
                MessageBox.Show("Value in \"Contrast Parameter\" fieid is incorrect");
                return;
            }
            else
            {
                MainForm.ContrastRange = ContrastParameter;
            }

            //===========================================
            double Maximum_gauss, Minimum_gauss;
            if (!Double.TryParse(textBox3.Text, out Maximum_gauss))
            {
                MessageBox.Show("Value in \"Maximum gauss\" fieid is incorrect");
                return;
            }
            else
            {
                MainForm.MaxSigma = Maximum_gauss;
            }


            if (!Double.TryParse(textBox4.Text, out Minimum_gauss))
            {
                MessageBox.Show("Value in \"Minimum gauss\" fieid is incorrect");
                return;
            }
            else
            {
                MainForm.MinSigma = Minimum_gauss;
            }
            //========================================================
            int MaximumFh_algoParam, MinimumFh_algoParam;
            if(!Int32.TryParse(textBox5.Text,out MaximumFh_algoParam))
            {
                MessageBox.Show("value in \"Maximum FH-algo parameter\" field is incorrect");
                return;
            }
            else
            {
                MainForm.MaxFh = MaximumFh_algoParam;
            }

            if (!Int32.TryParse(textBox6.Text, out MinimumFh_algoParam))
            {
                MessageBox.Show("value in \"Minimum FH-algo parameter\" field is incorrect");
                return;
            }
            else
            {
                MainForm.MinFh = MinimumFh_algoParam;
            }
            double K;
            int W;
            if (!Double.TryParse(textBox7.Text, out K))
            {
                MessageBox.Show("value in \"Binarization K\" field is incorrect");
                return;
            }
            else
            {
                MainForm.Binarization_K = K;
            }
            if (!Int32.TryParse(textBox8.Text, out W))
            {
                MessageBox.Show("value in \"Binarization W\" field is incorrect");
                return;
            }
            else
            {
                MainForm.Binarization_W = W;
            }
            double Dens;
            if (!Double.TryParse(textBox9.Text, out Dens))
            {
                MessageBox.Show("value in \"Density\" field is incorrect");
            }
            else
            {
                MainForm.Density = Dens;
            }
            double PatSize;
            if (!Double.TryParse(textBox10.Text, out PatSize))
            {
                MessageBox.Show("value in \"Natural pattern size\" field is incorrect");
            }
            else
            {
                MainForm.Nat_pat_size = PatSize;
            }
            this.Dispose();
            
            
          


        }
    }
}
