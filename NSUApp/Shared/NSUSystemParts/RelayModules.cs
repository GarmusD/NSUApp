using System;
using NSU.Shared.NSUTypes;
using NSU.NSUSystem;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using NSU.Shared.NSUSystemPart;

namespace NSUAppShared.NSUSystemParts
{
    public class RelayModules : NSUSysPartsBase
    {
        public const int MAX_RELAY_MODULES = 5;

        private const string LogTag = "RelayModules";

        List<RelayModule> rmodules = new List<RelayModule>();

        public RelayModules(NSUSys sys, PartsTypes type)
            : base(sys, type)
        {
        }

        public override string[] RegisterTargets()
        {
            return new string[] { JKeys.RelayModule.TargetName, "RELAY:" };
        }

        public override void Clear()
        {
            rmodules.Clear();
        }

        public int Count
        {
            get { return rmodules.Count; }
        }

        public RelayModule this[int index]
        {
            get
            {
                if(index >= 0 && index < rmodules.Count)
                {
                    return rmodules[index];
                }
                return null;
            }
        }

        public override void ParseNetworkData(JObject data)
        {
            RelayModule rm;       
            switch ((string)data[JKeys.Generic.Action])
            {
                case JKeys.Syscmd.Snapshot:
                    if (((string)data[JKeys.Generic.Result]).Equals(JKeys.Result.Done))
                    {
                        rmodules.Clear();
                        foreach (var item in nsusys.XMLConfig.GetConfigSection(NSU.Shared.NSUXMLConfig.ConfigSection.RelayModules).Elements())
                        {
                            rm = new RelayModule();
                            rm.ReadXMLNode(item);
                            rmodules.Add(rm);
                        }
                    }
                    break;
                case JKeys.RelayModule.ActionCloseChannel:                    
                    break;
                case JKeys.RelayModule.ActionOpenChannel:
                    break;
            }
        }

    }
}

