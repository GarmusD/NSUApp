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
    public class KTypes : NSUSysPartsBase
    {
        public const int MAX_KTYPES = 1;

        private const string LogTag = "KTypes";

        List<KType> ktypes = new List<KType>();

		public KTypes(NSUSys sys, PartsTypes type)
            : base(sys, type)
        {
        }

        public override string[] RegisterTargets()
        {
            return new string[] { JKeys.KType.TargetName, "KTYPE:" };
        }

        public override void Clear()
        {
            ktypes.Clear();
        }

        public int Count { get { return ktypes.Count; } }

        public KType this[int index]
        {
            get
            {
                if (index < 0 || index >= MAX_KTYPES)
                    throw new IndexOutOfRangeException();
                if (index < ktypes.Count)
                    return ktypes[index];
                else
                    return null;
            }
        }

        public KType FindKType(string name)
        {
            for (int i = 0; i < ktypes.Count; i++)
            {
                if (ktypes[i].Name.Equals(name))
                    return ktypes[i];
            }
            return null;
        }

        public override void ParseNetworkData(JObject data)
        {
            KType ktp;
            switch ((string)data[JKeys.Generic.Action])
            {
                case JKeys.Syscmd.Snapshot:
                    if (((string)data[JKeys.Generic.Result]).Equals(JKeys.Result.Done))
                    {
                        ktypes.Clear();
                        foreach (var item in nsusys.XMLConfig.GetConfigSection(NSU.Shared.NSUXMLConfig.ConfigSection.KTypes).Elements())
                        {
                            ktp = new KType();
                            ktp.ReadXMLNode(item);
                            ktypes.Add(ktp);
                        }
                    }
                    break;
                case JKeys.Action.Info:
                    ktp = FindKType((string)data[JKeys.Generic.Name]);
                    if (ktp != null)
                    {
                        ktp.Temperature = (int)data[JKeys.Generic.Value];
                    }
                    break;
            }
        }

    }
}
