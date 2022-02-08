using System;
using NSU.Shared.NSUUI;
using Android.Content;
using Android.App;
using NSU.Shared.NSUSystemPart;
using NSU.Shared;
using NSUAppShared.NSUSystemParts;
using NSU.NSUSystem;

namespace NSU.Droid.UI.Elements
{
	public class UIExhaustFan : UIMonoBitmap, INSUUIExhaustFan
	{
        const string LogTag = "UIExhaustFan";
        Activity activity;
        string woodBoilerName;
        WoodBoiler katilas;
		bool disposed;

		public UIExhaustFan(Context context, float scale)
			: base(context, scale)
		{
			activity = (Activity)context;
			SetResource(NSUBitmapResource.Ventiliatorius);
            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemUnavailable += OnNSUSystemUnavailableHandler;
            sys.OnNSUSystemReady += OnNSUSystemReadyHandler;
        }

        private void OnNSUSystemReadyHandler(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(woodBoilerName))
            {
                var nsusys = NSUSys.Instance;
                var wbs = nsusys.GetNSUSysPart(NSUAppShared.NSUSystemParts.PartsTypes.WoodBoilers) as WoodBoilers;
                katilas = wbs.FindWoodBoiler(woodBoilerName);
                if (katilas != null)
                {
                    katilas.OnExhaustFanStatusChange += WoodBoilerOnExhaustFanStatusChangeHandler;
                    WoodBoilerOnExhaustFanStatusChangeHandler(katilas, katilas.ExhaustFanStatus);
                    OnMonoBitmapClicked += UIExhaustFanClickedHandler;
                }
            }
        }

        private void UIExhaustFanClickedHandler(object sender, EventArgs e)
        {
            if(katilas != null)
            {
                katilas.SwitchState(SwitchTarget.ExhaustFan);
            }
        }

        private void OnNSUSystemUnavailableHandler(object sender, System.EventArgs e)
        {
            NSULog.Debug(LogTag, $"OnNSUSystemUnavailableHandler(). UIID: {UIID}");
            WoodBoilerOnExhaustFanStatusChangeHandler(null, ExhaustFanStatus.UNKNOWN);
            OnMonoBitmapClicked -= UIExhaustFanClickedHandler;
        }

        public void AttachSmokeFan(string name)
        {
            woodBoilerName = name;
            if (NSUSys.Instance.NSUSystemReady)
                OnNSUSystemReadyHandler(null, null);
        }

        new public NSUUIClass UIClass { get { return NSUUIClass.Ventilator; } }

        protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			Free();
		}

		public new void Free()
		{
			NSULog.Debug(string.Format("UIVeltiliatorius [UIID - '{0}']", UIID), "Free() is called.");
			if (!disposed)
			{
				disposed = true;
				base.Free();
                NSUSys sys = NSUSys.Instance;
                sys.OnNSUSystemReady += OnNSUSystemReadyHandler;
                sys.OnNSUSystemUnavailable += OnNSUSystemUnavailableHandler;

                if (katilas != null)
				{
					katilas.OnExhaustFanStatusChange -= WoodBoilerOnExhaustFanStatusChangeHandler;
				}
				katilas = null;
			}
		}

		void UpdateStatus(ExhaustFanStatus status)
		{
			NSULog.Debug("Updating Ventilator status", status.ToString());
			switch (status)
			{
				case ExhaustFanStatus.MANUAL:
					SetColor(NSUColor.MANUAL);
					break;
				case ExhaustFanStatus.ON:
					SetColor(NSUColor.ON);
					break;
				case ExhaustFanStatus.OFF:
					SetColor(NSUColor.OFF);
					break;
                default:
                    SetColor(NSUColor.UNKNOWN);
                    break;
			}
		}

		void WoodBoilerOnExhaustFanStatusChangeHandler(WoodBoiler sender, ExhaustFanStatus status)
		{
			NSULog.Debug("HandleOnVentilatorStatusChange", status.ToString());
			activity.RunOnUiThread(() =>
				{
					UpdateStatus(status);
				});
		}

		void HandleOnNSUPartsReady(object sender, EventArgs e)
		{
			AttachSmokeFan(((WoodBoilers)(App.Current.NSUSys.GetNSUSysPart(PartsTypes.WoodBoilers))).FindWoodBoiler(0).Name);
			App.Current.NSUSys.OnNSUSystemReady -= HandleOnNSUPartsReady;
		}

        public new void AttachedToWindow()
        {

        }

        public new void DeatachedFromWindow()
        {

        }
    }
}

