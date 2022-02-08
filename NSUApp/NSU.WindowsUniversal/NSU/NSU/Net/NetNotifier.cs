using NSU.Shared.NSUNet;
using System;
using Windows.Networking.Connectivity;

namespace NSU.NSU_UWP.Net
{
    class NetNotifier : INetChangeNotifier
    {
        public event EventHandler<NetChangedEventArgs> OnNetworkAvailableChange;

        public NetNotifier()
        {
            NetworkInformation.NetworkStatusChanged += NetworkInformationOnNetworkStatusChanged;
            CheckInternetAccess();
        }

        private void NetworkInformationOnNetworkStatusChanged(object sender)
        {
            CheckInternetAccess();
        }

        private void CheckInternetAccess()
        {            
            IsOnline = false;
            NetType = NetworkType.None;
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            if (connectionProfile != null)
            {
                IsOnline = (connectionProfile != null &&
                            connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
                if (IsOnline)
                {
                    if (connectionProfile.IsWlanConnectionProfile)
                    {
                        NetType = NetworkType.Wifi;
                    }
                    else if (connectionProfile.IsWwanConnectionProfile)
                    {
                        NetType = NetworkType.Mobile;
                    }
                    else
                    {
                        NetType = NetworkType.Ethernet;
                    }
                }
            }
            OnNetworkAvailableChange?.Invoke(this, new NetChangedEventArgs(IsOnline, NetType));
        }

        public NetworkType NetType { get; private set; } = NetworkType.None;
        public bool IsOnline { get; private set; } = false;
    }
}
