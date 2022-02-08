using Android.App;
using Android.Content;
using Android.OS;

using NSU.NSUSystem;
using Android.Widget;
using Android.Content.PM;
using NSU.Shared;
using System;

namespace NSU.Droid
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/ic_icon", ScreenOrientation = ScreenOrientation.Landscape)]			
    public class Loader : Activity
    {
        readonly string LogTag = "NSULoader";
        TextView infoText;
        private bool startingActivity = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            NSULog.Debug(LogTag, "OnCreate()");
            base.OnCreate(savedInstanceState);
            ActionBar.Hide();

            SetContentView(Resource.Layout.Loader);

            infoText = FindViewById<TextView>(Resource.Id.loaderInfoText);

            // This event fires when the ServiceConnection lets the client (our App class) know that
            // the Service is connected. We use this event to start updating the UI with location
            // updates from the Service

            //serviceConnected = true;
            //if (!App.Current.ServiceIsRunning)
            //{
            //    serviceConnected = false;
            //    App.Current.NSUServiceConnected += HandleNSUServiceConnected;
            //}
        }

        //void HandleNSUServiceConnected (object sender, ServiceConnectedEventArgs e)
        //{
        //    serviceConnected = true;
        //    CheckAndContinue();
        //}

        protected override void OnStart()
        {
            base.OnStart();
            NSULog.Debug(LogTag, "OnStart()");

            var nsuSys = NSUSys.Instance;
            nsuSys.AddNSUNetworkSocket(new NSUSocket());
            nsuSys.NSUNetwork.NetChangeNotifier = App.Current.NetNotifier;

            Preferences p = App.Current.Prefs;
            NSUAppShared.Credentials c = NSUAppShared.Credentials.Current;

            if (!nsuSys.NSUNetwork.NetworkAvailable)
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Internetas");
                alert.SetMessage("Reikalingas interneto ryšys.");
                alert.SetPositiveButton(GetString(Android.Resource.String.Ok), (senderAlert, args) =>
                {
                    Finish();
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            }
            else
            {
                if (c.HostOk && c.CredentialsOk)
                {
                    infoText.Text = "Jungiamasi...";
                    nsuSys.OnNSUSystemReady += NsuSys_OnNSUSystemReady;
                    nsuSys.NSUNetwork.OnConnectFailure += NSUNetwork_OnConnectFailure;
                    nsuSys.NSUNetwork.OnLoggedIn += NSUNetwork_OnLoggedIn;
                    nsuSys.NSUNetwork.Connect(c.Host, c.Port);                    
                }
                else
                {
                    ShowLoginScreen();
                }
            }
        }

        private void NSUNetwork_OnLoggedIn(object sender, EventArgs e)
        {
            this.RunOnUiThread(() => 
            {
                infoText.Text = "Gaunami duomenys...";
            });
        }

        private void NSUNetwork_OnConnectFailure(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                infoText.Text = "Negaliu prisijungti. Bandau dar kartą...";
            });
        }

        private void NsuSys_OnNSUSystemReady(object sender, System.EventArgs e)
        {
            var nsuSys = NSUSys.Instance;
            nsuSys.OnNSUSystemReady -= NsuSys_OnNSUSystemReady;
            nsuSys.NSUNetwork.OnConnectFailure -= NSUNetwork_OnConnectFailure;
            nsuSys.NSUNetwork.OnLoggedIn -= NSUNetwork_OnLoggedIn;
            StartMainActivityAndFinish();
        }

        protected override void OnResume()
        {
            base.OnResume();
            NSULog.Debug(LogTag, "OnResume()");
            //CheckAndContinue();
        }

        protected override void OnStop()
        {
            base.OnStop();
            NSULog.Debug(LogTag, "OnStop()");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            NSULog.Debug(LogTag, "OnDestroy()");
            if(!startingActivity)
            {
                NSUSys.Instance.Free();
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            NSULog.Debug(LogTag, "OnActivityResult()");
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                StartMainActivityAndFinish();
                return;
            }
            startingActivity = false;
            Finish();
        }

        void ShowLoginScreen()
        {
            NSULog.Debug(LogTag, "ShowLoginScreen()");
            startingActivity = true;
            var login = new Intent(this, typeof(LoginActivity));
            StartActivityForResult(login, 1);
        }

        void StartMainActivityAndFinish()
        {
            NSULog.Debug(LogTag, "StartMainActivityAndFinish()");
            startingActivity = true;
            var main = new Intent(this, typeof(MainActivity));
            main.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            StartActivity(main);
            Finish();
        }
            
        //void ServiceTryConnect()
        //{
        //    NSULog.Debug(LogTag, "ServiceTryConnect()");
        //    if (serviceConnected && credentialsOk && !NSUSys.Instance.NSUNetwork.Connected())
        //    {
        //        NSULog.Debug(LogTag, "OnCreate() - Connecting to NSUServer.");
        //        Preferences p = App.Current.Prefs;
        //        NSUSys.Instance.NSUNetwork.Connect(p.Host, int.Parse(p.Port));
        //    }
        //}
            
    }
}

