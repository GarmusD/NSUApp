using NSU.NSU_UWP.ViewModels.Helpers;
using NSU.Shared.NSUSystemPart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSU.NSU_UWP.ViewModels.Models
{
    public class WoodBoilerModel : ModelBase
    {
        public int Index { get; set; }

        public bool? Enabled
        {
            get { return enabled; }
            set
            {
                if(enabled != value)
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

        public string TempSensorName
        {
            get { return tsName; }
            set
            {
                if(tsName != value)
                {
                    tsName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public NameClass TempSensorNameSelection
        {
            get { return tsNameSelection; }
            set
            {
                if(value != null)
                {
                    tsNameSelection = value;
                    TempSensorName = value.FakeName ? "" : value.Name;
                }
            }
        }

        public AvailableTempSensorNamesList TempSensorNameList { get; }

        public string KTypeName
        {
            get { return ktpName; }
            set
            {
                if(ktpName != value)
                {
                    ktpName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public NameClass KTypeNameSelection
        {
            get { return ktpNameSelection; }
            set
            {
                if(value != null)
                {
                    ktpNameSelection = value;
                    KTypeName = value.FakeName ? "" : value.Name;
                }
            }
        }

        public AvailableTempTriggerNamesList TempTriggerNameList { get; }

        public AvailableKTypeNamesList KTypeNameList { get; }

        public AvailableChannels ChannelList { get; }

        public byte LadomatChannel
        {
            get { return ladomatChannel; }
            set
            {
                if(ladomatChannel != value)
                {
                    ladomatChannel = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ChannelData LadomatChannelSelection
        {
            get { return ladomatChannelSelection; }
            set
            {
                if(value != null)
                {
                    ladomatChannelSelection = value;
                    LadomatChannel = value.Channel;
                }
            }
        }

        public byte ExhaustFanChannel
        {
            get { return exhaustFanChannel; }
            set
            {
                if(exhaustFanChannel != value)
                {
                    exhaustFanChannel = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ChannelData ExhaustFanChannelSelection
        {
            get { return exhaustChannelSelection; }
            set
            {
                if(value != null)
                {
                    exhaustChannelSelection = value;
                    ExhaustFanChannel = value.Channel;
                }
            }
        }

        public float WorkingTemperature
        {
            get { return workingTemp; }
            set
            {
                if(workingTemp != value)
                {
                    workingTemp = value;
                    RaisePropertyChanged();
                }
            }
        }

        public float WorkingHisteresis
        {
            get { return workingHisteresis; }
            set
            {
                if(workingHisteresis != value)
                {
                    workingHisteresis = value;
                    RaisePropertyChanged();
                }
            }
        }

        public float LadomatTemperature
        {
            get { return ladomatTemp; }
            set
            {
                if(ladomatTemp != value)
                {
                    ladomatTemp = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string LadomatTriggerName
        {
            get { return ladomatTempTrigger; }
            set
            {
                if(ladomatTempTrigger != value)
                {
                    ladomatTempTrigger = value;
                    RaisePropertyChanged();
                }
            }
        }

        public NameClass LadomatTempTriggerNameSelection
        {
            get { return ladomatTempTriggerSelection; }
            set
            {
                if(value != null)
                {
                    ladomatTempTriggerSelection = value;
                    LadomatTriggerName = value.FakeName ? "" : value.Name;
                }
            }
        }

        public string WaterBoilerName
        {
            get { return wtbName; }
            set
            {
                if(wtbName != value)
                {
                    wtbName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public AvailableWaterBoilerNamesList WaterBoilerNameList { get; }

        public NameClass WaterBoilerNameSelection
        {
            get { return wtbNameSelection; }
            set
            {
                if(value != null)
                {
                    wtbNameSelection = value;
                    WaterBoilerName = value.FakeName ? "" : value.Name;
                }
            }
        }

        private bool enabled = false;
        private string name = string.Empty;
        private string tsName = string.Empty;
        private NameClass tsNameSelection;
        private string ktpName = string.Empty;
        private NameClass ktpNameSelection;
        private byte ladomatChannel = 0xFF;
        private ChannelData ladomatChannelSelection;
        private byte exhaustFanChannel = 0xFF;
        private ChannelData exhaustChannelSelection;
        private float workingTemp = 0;
        private float workingHisteresis = 0;
        private float ladomatTemp = 0; //<- nauja
        private string ladomatTempTrigger = string.Empty;
        private NameClass ladomatTempTriggerSelection;
        private string wtbName;
        private NameClass wtbNameSelection;

        private WoodBoiler wb;

        public WoodBoilerModel(WoodBoiler wboiler, 
                                AvailableChannels channelList, 
                                AvailableTempSensorNamesList sensorList, 
                                AvailableTempTriggerNamesList triggerList, 
                                AvailableKTypeNamesList ktypeList,
                                AvailableWaterBoilerNamesList wbList)
        {
            wb = wboiler;
            ChannelList = channelList;
            TempSensorNameList = sensorList;
            TempTriggerNameList = triggerList;
            KTypeNameList = ktypeList;
            WaterBoilerNameList = wbList;

            if (wb != null)
            {
                enabled = wb.Enabled;
                name = wb.Name;
                tsName = wb.TSensorName;
                ktpName = wb.KTypeName;
                ladomatChannel = wb.LadomChannel;
                exhaustFanChannel = wb.ExhaustFanChannel;
                workingTemp = wb.WorkingTemp;
                workingHisteresis = wb.Histeresis;
                ladomatTemp = wb.LadomatTemp;
                ladomatTempTrigger = wb.LadomatTriggerName;
            }

            UpdateTempSensorNameSelection();
            UpdateKTypeNameSelection();
            UpdateChannelSelection();
            UpdateTempTriggerNameSelection();
            UpdateWaterBoilerNameSelection();

            TempSensorNameList.CollectionChanged += TempSensorNameList_CollectionChanged;
            KTypeNameList.CollectionChanged += KTypeNameList_CollectionChanged;
            ChannelList.CollectionChanged += ChannelList_CollectionChanged;
            TempTriggerNameList.CollectionChanged += TempTriggerNameList_CollectionChanged;
            WaterBoilerNameList.CollectionChanged += WaterBoilerNameList_CollectionChanged;
        }

        private void TempSensorNameList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateTempSensorNameSelection();
        }

        private void KTypeNameList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateKTypeNameSelection();
        }

        private void ChannelList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateChannelSelection();
        }

        private void TempTriggerNameList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateTempTriggerNameSelection();
        }

        private void WaterBoilerNameList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateWaterBoilerNameSelection();
        }

        private void UpdateTempSensorNameSelection()
        {
            tsNameSelection = TempSensorNameList.FindItemOrDefault(tsName);
        }

        private void UpdateKTypeNameSelection()
        {
            ktpNameSelection = KTypeNameList.FindItemOrDefault(ktpName);
        }

        private void UpdateChannelSelection()
        {
            ladomatChannelSelection = ChannelList.GetByChannel(ladomatChannel);
            exhaustChannelSelection = ChannelList.GetByChannel(exhaustFanChannel);
        }

        private void UpdateTempTriggerNameSelection()
        {
            ladomatTempTriggerSelection = TempTriggerNameList.FindItemOrDefault(ladomatTempTrigger);
        }

        private void UpdateWaterBoilerNameSelection()
        {
            wtbNameSelection = WaterBoilerNameList.FindItemOrDefault(wtbName);
        }
    }
}
