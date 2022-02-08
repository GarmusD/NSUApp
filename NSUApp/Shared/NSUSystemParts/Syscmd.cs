using System;
using System.Collections.Generic;
using System.Text;
using NSU.Shared.NSUTypes;
using NSU.NSUSystem;
using Newtonsoft.Json.Linq;
using NSU.Shared;

namespace NSUAppShared.NSUSystemParts
{
    public class Syscmd : NSUSysPartsBase
    {
        readonly string LogTag = "SystemCMD";

        public Syscmd(NSUSys sys, PartsTypes part):base(sys, part)
        {
        }        

        public override string[] RegisterTargets()
        {
            return new string[] {JKeys.Syscmd.TargetName };
        }

        public override void Clear()
        {
            
        }

        public override void ParseNetworkData(JObject data)
        {
            switch((string)data[JKeys.Generic.Action])
            {
                case JKeys.SystemAction.Handshake:
                    NSULog.Debug(LogTag, "Handshake response received.");
                    //scheck name, version, protocol...
                    nsusys.NSUNetwork.RaiseOnHandshakeReceived();
                    break;
                case JKeys.SystemAction.Login:
                    if(data.Property(JKeys.Generic.Result) != null)
                    {
                        if(((string)data[JKeys.Generic.Result]).Equals(JKeys.Result.Ok))
                        {
                            //Save Device id and hash
                            Credentials.Current.DeviceID = (string)data[JKeys.ActionLogin.DeviceID];
                            Credentials.Current.Hash = (string)data[JKeys.ActionLogin.Hash];
                            nsusys.NSUNetwork.RaiseOnLoggedInEvent();
                            RequestSnapshot();
                        }
                        else if (((string)data[JKeys.Generic.Result]).Equals(JKeys.Result.Error))
                        {
                            //Failed
                            Credentials.Current.DeviceID = "";
                            Credentials.Current.Hash = "";
                            nsusys.NSUNetwork.RaiseOnLoginFailedEvent((string)data[JKeys.Generic.ErrCode]);
                        }
                    }
                    break;
                case JKeys.Syscmd.Snapshot:
                    if(((string)data[JKeys.Generic.Result]).Equals(JKeys.Result.Ok))
                    {
                        if(nsusys.XMLConfig.Load((string)data[JKeys.Generic.Value]))
                        {
                            JObject jo = new JObject();
                            jo[JKeys.Generic.Action] = JKeys.Syscmd.Snapshot;
                            jo[JKeys.Generic.Result] = JKeys.Result.Done;

                            foreach(var item in nsusys.Parts)
                            {
                                try
                                {
                                    item.Part.ParseNetworkData(jo);
                                }
                                catch(Exception ex)
                                {
                                    NSULog.Exception(LogTag, $"Snapshot DONE. Item '{item.PartType}' exception: "+ex);
                                }
                            }
                            nsusys.MakeReady();
                        }
                        else
                        {
                            //show some error
                        }
                    }
                    break;
                case JKeys.Action.Status:
                    if((string)data[JKeys.Generic.Value] == JKeys.SystemAction.Ready)
                    {
                        nsusys.ReInit();
                    }
                    break;
            }
        }

        public void RequestSnapshot()
        {
            //Request snapshot
            JObject jo = new JObject();
            jo[JKeys.Generic.Target] = JKeys.Syscmd.TargetName;
            jo[JKeys.Generic.Action] = JKeys.Syscmd.Snapshot;
            jo = nsusys.NSUNetwork.RequestAck(jo, "snapshot");
            SendCommand(jo);
        }
    }
}
