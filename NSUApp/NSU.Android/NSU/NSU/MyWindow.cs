using System;
using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Runtime;
using NSU.Shared.NSUUI;
using NSU.Droid.UI.Elements;
using System.Collections.Generic;
using NSU.NSUSystem;
using NSU.Shared;

namespace NSU.Droid
{
    public class MyWindow : RelativeLayout
    {
        readonly string LogTag = "MyWindow";        

        public delegate void OnSizeChangedArgs(int w, int h);
        public event OnSizeChangedArgs OnCanCreateLayout;

        float scale = 1.0f;
        bool disposed;
        INSUWindow currentWnd;

        /*
        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            NSULog.Debug(LogTag, "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
            NSULog.Debug(LogTag, $"OnSizeChanged() - New Width: {w}  New Height: {h}. Old Width: {oldw}, Old Height: {oldh}");
            OnCanCreateLayout?.Invoke(w, h);
        }
        */

        public MyWindow(IntPtr a, JniHandleOwnership b)
            : base(a, b)
        {
        }

        public MyWindow(Context context)
            : base(context)
        {
        }

        public MyWindow(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        public MyWindow(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            NSULog.Debug(LogTag, "Dispose() is called");
            if (!disposed)
            {
                disposed = true;
                if (currentWnd != null)
                    for (int i = 0; i < currentWnd.Count; i++)
                    {
                        currentWnd[i]?.DeatachedFromWindow();
                    }
                RemoveAllViews();
            }
        }

        public float Scale { get { return scale; } set { scale = value; } }

        public void AttachNSUWindow(INSUWindow wnd)
        {
            if (currentWnd != null)
                for (int i = 0; i < currentWnd.Count; i++)
                {
                    currentWnd[i]?.DeatachedFromWindow();
                }
            RemoveAllViews();
            currentWnd = wnd;

            if (wnd == null) return;
            for (int i = 0; i < wnd.Count; i++)
            {
                INSUUIBase item = wnd[i];
                if(item == null)
                {
                    NSULog.Debug(LogTag, $"Attaching: Item = null. Continuing.");
                    continue;
                }
                try
                {
                    NSULog.Debug(LogTag, $"Attaching Class: {item.UIClass}, UIID: {item.UIID}");
                    switch (item.UIClass)
                    {
                        case NSUUIClass.None:
                            break;
                        case NSUUIClass.MonoBitmap:
                            var mb = item as UIMonoBitmap;
                            AddView(mb);
                            break;
                        case NSUUIClass.Button:
                            var btn = item as UIButton;
                            AddView(btn);
                            break;
                        case NSUUIClass.Input:
                            break;
                        case NSUUIClass.Label:
                            var lbl = item as UILabel;
                            AddView(lbl);
                            break;
                        case NSUUIClass.TempLabel:
                            var t = item as UITempLabel;
                            AddView(t);
                            break;
                        case NSUUIClass.Graphics:
                            var g = item as UIGraphics;
                            AddView(g);
                            break;
                        case NSUUIClass.SwitchButton:
                            break;
                        case NSUUIClass.Ladomat:
                            var l = item as UILadomatas;
                            AddView(l);
                            break;
                        case NSUUIClass.Ventilator:
                            var v = item as UIExhaustFan;
                            AddView(v);
                            break;
                        case NSUUIClass.CircPump:
                            var c = item as UICircPump;
                            AddView(c);
                            break;
                        case NSUUIClass.TempBar:
                            var tb = item as UITempBar;
                            AddView(tb);
                            break;
                        case NSUUIClass.WeatherInfo:
                            var wi = item as UIWeatherInfo;
                            AddView(wi);
                            break;
                        case NSUUIClass.ComfortZone:
                            var cz = item as UIComfortZone;
                            AddView(cz);
                            break;
                    }
                    item?.AttachedToWindow();
                }
                catch(Exception ex)
                {
                    NSULog.Exception(LogTag, $"AttachNSUWindow({wnd.Name}). Item: {item}. Exception: {ex.Message}");
                }
            }
        }
    }
}

