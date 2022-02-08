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

    public class WoodBoilers : NSUSysPartsBase
    {
        public const int MAX_WOOD_BOILERS = 1;

        private const string LogTag = "WoodBoilers";

        readonly List<WoodBoiler> boilers;

        public WoodBoilers(NSUSys sys, PartsTypes type)
            : base(sys, type)
        {
            boilers = new List<WoodBoiler>();
        }

        public override string[] RegisterTargets()
        {
            return new string[] { JKeys.WoodBoiler.TargetName };
        }

        public override void Clear()
        {
            boilers.Clear();
        }

        public int Count => boilers.Count;

        public WoodBoiler this[int index] => boilers[index];

        public WoodBoiler FindWoodBoiler(string name)
        {
            foreach (var item in boilers)
            {
                if (item.Name.Equals(name))
                {
                    return item;
                }
            }
            return null;
        }

        public WoodBoiler FindWoodBoiler(byte cfg_pos)
        {
            foreach (var item in boilers)
            {
                if (item.ConfigPos == cfg_pos)
                {
                    return item;
                }
            }
            return null;
        }


        public override void ParseNetworkData(JObject data)
        {
            WoodBoiler wb;
            switch ((string)data[JKeys.Generic.Action])
            {
                case JKeys.Syscmd.Snapshot:
                    if (((string)data[JKeys.Generic.Result]).Equals(JKeys.Result.Done))
                    {
                        boilers.Clear();
                        foreach (var item in nsusys.XMLConfig.GetConfigSection(NSU.Shared.NSUXMLConfig.ConfigSection.WoodBoilers).Elements())
                        {
                            wb = new WoodBoiler();
                            wb.ReadXMLNode(item);
                            boilers.Add(wb);
                            wb.OnSwitchState += OnSwitchStateHandler;
                            wb.OnAction += OnActionHandler;
                        }
                    }
                    break;
                case JKeys.Action.Info:
                    switch ((string)data[JKeys.Generic.Content])
                    {
                        case JKeys.WoodBoiler.TemperatureChange:
                            wb = FindWoodBoiler((string)data[JKeys.Generic.Name]);
                            if (wb != null)
                            {
                                wb.TempStatus = NSU.Shared.NSUUtils.Utils.GetStatusFromString((string)data[JKeys.WoodBoiler.TemperatureStatus], WoodBoilerTempStatus.Stable);
                                wb.CurrentTemp = (float)data[JKeys.WoodBoiler.CurrentTemp];
                            }
                            break;

                        case JKeys.Generic.Status:
                            wb = FindWoodBoiler((string)data[JKeys.Generic.Name]);
                            if (wb != null)
                            {
                                wb.Status = NSU.Shared.NSUUtils.Utils.GetStatusFromString((string)data[JKeys.Generic.Value], WoodBoilerStatus.UNKNOWN);
                            }
                            break;
                        case JKeys.WoodBoiler.LadomatStatus:
                            wb = FindWoodBoiler((string)data[JKeys.Generic.Name]);
                            if (wb != null)
                            {
                                wb.LadomStatus = NSU.Shared.NSUUtils.Utils.GetStatusFromString((string)data[JKeys.Generic.Value], LadomatStatus.OFF);
                            }
                            break;
                        case JKeys.WoodBoiler.ExhaustFanStatus:
                            wb = FindWoodBoiler((string)data[JKeys.Generic.Name]);
                            if (wb != null)
                            {
                                wb.ExhaustFanStatus = NSU.Shared.NSUUtils.Utils.GetStatusFromString((string)data[JKeys.Generic.Value], ExhaustFanStatus.OFF);
                            }
                            break;
                    }
                    break;
            }
        }

        private void OnActionHandler(WoodBoiler sender, NSUAction target)
        {
            var jo = new JObject
            {
                [JKeys.Generic.Target] = JKeys.WoodBoiler.TargetName,
                [JKeys.Generic.Action] = JKeys.WoodBoiler.ActionIkurimas,
                [JKeys.Generic.Name] = sender.Name
            };
            SendCommand(jo);
        }

        private void OnSwitchStateHandler(WoodBoiler sender, SwitchTarget target)
        {
            var jo = new JObject
            {
                [JKeys.Generic.Target] = JKeys.WoodBoiler.TargetName,
                [JKeys.Generic.Name] = sender.Name,
                [JKeys.Generic.Action] = JKeys.WoodBoiler.ActionSwitch
            };
            switch (target)
            {
                case SwitchTarget.ExhaustFan:
                    jo[JKeys.Generic.Value] = JKeys.WoodBoiler.TargetExhaustFan;
                    break;
                case SwitchTarget.Ladomat:
                    jo[JKeys.Generic.Value] = JKeys.WoodBoiler.TargetLadomat;
                    break;
                default:
                    break;
            }
            SendCommand(jo);
        }
    }
}
