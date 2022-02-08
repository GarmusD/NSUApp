using Android.Content;
using NSU.Shared;
using NSU.Shared.NSUUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSU.Droid.UI.NSUWindows
{
    class UIWindowsGroup : INSUWindowsGroup
    {
        const string LogTag = "WindowsGroup";
        Context context;
        string grpName;
        UISideWindow sideWindow;
        List<UIWindow> windows;
        UIWindow current;
        UIWindowsManager manager;
        float scale;

        public UIWindowsGroup(Context cntx, string name, UIWindowsManager parent, float scale)
        {
            context = cntx;
            grpName = name;
            windows = new List<UIWindow>();
            sideWindow = new UISideWindow(cntx, this, scale);
            manager = parent;
            current = null;
            this.scale = scale;
        }

        public event EventHandler OnWindowChanged;
        public INSUWindow CurrentWindow
        {
            get
            {
                if (current == null)
                {
                    foreach (var item in windows)
                    {
                        if (item.IsDefault)
                        {
                            current = item;
                            break;
                        }
                    }
                    if (current == null && windows.Count > 0)
                        current = windows[0];
                    if (current != null)
                        OnWindowChanged?.Invoke(current, new EventArgs());
                }
                return current;
            }
        }

        public bool IsDefault { get; set; }

        public string Name
        {
            get
            {
                return grpName;
            }
        }

        public INSUSideWindow SideWindow
        {
            get
            {
                return sideWindow;
            }
        }

        public INSUWindow CreateWindow(string windowName)
        {
            var wnd = new UIWindow(context, windowName, scale);
            windows.Add(wnd);
            return wnd;
        }

        public void ActivateWindow(string name)
        {
            //if (CurrentWindow != null && CurrentWindow.Name.Equals(name))
            //    return;
            NSULog.Debug(LogTag, $"ActivateWindow({name})");
            UIWindow old = current;
            string cname = "";
            if (current != null)
                cname = current.Name;
            if (!cname.Equals(name) && !string.IsNullOrEmpty(name))
                foreach (var item in windows)
                {
                    if (item.Name.Equals(name))
                    {
                        current = item;
                        OnWindowChanged?.Invoke(item, new EventArgs());
                    }
                }
        }

        public void ActivateGroup(string name)
        {
            NSULog.Debug(LogTag, $"ActivateGroup({name})");
            manager.ActivateWindowsGroup(name);
        }

        public void Dispose()
        {
            NSULog.Debug(LogTag, $"Dispose(). Name: {grpName}");
            sideWindow.Dispose();
            foreach(var item in windows)
            {
                item.Dispose();
            }
            windows.Clear();
            windows = null;
        }
    }
}
