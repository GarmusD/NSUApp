using NSU.Shared.NSUUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSU.Shared.NSUUI.NSUWindows
{
    class WindowsGroup : INSUWindowsGroup
    {
        const string LogTag = "WindowsGroup";
        string grpName;
        SideWindow sideWindow;
        List<Window> windows;
        Window current;
        WindowsManager manager;

        public WindowsGroup(string name, WindowsManager parent)
        {
            grpName = name;
            windows = new List<Window>();
            sideWindow = new SideWindow(this);
            manager = parent;
            current = null;
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
            var wnd = new Window(windowName);
            windows.Add(wnd);
            return wnd;
        }

        public void ActivateWindow(string name)
        {
            //if (CurrentWindow != null && CurrentWindow.Name.Equals(name))
            //    return;
            NSULog.Debug(LogTag, $"ActivateWindow({name})");
            Window old = current;
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
    }
}
