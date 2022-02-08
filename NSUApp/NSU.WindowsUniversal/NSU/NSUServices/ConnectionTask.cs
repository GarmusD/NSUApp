using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Notifications;

namespace NSU.NSUServices
{
    public sealed class ConnectionTask : IBackgroundTask
    {
        private const string socketId = "NSUNetSocket";

        BackgroundTaskDeferral taskDeferral;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            taskDeferral = taskInstance.GetDeferral();
            try
            {
                var details = taskInstance.TriggerDetails as SocketActivityTriggerDetails;
                var socketInformation = details.SocketInformation;
                switch (details.Reason)
                {
                    case SocketActivityTriggerReason.SocketActivity:
                        Debug.WriteLine("SocketActivityTriggerReason.SocketActivity");
                        var socket = socketInformation.StreamSocket;
                        DataReader reader = new DataReader(socket.InputStream);
                        reader.InputStreamOptions = InputStreamOptions.Partial;
                        await reader.LoadAsync(512);
                        byte[] buff = new byte[reader.UnconsumedBufferLength];
                        reader.ReadBytes(buff);//.ReadBuffer(.ReadString(reader.UnconsumedBufferLength);
                        Debug.WriteLine(Encoding.ASCII.GetString(buff));
                        if(Shared.Socket != null)
                        {
                            //Shared.Socket.RaiseDataReceived(buff);
                        }
                        socket.TransferOwnership(socketInformation.Id);
                        break;
                    case SocketActivityTriggerReason.KeepAliveTimerExpired:
                        Debug.WriteLine("SocketActivityTriggerReason.KeepAliveTimerExpired");
                        socket = socketInformation.StreamSocket;
                        DataWriter writer = new DataWriter(socket.OutputStream);
                        writer.WriteBytes(Encoding.UTF8.GetBytes("PING"));
                        await writer.StoreAsync();
                        writer.DetachStream();
                        writer.Dispose();
                        socket.TransferOwnership(socketInformation.Id);
                        break;
                    case SocketActivityTriggerReason.SocketClosed:
                        Debug.WriteLine("SocketActivityTriggerReason.SocketClosed");
                        if(Shared.Socket != null)
                        {
                            //Shared.Socket.RaiseOnDisconnected();
                        }
                        /*
                        socket = new StreamSocket();
                        socket.EnableTransferOwnership(taskInstance.Task.TaskId, SocketActivityConnectedStandbyAction.Wake);
                        string hostname = "89.117.236.213";// (String)ApplicationData.Current.LocalSettings.Values["hostname"];
                        string port = "5152"; // (String)ApplicationData.Current.LocalSettings.Values["port"];
                        await socket.ConnectAsync(new HostName(hostname), port);
                        socket.TransferOwnership(socketId);
                        */
                        break;
                    default:
                        break;
                }
                taskDeferral.Complete();
            }
            catch (Exception exception)
            {
                ShowToast("ConnectionTask.Run() -> "+exception.Message);
                taskDeferral.Complete();
            }
        }

        public void ShowToast(string text)
        {
            var toastNotifier = ToastNotificationManager.CreateToastNotifier();
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            var textNodes = toastXml.GetElementsByTagName("text");
            textNodes.First().AppendChild(toastXml.CreateTextNode(text));
            var toastNotification = new ToastNotification(toastXml);
            toastNotifier.Show(new ToastNotification(toastXml));
        }
    }
}
