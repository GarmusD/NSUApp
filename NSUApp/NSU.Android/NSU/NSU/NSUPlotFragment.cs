using System;
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Webkit;
using NSUAppShared.NSUSystemParts;
using NSUAppShared;

namespace NSU.Droid
{
	public class NSUPlotFragment : Android.Support.V4.App.Fragment
    {
        readonly string logTag = "NSUPlotFragment";

        private View view;
        //PlotView plotView;
        WebView webView;
        Button btn1Day, btn2Days, btn7Days, btn14Days, btn30Days, btnRefresh;
        ListView lview;

        string myurl;
        int period = 1;
        string tsname = string.Empty;

        public static NSUPlotFragment NewInstance(int viewID, string title)
        {
            var f = new NSUPlotFragment { Arguments = new Bundle()};
            f.Arguments.PutInt("viewID", viewID);
            f.Arguments.PutString("title", title);
            return f;
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            Log.Debug(logTag, "OnDestroyView()");
            App.Current.NSUSys.OnNSUSystemReady -= HandleOnNSUPartsReady;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Log.Debug(logTag, "OnDestroy()");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            if (container == null)
            {
                return null;
            }

            view = inflater.Inflate(Arguments.GetInt("viewID"), null);

            //plotView = view.FindViewById<PlotView>(Resource.Id.plotView);
            webView = view.FindViewById<WebView>(Resource.Id.webView);
            webView.Settings.JavaScriptEnabled = true;
            //webView.ClearView();

            btn1Day = view.FindViewById<Button>(Resource.Id.btn1Day);
            btn1Day.Click += Handlebtn1DayClick;

            btn2Days = view.FindViewById<Button>(Resource.Id.btn2Days);
            btn2Days.Click += Handlebtn2DaysClick;

            btn7Days = view.FindViewById<Button>(Resource.Id.btn7Days);
            btn7Days.Click += Handlebtn7DaysClick;

            btn14Days = view.FindViewById<Button>(Resource.Id.btn14Days);
            btn14Days.Click += Handlebtn14DaysClick;

            btn30Days = view.FindViewById<Button>(Resource.Id.btn30Days);
            btn30Days.Click += Handlebtn30DaysClick;

            btnRefresh = view.FindViewById<Button>(Resource.Id.btnRefreshPlot);
            btnRefresh.Click += HandlebtnRefreshClick;

            lview = view.FindViewById<ListView>(Resource.Id.listPlotItems);
            lview.Clickable = true;
            lview.ChoiceMode = ChoiceMode.Single;
            lview.ItemsCanFocus = false;
            lview.ItemClick += HandleSensorsListViewItemClick;

            Preferences p = App.Current.Prefs;
            myurl = Credentials.Current.Host;

            App.Current.NSUSys.OnNSUSystemReady += HandleOnNSUPartsReady;
                if (App.Current.NSUSys.NSUSystemReady)
                {
					PopulateListTSensorNames((App.Current.NSUSys.GetNSUSysPart(PartsTypes.TSensors) as TempSensors).GetTSensorsNames());
                }
            return view;
        }

        void HandleOnNSUPartsReady (object sender, EventArgs e)
        {
			PopulateListTSensorNames((App.Current.NSUSys.GetNSUSysPart(PartsTypes.TSensors) as TempSensors).GetTSensorsNames());
        }

        void Handlebtn30DaysClick (object sender, EventArgs e)
        {
            period = 30;
            HandlebtnRefreshClick(null, null);
        }

        void HandleSensorsListViewItemClick (object sender, AdapterView.ItemClickEventArgs e)
        {
            tsname = (string)lview.GetItemAtPosition(e.Position);
            tsname = tsname.Replace(' ', '_');
            HandlebtnRefreshClick(null, null);
        }

        void Handlebtn14DaysClick (object sender, EventArgs e)
        {
            period = 14;
            HandlebtnRefreshClick(null, null);
        }

        void Handlebtn7DaysClick (object sender, EventArgs e)
        {
            period = 7;
            HandlebtnRefreshClick(null, null);
        }

        void Handlebtn2DaysClick (object sender, EventArgs e)
        {
            period = 2;
            HandlebtnRefreshClick(null, null);
        }

        void Handlebtn1DayClick (object sender, EventArgs e)
        {
            period = 1;
            HandlebtnRefreshClick(null, null);
        }

        string BuildUrl()
        {
            if(!string.IsNullOrWhiteSpace(myurl))
            {
                return "http://" + myurl +"/mobile/mobile.php"+ string.Format("?data={0:yyyy-MM-dd}&period={1}&tsname={2}", DateTime.Now, period, tsname);
            }
            return string.Empty;
        }

        void HandlebtnRefreshClick (object sender, EventArgs e)
        {
            string url = BuildUrl();
            Log.Debug(logTag, "Refreshing. Getting URL: " + url);
            webView.LoadUrl(url);
        }

        public override void OnStart()
        {
            base.OnStart();
            HandlebtnRefreshClick(null, null);
        }

        public override void OnResume()
        {
            base.OnResume();
        }


        void PopulateListTSensorNames(List<string> list)
        {
			Activity.RunOnUiThread(() => {
                Log.Debug(logTag, "Populating TSensor names list.");
                list.Sort();
                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = list[i].Replace('_', ' ');
                }
                lview.Adapter = new ArrayAdapter<string>(Application.Context, Android.Resource.Layout.SimpleListItemSingleChoice, list);
                if (lview.Adapter != null && lview.Adapter.Count > 0)
                {
                    if (!string.IsNullOrEmpty(tsname))
                    {
                        for (int i = 0; i < lview.Adapter.Count; i++)
                        {
                            var s = (string)lview.Adapter.GetItem(i);
                            if (s.Equals(tsname))
                            {
                                lview.SetItemChecked(i, true);
                                lview.SetSelection(i);
                                break;
                            }
                        }
                    }
                    else
                    {
                        tsname = list[0].Replace(' ', '_');
                        lview.SetItemChecked(0, true);
                        HandlebtnRefreshClick(null, null);
                    }
                }
            });
        }
    }
}

