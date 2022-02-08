using System;
using System.Collections.Generic;
using NSU.Shared.NSUTypes;
using NSU.Shared.NSUSystemPart;
using NSU.NSUSystem;
using NSU.Shared.NSUNet;
using NSU.Shared;
using Newtonsoft.Json.Linq;

namespace NSUAppShared.NSUSystemParts
{

	public class TempSensors : NSUSysPartsBase
    {
        public const int MAX_TEMP_SENSORS = 64;

        private const string LogTag = "TempSensors";

        List<TempSensor> tsensors = new List<TempSensor>();

        public TempSensors(NSUSys sys, PartsTypes type) : base(sys, type)
        {
        }

        public override string[] RegisterTargets()
        {
            return new string[] { JKeys.TempSensor.TargetName };
        }

        public override void Clear()
        {
            tsensors.Clear();
        }

        public List<string> GetTSensorsNames()
        {
            var snames = new List<string>();
            for (int i = 0; i < tsensors.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(tsensors[i].Name))
                {
                    snames.Add(tsensors[i].Name);
                }
            }
            return snames;
        }

        public TempSensor FindSensor(byte[] sid)
        {
            for (int i = 0; i < tsensors.Count; i++)
            {
                if (tsensors[i].CompareAddr(sid))
                {
                    return tsensors[i];
                }
            }
            return null;
        }

        public TempSensor FindSensor(string tsname)
        {
            for (int i = 0; i < tsensors.Count; i++)
            {
                if (tsensors[i].Name.Equals(tsname))
                {
                    return tsensors[i];
                }
            }
            return null;
        }        

        public TempSensor this[int index]
        {
            get
            {
                if(index >= 0 && index < tsensors.Count)
                {
                    return tsensors[index];
                }
                return null;
            }
        }

        public int Count
        {
            get { return tsensors.Count; }
        }

        public override void ParseNetworkData(JObject data)
        {
            TempSensor ts;
            switch ((string)data[JKeys.Generic.Action])
            {
                case JKeys.Syscmd.Snapshot:
                    if(((string)data[JKeys.Generic.Result]).Equals(JKeys.Result.Done))
                    {
                        tsensors.Clear();                        
                        foreach (var item in nsusys.XMLConfig.GetConfigSection(NSU.Shared.NSUXMLConfig.ConfigSection.TSensors).Elements())
                        {
                            ts = new TempSensor();
                            ts.ReadXMLNode(item);
                            tsensors.Add(ts);
                        }
                    }
                    break;
                case JKeys.Action.Info:
                    ts = FindSensor(TempSensor.StringToAddr((string)data[JKeys.TempSensor.SensorID]));
                    if(ts != null)
                    {
                        ts.Temperature = (float)data[JKeys.Generic.Value];
                    }
                    break;
            }
        }
    }
}

