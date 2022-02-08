using NSU.NSU_UWP.UI.Elements;
using NSU.NSU_UWP.ViewModels.Helpers;
using NSU.Shared.NSUSystemPart;
using NSUAppShared.NSUSystemParts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NSU.NSU_UWP.ViewModels.Models
{
    public class TempTriggerPieceModel : ModelBase
    {
        const string TrueIfLowerStr = "TrueIfLower";
        const string TrueIfHigherStr = "TrueIfHigher";

        public byte Index { get { return (byte)(idx+1); } set { idx = value; } }

        public bool? Enabled
        {
            get { return enabled; }
            set
            {
                if(enabled != value && value != null)
                {
                    enabled = (bool)value;
                    RaisePropertyChanged();
                }
            }
        }

        public string TempSensorName
        {
            get { return tsName; }
            set
            {
                if(tsName != value && value != null)
                {
                    tsName = value.Trim();
                    RaisePropertyChanged();
                }
            }
        }

        public NameClass TempSensorNameSelection
        {
            get { return tsNameSelection; }
            set
            {
                if(tsNameSelection != value && value != null)
                {
                    tsNameSelection = value;
                    TempSensorName = tsNameSelection.FakeName ? string.Empty : tsNameSelection.Name;
                }
            }
        }

        private TriggerCondition TrgCondition
        {
            get { return condition; }
            set
            {
                if(condition != value)
                {
                    condition = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string TrgConditionStr
        {
            get
            {
                switch(condition)
                {
                    case TriggerCondition.TrueIfLower:
                        return TrueIfLowerStr;
                    case TriggerCondition.TrueIfHigher:
                        return TrueIfHigherStr;
                    default:
                        return TrueIfLowerStr;
                }
            }
            set
            {
                if (value != TrgConditionStr && value != null)
                {
                    switch (value)
                    {
                        case TrueIfLowerStr:
                            TrgCondition = TriggerCondition.TrueIfLower;
                            RaisePropertyChanged();
                            break;
                        case TrueIfHigherStr:
                            TrgCondition = TriggerCondition.TrueIfHigher;
                            RaisePropertyChanged();
                            break;
                    }
                }
            }
        }

        public float Temperature
        {
            get { return temp; }
            set
            {
                if(temp != value)
                {
                    temp = value;
                    RaisePropertyChanged();
                }
            }
        }

        public float Histeresis
        {
            get { return histeresis; }
            set
            {
                if (histeresis != value)
                {
                    histeresis = value;
                    RaisePropertyChanged();
                }
            }
        }

        public List<string> ConditionList { get; set; } = new List<string>();

        public AvailableTempSensorNamesList TempSensorNameList { get; }

        public bool IsModified { get; set; } = false;

        private byte idx = 255;
        private bool enabled = false;
        private string tsName = string.Empty;
        private NameClass tsNameSelection;
        private TriggerCondition condition = TriggerCondition.TrueIfLower;
        private float temp = 0;
        private float histeresis = 0;

        private TempTriggerPiece ttp;

        public TempTriggerPieceModel(TempTriggerPiece temTrgPiece)//, AvailableTempSensorNamesList tempSensorNames
        {
            TempSensorNameList = DataCollections.Current.TempSensorNameList;
            ttp = temTrgPiece;
            if (ttp != null)
            {
                enabled = ttp.Enabled;
                tsName = ttp.TSensorName;
                condition = ttp.Condition;
                temp = ttp.Temperature;
                histeresis = ttp.Histeresis;
            }
            ConditionList.Add(TrueIfLowerStr);
            ConditionList.Add(TrueIfHigherStr);
            UpdateTSensorNameSelection();
            TempSensorNameList.CollectionChanged += TempSensorNameList_CollectionChanged;
        }

        private void TempSensorNameList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateTSensorNameSelection();
        }

        private void UpdateTSensorNameSelection()
        {
            tsNameSelection = TempSensorNameList.FindItemOrDefault(tsName);
        }
    }

    public class TempTriggerModel : ModelBase
    {
        public int Index { get; set; }

        public bool? Enabled
        {
            get { return enabled; }
            set
            {
                if(value != enabled)
                {
                    enabled = (bool)value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if(name != value)
                {
                    name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<TempTriggerPieceModel> TempTriggerPieces { get; } = new ObservableCollection<TempTriggerPieceModel>();

        public new bool IsModified
        {
            get
            {
                for(int i=0; i < TempTrigger.MAX_TEMPTRIGGERPIECES; i++)
                {
                    if (TempTriggerPieces[i].IsModified) return true;
                }
                return base.IsModified;
            }
            private set
            {
                for (int i = 0; i < TempTrigger.MAX_TEMPTRIGGERPIECES; i++)
                {
                    TempTriggerPieces[i].IsModified = false;
                }
                base.IsModified = value;
            }
        }

        private int cfgpos = -1;
        private bool enabled = false;
        private string name = string.Empty;

        private TempTrigger tt;

        public TempTriggerModel(TempTrigger tempTrigger)//, AvailableTempSensorNamesList tempSensorNames
        {
            tt = tempTrigger;
            if(tt != null)
            {
                enabled = tt.Enabled;
                name = tt.Name;                
            }

            for (int i = 0; i < TempTrigger.MAX_TEMPTRIGGERPIECES; i++)
            {
                if (tt != null && tt[i] != null)
                {
                    TempTriggerPieces.Add(new TempTriggerPieceModel(tt[i]));
                }
                else
                {
                    TempTriggerPieceModel ttpvm = new TempTriggerPieceModel(null);
                    ttpvm.Index = (byte)i;
                    TempTriggerPieces.Add(ttpvm);
                }
            }
        }

    }
}
