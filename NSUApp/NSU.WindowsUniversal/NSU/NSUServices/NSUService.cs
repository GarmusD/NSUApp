using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace NSU.NSUServices
{
    public sealed class NSUService : IBackgroundTask
    {
        private readonly string socketTaskName = "NSUSocketActivityBackgroundTask";
        private const string socketId = "NSUSocket";
        private StreamSocket socket = null;
        private IBackgroundTaskRegistration task = null;

        BackgroundTaskDeferral serviceDeferral;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("NSUService.Run()");
            serviceDeferral = taskInstance.GetDeferral();

            taskInstance.Canceled += OnTaskCanceled;  

            var details = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            Shared.ServiceConnection = details.AppServiceConnection;
            Shared.ServiceConnection.RequestReceived += OnRequestReceived;
            PrepareToStart();
        }

        private void PrepareToStart()
        {
            RegisterBackgrounds();
        }

        private void RegisterBackgrounds()
        {
            try
            {
                foreach (var current in BackgroundTaskRegistration.AllTasks)
                {
                    if (current.Value.Name == "NSUSocketActivityBackgroundTask")
                    {
                        task = current.Value;
                        break;
                    }
                }

                // If there is no task allready created, create a new one
                if (task == null)
                {
                    var socketTaskBuilder = new BackgroundTaskBuilder();
                    socketTaskBuilder.Name = socketTaskName;
                    socketTaskBuilder.TaskEntryPoint = "NSUServices.ConnectionTask";
                    var trigger = new SocketActivityTrigger();
                    socketTaskBuilder.SetTrigger(trigger);
                    task = socketTaskBuilder.Register();
                }

            }
            catch (Exception exception)
            {
                //Log exception
                Debug.WriteLine("MainPage.RegisterBackgrounds() -> " + exception.Message);
            }
        }

        private async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var def = args.GetDeferral();
            Debug.WriteLine("Service.OnRequestReceived()");
            try
            {
                ValueSet result = await ProccessMessages(args.Request.Message);
                if (result != null)
                {
                    //Send the response
                    await args.Request.SendResponseAsync(result);
                }
            }
            finally
            {
                def.Complete();
            }
        }

        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            Debug.WriteLine("Service.OnTaskCanceled");
            Shared.ServiceConnection = null;
            if (serviceDeferral != null)
            {
                //Complete the service deferral
                serviceDeferral.Complete();
                serviceDeferral = null;
            }
        }

        private async void Connect()
        {
            //ApplicationData.Current.LocalSettings.Values["hostname"] = TargetServerTextBox.Text;
            //ApplicationData.Current.LocalSettings.Values["port"] = port;

            try
            {
                SocketActivityInformation socketInformation;
                if (!SocketActivityInformation.AllSockets.TryGetValue(socketId, out socketInformation))
                {
                    socket = new StreamSocket();
                    socket.EnableTransferOwnership(task.TaskId, SocketActivityConnectedStandbyAction.Wake);
                    var targetServer = new HostName("89.117.236.213");
                    await socket.ConnectAsync(targetServer, "5152");
                    socket.TransferOwnership(socketId);
                    socket = null;
                }
                Debug.WriteLine("Connected. You may close the application");
            }
            catch (Exception exception)
            {
                Debug.WriteLine("NSUService.Connect() -> "+exception.Message);
            }
        }



        private async Task<ValueSet> ProccessMessages(ValueSet msg)
        {
            if(msg.ContainsKey("action"))
            {
                switch((string)msg["action"])
                {
                    case "connect":
                        return await msgConnect(msg);
                    case "login":
                        try
                        {

                            if(task == null)
                            {

                            }
                            else
                            {
                                //We are connected
                                ValueSet ret = new ValueSet();
                                ret.Add("result", "fail");
                                ret.Add("text", "");
                                return ret;
                            }
                        }
                        catch(Exception e)
                        {
                            ValueSet ret = new ValueSet();
                            ret.Add("result", "fail");
                            ret.Add("code", -1);
                            ret.Add("text", e.Message);
                            return ret;
                        }
                        break;
                }
            }
            return null;
        }

        private async Task<ValueSet> msgConnect(ValueSet msg)
        {
            try
            {
                string host = (string)msg["host"];
                string port = (string)msg["port"];
                bool save = (bool)msg["save"];

                try
                {
                    SocketActivityInformation socketInformation;
                    if (SocketActivityInformation.AllSockets.TryGetValue(socketId, out socketInformation))
                    {
                        //disconnect first
                        socket = socketInformation.StreamSocket;
                        await socket.CancelIOAsync();
                        socket.Dispose();
                        socket = null;
                    }
                    socket = new StreamSocket();
                    socket.EnableTransferOwnership(task.TaskId, SocketActivityConnectedStandbyAction.Wake);
                    var targetServer = new HostName(host);//"89.117.236.213"
                    await socket.ConnectAsync(targetServer, port);//"5152"
                    socket.TransferOwnership(socketId);
                    socket = null;
                    Debug.WriteLine("Connected.");
                    ValueSet ret = new ValueSet();
                    ret.Add("result", "ok");
                    return ret;
                }
                catch (Exception exception)
                {
                    Debug.WriteLine("NSUService.Connect() -> " + exception.Message);
                    ValueSet ret = new ValueSet();
                    ret.Add("result", "fail");
                    ret.Add("code", -1);
                    ret.Add("text", exception.Message);
                    return ret;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine("NSUService.Connect() -> " + exception.Message);
                ValueSet ret = new ValueSet();
                ret.Add("result", "fail");
                ret.Add("code", 1);
                ret.Add("text", exception.Message);
                return ret;
            }
        }

        private void msgLogin(ValueSet msg)
        {
            return;
        }
    }
}
