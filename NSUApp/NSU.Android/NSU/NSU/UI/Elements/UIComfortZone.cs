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
using NSUAppShared.NSUSystemParts;
using Android.Graphics;
using System.Threading.Tasks;
using NSU.Shared.NSUSystemPart;

namespace NSU.Droid.UI.Elements
{
    public class UIComfortZone : LinearLayout, INSUUIComfortZone
    {
        private const string LogTag = "UIComfortZone";
        private Activity act;
        private float scale;
        private TextView roomTemp, floorTemp;//title
        private ImageView btnSettings, btnShowGraph, heatFloor;
        private string czName, czTitle, rsName, fsName;
        private bool rsAvailable, fsAvailable;
        private bool cpOn, valveOpened;


        public UIComfortZone(Context context, float scale):base(context)
        {
            act = (Activity)context;
            this.scale = scale;
            LayoutParameters = new ViewGroup.LayoutParams(Utilities.ConvertDpToPixel(80, scale), LayoutParams.WrapContent);
            Inflate(context, Resource.Layout.ComfortZone, this);

            //title = FindViewById<TextView>(Resource.Id.CZTitleTextView);
            roomTemp = FindViewById<TextView>(Resource.Id.CZCurrentTemperatureTextView);
            floorTemp = FindViewById<TextView>(Resource.Id.CZTemperatureTextView);
            btnSettings = FindViewById<ImageView>(Resource.Id.CZSettingsImageViewButton);
            btnShowGraph = FindViewById<ImageView>(Resource.Id.CZShowGraphImageViewButton);
            heatFloor = FindViewById<ImageView>(Resource.Id.CZHeatFloorImageView);

            rsAvailable = fsAvailable = false;
            roomTemp.Text = "-.- °C";
            roomTemp.SetTextColor(Color.Orange);
            roomTemp.SetTextSize(Android.Util.ComplexUnitType.Px,  12 * scale);

            floorTemp.Text = "-.- °C";
            floorTemp.SetTextColor(Color.Orange);
            floorTemp.SetTextSize(Android.Util.ComplexUnitType.Px, 10 * scale);

            btnSettings.LayoutParameters.Width = (int)(btnSettings.LayoutParameters.Width * scale);
            btnSettings.LayoutParameters.Height = (int)(btnSettings.LayoutParameters.Height * scale);


            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemReady += OnNSUSystemReadyHandler;
            sys.OnNSUSystemUnavailable += OnNSUSystemUnavailableHandler;
            btnShowGraph.Click += BtnShowGraph_ClickHandler;
        }

        private void BtnShowGraph_ClickHandler(object sender, EventArgs e)
        {
            if (rsAvailable || fsAvailable)
                ShowChart();
        }

        private void OnNSUSystemReadyHandler(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(czName))
            {
                NSUSys sys = NSUSys.Instance;
                var czns = sys.GetNSUSysPart(PartsTypes.ComfortZones) as ComfortZones;
                if(czns != null)
                {
                    var cz = czns.FindComfortZone(czName);
                    if(cz != null)
                    {
                        rsName = cz.RoomSensorName;
                        fsName = cz.FloorSensorName;
                        czTitle = cz.Title;
                        if (!string.IsNullOrEmpty(cz.RoomSensorName))
                        {
                            rsAvailable = true;
                            cz.OnRoomTemperatureChanged += OnRoomTemperatureChangedHandler;
                            OnRoomTemperatureChangedHandler(cz, new TempChangedEventArgs(cz.CurrentRoomTemperature));
                        }
                        if (!string.IsNullOrEmpty(cz.FloorSensorName))
                        {
                            fsAvailable = false;
                            cz.OnFloorTemperatureChanged += OnFloorTemperatureChangedHandler;
                            OnFloorTemperatureChangedHandler(cz, new TempChangedEventArgs(cz.CurrentFloorTemperature));
                        }
                        cz.OnActuatorOpenedChanged += OnActuatorOpenedChangedHandler;
                        OnActuatorOpenedChangedHandler(cz, new ActuatorOpenedEventArgs(cz.ActuatorOpened));

                        if (!string.IsNullOrEmpty(cz.CollectorName))
                        {
                            var cls = sys.GetNSUSysPart(PartsTypes.Collectors) as Collectors;
                            var cl = cls.FindCollector(cz.CollectorName);
                            if(cl != null)
                            {
                                var cpumps = sys.GetNSUSysPart(PartsTypes.CircPumps) as CircPumps;
                                var cp = cpumps.FindCircPump(cl.CircPumpName);
                                if(cp != null)
                                {
                                    cp.OnStatusChanged += OnCircPumpStatusChangedHandler;
                                    OnCircPumpStatusChangedHandler(cp, new StatusChangedEventArgs(cp.Status));
                                }
                            }
                        }
                    }
                }
            }
        }


        private void OnCircPumpStatusChangedHandler(object sender, StatusChangedEventArgs e)
        {
            cpOn = true;
            if( e.Status == Shared.NSUSystemPart.Status.DISABLED || 
                e.Status == Shared.NSUSystemPart.Status.DISABLED_OFF || 
                e.Status == Shared.NSUSystemPart.Status.OFF)
            {
                cpOn = false;
            }
            UpdateFloorHeatImage();
        }

        private void OnActuatorOpenedChangedHandler(object sender, ActuatorOpenedEventArgs e)
        {
            valveOpened = e.Opened;
            UpdateFloorHeatImage();
        }

