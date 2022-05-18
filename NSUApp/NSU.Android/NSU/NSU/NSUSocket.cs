using System;
using System.Net;
using System.Net.Sockets;
using NSU.Shared.NSUNet;
using System.Threading;
using System.Threading.Tasks;
using NSU.Shared;

namespace NSU.Droid
{
    public class NSUSocket : INSUNetworkSocket
    {
        readonly string LogTag = "NSUSocket";

        TcpClient client;
        readonly AutoResetEvent are = new AutoResetEvent(false);
        volatile bool stop_request;
        NetworkStream netStream = null;
        volatile int net_read_count;
        Thread worker;

        bool silentDC;
        private readonly object lockobj = new object();
        private readonly object dclock = new object();
        volatile bool connected, connecting;
        private bool disconnecting = false;

        #region INSUNetworkSocket implementation

        public event NSUSocketEmptyArgs OnConnectStart;
        public event NSUSocketEmptyArgs OnConnected;
        public event NSUSocketEmptyArgs OnDisconnected;
        public event NSUSocketEmptyArgs OnConnectTimeout;
        public event NSUSocketEmptyArgs OnConnectFailed;
        public event NSUSocketDataReceivedArgs OnDataReceived;
        public event NSUSocketEmptyArgs OnInvalidHost;

        public Task ConnectAsync(string ipOrHost, int port)
        {            
            return Task.Run(() =>
            {
                lock (lockobj)
                {
                    if (connected)
                        return;

                    NSULog.Debug(LogTag, "Connect(). Not connected. Continuing....");
                    connecting = true;
                    connected = false;

                    IPAddress ip;

                    try
                    {
                        var addrs = Dns.GetHostAddressesAsync(ipOrHost).Result;
                        NSULog.Debug(LogTag, $"Resolved IP addresses:");
                        if (addrs != null)
                        {
                            for (int i = 0; i < addrs.Length; i++)
                            {
                                NSULog.Debug(LogTag, $"    {i + 1}: {addrs[i]}");
                            }
                        }
                        else
                        {
                            NSULog.Debug(LogTag, $"GetHostAddressesAsync({ipOrHost}) result is NULL.");
                        }

                        if (addrs != null && addrs.Length > 0)
                        {
                            ip = IPAddress.Parse(addrs[0].ToString());
                        }
                        else
                        {
                            OnInvalidHost?.Invoke();
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        connecting = false;
                        NSULog.Exception(LogTag, "HostToIP exception: " + e.Message);                        
                        OnInvalidHost?.Invoke();
                        return;
                    }

                    NSULog.Debug(LogTag, "Connecting started...");
                    OnConnectStart?.Invoke();
                    try
                    {
                        client = new TcpClient();
                        IAsyncResult ar = client.BeginConnect(ip, port, null, null);
                        if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(15), false))
                        {
                            connecting = false;
                            client.Close();
                            client = null;
                            OnConnectTimeout?.Invoke();
                            return;
                        }
                        client.EndConnect(ar);
                        connecting = false;
                        connected = true;

                        NSULog.Debug(LogTag, $"Connected={client.Connected}. Starting listener thread.");
                        netStream = client.GetStream();
                        runner();

                        if (connected && !disconnecting)
                        {
                            NSULog.Debug(LogTag, "Invoking OnConnected event.");
                            OnConnected?.Invoke();
                        }
                        else
                        {
                            NSULog.Debug(LogTag, $"OnConnected event NOT Invoked. connected={connected} disconnecting={disconnecting}");
                        }

                    }
                    catch (SocketException)
                    {
                        connecting = false;
                        OnConnectFailed?.Invoke();
                        return;
                    }
                    catch (Exception ex)
                    {
                        connecting = false;
                        NSULog.Exception(LogTag, "Connect(): " + ex);
                        NSULog.Exception(LogTag, "Connect(): Calling DoDisconnect()");
                        DoDisconnectAsync();
                        return;
                    }
                }
            });
        }


