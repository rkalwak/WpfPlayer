using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.Data;

namespace Wpf_Player
{
    /// <summary>
    /// Interaction logic for ChartWindow.xaml
    /// </summary>
    public partial class ChartWindow : Window
    {
      
        private Bitmap bitmapd;
        private List<string> charts = new List<string>();
        private DatabaseManager db;
        private Graphics g;
        private int x, y;
        private System.Drawing.Color c1 = System.Drawing.Color.Red;
        private System.Drawing.Color c2 = System.Drawing.Color.Blue;
        private System.Drawing.Color c3 = System.Drawing.Color.Green;
        private System.Drawing.Color c4 = System.Drawing.Color.Yellow;
        
        public ChartWindow()
        {
            InitializeComponent();
            db = new DatabaseManager("192.168.1.20", "musicclient", "test", "test");
            x = (int)chart.Width;
            y = (int)chart.Height;
            bitmapd = new System.Drawing.Bitmap(x, y);
            charts.Add("Circle chart");
            charts.Add("Bar chart");
            charts.Add("Points chart");
            cb_chartselector.ItemsSource = charts;
            cb_chartselector.SelectedIndex = 0;
        }
        private System.Windows.Media.Imaging.BitmapSource GetBitmapSource(System.Drawing.Bitmap _image)
        {
            System.Drawing.Bitmap bitmap = _image;
            System.Windows.Media.Imaging.BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    bitmap.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            return bitmapSource;
        }
     

        private void cb_chartselector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cb_chartselector.SelectedIndex)
            {
                case 0:
                    {
                        DrawPieChart();
                        break;
                    }
                case 1:
                    {
                        DrawBarChart();
                        break;
                    }
                case 2:
                    {
                        DrawLineChart();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            chart.Source = GetBitmapSource(bitmapd);
        }
        
        private void DrawLineChart()
        {
            
            int margin = 30;
            float scale = 3.0f;
            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Black);
            
            g = Graphics.FromImage(bitmapd);
            g.Clear(System.Drawing.Color.AliceBlue);

            int space = 50;
            int space2 = 10;
            int a = db.getCountFromDB("select count(*) from music");
            SolidBrush b1 = new System.Drawing.SolidBrush(c1);
            SolidBrush b2 = new System.Drawing.SolidBrush(c2);
            SolidBrush b3 = new System.Drawing.SolidBrush(c3);
            SolidBrush b4 = new System.Drawing.SolidBrush(c4);
            SolidBrush[] brushes = { b1, b2, b3, b4 };
            int k = 0;
            DataTable dt = db.getQueryFromDB("select distinct Genre from music");
            System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix();
            matrix = new System.Drawing.Drawing2D.Matrix(1, 0, 0, -1, 0, y);
            g.Transform = matrix; // zamiana ukladu na kartezjanski

            g.DrawLine(pen, margin, margin, margin, margin + 100 * scale); //OY
            g.DrawLine(pen, x - margin, margin, x - margin, margin + 100 * scale);//OY
            g.DrawLine(pen, margin, margin, x - margin, margin); // OX

            for (int i = 1; i <= 10; i++)
            {
                g.DrawLine(pen, margin, margin + i * 10.0f * scale, x - margin, margin + i * 10.0f * scale); //podzialka
            }
            matrix = new System.Drawing.Drawing2D.Matrix(1, 0, 0, 1, 0, 0);
            g.Transform = matrix;
            for (int l = 0; l <= 10; l++)
            {
                g.DrawString((l * 10).ToString(), new Font("Sans", 10), System.Drawing.Brushes.Black, 5, y - margin - l * 10.0f * scale);// % podzialki
            }
            System.Drawing.Point[] points = new System.Drawing.Point[dt.Rows.Count];
            int index = 0;
            foreach (DataRow r in dt.Rows)
            {
                string s = r[0].ToString();

                g.DrawString(s, new Font("Sans", 8), System.Drawing.Brushes.Black, space, y - margin);
                int b = db.getCountFromDB(string.Format("select count(*) from music where Genre='{0}'", s));
                points[index] = new System.Drawing.Point(space, margin + (int)(b * scale * 100 / a));
                index++;
                g.DrawString(((int)b * 100 / a).ToString() + "%", new Font("Sans", 11), System.Drawing.Brushes.Black, margin + space2 + 10, y - margin - b * scale);
                space += 80;
                space2 += 80;

                k++;
            }
            matrix = new System.Drawing.Drawing2D.Matrix(1, 0, 0, -1, 0, y);
            g.Transform = matrix; // zamiana ukladu na kartezjanski
            System.Drawing.Pen pen2 = new System.Drawing.Pen(System.Drawing.Color.Red, 5);
            foreach (DataRow r in dt.Rows)
            {
                g.DrawLines(pen2, points);
            }



            



        }


