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

namespace Wpf_Player
{
    /// <summary>
    /// Interaction logic for ChartWindow.xaml
    /// </summary>
    public partial class ChartWindow : Window
    {
        private BitmapSource bitmap;
        private List<string> charts = new List<string>();
        public ChartWindow()
        {
            InitializeComponent();
            

            charts.Add("Circle chart");
            charts.Add("Column chart");
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
        public void SetBitmap(BitmapSource b)
        {
            bitmap = b;
            chart.Source = bitmap;
        }

        private void cb_chartselector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Graphics g;
            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Black);

            Bitmap b = new Bitmap(200, 200);
            g = Graphics.FromImage(b);
            g.Clear(System.Drawing.Color.AliceBlue);


            switch (cb_chartselector.SelectedIndex)
            {
                case 0:
                    {
                        g.DrawRectangle(pen, 25, 45, 100, 100);
                        System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
                        g.FillRectangle(brush, 25, 45, 100, 100);



                        break;
                    }
                case 1:
                    {
                        g.DrawEllipse(pen, 50, 50, 50, 50);
                        System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Blue);
                        g.FillEllipse(brush, 50, 50, 50, 50);
                        break;
                    }
                case 2:
                    {
                        g.DrawLine(pen, new PointF(20, 20), new PointF(50, 50));
                        break;
                    }
                default:
                    {
                        break;
                    }


            }
            BitmapSource bit = GetBitmapSource(b);
            chart.Source = bit;
        }
    }
}
