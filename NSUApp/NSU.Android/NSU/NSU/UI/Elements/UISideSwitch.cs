using System;
using Android.Content;
using NSU.Shared.NSUUI;
using NSUAppShared.NSUSystemParts;
using NSU.NSUSystem;
using Android.App;
using NSU.Shared.NSUSystemPart;

namespace NSU.Droid.UI.Elements
{
    public class UISideSwitch : UISideElementBase, INSUUISwitchButton
    {
        string switchName;
        Switch swth;
        Activity act;

        public event EventHandler<SideEventArgs> OnClicked;

        public UISideSwitch(Context cntx, float scale) : base(cntx, scale)
        {
            act = (Activity)cntx;
            OnImageClicked += UISideSwitch_OnImageClicked;
        }

        private void UISideSwitch_OnImageClicked(object sender, EventArgs e)
        {
            if (swth != null)
            {
                swth.Clicked();
            }
        }

        new public NSUUISideElementClass UIClass
        {
            get
            {
                return NSUUISideElementClass.Switch;
            }
        }

        

        public void SetSwitchName(string name)
        {
            if(swth != null)
            {
                swth.OnStatusChanged -= SwitchOnStatusChanged;
                swth = null;
            }
            switchName = name;
            NSUSys nsusys = NSUSys.Instance;
            if (nsusys.NSUSystemReady)
            {
                var swts = nsusys.GetNSUSysPart(NSUAppShared.NSUSystemParts.PartsTypes.Switches) as Switches;
                if (swts != null)
                {
                    swth = swts.FindSwitch(switchName);
                    if (swth != null)
                    {                        
                        swth.OnStatusChanged += SwitchOnStatusChanged;
                        SwitchOnStatusChanged(swth, new SwitchStatusChangedEventArgs(swth.Status, swth.IsForced));
                    }
                }
            }
            else
            {
                nsusys.OnNSUSystemReady += OnNSUPartsReady;
            }
        }

        private void SwitchOnStatusChanged(object sender, SwitchStatusChangedEventArgs e)
        {
            act.RunOnUiThread(() =>
            {
                switch (e.Status)
                {
                    case Shared.NSUSystemPart.Status.OFF:
                        ShowOffImage();
                        break;
                    case Shared.NSUSystemPart.Status.ON:
                        ShowOnImage();
                        break;
                    case Shared.NSUSystemPart.Status.MANUAL:
                        break;
                    case Shared.NSUSystemPart.Status.DISABLED:
                        break;
                    case Shared.NSUSystemPart.Status.DISABLED_OFF:
                        ShowOffImage();
                        break;
                    case Shared.NSUSystemPart.Status.DISABLED_ON:
                        ShowOnImage();
                        break;
                    default:
                        break;
                }
            });
        }

        private void OnNSUPartsReady(object sender, EventArgs e)
        {
            NSUSys.Instance.OnNSUSystemReady -= OnNSUPartsReady;
            SetSwitchName(switchName);
        }
    }
}