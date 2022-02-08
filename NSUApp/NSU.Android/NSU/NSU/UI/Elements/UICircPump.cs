using System;
using Android.Content;
using Android.App;
using NSU.Shared.NSUSystemPart;
using NSU.Shared.NSUUI;
using NSU.NSUSystem;
using NSU.Shared;
using NSUAppShared.NSUSystemParts;

namespace NSU.Droid.UI.Elements
{
	public class UICircPump : UIMonoBitmap, INSUUICircPump
	{
        const string LogTag = "UICircPump";
		Activity activity;
		float scale;
		bool disposed;
		CircPump circPump;
		string cpName = string.Empty;

		public UICircPump(Context context, float scale) : base(context, scale)
		{
			this.scale = scale;
			activity = (Activity)context;
			SetResource(NSUBitmapResource.Cirkuliacinis);
            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemReady += OnNSUSystemReadyHandler;
            sys.OnNSUSystemUnavailable += OnNSUSystemUnavailableHandler;
        }

        private void OnNSUSystemUnavailableHandler(object sender, EventArgs e)
        {
            NSULog.Debug(LogTag, $"OnNSUSystemUnavailableHandler(). UIID: {UIID}");
            OnCircPumpStatusChange(null, new StatusChangedEventArgs(Status.UNKNOWN));
            if (circPump != null)
                OnMonoBitmapClicked -= OnMonoBitmapClickedHandler;
            circPump = null;
        }

        private void OnNSUSystemReadyHandler(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(cpName))
            {
                NSULog.Debug(LogTag, $"AttachPump(pumpName: {cpName})");
                var nsusys = NSUSys.Instance;
                var cps = nsusys.GetNSUSysPart(PartsTypes.CircPumps) as CircPumps;
                if (cps != null)
                {
                    circPump = cps.FindCircPump(cpName);
                    if (circPump != null)
                    {
                        circPump.OnStatusChanged += OnCircPumpStatusChange;
                        OnCircPumpStatusChange(circPump, new StatusChangedEventArgs(circPump.Status));
                        OnMonoBitmapClicked += OnMonoBitmapClickedHandler;
                    }
                    else
                    {
                        NSULog.Debug(LogTag, $"AttachPump(pumpName: {cpName}) - not found.");
                    }
                }
            }
            else
            {
                circPump = null;
            }
        }

        private void OnMonoBitmapClickedHandler(object sender, EventArgs e)
        {
            if(circPump != null)
            {
                circPump.Clicked();
            }
        }

        protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			Free();
		}

		public new void Free()
		{
			NSULog.Debug(string.Format("UICircPump [UIID - '{0}']", UIID), "Free() is called.");
			if (!disposed)
			{
                NSUSys sys = NSUSys.Instance;
                sys.OnNSUSystemReady += OnNSUSystemReadyHandler;
                sys.OnNSUSystemUnavailable += OnNSUSystemUnavailableHandler;

                base.Free();
				disposed = true;
				if (circPump != null)
				{
					circPump.OnStatusChanged -= OnCircPumpStatusChange;
					circPump = null;
				}
			}
		}

		public void AttachPump(string pumpName)
		{
            cpName = pumpName;
            if (NSUSys.Instance.NSUSystemReady)
            {
                OnNSUSystemReadyHandler(null, null);
            }
        }

		void OnCircPumpStatusChange(object sender, StatusChangedEventArgs e)
		{
			activity.RunOnUiThread(() =>
				{
					UpdateStatus(e.Status);
				});
		}

		void HandleOnNSUPartsReady(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(cpName))
			{
				AttachPump(cpName);
                if(circPump != null)
                {
                    
                }
			}
			App.Current.NSUSys.OnNSUSystemReady -= HandleOnNSUPartsReady;
		}

		void UpdateStatus(Status status)
		{
			switch (status)
			{
				case Status.UNKNOWN:
					SetColor(NSUColor.UNKNOWN);
					break;
				case Status.MANUAL:
					SetColor(NSUColor.MANUAL);
					break;
				case Status.ON:
					SetColor(NSUColor.ON);
					break;
				case Status.OFF:
					SetColor(NSUColor.OFF);
					break;
				case Status.DISABLED:
					SetColor(NSUColor.DISABLED);
					break;
			}
		}

		public void SetGraphicsBytes(byte[] buf)
		{
			throw new NotImplementedException();
		}

		public void SetVectorBytes(byte[] buf)
		{
			throw new NotImplementedException();
		}

		public void SetBackColor(NSUColor color)
		{
			throw new NotImplementedException();
		}

		public void SetPosition(float x, float y)
		{
			throw new NotImplementedException();
		}

		public INSUUIDrawer GetDrawer()
		{
            return null;
		}

        new public NSUUIClass UIClass { get { return NSUUIClass.CircPump; } }

        new public void AttachedToWindow()
        {

        }

        new public void DeatachedFromWindow()
        {

        }
    }
}

