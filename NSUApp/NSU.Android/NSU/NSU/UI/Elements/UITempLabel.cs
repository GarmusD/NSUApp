using System;
using Android.Widget;
using Android.Content;
using Android.Graphics;
using Android.App;
using NSU.Shared.NSUUI;
using NSU.Shared.NSUSystemPart;
using NSU.Shared;
using NSUAppShared.NSUSystemParts;
using NSU.NSUSystem;
using Android.Webkit;
using Android.Views;
using System.Threading.Tasks;

namespace NSU.Droid.UI.Elements
{
    public class UITempLabel : TextView, INSUUITempLabel
    {
        const string LogTag = "UITempLabel";
		readonly Activity activity;
        readonly Context context;
		float scale;
        string tsname = string.Empty;
        string ktpname = string.Empty;
        TempSensor tsi;
        KType ktp;
        bool disposed;

        public UITempLabel(Context context, float scale)
            : base(context)
        {
            //WebView.SetSafeBrowsingWhitelist()
            this.scale = scale;
			activity = (Activity)context;
            this.context = context;

			SetBackgroundResource(Resource.Drawable.temp_label_shape);
            LayoutParameters = new Android.Views.ViewGroup.LayoutParams(Utilities.ConvertDpToPixel(80, scale), Utilities.ConvertDpToPixel(27, scale));

			SetTextColor(Color.ParseColor("#ff0000ff"));
			Gravity = Android.Views.GravityFlags.Center;
			//this.SetTypeface(                    android:typeface="normal"
			SetTextSize(Android.Util.ComplexUnitType.Px, Utilities.ConvertDpToPixel(17, scale));
			SetMinimumHeight(0);
			SetMinimumWidth(0);

            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemReady += OnNSUSystemReadyHandler;
            sys.OnNSUSystemUnavailable += OnNSUSystemUnavailableHandler;

            Click += UITempLabel_ClickHandler;
        }

        private void UITempLabel_ClickHandler(object sender, EventArgs e)
        {
            //switch(TempLableClickAction)
            //var b = new AlertDialog.Builder(context);
            //b.SetTitle(GetString(Resource.String.strLoginErrorTitle));
            //b.SetMessage(GetString(Resource.String.strBadUsrNamePassword));
            Dialog dialog = new Dialog(activity);
            dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            dialog.SetCancelable(true);
            dialog.SetContentView(Resource.Layout.SensorGraph);
                        
            TextView sname = dialog.FindViewById<TextView>(Resource.Id.SGTSNameTextView);
            if(!string.IsNullOrEmpty(tsname))
                sname.Text = tsname;
            else if (!string.IsNullOrEmpty(ktpname))
                sname.Text = ktpname;
            WebView web = dialog.FindViewById<WebView>(Resource.Id.SGwebView);
            web.Settings.JavaScriptEnabled = true;
            string url = $"{NSUAppShared.NSUConsts.MobileURL}/sensor_chart.php?tsname={sname.Text}";
            web.SetWebViewClient(new WebViewHelper());
            web.LoadUrl(url);            
            Button btn = dialog.FindViewById<Button>(Resource.Id.SGOkButton);

            btn.Click += (o, i) => {
                dialog.Dismiss();
            };

            WindowManagerLayoutParams lp = new WindowManagerLayoutParams();
            lp.CopyFrom(dialog.Window.Attributes);
            Rect r = new Rect();
            activity.Window.DecorView.GetWindowVisibleDisplayFrame(r);
            lp.Width = WindowManagerLayoutParams.MatchParent;// (int)(r.Width() * 0.9f);//
            lp.Height = WindowManagerLayoutParams.MatchParent; //(int)(r.Height() * 1.0f);//WindowManagerLayoutParams.MatchParent;

            dialog.Show();
            dialog.Window.Attributes = lp;
        }

