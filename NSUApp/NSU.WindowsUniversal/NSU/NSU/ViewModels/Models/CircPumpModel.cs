using NSU.NSU_UWP.ViewModels.Helpers;
using NSU.Shared.NSUSystemPart;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NSU.NSU_UWP.ViewModels.Models
{
    public class CircPumpModel : ModelBase
    {
        public int Index { get; set; }

        public bool? Enabled
        {
            get { return enabled; }
            set
            {
                if(value != null && enabled != value)
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
                if(value != null && name != value)
                {
                    name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string TempTriggerName
        {
            get { return tempTriggerName; }
            set
            {
                if(value != null && tempTriggerName != value)
                {
                    tempTriggerName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public NameClass TempTriggerSelection
        {
            get { return tempTriggerSelection; }
            set
            {
                if(value != null && TempTriggerSelection != value)
                {
                    tempTriggerSelection = value;
                    TempTriggerName = tempTriggerSelection.FakeName ? string.Empty : tempTriggerSelection.Name ;
                }
            }
        }

        public string MaxSpeed
        {
            get { return maxSpeed.ToString(); }
            set
            {
                if (value == null) return;
                byte new_value = byte.Parse(value);
                if(maxSpeed != new_value)
                {
                    maxSpeed = new_value;
                    if (maxSpeed < 1) maxSpeed = 1;
                    if (maxSpeed > 3) maxSpeed = 3;
                    RaisePropertyChanged();
                }
            }
        }

        public List<string> SpeedList { get { return speedsList; } }

        public AvailableChannels ChannelsList { get; }

        public byte Speed1Channel
        {
            get { return spd1Channel; }
            set
            {
                if(spd1Channel != value)
                {
                    spd1Channel = value;
                    RaisePropertyChanged();
                }
            }
        }

        public byte Speed2Channel
        {
            get { return spd2Channel; }
            set
            {
                if (spd2Channel != value)
                {
                    spd2Channel = value;
                    RaisePropertyChanged();
                }
            }
        }

        public byte Speed3Channel
        {
            get { return spd3Channel; }
            set
            {
                if (spd3Channel != value)
                {
                    spd3Channel = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ChannelData Speed1ChannelSelection
        {
            get { return spd1ChannelSelection; }
            set
            {
                if (value == null) return;
                if(spd1ChannelSelection != value)
                {
                    spd1ChannelSelection = value;
                    Speed1Channel = value.Channel;
                }
            }
        }

        public ChannelData Speed2ChannelSelection
        {
            get { return spd2ChannelSelection; }
            set
            {
                if (value == null) return;
                if (spd2ChannelSelection != value)
                {
                    spd2ChannelSelection = value;
                    Speed2Channel = value.Channel;
                }
            }
        }

        public ChannelData Speed3ChannelSelection
        {
            get { return spd3ChannelSelection; }
            set
            {
                if (value == null) return;
                if (spd3ChannelSelection != value)
                {
                    spd3ChannelSelection = value;
                    Speed3Channel = value.Channel;
                }
            }
        }

        public AvailableTempTriggerNamesList TempTriggerNamesList { get; }

        public bool IsModified { get; set; } = false;

        private bool enabled = false;
        private string name = string.Empty;
        private string tempTriggerName = string.Empty;
        private byte maxSpeed = 1;
        private byte spd1Channel = 255;
        private byte spd2Channel = 255;
        private byte spd3Channel = 255;
        private NameClass tempTriggerSelection;
        private ChannelData spd1ChannelSelection;
        private ChannelData spd2ChannelSelection;
        private ChannelData spd3ChannelSelection;

        private readonly List<string> speedsList;

        private CircPump cp;

        public CircPumpModel(CircPump circPump, AvailableChannels channels, AvailableTempTriggerNamesList tempTriggerNames)
        {
            ChannelsList = channels;
            TempTriggerNamesList = tempTriggerNames;
            speedsList = new List<string>();
            speedsList.Add("1");
            speedsList.Add("2");
            speedsList.Add("3");

            cp = circPump;
            if(cp != null)
            {
                enabled = cp.Enabled;
                name = cp.Name;
                tempTriggerName = cp.TempTriggerName;
                maxSpeed = cp.MaxSpeed;
                spd1Channel = cp.Spd1Channel;
                spd2Channel = cp.Spd2Channel;
                spd3Channel = cp.Spd3Channel;
            }

            UpdateChannelSelection();
            UpdateTTriggerNameSelection();

            ChannelsList.CollectionChanged += ChannelsList_CollectionChanged;
            TempTriggerNamesList.CollectionChanged += TempTriggerNamesList_CollectionChanged;
        }

        private void ChannelsList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateChannelSelection();
        }

        private void TempTriggerNamesList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateTTriggerNameSelection();
        }

        private void UpdateChannelSelection()
        {
            spd1ChannelSelection = ChannelsList.GetByChannel(spd1Channel);
            if (spd1ChannelSelection.Channel != 0xFF)
                spd1ChannelSelection.SetUsedBy($"CP {name} ch1");

            spd2ChannelSelection = ChannelsList.GetByChannel(spd2Channel);
            if (spd2ChannelSelection.Channel != 0xFF)
                spd2ChannelSelection.SetUsedBy($"CP {name} ch2");

            spd3ChannelSelection = ChannelsList.GetByChannel(spd3Channel);
            if (spd3ChannelSelection.Channel != 0xFF)
                spd3ChannelSelection.SetUsedBy($"CP {name} ch3");
        }

        private void UpdateTTriggerNameSelection()
        {
            tempTriggerSelection = TempTriggerNamesList.FindItemOrDefault(tempTriggerName);
        }
    }
}
