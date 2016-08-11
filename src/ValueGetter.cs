using System;
using System.Collections.Generic;
using System.Text;
using Meta.Numerics.Matrices;
using Meta.Numerics;

using System.Drawing;
namespace WindowsFormsApplication3
{
    class Elipse
    {
        public double[] a = new double[6];
        //a[5]*x^2+a[4]*xy+a[3]*y^2+a[2]*x+a[1]*y+a[0]-ellipse!

    }
    
    public class ValueGetter
    {
        static public void DrawEllipse(double []a,Graphics g,double dx,double dy,bool add)
        {
           // a[1] /= 2;
           // a[3] /= 2;
           // a[4] /= 2;
            
            double x0 = (a[2] * a[3] - a[1] * a[4]) / (a[1] * a[1] - a[0] * a[2]);
            double y0 = (a[0] * a[4] - a[1] * a[3]) / (a[1] * a[1] - a[0] * a[2]);
            
            if(Double.IsNaN(x0))
            {
                x0 += y0;
            }

            x0 /= dx;
            y0 /= dy;
            
            double a1=2*Math.Sqrt(2*(a[0]*a[4]*a[4]+a[2]*a[3]*a[3]+a[5]*a[1]*a[1]-2*a[1]*a[3]*a[4]-a[0]*a[2]*a[5])/((a[1] * a[1] - a[0] * a[2])*(Math.Sqrt((a[0]-a[2])*(a[0]-a[2])+4*a[1]*a[1])-(a[0]+a[2]))));
            double a2 = 2 * Math.Sqrt(2 * (a[0] * a[4] * a[4] + a[2] * a[3] * a[3] + a[5] * a[1] * a[1] - 2 * a[1] * a[3] * a[4] - a[0] * a[2] * a[5]) / ((a[1] * a[1] - a[0] * a[2]) * (-Math.Sqrt((a[0] - a[2]) * (a[0] - a[2]) + 4 * a[1] * a[1]) - (a[0] + a[2]))));
            a1 /= dx;
            a2 /= dy;
            if (add) MainForm.Ellipses.Add(new ElipseToDraw(x0, y0, a1, a2));
            g.DrawEllipse(new Pen(Color.YellowGreen), (float)(x0 - a1 / 2), (float)(y0 - a2 / 2), (float)a1, (float)a2);
        }



        static public AnyMatrix<Double> GetBestFitEllipse(List<MyShortPoint> Points)
        {
      
            int numPoints = Points.Count;
			RectangularMatrix D1 = new RectangularMatrix(numPoints, 3);
			RectangularMatrix D2 = new RectangularMatrix(numPoints, 3);
            SquareMatrix S1 = new SquareMatrix(3);
            SquareMatrix S2 = new SquareMatrix(3);
            SquareMatrix S3 = new SquareMatrix(3);
            SquareMatrix T = new SquareMatrix(3);
            SquareMatrix M = new SquareMatrix(3);
            SquareMatrix C1 = new SquareMatrix(3);
			RectangularMatrix a1 = new RectangularMatrix(3, 1);
			RectangularMatrix a2 = new RectangularMatrix(3, 1);
			RectangularMatrix result = new RectangularMatrix(6, 1);
			RectangularMatrix temp;
            C1[0, 0] = 0;
            C1[0, 1] = 0;
            C1[0, 2] = 0.5;
            C1[1, 0] = 0;
            C1[1, 1] = -1;
            C1[1, 2] = 0;
            C1[2, 0] = 0.5;
            C1[2, 1] = 0;
            C1[2, 2] = 0;
            //2 D1 = [x .? 2, x .* y, y .? 2]; % quadratic part of the design matrix
            //3 D2 = [x, y, ones(size(x))]; % linear part of the design matrix
            for (int xx = 0; xx < Points.Count; xx++)
            {
              MyShortPoint p = Points[xx];
                D1[xx, 0] = p.x * p.x;
                D1[xx, 1] = p.x * p.y;
                D1[xx, 2] = p.y * p.y;
                D2[xx, 0] = p.x;
                D2[xx, 1] = p.y;
                D2[xx, 2] = 1;
           }
            //4 S1 = D1’ * D1; % quadratic part of the scatter matrix
            temp = D1.Transpose() * D1;
            for (int xx = 0; xx < 3; xx++)
                for (int yy = 0; yy < 3; yy++)
                    S1[xx, yy] = temp[xx, yy];
            //5 S2 = D1’ * D2; % combined part of the scatter matrix
            temp = D1.Transpose() * D2;
            for (int xx = 0; xx < 3; xx++)
                for (int yy = 0; yy < 3; yy++)
                    S2[xx, yy] = temp[xx, yy];
            //6 S3 = D2’ * D2; % linear part of the scatter matrix
            temp = D2.Transpose() * D2;
           for (int xx = 0; xx < 3; xx++)
           for (int yy = 0; yy < 3; yy++)
            S3[xx, yy] = temp[xx, yy];
            //7 T = – inv(S3) * S2’; % for getting a2 from a1
            T = -1 * S3.Inverse() * S2.Transpose();
            //8 M = S1 + S2 * T; % reduced scatter matrix
            M = S1 + S2 * T;
            //9 M = [M(3,  ./ 2; - M(2,  ; M(1,  ./ 2]; % premultiply by inv(C1)
            M = C1 * M;
            //10 [evec, eval] = eig(M); % solve eigensystem
            ComplexEigensystem eigenSystem = M.Eigensystem();
            //11 cond = 4 * evec(1,  .* evec(3,  – evec(2,  .? 2; % evaluate a’Ca
            //12 a1 = evec(:, find(cond > 0)); % eigenvector for min. pos. eigenvalue
            for (int xx = 0; xx < eigenSystem.Dimension; xx++)
            {
                Complex[] vector = eigenSystem.Eigenvector(xx);
          
                Complex condition = 4 * vector[0] * vector[2] - vector[1] * vector[1];
                if (condition.Im == 0 && condition.Re > 0)
                {
                    // Solution is found
 // Console.WriteLine(“\nSolution Found!”);
                    
                    //System.Windows.Forms.MessageBox.Show("Ellipse gOT");

                    for (int yy = 0; yy < vector.Length; yy++)
                    {
                      a1[yy, 0] = vector[yy].Re;
                    }
                }
            }
            //13 a2 = T * a1; % ellipse coefficients
            a2 = T * a1;
            //14 a = [a1; a2]; % ellipse coefficients
            result[0, 0] = a1[0, 0];
            result[1, 0] = a1[1, 0];
            result[2, 0] = a1[2, 0];
            result[3, 0] = a2[0, 0];
            result[4, 0] = a2[1, 0];
            result[5, 0] = a2[2, 0];
            return result;

        }
    }
}






 



     


  
