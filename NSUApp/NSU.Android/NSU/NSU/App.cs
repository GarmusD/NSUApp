using System;
using System.Threading.Tasks;
using Android.Util;
using Android.Content;
using NSUAppShared;
using NSU.NSUSystem;

namespace NSU.Droid
{
    public class App
    {
        // events
        //public event EventHandler<ServiceConnectedEventArgs> NSUServiceConnected = delegate {};

        // declarations
        private const string LogTag = "App";

        // properties

        public static App Current { get; private set; }

        public NSUSys NSUSys
        {
            get
            {
                return NSUSys.Instance;
            }
        }

        public Preferences Prefs { get; }

        public NetNotifier NetNotifier
        {
            get;
            set;
        }

        #region Application context

        static App ()
        {
            Current = new App();
        }
        protected App ()
        {
            Log.Debug(LogTag, "Creating App() instance.");
            Prefs = new Preferences(Android.App.Application.Context);            
            NetNotifier = new NetNotifier(Android.App.Application.Context);
        }

        #endregion
    }
}

