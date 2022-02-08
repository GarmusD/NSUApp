using System;
using NSU.Shared.NSUUI;
using Android.Content;
using NSU.NSUSystem;
using Android.App;
using NSU.Shared.NSUSystemPart;
using NSU.Shared;
using NSUAppShared.NSUSystemParts;

namespace NSU.Droid.UI.Elements
{
    public class UILadomatas : UIMonoBitmap, INSUUILadomat
    {
        const string LogTag = "UILadomatas";
        WoodBoiler katilas;
        Activity activity;
        bool disposed;
        string woodBoilerName;

        public UILadomatas(Context context, float scale):base(context, scale)
        {
            activity = (Activity)context;
            SetResource(NSUBitmapResource.Ladomatas);
            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemReady += OnNSUSystemReadyHandler;
            sys.OnNSUSystemUnavailable += OnNSUSystemUnavailableHandler;
        }

        private void OnNSUSystemReadyHandler(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(woodBoilerName))
            {
                var nsusys = NSUSystem.NSUSys.Instance;
                var wbs = nsusys.GetNSUSysPart(PartsTypes.WoodBoilers) as WoodBoilers;
                katilas = wbs.FindWoodBoiler(woodBoilerName);
                if (katilas != null)
                {
                    katilas.OnLadomatStatusChange += WoodBoilerOnLadomatStatusChangeHandler;
                    WoodBoilerOnLadomatStatusChangeHandler(katilas, katilas.LadomStatus);
                    OnMonoBitmapClicked += UILadomatasClickedHandler;
                }
            }
        }

        private void OnNSUSystemUnavailableHandler(object sender, EventArgs e)
        {
            NSULog.Debug(LogTag, $"OnNSUSystemUnavailableHandler(). UIID: {UIID}");
            WoodBoilerOnLadomatStatusChangeHandler(null, LadomatStatus.UNKNOWN);
            OnMonoBitmapClicked -= UILadomatasClickedHandler;
        }

        private void UILadomatasClickedHandler(object sender, EventArgs e)
        {
            if (katilas != null)
            {
                katilas.SwitchState(SwitchTarget.Ladomat);
            }
        }

        public void AttachLadomat(string name)
        {
            woodBoilerName = name;
            if (NSUSys.Instance.NSUSystemReady)
                OnNSUSystemReadyHandler(null, null);
        }

        new public NSUUIClass UIClass { get { return NSUUIClass.Ladomat; } }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Free();
        }

        public new void Free()
        {
            NSULog.Debug(string.Format("UILadomatas [UIID - '{0}']", UIID), "Free() is called.");
            if (!disposed)
            {
                disposed = true;
                base.Free();
                NSUSys sys = NSUSys.Instance;
                sys.OnNSUSystemReady += OnNSUSystemReadyHandler;
                sys.OnNSUSystemUnavailable += OnNSUSystemUnavailableHandler;
                if (katilas != null)
                {
					katilas.OnLadomatStatusChange -= WoodBoilerOnLadomatStatusChangeHandler;
                }
                katilas = null;
            }
        }

		void WoodBoilerOnLadomatStatusChangeHandler(WoodBoiler sender, LadomatStatus status)
        {
            NSULog.Debug("HandleOnLadomatStatusChange", status.ToString());
            activity.RunOnUiThread(() =>
                {
                    UpdateStatus(status);
                });
        }

		void UpdateStatus(LadomatStatus status)
        {
            NSULog.Debug("Updating Ladomat status", status.ToString());
            switch (status)
            {
				case LadomatStatus.MANUAL:
                    SetColor(NSUColor.MANUAL);
                    break;
				case LadomatStatus.ON:
                    SetColor(NSUColor.ON);
                    break;
				case LadomatStatus.OFF:
                    SetColor(NSUColor.OFF);
                    break;
                default:
                    SetColor(NSUColor.UNKNOWN);
                    break;
            }
        }

        public new void AttachedToWindow()
        {

        }

        public new void DeatachedFromWindow()
        {

        }
    }
}

