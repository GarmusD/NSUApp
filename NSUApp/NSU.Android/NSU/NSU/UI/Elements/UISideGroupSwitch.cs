using System;
using Android.Content;
using NSU.Shared.NSUUI;
using NSUAppShared.NSUSystemParts;
using NSU.NSUSystem;
using NSU.Shared;
using Android.App;

namespace NSU.Droid.UI.Elements
{
    public class UISideGroupSwitch : UISideElementBase, INSUUIWindowGroupChangeButton
    {
        const string LogTag = "UISideGroupSwitch";
        string groupName = string.Empty;
        Activity act;

        public event EventHandler<SideEventArgs> OnClicked;

        public UISideGroupSwitch(Context cntx, float scale) : base(cntx, scale)
        {
            act = (Activity)cntx;
            OnImageClicked += OnImageClickedHandler;
        }

        private void OnImageClickedHandler(object sender, EventArgs e)
        {
            NSULog.Debug(LogTag, $"OnImageClickedHandler({groupName})");
            OnClicked?.Invoke(this, new SideEventArgs(groupName));
        }

        new public NSUUISideElementClass UIClass
        {
            get
            {
                return NSUUISideElementClass.GroupSwitch;
            }
        }

        public void SetGroupName(string name)
        {
            groupName = name;
        }

        public void GroupChanged(string name)
        {
            act.RunOnUiThread(() =>
            {
                if (string.IsNullOrEmpty(groupName) || !groupName.Equals(name))
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