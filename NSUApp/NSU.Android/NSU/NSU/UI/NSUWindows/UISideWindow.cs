using Android.Content;
using NSU.Droid.UI.Elements;
using NSU.Shared;
using NSU.Shared.NSUUI;
using System;
using System.Collections.Generic;

namespace NSU.Droid.UI.NSUWindows
{
    class UISideWindow : INSUSideWindow
    {
        const string LogTag = "UISideWindow";
        Context context;
        List<INSUUISideElementBase> elements;
        UIWindowsGroup group;
        float scale;

        public UISideWindow(Context cntx, UIWindowsGroup parent, float scale)
        {
            context = cntx;
            this.scale = scale;
            elements = new List<INSUUISideElementBase>();
            group = parent;
            group.OnWindowChanged += OnWindowChangedHandler;
        }

        private void OnWindowChangedHandler(object sender, EventArgs e)
        {
            var wnd = sender as UIWindow;
            if(wnd != null)
            {
                foreach(var item in elements)
                {
                    if(item.UIClass == NSUUISideElementClass.WindowSwitch)
                    {
                        (item as UISideWindowSwitch).WindowChanged(wnd.Name);
                    }
                }
            }
        }

        public INSUUISideElementBase this[int index]
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

        public INSUUISideElementBase AddUIElement(NSUUISideElementClass uiclass)
        {
            switch (uiclass)
            {
                case NSUUISideElementClass.Switch:
                    var swt = new UISideSwitch(context, scale);
                    elements.Add(swt);
                    return swt;
                case NSUUISideElementClass.WindowSwitch:
                    var wc = new UISideWindowSwitch(context, scale);
                    wc.OnClicked += WindowSwitchOnClickedHandler;
                    elements.Add(wc);
                    return wc;
                case NSUUISideElementClass.GroupSwitch:
                    var gs = new UISideGroupSwitch(context, scale);
                    gs.OnClicked += GroupSwitchOnClickedHandler;
                    elements.Add(gs);
                    return gs;
                default:
                    break;
            }
            return null;
        }

        private void GroupSwitchOnClickedHandler(object sender, SideEventArgs args)
        {
            group.ActivateGroup(args.Name);
        }

        private void WindowSwitchOnClickedHandler(object sender, SideEventArgs args)
        {
            group.ActivateWindow(args.Name);
        }

        public void Dispose()
        {
            NSULog.Debug(LogTag, "Dispose()");
            foreach(var item in elements)
            {
                item.Free();
            }
            elements.Clear();
            elements = null;
        }
    }
}
