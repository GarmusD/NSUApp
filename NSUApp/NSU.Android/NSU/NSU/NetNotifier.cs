using Android.Net;
using Android.Content;
using NSU.Shared.NSUNet;
using System;

namespace NSU.Droid
{
	public class NetNotifier : INetChangeNotifier
    {
		readonly Context cntx;        

        public NetNotifier(Context value)
        {
            cntx = value;
        }        

        #region INetChangeNotifier implementation
        public event EventHandler<NetChangedEventArgs> OnNetworkAvailableChange;

        public void RaiseNetChangeEvent()
        {
            OnNetworkAvailableChange?.Invoke(this, new NetChangedEventArgs(IsOnline, NetType));
        }

        public NetworkType NetType
        {
            get
            {
                var cm = (ConnectivityManager)cntx.GetSystemService(Context.ConnectivityService);
                NetworkInfo ni = cm.ActiveNetworkInfo;
                if (ni != null && ni.IsConnected)
                {
                    switch (ni.Type)
                    {
                        case ConnectivityType.Ethernet:
                            return NetworkType.Ethernet;
                        case ConnectivityType.Mobile:
                        case ConnectivityType.MobileDun:
                        case ConnectivityType.MobileHipri:
                        case ConnectivityType.MobileMms:
                        case ConnectivityType.MobileSupl:
                            return NetworkType.Mobile;
                        case ConnectivityType.Wifi:
                            return NetworkType.Wifi;
                        default:
                            return NetworkType.None;
                    }
                }
                else
                {
                    return NetworkType.None;
                }
            }
        }

        public bool IsOnline
        {
            get
            {
                var cm = (ConnectivityManager)cntx.GetSystemService(Context.ConnectivityService);
                NetworkInfo ni = cm.ActiveNetworkInfo;
                if (ni != null && ni.IsConnected)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion
    }
}

