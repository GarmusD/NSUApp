using System;
using Android.Graphics;
using Android.Widget;
using Android.Content;
using Android.App;
using NSU.Shared.NSUUI;
using NSU.NSUSystem;
using NSU.Shared;
using Android.Util;

namespace NSU.Droid.UI.Elements
{
    public class UIGraphics : ImageView, INSUUIGraphics
    {
        const string LogTag = "UIGraphics";
        public event EventHandler Clicked;

        Bitmap bmp = null;
        Canvas canvas = null;
        INSUUIDrawer drawer = null;
        float scale;
        bool disposed = false;
        int dpLeft, dpTop, dpWidth, dpHeight, pxLeft, pxTop, pxWidth, pxHeight;
        Context context;

        public UIGraphics(Context context, float scale)
            : base(context)
        {
            this.context = context;
            this.scale = scale;
            Clickable = true;
            Click += UIGraphicsClickHandler;
            LayoutParameters = new Android.Views.ViewGroup.LayoutParams(0, 0);
            pxHeight = pxWidth = 0;
        }

        private void UIGraphicsClickHandler(object sender, EventArgs e)
        {
            Clicked?.Invoke(this, e);
        }

        void CreateBitmap(int w = -1, int h=-1)
        {
            if (bmp != null)
            {
                bmp.Dispose();
                bmp = null;
            }
                w = w ==-1 ? pxWidth : w;
                h = h == -1 ? pxHeight : h;
                if (w > 0 && h > 0)
                {
                    Bitmap.Config bc = Bitmap.Config.Argb8888;
                    bmp = Bitmap.CreateBitmap(w, h, bc);
                    canvas = new Canvas(bmp);
                    drawer = new UIDrawer(canvas);
                    this.SetImageBitmap(bmp);
                }
        }

        
        protected override void Dispose(bool disposing)
        {
            NSULog.Debug(LogTag, $"Dispose(). UIID: {UIID} is DISPOSING!!!!!!!!!");
            base.Dispose(disposing);
            Free();
        }

        #region INSUUIGraphics implementation
        public void Free()
        {
            NSULog.Debug(String.Format("UIGraphics [UIID - '{0}']", UIID), "Free() is called.");
            if (!disposed)
            {
                bmp = null;
                canvas = null;
                drawer = null;
                disposed = true;
            }
        }

        public new int Left
        {   get { return Utilities.ConvertPixelsToDp((int)GetX(), scale); }
            set
            {
                pxLeft = value;
                dpLeft = Utilities.ConvertDpToPixel(value, scale);
                SetX(dpLeft);
            }
        }

        public new int Top
        {   get { return Utilities.ConvertPixelsToDp((int)GetY(), scale); }
            set
            {
                pxTop = value;
                dpTop = Utilities.ConvertDpToPixel(value, scale);
                SetY(value * scale);
            }
        }

        public new int Width
        {
            get{ return Utilities.ConvertPixelsToDp(LayoutParameters.Width, scale); }
            set
            {
                var p1 = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, value, Resources.DisplayMetrics);
                var p2 = Utilities.ConvertDpToPixel(value, scale);
                pxWidth = value;
                //dpWidth = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, value, Resources.DisplayMetrics);
                if (p1 > p2)
                    dpWidth = p1;
                else
                    dpWidth = p2;
                this.LayoutParameters.Width = dpWidth;// (int)((float)value * scale);
                CreateBitmap();
            }
        }

        public new int Height
        {
            get{ return Utilities.ConvertPixelsToDp(this.LayoutParameters.Height, scale); }
            set
            {
                pxHeight = value;
                dpHeight = Utilities.ConvertDpToPixel(value, scale);
                this.LayoutParameters.Height = dpHeight;// (int)((float)value * scale);
                CreateBitmap();
            }
        }

        public NSUUIClass UIClass { get { return NSUUIClass.Graphics; }  }

        public string UIID{ get; set; }

        public void SetGraphicsBytes(byte[] buf)
        {
            //byte 1 - type {1-MonoBitmap, 2-CommandArray, 3-ColorBitmap }
            short w1 = buf[1];
            short w2 = buf[2];
            short h1 = buf[3];
            short h2 = buf[4];

            short w = (short)((w1 << 8) | w2);
            short h = (short)((h1 << 8) | h2);
            if (bmp == null || bmp.Width != w || bmp.Height != h)
            {                
                CreateBitmap(w, h);
            }
            Scenario sc = new Scenario();
            sc.PlayScenario(GetDrawer(), 0, 0, buf);
        }

        public void SetVectorBytes(byte[] buf)
        {
            throw new NotImplementedException();
        }

        public void SetColor(NSUColor color)
        {
            if (drawer != null)
            {
                drawer.SetColor(color.R, color.G, color.B);
            }
        }

        public void SetBackColor(NSUColor color)
        {
            throw new NotImplementedException();
        }

        public void SetPosition(float x, float y)
        {
            this.SetX(x * scale);
            this.SetY(y * scale);
        }

        public void SetSize(int w, int h)
        {            
            Width = w;
            Height = h;
        }

        public INSUUIDrawer GetDrawer()
        {
            if (drawer == null)
            {
                drawer = new UIDrawer(canvas);
            }
            return drawer;
        }

        public void AttachedToWindow()
        {

        }

        public void DeatachedFromWindow()
        {

        }
        #endregion
    }
}

