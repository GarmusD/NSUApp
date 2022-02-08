using System;
using NSU.Shared.NSUTypes;
using System.Collections.Generic;
using NSU.Shared.NSUSystemPart;
using NSU.NSUSystem;
using Newtonsoft.Json.Linq;

namespace NSUAppShared.NSUSystemParts
{
    public class TempTriggers : NSUSysPartsBase
    {
        public const int MAX_TEMP_TRIGGERS = 16;

        private const string LogTag = "TempTriggers";

        List<TempTrigger> triggers = new List<TempTrigger>();

        public TempTriggers(NSUSys sys, PartsTypes type)
            : base(sys, type)
        {
        }

        public override string[] RegisterTargets()
        {
            return new string[]{ JKeys.TempTrigger.TargetName };
        }

        public override void Clear()
        {
            triggers.Clear();
        }

        public int Count { get { return triggers.Count; } }

        public TempTrigger this[int index]
        {
            get
            {
                if(index >=0 && index < MAX_TEMP_TRIGGERS)
                {
                    if(index < triggers.Count)
                    {
                        return triggers[index];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }

        public TempTrigger FindTrigger(string name)
        {
            for (int i = 0; i < triggers.Count; i++)
            {
                if (triggers[i].Name.Equals(name)) return triggers[i];
            }
            return null;
        }

        public TempTrigger FindTrigger(int cfg_pos)
        {
            if (cfg_pos != -1)
            {
                for (int i = 0; i < triggers.Count; i++)
                {
                    if (triggers[i].ConfigPos == cfg_pos) return triggers[i];
                }
            }
            return null;
        }

        public override void ParseNetworkData(JObject data)
        {
            TempTrigger trg;
            switch ((string)data[JKeys.Generic.Action])
            {
                case JKeys.Syscmd.Snapshot:
                    if (((string)data[JKeys.Generic.Result]).Equals(JKeys.Result.Done))
                    {
                        triggers.Clear();
                        foreach (var item in nsusys.XMLConfig.GetConfigSection(NSU.Shared.NSUXMLConfig.ConfigSection.TempTriggers).Elements())
                        {
                            trg = new TempTrigger();
                            trg.ReadXMLNode(item);
                            triggers.Add(trg);
                        }
                    }
                    break;
                case JKeys.Action.Info:
                    trg = FindTrigger((string)data[JKeys.Generic.Name]);
                    if (trg != null)
                    {
                        trg.Status = NSU.Shared.NSUUtils.Utils.GetStatusFromString((string)data[JKeys.Generic.Status], Status.UNKNOWN);
                    }
                    break;
            }
        }
    }
}