        private void OnNSUSystemReadyHandler(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tsname))
            {
                NSULog.Debug(LogTag, $"OnNSUSystemReadyHandler(). UIID: {UIID}. Attaching TSensor.");
                NSUSys s = NSUSys.Instance;
                TempSensors ts = s.GetNSUSysPart(PartsTypes.TSensors) as TempSensors;
                tsi = ts.FindSensor(tsname);
                if (tsi != null)
                {
                    tsi.OnTempChanged += HandleOnTempChange;
                    HandleOnTempChange(tsi, new TempChangedEventArgs(tsi.Temperature));
                }
            }
            else if (!string.IsNullOrWhiteSpace(ktpname))
            {
                NSULog.Debug(LogTag, $"OnNSUSystemReadyHandler(). UIID: {UIID}. Attaching KType.");
                NSUSys s = NSUSys.Instance;
                KTypes ktps = s.GetNSUSysPart(PartsTypes.KTypes) as KTypes;
                var ktp = ktps.FindKType(ktpname);
                if (ktp != null)
                {
                    ktp.OnTempChanged += HandleOnKTypeTempChanged;
                    HandleOnKTypeTempChanged(ktp, new KTypeTempChangedEventArgs(ktp.Temperature));
                }
            }
            activity.RunOnUiThread(() =>
            {
                SetTextColor(Color.Blue);
            });
        }

        private void OnNSUSystemUnavailableHandler(object sender, EventArgs e)
        {
            activity.RunOnUiThread(() =>
            {
                NSULog.Debug(LogTag, $"OnNSUSystemUnavailableHandler(). UIID: {UIID}");
                SetTextColor(Color.Black);
            });
        }

        protected override void Dispose(bool disposing)
        {
            NSULog.Debug(string.Format("UITempLabel [{0}]", UIID), "Dispose() is called.");
            base.Dispose(disposing);
            Free();
        }
        #region INSUUITempLabel implementation

        public void Free()
        {
            NSULog.Debug(string.Format("UITempLabel [UIID - '{0}']", UIID), "Free() is called.");
            if (!disposed)
            {
                disposed = true;
                NSUSys sys = NSUSys.Instance;
                sys.OnNSUSystemReady += OnNSUSystemReadyHandler;
                sys.OnNSUSystemUnavailable += OnNSUSystemUnavailableHandler;

                if (tsi != null)
                {
					tsi.OnTempChanged -= HandleOnTempChange;
                    tsi = null;
                    tsname = string.Empty;
                }
                if (ktp != null)
                {
					ktp.OnTempChanged -= HandleOnKTypeTempChanged;
                    ktp = null;
                    ktpname = string.Empty;
                }
            }
        }

        public new int Left
        {
            get { return Utilities.ConvertPixelsToDp((int)GetX(), scale); }
            set { SetX(Utilities.ConvertDpToPixel(value, scale)); }
        }

        public new int Top
        {
            get { return Utilities.ConvertPixelsToDp((int)GetY(), scale); }
            set { SetY(Utilities.ConvertDpToPixel(value, scale)); }
        }

        public new int Width
        {
            get{ return Utilities.ConvertPixelsToDp(LayoutParameters.Width, scale); }
            set{ SetWidth(Utilities.ConvertDpToPixel(value, scale)); }
        }

        public new int Height
        {
            get{ return Utilities.ConvertPixelsToDp(LayoutParameters.Height, scale); }
            set{ SetWidth(Utilities.ConvertDpToPixel(value, scale)); }
        }

        public int CenterX
        {
            set
            {
                Left = value - (Width / 2);
            }
        }

        public NSUUIClass UIClass { get { return NSUUIClass.TempLabel; } }

        public string UIID { get; set; }

        public void AttachTempSensor(string name)
        {
            tsname = name;
            if (NSUSys.Instance.NSUSystemReady)
                OnNSUSystemReadyHandler(null, null);
        }

        void HandleOnTempChange(object sender, TempChangedEventArgs e)
        {
            NSULog.Debug(LogTag, "HandleOnTempChange");
            UpdateTempF(e.Temperature);
        }

        void UpdateTempF(float temp)
        {
            NSULog.Debug(string.Format("UITempLabel - '{0}'", UIID), string.Format("Updating to temp: {0}", temp));
            activity.RunOnUiThread(() =>
            {
                Text = $"{temp:0.0} °C";
            });
        }

        void UpdateTempI(int temp)
        {
            activity.RunOnUiThread(() =>
            {
                Text = string.Format("{0} °C", temp);
            });
        }

        void HandleOnNSUPartsReady(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tsname))
            {
                AttachTempSensor(tsname);
            }
            if (!string.IsNullOrWhiteSpace(ktpname))
            {
                AttachKType(ktpname);
            }
            App.Current.NSUSys.OnNSUSystemReady -= HandleOnNSUPartsReady;
        }

        public void AttachKType(string name)
        {
            ktpname = name;
            if (NSUSys.Instance.NSUSystemReady)
                OnNSUSystemReadyHandler(null, null);
        }

        void HandleOnKTypeTempChanged (object sender, KTypeTempChangedEventArgs e)
        {
            NSULog.Debug("UITempLabel", "HandleOnKTypeTempChange");
            UpdateTempI(e.Temperature);
        }

        public void AttachedToWindow()
        {

        }

        public void DeatachedFromWindow()
        {

        }
        #endregion
    }
}

