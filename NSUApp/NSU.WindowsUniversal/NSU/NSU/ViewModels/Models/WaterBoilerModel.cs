using NSU.NSU_UWP.ViewModels.Helpers;
using NSU.Shared.NSUSystemPart;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSU.NSU_UWP.ViewModels.Models
{
    public class ElHeatingDataModelList : ObservableCollection<ElHeatingDataModel> { }

    public class ElHeatingDataModel : ModelBase
    {
        public byte Index { get; set; }

        public string IndexAsWeekDay
        {
            get
            {
                string res;
                switch (Index)
                {
                    case 0:
                        res = "Pirmadienis";
                        break;
                    case 1:
                        res = "Antradienis";
                        break;
                    case 2:
                        res = "Trečiadienis";
                        break;
                    case 3:
                        res = "Ketvirtadienis";
                        break;
                    case 4:
                        res = "Penktadienis";
                        break;
                    case 5:
                        res = "Šeštadienis";
                        break;
                    case 6:
                        res = "Sekmadienis";
                        break;
                    default:
                        res = "KLAIDA";
                        break;
                }
                return res;
            }
        }

        public byte StartHour
        {
            get { return startHour; }
            set
            {
                if(startHour != value)
                {
                    startHour = value;
                    RaisePropertyChanged();
                }
            }
        }

        public byte StartMin
        {
            get { return startMin; }
            set
            {
                if(startMin != value)
                {
                    startMin = value;
                    RaisePropertyChanged();
                }
            }
        }

        public TimeSpan StartTime
        {
            get 
            {                
                return new TimeSpan(startHour, startMin, 0);
            }
            set
            {
                StartHour = (byte)value.Hours;
                StartMin = (byte)value.Minutes;
            }
        }

        public byte EndHour
        {
            get { return endHour; }
            set
            {
                if(endHour != value)
                {
                    endHour = value;
                    RaisePropertyChanged();
                }
            }
        }

        public byte EndMin
        {
            get { return endMin; }
            set
            {
                if(endMin != value)
                {
                    endMin = value;
                    RaisePropertyChanged();
                }
            }
        }

        public TimeSpan EndTime
        {
            get { return new TimeSpan(endHour, endMin, 0); }
            set
            {
                EndHour = (byte)value.Hours;
                EndMin = (byte)value.Minutes;
            }
        }

        private byte startHour;
        private byte startMin;
        private byte endHour;
        private byte endMin;
        private TimeSpan startTime = new TimeSpan();
        private TimeSpan endTime = new TimeSpan();
    }

    public class WaterBoilerModel : ModelBase
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
                    System.Diagnostics.Debug.WriteLine($"WaterBoiler changing name from '{name}' to '{value}'.");
                    RaisePropertyChanging(name, value);
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
                    if(tsNameSelection != value)
                    {
                        tsNameSelection = value;
                        TempSensorName = value.FakeName ? "" : value.Name;
                    }
                }
            }
        }

        public AvailableTempSensorNamesList TempSensorNameList { get; }

        public string TempTriggerName
        {
            get { return ttrgName; }
            set
            {
                if(ttrgName != value)
                {
                    ttrgName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public AvailableTempTriggerNamesList TempTrigerNameList { get; }

        public NameClass TempTriggerNameSelection
        {
            get { return ttrgNameSelection; }
            set
            {
                if(value != null)
                {
                    ttrgNameSelection = value;
                    TempTriggerName = value.FakeName ? "" : value.Name;
                }
            }
        }

        public string CircPumpName
        {
            get { return cpName; }
            set
            {
                if(cpName != value)
                {
                    cpName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public AvailableCircPumpNamesList CircPumpNameList{ get; }

        public NameClass CircPumpNameSelection
        {
            get { return cpNameSelection; }
            set
            {
                if(value != null)
                {
                    cpNameSelection = value;
                    CircPumpName = value.FakeName ? "" : value.Name;
                }
            }
        }

        public string WaterBoilerName
        {
            get { return waterBoilerName; }
            set
            {
                if(waterBoilerName != value)
                {
                    waterBoilerName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public NameClass WaterBoilerNameSelection
        {
            get { return waterBoilerNameSelection; }
            set
            {
                if(value != null)
                {
                    waterBoilerNameSelection = value;
                    WaterBoilerName = value.FakeName ? "" : value.Name;
                }
            }
        }

        public bool? ElHeatingEnabled
        {
            get { return elHeatingEnabled; }
            set
            {
                if(elHeatingEnabled != value)
                {
                    elHeatingEnabled = (bool)value;
                    RaisePropertyChanged();
                }
            }
        }

        public byte ElHeatingChannel
        {
            get { return elHeatingChannel; }
            set
            {
                if(elHeatingChannel != value)
                {
                    elHeatingChannel = value;
                    RaisePropertyChanged();
                }
            }
        }

        public AvailableChannels ChannelList { get; }

        public ChannelData ElHeatingChannelSelection
        {
            get { return elHeatingChannelSelection; }
            set
            {
                if(value != null)
                {
                    elHeatingChannelSelection = value;
                    ElHeatingChannel = value.Channel;
                }
            }
        }

        public ElHeatingDataModelList ElHeatingData { get; }

        private bool enabled = false;
        private string name = string.Empty;
        private string tsName = string.Empty;
        private NameClass tsNameSelection;
        private string ttrgName = string.Empty;
        private NameClass ttrgNameSelection;
        private string cpName = string.Empty;
        private NameClass cpNameSelection;
        private string waterBoilerName = string.Empty;
        private NameClass waterBoilerNameSelection;
        private bool elHeatingEnabled = false;
        private byte elHeatingChannel = 0xFF;
        private ChannelData elHeatingChannelSelection;
        private WaterBoiler boiler;

        public WaterBoilerModel(WaterBoiler waterBoiler)//, AvailableTempSensorNamesList tempSensors, AvailableChannels availableChannels, AvailableTempTriggerNamesList availableTriggers, AvailableCircPumpNamesList availableCircpumps
        {
            boiler = waterBoiler;

            TempSensorNameList = DataCollections.Current.TempSensorNameList;
            ChannelList = DataCollections.Current.ChannelList;
            TempTrigerNameList = DataCollections.Current.TempTriggerNameList;
            CircPumpNameList = DataCollections.Current.CircPumpNameList;

            ElHeatingData = new ElHeatingDataModelList();
            for (int i = 0; i < WaterBoiler.MAX_WATERBOILER_EL_HEATING_COUNT; i++)
            {
                ElHeatingData.Add(new ElHeatingDataModel());
                ElHeatingData[i].Index = (byte)i;
            }

            if (boiler != null)
            {
                enabled = boiler.Enabled;
                name = boiler.Name;
                tsName = boiler.TempSensorName;
                ttrgName = boiler.TempTriggerName;
                cpName = boiler.CircPumpName;
                elHeatingEnabled = boiler.ElHeatingEnabled;
                elHeatingChannel = boiler.ElHeatingChannel;
                for (int i = 0; i < WaterBoiler.MAX_WATERBOILER_EL_HEATING_COUNT; i++)
                {
                    var item = boiler.GetHeatingDataByIndex((byte)i);
                    if (item != null)
                    {
                        ElHeatingData[i].StartHour = item.StartHour;
                        ElHeatingData[i].StartMin = item.StartMin;
                        ElHeatingData[i].EndHour = item.EndHour;
                        ElHeatingData[i].EndMin = item.EndMin;
                    }
                }
            }

            UpdateTempSensorNameSelection();
            UpdateTempTriggerNameSelection();
            UpdateCircPumpNameSelection();
            UpdateElHeatingChannelSelection();

            TempSensorNameList.CollectionChanged += TempSensorNameList_CollectionChanged;
            TempTrigerNameList.CollectionChanged += TempTrigerNameList_CollectionChanged;
            CircPumpNameList.CollectionChanged += CircPumpNameList_CollectionChanged;
            ChannelList.CollectionChanged += ChannelList_CollectionChanged;
        }

        private void TempSensorNameList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateTempSensorNameSelection();
        }

        private void TempTrigerNameList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateTempTriggerNameSelection();
        }

        private void CircPumpNameList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateCircPumpNameSelection();
        }

        private void ChannelList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateElHeatingChannelSelection();
        }

        private void UpdateTempSensorNameSelection()
        {
            tsNameSelection = TempSensorNameList?.FindItemOrDefault(tsName);
        }

        private void UpdateTempTriggerNameSelection()
        {
            ttrgNameSelection = TempTrigerNameList?.FindItemOrDefault(ttrgName);
        }

        private void UpdateCircPumpNameSelection()
        {
            cpNameSelection = CircPumpNameList?.FindItemOrDefault(cpName);
        }

        private void UpdateElHeatingChannelSelection()
        {
            elHeatingChannelSelection = ChannelList.GetByChannel(elHeatingChannel);
        }
    }
}