        bool Connected()
        {
            return connected && !disconnecting;// (client != null && client.Connected);
        }

        public async Task Disconnect(bool silent=true)
        {
            NSULog.Debug(LogTag, $"Disconnect(silent={silent})");
            silentDC = silent;
            await DoDisconnectAsync();
        }

        Task DoDisconnectAsync()
        {
            return Task.Run(() =>
            {
                lock (dclock)
                {
                    if (connected && !disconnecting)
                    {
                        NSULog.Debug(LogTag, "DoDisconnect()");
                        disconnecting = true;
                        try
                        {
                            stop_request = true;
                            if (worker != null && worker.IsAlive)
                            {
                                net_read_count = -1;
                                are.Set();
                                worker.Join(500);
                            }

                            if (netStream != null)
                            {
                                netStream.Dispose();
                                netStream = null;
                            }

                            if (client != null)
                            {
                                client.Close();
                                client = null;
                            }                            
                        }
                        finally
                        {
                            connected = false;
                            disconnecting = false;
                            worker = null;
                            netStream = null;
                            client = null;
                            if (!silentDC)
                            {
                                silentDC = true;
                                OnDisconnected?.Invoke();
                            }
                        }
                    }
                }
            });
        }

        public bool IsConnected()
        {
            return Connected();
        }

        public bool IsConnecting()
        {
            return connecting;
        }

        public async Task SendData(byte[] buf, int offset = 0, int cnt = 0)
        {
            try
            {
                if (netStream != null)
                {
                    cnt = cnt == 0 ? buf.Length : cnt;
                    await netStream.WriteAsync(buf, offset, cnt);
                }
            }
            catch (Exception ex)
            {
                NSULog.Exception(LogTag, "SendData(): " + ex.Message + ". ReThrowing.");
                throw;
            }
        }

        #endregion

        void runner()
        {
            const string LogTag = "ClientRunner";
            stop_request = false;
            byte[] buffer = new byte[client.ReceiveBufferSize];
            are.Reset();

            worker = new Thread(new ThreadStart(async () =>
            {
                while (!stop_request)
                {
                    try
                    {
                        if (netStream != null)
                        {
                            var asres = netStream.BeginRead(buffer, 0, buffer.Length, TCPAsyncReader, netStream);
                            are.WaitOne();
                            if (net_read_count > 0)
                            {
                                NSULog.Debug(LogTag, $"Data received. Count={net_read_count}.");
                                OnDataReceived?.Invoke(buffer, net_read_count);
                            }
                            else if (net_read_count <= 0)
                            {
                                NSULog.Error(LogTag, $"net_read_count={net_read_count}.");
                                stop_request = true;
                            }
                            else
                            {
                                NSULog.Debug(LogTag, $"net_read_count={net_read_count}. WTF? (Continuing work...)");
                            }
                        }
                        else
                        {
                            NSULog.Debug(LogTag, $"TCPClient is null={client == null} or not connected={client?.Connected}. Quiting thread.");
                            break;
                        }
                    }
                    catch (SocketException e)
                    {
                        NSULog.Exception(LogTag, "SocketException: " + e.Message);
                        break;
                    }
                    catch (Exception ex)
                    {
                        NSULog.Exception(LogTag, "Exception: " + ex.Message);
                        break;
                    }
                }
                NSULog.Debug(LogTag, "Quiting TCPClient runner");
                await DoDisconnectAsync();
            }));
            worker.Start();
        }

        internal async void TCPAsyncReader(IAsyncResult ar)
        {
            net_read_count = -1;
            if (netStream != null)
            {
                try
                {
                    net_read_count = netStream.EndRead(ar);
                }
                catch (Exception ex)
                {
                    //if (ex is ObjectDisposedException)
                    //    return;

                    NSULog.Exception(LogTag, "TCPAsyncReader(): " + ex.Message);
                    net_read_count = -1;
                    if (!stop_request)
                    {
                        await DoDisconnectAsync();
                        return;
                    }
                }
            }
            are.Set();
        }
    }
}

