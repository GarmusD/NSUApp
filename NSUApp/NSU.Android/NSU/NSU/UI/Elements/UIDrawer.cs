using System;
using Android.Graphics;
using NSU.Shared.NSUUI;

namespace NSU.Droid.UI.Elements
{
    public class UIDrawer : INSUUIDrawer
    {
        Canvas canvas;
        Color clr, bclr;
        Paint paint;

        public UIDrawer(Canvas cnv)
        {
            canvas = cnv;
            clr = Color.Black;
            bclr = Color.Black;
            paint = new Paint();
            paint.SetStyle(Paint.Style.Fill);
            paint.Color = clr;
        }

        #region INSUUIDrawer implementation

        public void SetColor(byte r, byte g, byte b)
        {
            clr = Color.Rgb(r, g, b);
            paint.Color = clr;
        }

        public void SetBackColor(byte r, byte g, byte b)
        {
            bclr = Color.Rgb(r, g, b);
        }

        public void DrawPixel(int x, int y)
        {
            canvas.DrawPoint(x, y, paint);
        }

        public void DrawLine(int x1, int y1, int x2, int y2)
        {
            Paint p = new Paint();
            p.Color = clr;
            p.SetStyle(Paint.Style.Stroke);
            canvas.DrawLine(x1, y1, x2, y2, p);
        }

        public void DrawRect(int x1, int y1, int x2, int y2)
        {
            Paint p = new Paint();
            p.Color = clr;
            p.SetStyle(Paint.Style.Stroke);
            canvas.DrawRect(new Rect(x1, y1, x2, y2), p);
        }

        public void DrawRoundRect(int x1, int y1, int x2, int y2)
        {
            Paint p = new Paint();
            p.Color = clr;
            p.SetStyle(Paint.Style.Stroke);
            canvas.DrawRoundRect(new RectF(x1, y1, x2, y2), 5, 5, p);
        }

        public void FillRect(int x1, int y1, int x2, int y2)
        {
            Paint p = new Paint();
            p.Color = clr;
            p.SetStyle(Paint.Style.Fill);
            canvas.DrawRect(new Rect(x1, y1, x2, y2), p);
        }

        public void FillRoundRect(int x1, int y1, int x2, int y2)
        {
            Paint p = new Paint();
            p.Color = clr;
            p.SetStyle(Paint.Style.Fill);
            canvas.DrawRoundRect(new RectF(x1, y1, x2, y2), 5, 5, p);
        }

        public void DrawCircle(int x, int y, int radius)
        {
            Paint p = new Paint();
            p.Color = clr;
            p.SetStyle(Paint.Style.Stroke);
            canvas.DrawCircle(x, y, radius, p);
        }

        public void FillCircle(int x, int y, int radius)
        {
            Paint p = new Paint();
            p.Color = clr;
            p.SetStyle(Paint.Style.Fill);
            canvas.DrawCircle(x, y, radius, p);
        }

        #endregion
    }
}

