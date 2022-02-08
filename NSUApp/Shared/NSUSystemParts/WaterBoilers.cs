using System;
using NSU.Shared.NSUTypes;
using NSU.NSUSystem;
using NSU.Shared;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using NSU.Shared.NSUSystemPart;

namespace NSUAppShared.NSUSystemParts
{
    public class WaterBoilers : NSUSysPartsBase
    {
        public const int MAX_WATER_BOILERS = 1;

        List<WaterBoiler> boilers = new List<WaterBoiler>();

        public WaterBoilers(NSUSys sys, PartsTypes type)
            : base(sys, type)
        {
        }

        public override string[] RegisterTargets()
        {
            return new string[] { JKeys.WaterBoiler.TargetName };
        }

        public override void Clear()
        {
            boilers.Clear();
        }

        public int Count => boilers.Count;

        public WaterBoiler this[int index] => boilers[index];

        public override void ParseNetworkData(JObject data)
        {
            WaterBoiler wb;
            switch ((string)data[JKeys.Generic.Action])
            {
                case JKeys.Syscmd.Snapshot:
                    if (((string)data[JKeys.Generic.Result]).Equals(JKeys.Result.Done))
                    {
                        boilers.Clear();
                        foreach (var item in nsusys.XMLConfig.GetConfigSection(NSU.Shared.NSUXMLConfig.ConfigSection.WaterBoilers).Elements())
                        {
                            wb = new WaterBoiler();
                            wb.ReadXMLNode(item);
                            boilers.Add(wb);
                        }
                    }
                    break;
            }
        }
    }
}
