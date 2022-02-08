using NSU.Shared;
using NSU.Shared.NSUUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSU.NSU_UWP.UI.Elements
{
    public class UISideGroupSwitch : UISideElementBase, INSUUIWindowGroupChangeButton
    {
        const string LogTag = "UISideGroupSwitch";
        string groupName = string.Empty;

        public event EventHandler<SideEventArgs> OnClicked;
        public UISideGroupSwitch()
        {
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
            if (string.IsNullOrEmpty(groupName) || !groupName.Equals(name))
            {
                ShowOffImage();
            }
            else
            {
                ShowOnImage();
            }
        }
    }
}
