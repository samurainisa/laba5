using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MathNet.Symbolics;
using Expr = MathNet.Symbolics.SymbolicExpression;

namespace laba5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public Expr expr;
        public double eps;
        public double h;
        public double a;
        public int N;
        public double b;

        public Func<double, double> f;

        private void DrawPoint(double x)
        {
            chart1.Series[1].Points.AddXY(x, f(x));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart1.Series[2].Points.Clear();
            chart1.Series[3].Points.Clear();

            a = Convert.ToDouble(textboxmin.Text);
            b = Convert.ToDouble(textboxmax.Text);
            N = Convert.ToInt32(Ncolvo.Text);
            h = (b - a) / N;
            
            expr = Expr.Parse(textBox1.Text.ToString());
            f = expr.Compile("x");
            chart1.ChartAreas[0].AxisX.Minimum = 2 * a;
            chart1.ChartAreas[0].AxisX.Maximum = 2 * b;
            drawFunction();
            result.Text = parabolasMethod().ToString();
        }   


        void drawFunction()
        {
            var h1 = (b - a) / 100;
            for (double x = 1.5 * a; x <= 1.5 * (b + 0.001); x += h1)
            {
                var y = f(x);
                chart1.Series[2].Points.AddXY(x, y);
            }
        }
        double trapezesMethod()
        {
            double S = 0;
            for (double x = a; x < (b + 0.01); x += h)
            {
                var y = f(x);
                S += y;
                chart1.Series[0].Points.AddXY(x, y);
                chart1.Series[1].Points.AddXY(x, y);
            }
            var fa = f(a);
            var fb = f(b);

            S -= fa + fb; // not counting first and last one;
            S = S * h + (h / 2) * (fa + fb);
            return Math.Abs(S);
        }

        double rectanglesMethod()
        {
            double S = 0;
            for (double x = a; x <= (b + 0.01); x += h)
            {
                var y = f(x);
                S += y;
                chart1.Series[0].Points.AddXY(x, y);
                chart1.Series[0].Points.AddXY(x + h, y);
                chart1.Series[1].Points.AddXY(x, y);
            }
            S = S * h;
            return S;
        }

        double parabolasMethod()
        {
            double S = 0;
            int count = 0;
            int lastCount = (int)((b - a) / h);


            for (double x = a; x <= (b + 0.01); x += h)
            {
                var y = f(x);

                if (count == 0 || count == lastCount)
                {
                    S += y;
                } else
                {
                    S += 2 * (count % 2 + 1) * y;
                }
                
                ++count;                
            }

            for (double x = a; x <= (b + 0.01); x+= 2 * h)
            {
                var f1 = f(x);
                var f2 = f(x + h);
                var f3 = f(x + 2 * h);
                var min = x - h;
                var max = x + h;
                var h2 = (max - min) / 10;
                var funcA = (1.0 / 2) * (f1 - 2 * f2 + f3) / Math.Pow(h, 2);
                var funcB = -(1.0 / 2) * (3 * f1 - 4 * f2 + f3) / h;
                var funcC = f1;
                var expression = Expr.Parse("a*x^2+b*x+c");
                var func = expression.Compile("a", "b", "c", "x");
                chart1.Series.Last().ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                for (double x2 = x; x2 <= x + 2 * h + 0.001; x2 += h2)
                {
                    var y = func(funcA, funcB, funcC, x2);                    
                    chart1.Series[0].Points.AddXY(x2, y);
                }
            }            
            S = S * h / 3;
            return S;
        }
    }
}
