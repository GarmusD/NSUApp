using System;
using NSU.Shared.NSUTypes;
using System.Collections.Generic;
using NSU.Shared.NSUSystemPart;
using NSU.NSUSystem;
using Newtonsoft.Json.Linq;

namespace NSUAppShared.NSUSystemParts
{
    public class Collectors : NSUSysPartsBase
    {
        public const int MAX_COLLECTORS = 8;

        private const string LogTag = "Collectors";
        private List<Collector> collectors = new List<Collector>();

        public Collectors(NSUSys sys, PartsTypes type)
            : base(sys, type)
        {
        }

        public override string[] RegisterTargets()
        {
            return new string[] { JKeys.Collector.TargetName, "COLLECTOR:"};
        }

        public override void Clear()
        {
            collectors.Clear();
        }

        public int Count { get { return collectors.Count; } }

        public Collector this[int index]
        {
            get
            {
                if(index < 0 || index >= MAX_COLLECTORS)
                {
                    throw new IndexOutOfRangeException();
                }
                if (index < collectors.Count)
                    return collectors[index];
                return null;
            }
        }

        public Collector FindCollector(string name)
        {
            for (int i = 0; i < collectors.Count; i++)
            {
                if (collectors[i].Name.Equals(name))
                    return collectors[i];
            }
            return null;
        }

        public Collector FindCollector(byte cfg_pos)
        {
            for (int i = 0; i < collectors.Count; i++)
            {
                if (collectors[i].ConfigPos == cfg_pos)
                    return collectors[i];
            }
            return null;
        }

        public override void ParseNetworkData(JObject data)
        {
            Collector col;
            switch ((string)data[JKeys.Generic.Action])
            {
                case JKeys.Syscmd.Snapshot:
                    if (((string)data[JKeys.Generic.Result]).Equals(JKeys.Result.Done))
                    {
                        collectors.Clear();
                        foreach (var item in nsusys.XMLConfig.GetConfigSection(NSU.Shared.NSUXMLConfig.ConfigSection.Collectors).Elements())
                        {
                            col = new Collector();
                            col.ReadXMLNode(item);
                            collectors.Add(col);
                        }
                    }
                    break;
                case JKeys.Action.Info:
                    col = FindCollector((string)data[JKeys.Generic.Name]);
                    if (col != null)
                    {
                        JArray ja = (JArray)data[JKeys.Generic.Status];
                        for(int i=0; i < col.ActuatorsCount; i++)
                        {
                            col.Actuators[i].Opened = Convert.ToBoolean((string)ja[i]);
                        }
                    }
                    break;
            }
        }
    }
}

