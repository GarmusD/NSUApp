using NSU.Shared;
using NSU.Shared.NSUUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSU.NSU_UWP.UI.Elements
{
    public class UISideWindowSwitch : UISideElementBase, INSUUIWindowChangeButton
    {
        const string LogTag = "UISideWindowSwitch";
        string windowName;

        public event EventHandler<SideEventArgs> OnClicked;

        public UISideWindowSwitch()
        {
            OnImageClicked += OnImageClickedHandler;
        }

        private void OnImageClickedHandler(object sender, EventArgs e)
        {
            NSULog.Debug(LogTag, $"OnImageClickedHandler({windowName})");
            OnClicked?.Invoke(this, new SideEventArgs(windowName));
        }

        new public NSUUISideElementClass UIClass
        {
            get
            {
                return NSUUISideElementClass.WindowSwitch;
            }
        }

        public void SetWindowName(string name)
        {
            windowName = name;
        }

        public void WindowChanged(string name)
        {
            NSULog.Debug(LogTag, $"{windowName} WindowChanged({name})");
            if (string.IsNullOrEmpty(windowName) || !windowName.Equals(name))
            {
                NSULog.Debug(LogTag, $"{windowName} Setting Off image.");
                ShowOffImage();
            }
            else
            {
                NSULog.Debug(LogTag, $"{windowName} Setting On image.");
                ShowOnImage();
            }
        }
    }
}