        private void OnFloorTemperatureChangedHandler(object sender, TempChangedEventArgs e)
        {
            act.RunOnUiThread(() => {
                if (e.Temperature != -127)
                {
                    floorTemp.Text = $"{e.Temperature:0.0} °C";
                    //TODO Reikia LowTempMode kiekvienai CZ, ir tikrinti, pagal ka nustatyt warningus
                    if (e.Temperature < ((sender as ComfortZone).RoomTempHi - (sender as ComfortZone).Histeresis * 2))
                    {
                        floorTemp.SetTextColor(Color.Blue);
                    }
                    else if (e.Temperature > ((sender as ComfortZone).RoomTempHi + (sender as ComfortZone).Histeresis * 2))
                    {
                        floorTemp.SetTextColor(Color.Red);
                    }
                    else
                    {
                        floorTemp.SetTextColor(Color.Green);
                    }
                }
                else
                {
                    floorTemp.Text = "-.- °C";
                    floorTemp.SetTextColor(Color.Orange);
                }
            });
        }

        private void OnRoomTemperatureChangedHandler(object sender, TempChangedEventArgs e)
        {
            act.RunOnUiThread(() => {
                if (e.Temperature != -127)
                {
                    roomTemp.Text = $"{e.Temperature:0.0} °C";
                    //TODO Reikia LowTempMode kiekvienai CZ, ir tikrinti, pagal ka nustatyt warningus
                    if (e.Temperature < ((sender as ComfortZone).RoomTempHi - (sender as ComfortZone).Histeresis * 2))
                    {
                        roomTemp.SetTextColor(Color.Blue);
                    }
                    else if (e.Temperature > ((sender as ComfortZone).RoomTempHi + (sender as ComfortZone).Histeresis * 2))
                    {
                        roomTemp.SetTextColor(Color.Red);
                    }
                    else
                    {
                        roomTemp.SetTextColor(Color.Green);
                    }
                }
                else
                {
                    roomTemp.Text = "-.- °C";
                    roomTemp.SetTextColor(Color.Orange);
                }
            });
        }

        private void OnNSUSystemUnavailableHandler(object sender, EventArgs e)
        {
            act.RunOnUiThread(() => {
                if(rsAvailable)
                    roomTemp.Text = "!" + roomTemp.Text;
                if (fsAvailable)
                    floorTemp.Text = "!" + floorTemp.Text;
                rsAvailable = fsAvailable = false;
                heatFloor.SetImageResource(Resource.Drawable.heat_floor_black);
            });
        }

        private void UpdateFloorHeatImage()
        {
            if(valveOpened && cpOn)
            {
                heatFloor.SetImageResource(Resource.Drawable.heat_floor_red);
            }
            else if(valveOpened && !cpOn)
            {
                heatFloor.SetImageResource(Resource.Drawable.heat_floor_orange);
            }
            else
                heatFloor.SetImageResource(Resource.Drawable.heat_floor_blue);
        }

        private void ShowChart()
        {
            Dialog dialog = new Dialog(Context);
            dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            dialog.SetCancelable(true);
            dialog.SetContentView(Resource.Layout.SensorGraph);

            TextView sname = dialog.FindViewById<TextView>(Resource.Id.SGTSNameTextView);
            sname.Text = czTitle;

            WebView web = dialog.FindViewById<WebView>(Resource.Id.SGwebView);
            web.Settings.JavaScriptEnabled = true;
            web.SetWebViewClient(new WebViewHelper());
            string url = $"http://nsu.dgs.lt/mobile/sensor_chart.php?tsname={rsName}";
            web.LoadUrl(url);
            Button btn = dialog.FindViewById<Button>(Resource.Id.SGOkButton);

            btn.Click += (o, i) => {
                dialog.Dismiss();
            };

            WindowManagerLayoutParams lp = new WindowManagerLayoutParams();
            lp.CopyFrom(dialog.Window.Attributes);
            Rect r = new Rect();
            Activity activity = (Activity)Context;
            activity.Window.DecorView.GetWindowVisibleDisplayFrame(r);
            lp.Width = WindowManagerLayoutParams.MatchParent;// (int)(r.Width() * 0.9f);//
            lp.Height = WindowManagerLayoutParams.MatchParent; //(int)(r.Height() * 1.0f);//WindowManagerLayoutParams.MatchParent;

            dialog.Show();
            dialog.Window.Attributes = lp;
        }

        public NSUUIClass UIClass { get{return NSUUIClass.ComfortZone;} }

        public string UIID { get; set; }

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

        int INSUUIBase.Height
        {
            get
            {
                return Utilities.ConvertPixelsToDp(LayoutParameters.Height, scale);
            }

            set
            {
                LayoutParameters.Height = Utilities.ConvertDpToPixel(value, scale);
            }
        }

        int INSUUIBase.Width
        {
            get
            {
                return Utilities.ConvertPixelsToDp(LayoutParameters.Width, scale);
            }

            set
            {
                LayoutParameters.Width = Utilities.ConvertDpToPixel(value, scale);
            }
        }

        public void AttachComfortZone(string name)
        {
            czName = name;
            if (NSUSys.Instance.NSUSystemReady)
                OnNSUSystemReadyHandler(null, null);
        }

        public void Free()
        {
            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemReady += OnNSUSystemReadyHandler;
            sys.OnNSUSystemUnavailable += OnNSUSystemUnavailableHandler;
        }

        public void AttachedToWindow()
        {
            
        }

        public void DeatachedFromWindow()
        {
            
        }
    }
}