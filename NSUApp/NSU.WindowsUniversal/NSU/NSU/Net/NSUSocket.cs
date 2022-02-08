using NSU.NSUSystem;
using NSU.Shared;
using NSU.Shared.NSUNet;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace NSU.NSU_UWP.Net
{
    public class NSUSocket : INSUNetworkSocket
    {
        readonly string LogTag = "NSUSocket";
        private const int SocketReadLength = 512;

        public event NSUSocketEmptyArgs OnConnectStart;
        public event NSUSocketEmptyArgs OnConnected;
        public event NSUSocketEmptyArgs OnConnectFailed;
        public event NSUSocketEmptyArgs OnConnectTimeout;
        public event NSUSocketDataReceivedArgs OnDataReceived;
        public event NSUSocketEmptyArgs OnDisconnected;
        public event NSUSocketEmptyArgs OnInvalidHost;

        private StreamSocket socket = null;
        private DataWriter writer = null;

        private bool stopRequested = true;
        private bool silentDC = false;
        private volatile bool disconnected = true;
        private bool connecting = false;
        object lck = new object();

        public NSUSocket()
        {
        }

        public async Task ConnectAsync(string addr, int port)
        {
            await DoDisconnect();
            connecting = true;
            OnConnectStart?.Invoke();
            socket = new StreamSocket();
            socket.Control.KeepAlive = true;
#if DEBUG
            try
            {
                await socket.ConnectAsync(new HostName(addr), port.ToString());
                disconnected = false;
            }
            catch (Exception exception)
            {
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.HostNotFound)
                {
                    OnInvalidHost?.Invoke();
                    return;
                }
                OnConnectFailed?.Invoke();
                return;
            }
            finally
            {
                connecting = false;
            }
#else
            try
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(15000); // cancel after 15 seconds

                var connectAsync = socket.ConnectAsync(new HostName(addr), port.ToString());
                var connectTask = connectAsync.AsTask(cts.Token);
                await connectTask;
            }
            catch(TimeoutException)
            {
                OnConnectTimeout?.Invoke();
                return;
            }
            catch (Exception exception)
            {
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }
            if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.HostNotFound)
                {
                    OnInvalidHost?.Invoke();
                    return;
                }
                OnConnectFailed?.Invoke();
                return;
            }
            finally
            {
                connecting = false;
            }

#endif      
            writer = new DataWriter(socket.OutputStream);
            stopRequested = false;
            OnConnected?.Invoke();
            DoReadSocket();

        }

        private Task DoDisconnect()
        {
            return Task.Run(() =>
            {
                if (!disconnected)
                {
                    disconnected = true;
                    stopRequested = true;
                    NSULog.Debug(LogTag, "DoDisconnect() is called.");
                    try
                    {
                        if (writer != null)
                        {
                            writer.DetachStream();
                            writer.Dispose();
                            writer = null;
                        }
                        if (socket != null)
                        {
                            //await socket.CancelIOAsync();
                            socket.Dispose();
                            socket = null;
                        }
                    }
                    catch (Exception e)
                    {
                        NSULog.Debug(LogTag, "DoDisconnect() exception: " + e.Message);
                    }
                    NSULog.Debug(LogTag, "DoDisconnect() is finished.");
                    if (!silentDC)
                        OnDisconnected?.Invoke();
                }
            });
        }

        public async Task Disconnect(bool silent=true)
        {
            silentDC = silent;
            await DoDisconnect();
        }

        public bool IsConnected()
        {
            return socket != null;
        }

        public bool IsConnecting()
        {
            return connecting;
        }

        public async Task SendData(byte[] buf, int offset = 0, int cnt = 0)
        {
            if (writer != null)
            {
                writer.WriteBytes(buf);
                var x = await writer.StoreAsync();
            }
        }

        private async void DoReadSocket()
        {
            if (socket != null)
            {
                using (var reader = new DataReader(socket.InputStream))
                {
                    reader.InputStreamOptions = InputStreamOptions.Partial;
                    try
                    {
                        while (!stopRequested)
                        {
                            var bytesReaded = await reader.LoadAsync(SocketReadLength);
                            if (bytesReaded == 0)
                            {
                                //Disconnected
                                await DoDisconnect();
                                return;
                            }
                            byte[] buf = new byte[reader.UnconsumedBufferLength];
                            reader.ReadBytes(buf);
                            if (reader.UnconsumedBufferLength > 0)
                            {
                                throw new Exception();
                            }
                            OnDataReceived?.Invoke(buf, buf.Length);
                        }
                        reader.DetachStream();
                    }
                    catch (Exception ex)
                    {
                        NSULog.Exception(LogTag, "DoReadSocket() - " + ex.Message);
                        reader.DetachStream();
                        await DoDisconnect();
                    }
                }
            }
        }

    }
}
