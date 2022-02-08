using NSU.Shared.NSUUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace NSU.NSU_UWP.UI.Elements
{
    class UIGraphics : INSUUIGraphics
    {
        Canvas canvas/*, maincanvas*/;
        Image image;
        BitmapImage bmp;
        Color fclr, bclr;
        UIDrawer drawer;

        public UIGraphics(/*Canvas cnv*/)
        {
            canvas = new Canvas();
            drawer = new UIDrawer(canvas);
        }

        public UIElement uiElement { get { return canvas; } }

        public void SetGraphicsBytes(byte[] buf)
        {
            throw new NotImplementedException();
        }

        public void SetVectorBytes(byte[] buf)
        {
            throw new NotImplementedException();
        }

        public void SetColor(NSUColor color)
        {
            fclr = Color.FromArgb(255, color.R, color.G, color.B);
            drawer.SetColor(fclr.R, fclr.G, fclr.B);
        }

        public void SetBackColor(NSUColor color)
        {
            bclr = Color.FromArgb(255, color.R, color.G, color.B);
            drawer.SetBackColor(bclr.R, bclr.G, bclr.B);
        }

        public INSUUIDrawer GetDrawer()
        {
            return drawer;
        }

        public void Free()
        {
            //maincanvas.Children.Remove(canvas);
            canvas = null;
            image = null;
            bmp = null;
        }

        public void AttachedToWindow()
        {
            
        }

        public void DeatachedFromWindow()
        {
            
        }

        public NSUUIClass UIClass { get { return NSUUIClass.Graphics; } }

        public string UIID { get; set; }

        public int Width
        {
            get
            {
                return (int)canvas.Width;
            }
            set
            {
                canvas.Width = value;
            }
        }

        public int Height
        {
            get
            {
                return (int)canvas.Height;
            }
            set
            {
                canvas.Height = value;
            }
        }

        public int Left
        {
            get
            {
                return (int)Canvas.GetLeft(canvas);
            }
            set
            {
                Canvas.SetLeft(canvas, value);
            }
        }

        public int Top
        {
            get
            {
                return (int)Canvas.GetTop(canvas);
            }
            set
            {
                Canvas.SetTop(canvas, value);
            }
        }
    }
}
