using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using NSU.NSUSystem;
using NSU.Shared;
using NSU.Shared.NSUTypes;

namespace NSUAppShared.NSUSystemParts
{
    public class ConsoleOutEventArgs : EventArgs
    {
        public string Message { get; }
        public ConsoleOutEventArgs(string value)
        {
            Message = value;
        }
    }

    public class Console : NSUSysPartsBase
    {
        private const string LogTag = "Console";

        public event EventHandler<ConsoleOutEventArgs> ConsoleOutput;

        public Console(NSUSys sys, PartsTypes partsType):base(sys, partsType)
        {

        }

        public override string[] RegisterTargets()
        {
            return new string[] { JKeys.Console.TargetName };
        }

        public override void Clear()
        {
            
        }

        public void Start()
        {
            NSULog.Debug(LogTag, "Console output requested.");
            JObject jo = new JObject
            {
                [JKeys.Generic.Target] = JKeys.Console.TargetName,
                [JKeys.Generic.Action] = JKeys.Console.Start
            };
            SendCommand(jo);
        }

        public void Stop()
        {
            JObject jo = new JObject
            {
                [JKeys.Generic.Target] = JKeys.Console.TargetName,
                [JKeys.Generic.Action] = JKeys.Console.Stop
            };
            SendCommand(jo);
        }

        public void Send(string command)
        {
            JObject jo = new JObject
            {
                [JKeys.Generic.Target] = JKeys.Console.TargetName,
                [JKeys.Generic.Action] = JKeys.Console.ManualCommand,
                [JKeys.Generic.Value] = command
            };
            SendCommand(jo);
        }

        public override void ParseNetworkData(JObject data)
        {
            switch((string)data[JKeys.Generic.Action])
            {
                case JKeys.Console.ConsoleOut:
                    ConsoleOutput?.Invoke(this, new ConsoleOutEventArgs((string)data[JKeys.Generic.Value]));
                    break;
            }
        }

        
    }
}
