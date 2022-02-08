using System;
using Android.Content;
using NSU.Shared.NSUUI;
using NSUAppShared.NSUSystemParts;
using NSU.NSUSystem;
using NSU.Shared;
using Android.App;

namespace NSU.Droid.UI.Elements
{
    public class UISideWindowSwitch : UISideElementBase, INSUUIWindowChangeButton
    {
        const string LogTag = "UISideWindowSwitch";
        Activity act;
        string windowName;

        public event EventHandler<SideEventArgs> OnClicked;

        public UISideWindowSwitch(Context cntx, float scale) : base(cntx, scale)
        {
            act = (Activity)cntx;
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
            act.RunOnUiThread(() =>
            {
                if (string.IsNullOrEmpty(windowName) || !windowName.Equals(name))
                {
                    ShowOffImage();
                }
                else
                {
                    ShowOnImage();
                }
            });
        }
    }
}