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
    public class ComfortZoneModel : ModelBase
    {
        public int Index { get; set; } = -1;

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

        public string Name
        {
            get { return name; }
            set {
                if (name != value && value != null)
                {
                    name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Title
        {
            get { return title; }
            set
            {
                if (title != value && value != null)
                {
                    title = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string RoomTempSensorName
        {
            get { return rsname; }
            set
            {
                if (rsname != value && value != null)
                {
                    rsname = value.Trim();
                    RaisePropertyChanged();
                }
            }
        }

        public string FloorTemSensorName
        {
            get { return fsname; }
            set
            {
                if (fsname != value && value != null)
                {
                    fsname = value.Trim();
                    RaisePropertyChanged();
                }
            }
        }

        public float RoomTempHi
        {
            get { return roomTempHi; }
            set {
                if(roomTempHi != value)
                {
                    roomTempHi = value;
                    RaisePropertyChanged();
                }
            }
        }

        public float RoomTempLow
        {
            get { return roomTempLow; }
            set
            {
                if (roomTempLow != value)
                {
                    roomTempLow = value;
                    RaisePropertyChanged();
                }
            }
        }

        public float FloorTempHi
        {
            get { return floorTempHi; }
            set
            {
                if (floorTempHi != value)
                {
                    floorTempHi = value;
                    RaisePropertyChanged();
                }
            }
        }

        public float FloorTempLow
        {
            get { return floorTempLow; }
            set
            {
                if (floorTempLow != value)
                {
                    floorTempLow = value;
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

        public string CollectorName
        {
            get { return collname; }
            set
            {
                if (value != null && collname != value)
                {
                    collname = value;
                    UpdateActuatorsList(collname);
                    RaisePropertyChanged();
                }
            }
        }        

        public AvailableCollectorNamesList CollectorNameList { get; }

        public NameClass CollectorNameSelection
        {
            get { return collNameSelection; }
            set
            {
                if (collNameSelection != value && value != null)
                {
                    collNameSelection = value;
                    CollectorName = value.FakeName ? string.Empty : value.Name;
                }
            }
        }

        public byte Actuator
        {
            get { return actuator; }
            set
            {
                actuator = value;
                RaisePropertyChanged();
            }
        }

        public ChannelData ActuatorSelection
        {
            get { return actuatorSelection; }
            set
            {
                if (value != null && actuatorSelection != value)
                {
                    actuatorSelection = value;
                    Actuator = value.Channel;
                }
            }
        }

        public bool? LowTempMode
        {
            get { return lowTempMode; }
            set
            {
                if(lowTempMode != value && value != null)
                {
                    lowTempMode = (bool)value;
                    RaisePropertyChanged();
                }
            }
        }

        public AvailableTempSensorNamesList TempSensorNameList { get; }

        public AvailableChannels ActuatorChannelList { get; }

        public NameClass RoomTSensorNameSelection
        {
            get { return roomTSensorSelection; }
            set
            {
                if(roomTSensorSelection != value && value != null)
                {
                    roomTSensorSelection = value;
                    RoomTempSensorName = value.FakeName ? string.Empty : value.Name;
                }
            }
        }

        public NameClass FloorTSensorNameSelection
        {
            get { return floorTSensorSelection; }
            set
            {
                if (floorTSensorSelection != value && value != null)
                {
                    floorTSensorSelection = value;
                    FloorTemSensorName = value.FakeName ? string.Empty : value.Name;
                }
            }
        }

        private bool enabled = false;
        private string name = string.Empty;
        private string title = string.Empty;
        private string rsname = string.Empty;
        private string fsname = string.Empty;
        private string collname = string.Empty;
        private float roomTempHi = 0;
        private float roomTempLow = 0;
        private float floorTempHi = 0;
        private float floorTempLow = 0;
        private float histeresis = 0;
        private byte actuator = 0xFF;
        private bool lowTempMode = false;

        private NameClass roomTSensorSelection;
        private NameClass floorTSensorSelection;
        private NameClass collNameSelection;
        private ChannelData actuatorSelection = null;

        private ComfortZone cz;

        public ComfortZoneModel(ComfortZone czone)
        {
            TempSensorNameList = DataCollections.Current.TempSensorNameList;
            ActuatorChannelList = new AvailableChannels();
            CollectorNameList = DataCollections.Current.CollectorNameList;

            cz = czone;
            if (cz != null)
            {
                enabled = cz.Enabled;
                name = cz.Name;
                title = cz.Title;
                rsname = cz.RoomSensorName;
                fsname = cz.FloorSensorName;
                collname = cz.CollectorName;
                roomTempHi = cz.RoomTempHi;
                roomTempLow = cz.RoomTempLow;
                floorTempHi = cz.FloorTempHi;
                floorTempLow = cz.FloorTempLow;
                histeresis = cz.Histeresis;
                actuator = cz.Actuator;
                lowTempMode = cz.LowTempMode;
            }

            UpdateTSensorSelection();
            UpdateCollectorNameSelection();
            UpdateActuatorsList(collname);

            TempSensorNameList.CollectionChanged += TempSensorNameList_CollectionChanged;
            CollectorNameList.CollectionChanged += CollectorNameList_CollectionChanged;
            ActuatorChannelList.CollectionChanged += ChannelList_CollectionChanged;
        }

        private void ChannelList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateChannelListSelection();
        }

        private void CollectorNameList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateCollectorNameSelection();
        }

        private void TempSensorNameList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateTSensorSelection();
        }

        private void UpdateTSensorSelection()
        {
            roomTSensorSelection = TempSensorNameList.FindItemOrDefault(rsname);
            floorTSensorSelection = TempSensorNameList.FindItemOrDefault(fsname);
        }

        private void UpdateCollectorNameSelection()
        {
            collNameSelection = CollectorNameList.FindItemOrDefault(collname);
        }

        private void UpdateChannelListSelection()
        {
            actuatorSelection = ActuatorChannelList.GetByChannel(actuator);
        }

        private void UpdateActuatorsList(string collname)
        {
            ActuatorChannelList.Clear();
            for(int i=0; i < DataCollections.Current.CollectorsVM.AvailableActuatorsCount(collname); i++)
            {
                ActuatorChannelList.Add(new ChannelData((byte)(i + 1)));
            }
            UpdateChannelListSelection();
        }
    }
}
