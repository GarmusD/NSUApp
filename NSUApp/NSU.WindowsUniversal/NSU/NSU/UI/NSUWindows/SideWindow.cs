using NSU.NSU_UWP.UI.Elements;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;

namespace NSU.Shared.NSUUI.NSUWindows
{
    class SideWindow : INSUSideWindow
    {
        List<INSUUISideElementBase> elements;
        WindowsGroup group;

        public SideWindow(WindowsGroup parent)
        {
            group = parent;
            elements = new List<INSUUISideElementBase>();
            group.OnWindowChanged += OnWindowChangedHandler;
        }

        private void OnWindowChangedHandler(object sender, EventArgs e)
        {
            var wnd = sender as Window;
            if (wnd != null)
            {
                foreach (var item in elements)
                {
                    if (item.UIClass == NSUUISideElementClass.WindowSwitch)
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
                    var swt = new UISideSwitchButton();
                    Thickness margin = (swt.uiElement as FrameworkElement).Margin;
                    margin.Top = 4;
                    (swt.uiElement as FrameworkElement).Margin = margin;
                    elements.Add(swt);
                    return swt;
                case NSUUISideElementClass.WindowSwitch:
                    var wc = new UISideWindowSwitch();
                    margin = (wc.uiElement as FrameworkElement).Margin;
                    margin.Top = 4;
                    (wc.uiElement as FrameworkElement).Margin = margin;
                    wc.OnClicked += WindowSwitchOnClickedHandler;
                    elements.Add(wc);
                    return wc;
                case NSUUISideElementClass.GroupSwitch:
                    var gs = new UISideGroupSwitch();
                    margin = (gs.uiElement as FrameworkElement).Margin;
                    margin.Top = 4;
                    (gs.uiElement as FrameworkElement).Margin = margin;
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
    }
}
