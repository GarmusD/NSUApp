using NSU.NSU_UWP.UI.Elements;
using System;
using System.Collections.Generic;


namespace NSU.Shared.NSUUI.NSUWindows
{
    class Window : INSUWindow
    {
        string windowName;
        List<INSUUIBase> elements;

        public Window(string name)
        {
            windowName = name;
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

        public string Name
        {
            get
            {
                return windowName;
            }
        }

        public INSUUIBase AddUIElement(NSUUIClass uiclass)
        {
            switch (uiclass)
            {
                case NSUUIClass.None:
                case NSUUIClass.MonoBitmap:          
                    UIMonoBitmap mb = new UIMonoBitmap();
                    elements.Add(mb);
                    return mb;
                case NSUUIClass.Button:
                    UIButton btn = new UIButton();
                    elements.Add(btn);
                    return btn;
                case NSUUIClass.Input:
                    return null;
                case NSUUIClass.Label:
                    UILabel lbl = new UILabel();
                    elements.Add(lbl);
                    return lbl;
                case NSUUIClass.TempLabel:
                    UITempLabel t = new UITempLabel();
                    elements.Add(t);
                    return t;
                case NSUUIClass.Graphics:
                    UIGraphics g = new UIGraphics();
                    elements.Add(g);
                    return g;
                case NSUUIClass.SwitchButton:
                    return null;
                case NSUUIClass.Ladomat:
                    UILadomatas l = new UILadomatas();
                    elements.Add(l);
                    return l;
                case NSUUIClass.Ventilator:
                    UIExhaustFan v = new UIExhaustFan();
                    elements.Add(v);
                    return v;
                case NSUUIClass.CircPump:
                    UICircPump c = new UICircPump();
                    elements.Add(c);
                    return c;
                case NSUUIClass.TempBar:
                    UITempBar tb = new UITempBar();
                    elements.Add(tb);
                    return tb;
                case NSUUIClass.WeatherInfo:
                    UIWeatherInfo wi = new UIWeatherInfo();
                    elements.Add(wi);
                    return wi;
                case NSUUIClass.ComfortZone:
                    UIComfortZone cz = new UIComfortZone();
                    elements.Add(cz);
                    return cz;
            }
            return null;
        }
    }
}
