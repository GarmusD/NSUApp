using System.Collections.Generic;
using NSU.Shared.NSUUI;
using Android.Content;
using NSU.Droid.UI.Elements;
using NSU.Shared;

namespace NSU.Droid.UI.NSUWindows
{
    class UIWindow : INSUWindow
    {
        const string LogTag = "UIWindow";
        Context context;
        List<INSUUIBase> elements;
        string wndName;
        float scale;

        public UIWindow(Context cntx, string name, float scale)
        {
            context = cntx;
            wndName = name;
            this.scale = scale;
            elements = new List<INSUUIBase>();
        }

        public INSUUIBase this[int index]
        {
            get
            {
                return elements[index];
            }
        }

        public int Count
        {
            get
            {
                return elements.Count;
            }
        }

        public bool IsDefault { get; set; }

        public string Name { get { return wndName; } }

        public INSUUIBase AddUIElement(NSUUIClass uiclass)
        {
            switch (uiclass)
            {
                case NSUUIClass.None:
                case NSUUIClass.MonoBitmap:
                    var mb = new UIMonoBitmap(context, scale);
                    elements.Add(mb);
                    return mb;
                case NSUUIClass.Button:
                    var btn = new UIButton(context, scale);
                    elements.Add(btn);
                    return btn;
                case NSUUIClass.Input:
                    return null;
                case NSUUIClass.Label:
                    var lbl = new UILabel(context, scale);
                    elements.Add(lbl);
                    return lbl;
                case NSUUIClass.TempLabel:
                    var t = new UITempLabel(context, scale);
                    elements.Add(t);
                    return t;
                case NSUUIClass.Graphics:
                    var g = new UIGraphics(context, scale);
                    elements.Add(g);
                    return g;
                case NSUUIClass.SwitchButton:
                    return null;
                case NSUUIClass.Ladomat:
                    var l = new UILadomatas(context, scale);
                    elements.Add(l);
                    return l;
                case NSUUIClass.Ventilator:
                    var v = new UIExhaustFan(context, scale);
                    elements.Add(v);
                    return v;
                case NSUUIClass.CircPump:
                    var c = new UICircPump(context, scale);
                    elements.Add(c);
                    return c;
                case NSUUIClass.TempBar:
                    var tb = new UITempBar(context, scale);
                    elements.Add(tb);
                    return tb;
                case NSUUIClass.WeatherInfo:
                    var wi = new UIWeatherInfo(context, scale);
                    elements.Add(wi);
                    return wi;
                case NSUUIClass.ComfortZone:
                    var cz = new UIComfortZone(context, scale);
                    elements.Add(cz);
                    return cz;
            }
            return null;
        }

        public void Dispose()
        {
            NSULog.Debug(LogTag, $"Dispose(). Name: {wndName}");
            foreach(var item in elements)
            {
                item.Free();
            }
            elements.Clear();
            elements = null;
        }
    }
}
