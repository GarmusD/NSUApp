using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using NSU.Shared.NSUUI;
using Android.Webkit;
using NSU.Shared;
using NSU.NSUSystem;
using NSU.Shared.NSUSystemPart;
using NSUAppShared.NSUSystemParts;
using NSU.Shared.NSUNet;
using System.Threading.Tasks;
using NSUAppShared;

namespace NSU.Droid.UI.Elements
{
    public class UIWeatherInfo : LinearLayout, INSUUIWeatherInfo
    {
        const string LogTag = "UIWeatherInfo";
        TextView laukoTemp, pozemioTemp;
        string laukoTS, pozemioTS;
        bool laukoTSOk, pozemioTSOk;
        WebView web;
        Activity act;
        float scale;
        NSUTimer timer;        

        public UIWeatherInfo(Context context, float scale):base(context)
        {
            this.scale = scale;
            act = (Activity)context;
            LayoutParameters = new ViewGroup.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);// (Utilities.ConvertDpToPixel(704, context), Utilities.ConvertDpToPixel(470, context));
            Inflate(context, Resource.Layout.WeatherInfo, this);

            laukoTemp = FindViewById<TextView>(Resource.Id.WILaukoTempTextView);
            pozemioTemp = FindViewById<TextView>(Resource.Id.WIPozemioTempTextView);
            web = FindViewById<WebView>(Resource.Id.wvWeatherInfoGrafikas);
            web.Settings.JavaScriptEnabled = true;
            web.SetWebViewClient(new WebViewHelper());
            string url = $"{NSUConsts.MobileURL}/weather_info.php";
            web.LoadUrl(url);
            laukoTSOk = pozemioTSOk = false;
            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemReady += OnNSUSystemReadyHandler;
            sys.OnNSUSystemUnavailable += OnNSUSystemUnavailableHandler;
            timer = new NSUTimer(5 * 60 * 1000);
            timer.OnNSUTimer += OnNSUTimerHandler;
        }

        private void OnNSUTimerHandler()
        {
            web.Reload();
        }

        private void OnNSUSystemReadyHandler(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(laukoTS) && !laukoTSOk)
            {
                TempSensors tss = NSUSys.Instance.GetNSUSysPart(PartsTypes.TSensors) as TempSensors;
                if(tss != null)
                {
                    TempSensor ts = tss.FindSensor(laukoTS);
                    if (ts != null)
                    {
                        laukoTSOk = true;
                        ts.OnTempChanged += OnTempChangedLauko;
                        OnTempChangedLauko(ts, new TempChangedEventArgs(ts.Temperature));
                    }
                }
            }
            if (!string.IsNullOrEmpty(pozemioTS) && !pozemioTSOk)
            {
                TempSensors tss = NSUSys.Instance.GetNSUSysPart(PartsTypes.TSensors) as TempSensors;
                if (tss != null)
                {
                    TempSensor ts = tss.FindSensor(pozemioTS);
                    if (ts != null)
                    {
                        pozemioTSOk = true;
                        ts.OnTempChanged += OnTempChangedPozemio;
                        OnTempChangedPozemio(ts, new TempChangedEventArgs(ts.Temperature));
                    }
                }
            }
        }

        private void OnNSUSystemUnavailableHandler(object sender, EventArgs e)
        {
            laukoTSOk = pozemioTSOk = false;
            act.RunOnUiThread(() => {
                if (!laukoTemp.Text.StartsWith("!"))
                {
                    laukoTemp.Text = "!" + laukoTemp.Text;
                    pozemioTemp.Text = "!" + pozemioTemp.Text;
                }
            });
        }

        private void OnTempChangedPozemio(object sender, TempChangedEventArgs e)
        {
            act.RunOnUiThread(()=> {
                pozemioTemp.Text = $"{e.Temperature:0.0} °C";
            });
        }

        private void OnTempChangedLauko(object sender, TempChangedEventArgs e)
        {
            act.RunOnUiThread(() => {
                laukoTemp.Text = $"{e.Temperature:0.0} °C";
            });
        }

        public void AttachLaukoSensor(string name)
        {
            laukoTS = name;
            if (NSUSys.Instance.NSUSystemReady)
                OnNSUSystemReadyHandler(null, null);
        }

        public void AttachPozemioSensor(string name)
        {
            pozemioTS = name;
            if (NSUSys.Instance.NSUSystemReady)
                OnNSUSystemReadyHandler(null, null);
        }

        new public int Left
        {
            get
            {
                return this.Left;
            }

            set
            {
                SetX(Utilities.ConvertDpToPixel(value, scale));
            }
        }

        new public int Top
        {
            get
            {
                return this.Top;
            }

            set
            {
                SetY(Utilities.ConvertDpToPixel(value, scale));
            }
        }

        new public int Height
        {
            get
            {
                return this.Height;
            }

            set
            {
                LayoutParameters.Height = Utilities.ConvertDpToPixel(value, scale);
            }
        }

        new public int Width
        {
            get
            {
                return this.Width;
            }

            set
            {
                LayoutParameters.Width = Utilities.ConvertDpToPixel(value, scale);
            }
        }
        public NSUUIClass UIClass
        {
            get
            {
                return NSUUIClass.WeatherInfo;
            }
        }

        public string UIID { get; set; }

        public void Free()
        {
            timer.Stop();
            timer = null;
            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemReady += OnNSUSystemReadyHandler;
            sys.OnNSUSystemUnavailable += OnNSUSystemUnavailableHandler;
        }

        public void AttachedToWindow()
        {
            web.Reload();
            timer.Start();
        }

        public void DeatachedFromWindow()
        {
            timer.Stop();
        }
    }
}