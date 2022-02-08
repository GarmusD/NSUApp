using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using NSU.Droid.UI;
using NSU.Shared.NSUUI;
using NSU.NSUSystem;
using NSU.Shared;
using NSU.Droid.UI.NSUWindows;

namespace NSU.Droid
{
	public class NSUScreenFragment : Android.Support.V4.App.Fragment
    {
        readonly string LogTag = "NSUScreenView";

        bool layoutCreated;
        private View view;

        MyWindow MainWindow;
        SideWindow MainSideWindow;

        Scenario scn;

        UIWindowsManager winman;

        public static NSUScreenFragment NewInstance(int viewID, string title)
        {
            var f = new NSUScreenFragment { Arguments = new Bundle() };
            f.Arguments.PutInt("viewID", viewID);
            f.Arguments.PutString("title", title);
            return f;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            if (container == null)
            {
                return null;
            }
            layoutCreated = false;
            view = inflater.Inflate(Arguments.GetInt("viewID"), null);

            //Create UI
            Log.Debug(LogTag, "Finding MyWindow............................. xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
            MainWindow = view.FindViewById<MyWindow>(Resource.Id.mainWindow);
            MainSideWindow = view.FindViewById<SideWindow>(Resource.Id.sideWindow);

            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {            
            base.OnViewCreated(view, savedInstanceState);
            Log.Debug(LogTag, "OnViewCreated()............................. xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
            Log.Debug(LogTag, $"MainWindow Width: {MainWindow.MeasuredWidth} Height: {MainWindow.MeasuredHeight}");
            CreateUI();

        }

        public override void OnDestroyView()
        {
            NSULog.Debug(LogTag, "OnDestroyView() is called.");
            base.OnDestroyView();
            MainWindow.Dispose();
        }

        void HandleDoOnLayout(bool changed, int l, int t, int r, int b)
        {
        }

        void CreateUI()
        {
            NSULog.Debug(LogTag, "CreateUI()");
            if (!layoutCreated)
            {                
                NSULog.Debug(LogTag, "CreateUI() - Layout NOT created. Creating.");

                winman = new UIWindowsManager(Context, 1.0f);
                scn = new Scenario();
                scn.PlayScenario(winman, null);
                winman.OnWindowsGroupChanged += OnWindowsGroupChangedHandler;
                winman.OnWindowChanged += OnWindowChangedHandler;

                var grp = winman.CurrentGroup;
                if(grp != null)
                {
                    winman.ActivateWindowsGroup(grp.Name);
                }                

                layoutCreated = true;
            }            
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
    }
}