        public void DrawPieChart()
        {

            g = Graphics.FromImage(bitmapd);
            g.Clear(System.Drawing.Color.AliceBlue);
            int a = db.getCountFromDB("select count(*) from music");
                   
            DataTable dt = db.getQueryFromDB("select distinct Genre from music");
            int index = 0;
            int[] myPiePercent = new int[dt.Rows.Count];
            string [] values=new string[dt.Rows.Count];
            foreach (DataRow r in dt.Rows)
            {
                string s = r[0].ToString();

                
                int b = db.getCountFromDB(string.Format("select count(*) from music where Genre='{0}'", s));
                myPiePercent[index] = (int)Math.Ceiling(((double)b * 100 / a));
                values[index] = s;
                index++;
            }

            System.Drawing.Color[] myPieColors = { c1, c2, c3, c4 };
            g.Clear(System.Drawing.Color.AliceBlue);
			System.Drawing.Size myPieSize ;
            System.Drawing.Point myPieLocation ;
            if (x < y)
            {
                myPieSize= new System.Drawing.Size(x - 50, x - 50);
                myPieLocation = new System.Drawing.Point(x / 2, x / 2);
            }
            else
            {
                myPieSize = new System.Drawing.Size(y - 50, y - 50);
                myPieLocation = new System.Drawing.Point(y / 2, y / 2);
            }
            int PiePercentTotal = 0;
            for (int PiePercents = 0; PiePercents < myPiePercent.Length; PiePercents++)
            {
                using (SolidBrush brush = new SolidBrush(myPieColors[PiePercents]))
                {
                    g.FillPie(brush, new System.Drawing.Rectangle(new System.Drawing.Point(10, 10), myPieSize), Convert.ToSingle(PiePercentTotal * 360 / 100), Convert.ToSingle(myPiePercent[PiePercents] * 360 / 100));
                }
                    PiePercentTotal += myPiePercent[PiePercents];
            }
 
            double xx = 0, yy = 0;
            for (int k = 0; k < index; k ++)
            {
                yy = Math.Sin(100*k*Math.PI/180) * 90;
                xx = Math.Cos(k*100 * Math.PI / 180) * 90;
                //g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Black), myPieLocation.X+(int)xx,myPieLocation.Y+ (int)yy, 5, 5);
                g.DrawString(values[k], new System.Drawing.Font("Sans", 10), System.Drawing.Brushes.Black, myPieLocation.X-30+(float)xx, myPieLocation.Y+(float)yy);
            }      
        }

        private void DrawBarChart()
        {
 
            int margin = 30;
            float scale = 3.0f;
            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Black);



            g = Graphics.FromImage(bitmapd);
            g.Clear(System.Drawing.Color.AliceBlue);

            int space = 50;
            int space2 = 10;
            int a = db.getCountFromDB("select count(*) from music");
            SolidBrush b1 = new System.Drawing.SolidBrush(c1);
            SolidBrush b2 = new System.Drawing.SolidBrush(c2);
            SolidBrush b3 = new System.Drawing.SolidBrush(c3);
            SolidBrush b4 = new System.Drawing.SolidBrush(c4);
            SolidBrush[] brushes = { b1, b2, b3, b4 };
            int k = 0;
            DataTable dt = db.getQueryFromDB("select distinct Genre from music");
            System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix();
            matrix = new System.Drawing.Drawing2D.Matrix(1, 0, 0, -1, 0, y);
            g.Transform = matrix; // zamiana ukladu na kartezjanski

            g.DrawLine(pen, margin, margin, margin, margin + 100 * scale); //OY
            g.DrawLine(pen, x - margin, margin, x - margin, margin + 100 * scale);//OY
            g.DrawLine(pen, margin, margin, x - margin, margin); // OX

            for (int i = 1; i <= 10; i++)
            {

                g.DrawLine(pen, margin, margin + i * 10.0f * scale, x - margin, margin + i * 10.0f * scale);

            }
            for (int l = 0; l <= 10; l++)
            {
                matrix = new System.Drawing.Drawing2D.Matrix(1, 0, 0, 1, 0, 0);
                g.Transform = matrix;
                g.DrawString((l * 10).ToString(), new Font("Sans", 10), System.Drawing.Brushes.Black, 5, y - margin - l * 10.0f * scale);
            }
            foreach (DataRow r in dt.Rows)
            {
                string s = r[0].ToString();
                matrix = new System.Drawing.Drawing2D.Matrix(1, 0, 0, 1, 0, 0);
                g.Transform = matrix;
                g.DrawString(s, new Font("Sans", 8), System.Drawing.Brushes.Black, space, y - margin);
                int b = db.getCountFromDB(string.Format("select count(*) from music where Genre='{0}'", s));

                matrix = new System.Drawing.Drawing2D.Matrix(1, 0, 0, -1, 0, y);
                g.Transform = matrix; // zamiana ukladu na kartezjanski

                g.FillRectangle(brushes[k], margin + space2, margin, 50, b * 100 * scale / a);
                matrix = new System.Drawing.Drawing2D.Matrix(1, 0, 0, 1, 0, 0);
                g.Transform = matrix;
                g.DrawString(((int)b * 100 / a).ToString() + "%", new Font("Sans", 10), System.Drawing.Brushes.Black, margin + space2 + 10, y - margin - b * scale);
                space += 80;
                space2 += 80;

                k++;
            }





        }
    }
}
