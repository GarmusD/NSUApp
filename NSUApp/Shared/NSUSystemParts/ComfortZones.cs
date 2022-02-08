using System;
using NSU.Shared.NSUTypes;
using System.Collections.Generic;
using NSU.Shared.NSUSystemPart;
using NSU.NSUSystem;
using Newtonsoft.Json.Linq;

namespace NSUAppShared.NSUSystemParts
{ 
    public class ComfortZones : NSUSysPartsBase
    {
        public const int MAX_COMFORT_ZONES = 32;
        private const string LogTag = "ComfortZones";

        List<ComfortZone> czones = new List<ComfortZone>();

        public ComfortZones(NSUSys sys, PartsTypes type)
            : base(sys, type)
        {
        }

        public override string[] RegisterTargets()
        {
            return new string[] { JKeys.ComfortZone.TargetName };
        }

        public override void Clear()
        {
            czones.Clear();
        }

        public int Count { get { return czones.Count; } }

        public ComfortZone this[int index]
        {
            get
            {
                if(index >=0 && index < czones.Count)
                {
                    return czones[index];
                }
                return null;
            }
        }

        public ComfortZone FindComfortZone(int idx)
        {
            for (int i = 0; i < czones.Count; i++)
            {
                if (czones[i].ConfigPos == idx) return czones[i];
            }
            return null;
        }

        public ComfortZone FindComfortZone(string name)
        {
            for (int i = 0; i < czones.Count; i++)
            {
                if (czones[i].Name.Equals(name)) return czones[i];
            }
            return null;
        }

        public override void ParseNetworkData(JObject data)
        {
            ComfortZone cz;
            switch ((string)data[JKeys.Generic.Action])
            {
                case JKeys.Syscmd.Snapshot:
                    if (((string)data[JKeys.Generic.Result]).Equals(JKeys.Result.Done))
                    {
                        czones.Clear();
                        foreach (var item in nsusys.XMLConfig.GetConfigSection(NSU.Shared.NSUXMLConfig.ConfigSection.ComfortZones).Elements())
                        {
                            cz = new ComfortZone();
                            cz.ReadXMLNode(item);
                            czones.Add(cz);
                        }
                    }
                    break;
                case JKeys.Action.Info:
                    cz = FindComfortZone((string)data[JKeys.Generic.Name]);
                    if (cz != null)
                    {
                        switch ((string)data[JKeys.Generic.Content])
                        {
                            case JKeys.ComfortZone.CurrentRoomTemp:
                                cz.CurrentRoomTemperature = (float)data[JKeys.Generic.Value];
                                break;
                            case JKeys.ComfortZone.CurrentFloorTemp:
                                cz.CurrentFloorTemperature = (float)data[JKeys.Generic.Value];
                                break;
                            case JKeys.ComfortZone.ActuatorOpened:
                                cz.ActuatorOpened = (bool)data[JKeys.Generic.Value];
                                break;
                            default:
                                break;
                        }
                    }
                    break;
            }
        }

    }
}
