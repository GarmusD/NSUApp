using Android.App;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Android.Content.PM;
using NSU.Shared;
using NSU.Shared.NSUUI;
using NSU.Droid.UI.NSUWindows;
using System;
using Android.Widget;
using System.Threading.Tasks;

namespace NSU.Droid
{
    [Activity(Label = "NSU", Icon = "@drawable/ic_icon", ScreenOrientation = ScreenOrientation.Landscape, LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : Activity
    {
        const string LogTag = "MainActivity";

        MyWindow MainWindow;
        SideWindow MainSideWindow;
        Scenario scn;
        UIWindowsManager winman;
        bool layoutCreated;
        RelativeLayout rl;
        LinearLayout mainContainer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            NSULog.Debug(LogTag, "OnCreate()");
            ActionBar.Hide();
            SetContentView(Resource.Layout.windows);
            rl = FindViewById<RelativeLayout>(Resource.Id.rootLayout);
            mainContainer = FindViewById<LinearLayout>(Resource.Id.mainContainer);
            MainWindow = FindViewById<MyWindow>(Resource.Id.mainWindow);
            MainSideWindow = FindViewById<SideWindow>(Resource.Id.sideWindow);
            //MainWindow.OnCanCreateLayout += MainWindow_OnCanCreateLayout;
            //MainWindow_OnCanCreateLayout(0, 0);
            //InflateMainWindow(1f);
            rl.Post(() => { InflateMainWindow(); });
        }

        private void InflateMainWindow()
        {
            if (layoutCreated) return;
            layoutCreated = true;

            NSULog.Debug(LogTag, "InflateMainWindow()");

            // Compute the scaling ratio
            float xScale = (float)rl.Width / 714.0f;
            float yScale = (float)rl.Height / 480.0f;

            float scale = Math.Min(xScale, yScale);
            scale = scale == 0f ? 1f : scale;
            NSULog.Debug(LogTag, $"Scale factor: {scale}");
            mainContainer.LayoutParameters.Width = (int)(800.0f * scale);
            mainContainer.LayoutParameters.Height = (int)(480.0f * scale);

            MainWindow.LayoutParameters.Width = (int)(714.0f * scale);
            MainWindow.LayoutParameters.Height = (int)(480.0f * scale);
            MainSideWindow.LayoutParameters.Width = (int)(86.0f * scale);
            MainSideWindow.LayoutParameters.Height = (int)(480.0f * scale);

            winman = new UIWindowsManager(this, scale);
            scn = new Scenario();
            scn.PlayScenario(winman, null);

            winman.OnWindowsGroupChanged += OnWindowsGroupChangedHandler;
            winman.OnWindowChanged += OnWindowChangedHandler;

            var grp = winman.CurrentGroup;
            if (grp != null)
            {
                winman.ActivateWindowsGroup(grp.Name);
            }
            NSULog.Debug(LogTag, "InflateMainWindow() - Layout CREATED.");
        }

        private void OnWindowChangedHandler(object sender, System.EventArgs e)
        {
            NSULog.Debug(LogTag, $"OnWindowChangedHandler");
            MainWindow.AttachNSUWindow(sender as INSUWindow);
        }

        private void OnWindowsGroupChangedHandler(object sender, System.EventArgs e)
        {
            NSULog.Debug(LogTag, $"OnWindowsGroupChangedHandler");
            INSUWindowsGroup grp = sender as INSUWindowsGroup;
            //Change SideWindow
            MainSideWindow.AttachNSUSideWindow(grp.SideWindow);
            MainWindow.AttachNSUWindow(grp.CurrentWindow);
        }



        protected override void OnResume()
        {            
            base.OnResume();            
            NSULog.Debug(LogTag, "OnResume()");
        }

        protected override void OnStart()
        {
            base.OnStart();
            NSULog.Debug(LogTag, "OnStart()");
            var nsuSys = NSUSystem.NSUSys.Instance;
            var p = App.Current.Prefs;
            NSUAppShared.Credentials c = NSUAppShared.Credentials.Current;
            if (!nsuSys.NSUNetwork.Connected() && !nsuSys.NSUNetwork.Connecting)
            {
                if (c.HostOk)
                {
                    nsuSys.NSUNetwork.Connect(c.Host, c.Port);
                }
            }
            
        }

        protected override void OnStop()
        {
            base.OnStop();
            NSULog.Debug(LogTag, "OnStop()");
            var nsuSys = NSUSystem.NSUSys.Instance;
            nsuSys.NSUNetwork.Disconnect();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            NSULog.Debug(LogTag, "OnDestroy()");            
            MainWindow.Dispose();
            MainSideWindow.Dispose();
            winman.Dispose();
            var nsuSys = NSUSystem.NSUSys.Instance;
            nsuSys.Free();
            scn = null;
        }
    }
}


