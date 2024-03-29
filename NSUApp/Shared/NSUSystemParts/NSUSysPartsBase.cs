using NSU.Shared.NSUTypes;
using NSU.NSUSystem;
using NSU.Shared.NSUNet;
using System;
using Newtonsoft.Json.Linq;

namespace NSUAppShared.NSUSystemParts
{
    public enum PartsTypes
    {
        Unknown,
        System,
        TSensors,
        Switches,
        RelayModules,
        TempTriggers,
        CircPumps,
        Collectors,
        ComfortZones,
        KTypes,
        WaterBoilers,
        WoodBoilers,
        Vacation,
        Scenarios,
        FileUploader,
        BinUploader,
        Console
    }

    public abstract class NSUSysPartsBase
    {
        public const int MAX_NAME_LENGTH = 31;

        protected NSUSys nsusys;
		public PartsTypes PartType{ get; set; }

		protected NSUSysPartsBase(NSUSys sys, PartsTypes type)
        {
            nsusys = sys;
            PartType = type;
        }

		public abstract string[] RegisterTargets();

		public abstract void ParseNetworkData(JObject data);

        public abstract void Clear();

        public bool SendCommand(JObject cmd)
        {
            if (nsusys != null)
            {
                if (nsusys.NSUNetwork.Connected())
                {
                    nsusys.NSUNetwork.SendCommand(cmd);
                    return true;
                }
            }
            return false;
        }      
        
    }
}

