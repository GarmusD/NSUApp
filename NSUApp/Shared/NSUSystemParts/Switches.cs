using System;
using System.Collections.Generic;
using NSU.Shared.NSUSystemPart;
using NSU.Shared.NSUTypes;
using NSU.NSUSystem;
using NSU.Shared.NSUNet;
using NSU.Shared;
using Newtonsoft.Json.Linq;

namespace NSUAppShared.NSUSystemParts
{
    public class Switches : NSUSysPartsBase
    {
        public const int MAX_SWITCHES = 16;

        private const string LogTag = "Switches";

        List<Switch> switches = new List<Switch>();

		public Switches(NSUSys sys, PartsTypes type)
            : base(sys, type)
        {
        }

        public override string[] RegisterTargets()
        {
            return new string[] { JKeys.Switch.TargetName };
        }

        public override void Clear()
        {
            switches.Clear();
        }

        public Switch FindSwitch(string name)
        {
            for (int i = 0; i < switches.Count; i++)
            {
                var swth = (Switch)switches[i];
                if (swth.Name.Equals(name))
                    return swth;
            }
            return null;
        }

        public Switch FindSwitch(byte cfg_pos)
        {
            for (int i = 0; i < switches.Count; i++)
            {
                var swth = (Switch)switches[i];
                if (swth.ConfigPos == cfg_pos)
                    return swth;
            }
            return null;
        }

        public override void ParseNetworkData(JObject data)
        {
            Switch sw;
            switch ((string)data[JKeys.Generic.Action])
            {
                case JKeys.Syscmd.Snapshot:
                    if (((string)data[JKeys.Generic.Result]).Equals(JKeys.Result.Done))
                    {
                        switches.Clear();
                        foreach (var item in nsusys.XMLConfig.GetConfigSection(NSU.Shared.NSUXMLConfig.ConfigSection.Switches).Elements())
                        {
                            sw = new Switch();
                            sw.ReadXMLNode(item);
                            sw.OnClicked += Switch_OnClicked;
                            switches.Add(sw);
                        }
                    }
                    break;
                case JKeys.Action.Info:
                    sw = FindSwitch((string)data[JKeys.Generic.Name]);
                    if (sw != null)
                    {
                        sw.IsForced = Convert.ToBoolean((string)data[JKeys.Switch.IsForced]);
                        sw.Status = NSU.Shared.NSUUtils.Utils.GetStatusFromString((string)data[JKeys.Generic.Status], Status.UNKNOWN);
                    }
                    break;
            }
        }

        private void Switch_OnClicked(object sender, EventArgs e)
        {
            JObject jo = new JObject
            {
                [JKeys.Generic.Target] = JKeys.Switch.TargetName,
                [JKeys.Generic.Action] = JKeys.Action.Click,
                [JKeys.Generic.Name] = (sender as Switch).Name
            };
            SendCommand(jo);
        }
    }
}

