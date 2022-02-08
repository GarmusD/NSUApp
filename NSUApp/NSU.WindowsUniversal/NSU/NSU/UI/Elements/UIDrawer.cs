using NSU.Shared.NSUUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace NSU.NSU_UWP.UI.Elements
{
    class UIDrawer : INSUUIDrawer
    {
        Canvas canvas;
        Color clr, bclr;
        public UIDrawer(Canvas cnv)
        {
            canvas = cnv;
            clr = Colors.Black;
        }

        public void SetColor(byte r, byte g, byte b)
        {
            clr = Color.FromArgb(255, r, g, b);
        }

        public void SetBackColor(byte r, byte g, byte b)
        {
            bclr = Color.FromArgb(255, r, g, b);
        }

        public void DrawPixel(int x, int y)
        {
            Line l = new Line();
            l.Stroke = new SolidColorBrush(clr);
            l.StrokeThickness = 1;
            l.X1 = x;
            l.X2 = x;
            l.Y1 = y;
            l.Y2 = y;
            canvas.Children.Add(l);
        }

        public void DrawLine(int x1, int y1, int x2, int y2)
        {
            Line l = new Line();
            l.Stroke = new SolidColorBrush(clr);
            l.StrokeThickness = 1;
            l.X1 = x1;
            l.X2 = x2;
            l.Y1 = y1;
            l.Y2 = y2;
            canvas.Children.Add(l);
        }

        public void DrawRect(int x1, int y1, int x2, int y2)
        {
            Rectangle r = new Rectangle();
            r.Stroke = new SolidColorBrush(clr);
            r.StrokeThickness = 1;
            r.Width = x2 - x1;
            r.Height = y2 - y1;
            Canvas.SetLeft(r, x1);
            Canvas.SetTop(r, y1);
            canvas.Children.Add(r);
        }

        public void DrawRoundRect(int x1, int y1, int x2, int y2)
        {
            Rectangle r = new Rectangle();
            r.Stroke = new SolidColorBrush(clr);
            r.StrokeThickness = 1;
            r.RadiusY = 5;
            r.RadiusX = 5;
            r.Width = x2 - x1;
            r.Height = y2 - y1;
            Canvas.SetLeft(r, x1);
            Canvas.SetTop(r, y1);
            canvas.Children.Add(r);
        }

        public void FillRect(int x1, int y1, int x2, int y2)
        {
            Rectangle r = new Rectangle();
            r.Fill = new SolidColorBrush(clr);
            r.Width = x2 - x1;
            r.Height = y2 - y1;
            Canvas.SetLeft(r, x1);
            Canvas.SetTop(r, y1);
            canvas.Children.Add(r);
        }

        public void FillRoundRect(int x1, int y1, int x2, int y2)
        {
            Rectangle r = new Rectangle();
            r.Fill = new SolidColorBrush(clr);
            r.RadiusY = 5;
            r.RadiusX = 5;
            r.Width = x2 - x1;
            r.Height = y2 - y1;
            Canvas.SetLeft(r, x1);
            Canvas.SetTop(r, y1);
            canvas.Children.Add(r);
        }

        public void DrawCircle(int x, int y, int radius)
        {
            Ellipse e = new Ellipse();
            e.Stroke = new SolidColorBrush(clr);
            e.StrokeThickness = 1;
            e.Width = radius * 2;
            e.Height = radius * 2;
            Canvas.SetLeft(e, x - radius);
            Canvas.SetTop(e, y - radius);
            canvas.Children.Add(e);
        }

        public void FillCircle(int x, int y, int radius)
        {
            Ellipse e = new Ellipse();
            e.Fill = new SolidColorBrush(clr);
            e.Width = radius * 2;
            e.Height = radius * 2;
            Canvas.SetLeft(e, x - radius);
            Canvas.SetTop(e, y - radius);
            canvas.Children.Add(e);
        }
    }
}
