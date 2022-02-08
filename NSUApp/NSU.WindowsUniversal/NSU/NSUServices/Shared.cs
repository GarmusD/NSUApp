using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Networking.Sockets;

namespace NSU.NSUServices
{
    public static class Shared
    {
        public static AppServiceConnection ServiceConnection
        {
            get { return serviceConnection; }
            set { serviceConnection = value; }
        }private static AppServiceConnection serviceConnection = null;

        public static StreamSocket Socket
        {
            get { return streamSocket;  }
            set { streamSocket = value; }
        }private static StreamSocket streamSocket = null;

    }
}
