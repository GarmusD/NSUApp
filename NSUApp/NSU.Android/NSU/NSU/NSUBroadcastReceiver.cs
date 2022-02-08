using Android.App;
using Android.Content;
using Android.Util;

namespace NSU.Droid
{
	[BroadcastReceiver]
    [IntentFilter (new[]{"android.net.conn.CONNECTIVITY_CHANGE"})]
    public class NSUBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action.Equals("android.net.conn.CONNECTIVITY_CHANGE"))
            {
                App.Current.NetNotifier.RaiseNetChangeEvent();
            }
        }
    }
}

