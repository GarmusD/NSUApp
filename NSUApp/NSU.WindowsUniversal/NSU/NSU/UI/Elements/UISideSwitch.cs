using NSU.NSUSystem;
using NSU.Shared;
using NSU.Shared.NSUSystemPart;
using NSU.Shared.NSUUI;
using NSUAppShared.NSUSystemParts;
using System;

namespace NSU.NSU_UWP.UI.Elements
{
    public class UISideSwitchButton : UISideElementBase, INSUUISwitchButton
    {
        const string LogTag = "UISideSwitch";
        string switchName;
        Shared.NSUSystemPart.Switch swth;

        public event EventHandler<SideEventArgs> OnClicked;

        public UISideSwitchButton()
        {
            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemReady += OnNSUSystemReadyHandler;
            sys.OnNSUSystemUnavailable += OnNSUSystemUnavailableHandler;
        }
        
        public void Free()
        {
            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemReady -= OnNSUSystemReadyHandler;
            sys.OnNSUSystemUnavailable -= OnNSUSystemUnavailableHandler;
        }

        private void OnNSUSystemReadyHandler(object sender, EventArgs e)
        {
            NSUSys nsusys = NSUSys.Instance;
            var swts = nsusys.GetNSUSysPart(PartsTypes.Switches) as Switches;
            if (swts != null)
            {
                var swt = swts.FindSwitch(switchName);
                if (swt != null)
                {
                    swth = swt;
                    swth.OnStatusChanged += SwitchOnStatusChanged;
                    SwitchOnStatusChanged(swth, new SwitchStatusChangedEventArgs(swth.Status, swth.IsForced));
                }
            }
        }

        private void OnNSUSystemUnavailableHandler(object sender, EventArgs e)
        {
            NSULog.Debug(LogTag, $"OnNSUSystemUnavailableHandler(). UIID: {switchName}");
            SwitchOnStatusChanged(null, new SwitchStatusChangedEventArgs(Status.UNKNOWN, false));
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
            switchName = name;
            if (NSUSys.Instance.NSUSystemReady)
            {
                OnNSUSystemReadyHandler(null, null);
            }
        }

        private void SwitchOnStatusChanged(object sender, SwitchStatusChangedEventArgs e)
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
        }

        private void OnNSUPartsReady(object sender, EventArgs e)
        {
            NSUSys.Instance.OnNSUSystemReady -= OnNSUPartsReady;
            SetSwitchName(switchName);
        }
    }
}
