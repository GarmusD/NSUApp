using System;
using System.Collections.Generic;
using NSU.Shared.NSUSystemPart;
using NSU.Shared.NSUTypes;
using NSU.NSUSystem;
using NSU.Shared;
using Newtonsoft.Json.Linq;

namespace NSUAppShared.NSUSystemParts
{
    public class CircPumps : NSUSysPartsBase
	{
        public const int MAX_CIRC_PUMPS = 8;

        readonly string LogTag = "CircPumps";

        List<CircPump> circpumps = new List<CircPump>();

		public CircPumps(NSUSys sys, PartsTypes type)
            : base(sys, type)
        {
        }

		public override string[] RegisterTargets()
		{
			return new string[] { JKeys.CircPump.TargetName };
		}

        public override void Clear()
        {
            circpumps.Clear();
        }

        public int Count { get { return circpumps.Count; } }

        public CircPump this[int index]
        {
            get
            {
                if(index <0 || index >= MAX_CIRC_PUMPS)
                {
                    throw new IndexOutOfRangeException();
                }
                if(index < circpumps.Count)
                {
                    return circpumps[index];
                }
                return null;
            }
        }

		public CircPump FindCircPump(string name)
        {
            for (int i = 0; i < circpumps.Count; i++)
            {
                CircPump cpc = circpumps[i];
                if(cpc.Name.Equals(name))
                {
                    return cpc;
                }
            }
            return null;
        }

		public CircPump FindCircPump(int cfg_pos)
		{
			for (int i = 0; i < circpumps.Count; i++)
			{
				CircPump cpc = circpumps[i];
				if (cpc.ConfigPos == cfg_pos)
				{
					return cpc;
				}
			}
			return null;
		}

        public override void ParseNetworkData(JObject data)
        {
            try
            {
                CircPump cp;
                switch ((string)data[JKeys.Generic.Action])
                {
                    case JKeys.Syscmd.Snapshot:
                        if (((string)data[JKeys.Generic.Result]).Equals(JKeys.Result.Done))
                        {
                            circpumps.Clear();
                            foreach (var item in nsusys.XMLConfig.GetConfigSection(NSU.Shared.NSUXMLConfig.ConfigSection.CirculationPumps).Elements())
                            {
                                cp = new CircPump();
                                cp.ReadXMLNode(item);
                                cp.OnClicked += CircPumpOnClickedHandler;
                                circpumps.Add(cp);
                            }
                        }
                        break;
                    case JKeys.Action.Info:
                        cp = FindCircPump((string)data[JKeys.Generic.Name]);
                        if (cp != null)
                        {
                            cp.CurrentSpeed = (byte)data[JKeys.CircPump.CurrentSpeed];
                            cp.OpenedValvesCount = (byte)data[JKeys.CircPump.ValvesOpened];
                            cp.Status = NSU.Shared.NSUUtils.Utils.GetStatusFromString((string)data[JKeys.Generic.Status], Status.UNKNOWN);
                        }
                        break;
                }
            }
            catch(Exception ex)
            {
                var jo = new JObject();
                jo[JKeys.Generic.Target] = JKeys.CircPump.TargetName;
                jo[JKeys.Generic.Result] = JKeys.Result.Error;
                jo[JKeys.Generic.Message] = ex.Message;
                SendCommand(jo);
            }
        }

        private void CircPumpOnClickedHandler(object sender, EventArgs e)
        {
            NSULog.Debug(LogTag, $"CircPumpOnClickedHandler({(sender as CircPump)?.Name})");
            JObject jo = new JObject();
            jo[JKeys.Generic.Target] = JKeys.CircPump.TargetName;
            jo[JKeys.Generic.Action] = JKeys.Action.Click;
            jo[JKeys.Generic.Name] = (sender as CircPump).Name;
            SendCommand(jo);
        }
    }
}

