using NSU.NSU_UWP.ViewModels.Helpers;
using NSU.Shared.NSUSystemPart;
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
    public class ActuatorsList : ObservableCollection<ActuatorModel> { }
    public class ActuatorModel : ModelBase
    {
        public const string TYPE_NC = "NC";
        public const string TYPE_NO = "NO";

        public int Index { get; }

        public ActuatorType ActuatorType
        {
            get { return actuatorType; }
            set
            {
                actuatorType = value;
                RaisePropertyChanged();
            }
        }

        public List<string> ActuatorTypes { get; } = new List<string>() { TYPE_NC, TYPE_NO };

        public string ActuatorTypeSelection
        {
            get
            {
                switch(actuatorType)
                {
                    case ActuatorType.NC:
                        return TYPE_NC;
                    case ActuatorType.NO:
                        return TYPE_NO;
                }
                return TYPE_NC;
            }
            set
            {
                switch(value)
                {
                    case TYPE_NC:
                        ActuatorType = ActuatorType.NC;
                        break;
                    case TYPE_NO:
                        ActuatorType = ActuatorType.NO;
                        break;
                }
                //RaisePropertyChanged();
            }
        }

        public AvailableChannels ChannelList
        {
            get;
        }

        public ChannelData ChannelSelection
        {
            get { return channelSelection; }
            set
            {
                if(channelSelection != value)
                {
                    channelSelection = value;
                    Channel = value.Channel;
                }
            }
        }

        public byte Channel
        {
            get { return channel; }
            set
            {
                RemoveUsedBy();
                channel = value;
                channelSelection = ChannelList.GetByChannel(channel);
                SetUsedBy();
                RaisePropertyChanged();
            }
        }

        public void SetUsedBy()
        {
            ChannelList.SetUsedBy(channel, "Col:" + owner.Name + ":Ch:"+Index );
        }

        public void RemoveUsedBy()
        {
            ChannelList.RemoveUsedBy(channel);
        }

        ThermoActuator actuator;
        private ActuatorType actuatorType = ActuatorType.NC;
        private ChannelData channelSelection;
        private byte channel;
        private CollectorModel owner;

        public ActuatorModel(CollectorModel parent, int idx, ThermoActuator thermoActuator, AvailableChannels channelCollection)
        {
            owner = parent;
            Index = idx;
            ChannelList = channelCollection;
            actuator = thermoActuator;
            UpdateChannelSelection();
            ChannelList.CollectionChanged += ChannelList_CollectionChanged;
        }

        private void ChannelList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateChannelSelection();
        }

        private void UpdateChannelSelection()
        {
            if (actuator != null)
            {
                actuatorType = actuator.Type;
                channelSelection = ChannelList.GetByChannel(actuator.RelayChannel);
            }
            else
            {
                channelSelection = ChannelList.GetByChannel(0xFF);
            }
        }
    }

    public class CollectorModel : ModelBase
    {
        public int Index { get; set; }

        public bool? Enabled
        {
            get { return enabled; }
            set
            {
                if(value != null && value != enabled)
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
                if (value != null && name != value)
                {
                    name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string CircPumpName
        {
            get { return circPumpName; }
            set
            {
                if(value != null && circPumpName != value)
                {
                    circPumpName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public AvailableCircPumpNamesList CircPumpNameList
        {
            get { return circPumpNamesList; }
        }

        public NameClass CircPumpNameSelection
        {
            get { return circPumpNameSelection; }
            set
            {
                if(value != null && circPumpNameSelection != value)
                {
                    circPumpNameSelection = value;
                    CircPumpName = value.FakeName ? string.Empty : value.Name;
                }
            }
        }

        public byte ActuatorsCount
        {
            get { return actuatorsCount; }
            set
            {
                if(value != actuatorsCount)
                {
                    actuatorsCount = value;
                    if (value == 0)
                    {
                        actuators.Clear();
                    }
                    else if (actuators.Count > value)
                    {
                        //Delete/clear actuators
                        while (actuators.Count > value)
                        {
                            ActuatorModel am = actuators[actuators.Count - 1];
                            am.RemoveUsedBy();
                            actuators.RemoveAt(actuators.Count - 1);
                        }
                    }
                    else if (actuators.Count < value)
                    {
                        //Add actuators
                        while (actuators.Count < value)
                            actuators.Add(new ActuatorModel(this, actuators.Count+1, null, channelsList));
                    }
                    RaisePropertyChanged();
                }
            }
        }

        public NameClass ActuatorsCountSelection
        {
            get { return actuatorsCountSelection; }
            set
            {
                if(value != null && value != actuatorsCountSelection)
                {
                    actuatorsCountSelection = value;
                    int newCount = value.FakeName ? 0 : int.Parse(value.Name);
                    System.Diagnostics.Debug.WriteLine($"Setting ActuatorsCount for '{name}' to '{newCount}'");
                    ActuatorsCount = value.FakeName ? (byte)0 : byte.Parse(value.Name);
                }
            }
        }

        public SortedNameClassList ActuatorsCountList { get; }

        public ActuatorsList Actuators
        {
            get { return actuators; }
        }


        private bool? enabled = false;
        private string name = string.Empty;
        private string circPumpName = string.Empty;
        private byte actuatorsCount;

        private NameClass circPumpNameSelection;
        private NameClass actuatorsCountSelection;
        private ActuatorsList actuators;
        private AvailableChannels channelsList;
        private AvailableCircPumpNamesList circPumpNamesList;

        private Collector col;

        public CollectorModel(Collector collector)//AvailableChannels availableChannels, AvailableCircPumpNamesList circPumpList
        {
            actuators = new ActuatorsList();
            ActuatorsCount = 0;

            ActuatorsCountList = new SortedNameClassList();
            ActuatorsCountList.SetFixedItem(new NameClass("Nenustatyta") { FakeName = true });
            for(int i = 1; i <= Collector.MAX_COLLECTOR_ACTUATORS; i++)
            {
                ActuatorsCountList.Add(new NameClass(i.ToString()));
            }

            col = collector;
            channelsList = DataCollections.Current.ChannelList;
            circPumpNamesList = DataCollections.Current.CircPumpNameList;
            
            if (col != null)
            {
                enabled = col.Enabled;
                name = col.Name;
                circPumpName = col.CircPumpName;
                UpdateCircPumpNameSelection();
                ActuatorsCount = col.ActuatorsCount;
                actuatorsCountSelection = ActuatorsCountList.FindItemOrDefault(actuatorsCount.ToString());

                for (int i = 0; i < col.ActuatorsCount; i++)
                {
                    if (i < col.Actuators.Count && col.Actuators[i] != null)
                    {
                        actuators[i].ActuatorType = col.Actuators[i].Type;
                        actuators[i].Channel = col.Actuators[i].RelayChannel;
                    }
                }
            }
            actuatorsCountSelection = ActuatorsCountList.FindItemOrDefault(ActuatorsCount);
            UpdateCircPumpNameSelection();
            circPumpNamesList.CollectionChanged += CircPumpNamesList_CollectionChanged;
        }

        private void CircPumpNamesList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateCircPumpNameSelection();
        }

        private void UpdateCircPumpNameSelection()
        {
            circPumpNameSelection = circPumpNamesList.FindItemOrDefault(circPumpName);
        }
    }
}
