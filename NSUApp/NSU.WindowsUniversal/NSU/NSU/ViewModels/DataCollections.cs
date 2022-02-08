using NSU.NSU_UWP.ViewModels.Helpers;
using NSU.NSU_UWP.ViewModels.Models;
using NSU.NSUSystem;
using NSU.Shared.NSUSystemPart;
using NSUAppShared.NSUSystemParts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSU.NSU_UWP.ViewModels
{
    //Generic class
    public class SortedNameClassList : SortedObservableCollection<NameClass>
    {
        public SortedNameClassList() : base("Name") { }
    }

    //Name lists
    public class AvailableTempSensorNamesList : SortedObservableCollection<NameClass>
    {
        public AvailableTempSensorNamesList():base("Name") { }
    }

    public class AvailableTempTriggerNamesList : SortedObservableCollection<NameClass>
    {
        public AvailableTempTriggerNamesList() : base("Name") { }
    }

    public class AvailableCircPumpNamesList : SortedObservableCollection<NameClass>
    {
        public AvailableCircPumpNamesList() : base("Name") { }
    }

    public class AvailableCollectorNamesList : SortedObservableCollection<NameClass>
    {
        public AvailableCollectorNamesList() : base("Name") { }
    }

    public class AvailableKTypeNamesList : SortedObservableCollection<NameClass>
    {
        public AvailableKTypeNamesList() : base("Name") { }
    }

    public class AvailableWaterBoilerNamesList : SortedObservableCollection<NameClass>
    {
        public AvailableWaterBoilerNamesList() : base("Name") { }
    }

   


    //View models
    public class TSensorsViewModel : ObservableCollection<TSensorModel> { }
    public class RelayModulesViewModel : ObservableCollection<RelayModuleModel> { }
    public class TempTriggersViewModel : ObservableCollection<TempTriggerModel> { }
    public class CircPumpsViewModel : ObservableCollection<CircPumpModel> { }
    public class CollectorsViewModel : ObservableCollection<CollectorModel>
    {
        public byte AvailableActuatorsCount(string collectorName)
        {
            foreach(var item in this)
            {
                if (item.Name == collectorName)
                    return item.ActuatorsCount;
            }
            return 0;
        }
    }
    public class ComfortZonesViewModel : ObservableCollection<ComfortZoneModel> { }
    public class KTypeViewModel : ObservableCollection<KTypeModel> { }
    public class WaterBoilerViewModel : ObservableCollection<WaterBoilerModel> { }
    public class WoodBoilerViewModel : ObservableCollection<WoodBoilerModel> { }

    public class AvailableChannels : ObservableCollection<ChannelData>
    {
        private ChannelData builtInChannel;

        public AvailableChannels()
        {
            //Add BuiltIn 'Not Connected' channel
            builtInChannel = new ChannelData();
            builtInChannel.Channel = 0xFF;
            builtInChannel.UsedBy = "Nepajungtas";
            Add(builtInChannel);            
        }

        public new void Clear()
        {
            base.Clear();
            Add(builtInChannel);
        }

        public ChannelData GetByChannel(byte channel)
        {
            foreach (var item in this)
            {
                if (item.Channel == channel)
                    return item;
            }
            return GetByChannel(0xFF);
        }

        public string GetValueByChannel(byte channel)
        {
            foreach(var item in this)
            {
                if (item.Channel == channel)
                    return item.Value;
            }
            return channel.ToString();
        }

        public void SetUsedBy(byte channel, string owner)
        {
            var item = GetByChannel(channel);
            item?.SetUsedBy(owner);
        }

        public void RemoveUsedBy(byte channel)
        {
            var item = GetByChannel(channel);
            item?.SetUsedBy("");
        }
    }


    public class AvailableActuatorsList
    {
        public int this[string collectorName]
        {
            get { return 0; }
        }
    }


    public class DataCollections
    {
        public static DataCollections Current
        { get
            {
                if (instance == null)
                {
                    instance = new DataCollections();
                }
                return instance;
            }
        }
        private static DataCollections instance = null;

        #region Names Lists
        public AvailableChannels ChannelList => GetChannelList();
        public AvailableTempSensorNamesList TempSensorNameList => GetTempSensorNameList();
        public AvailableTempTriggerNamesList TempTriggerNameList => GetTempTriggerNameList();
        public AvailableCircPumpNamesList CircPumpNameList => GetCircPumpNamesList();
        public AvailableCollectorNamesList CollectorNameList => GetCollectorNameList();
        public AvailableKTypeNamesList KTypeNameList => GetKTypeNameList();
        public AvailableWaterBoilerNamesList WaterBoilerNameList => GetWaterBoilerNameList();
        #endregion

        #region View models
        public TSensorsViewModel TSensorsVM => GetTSensorsViewModel();
        public RelayModulesViewModel RelayModulesVM => GetRelayModulesViewModel();
        public TempTriggersViewModel TempTriggersVM => GetTempTriggerViewModel();
        public CircPumpsViewModel CircPumpsVM => GetCircPumpsViewModel();
        public CollectorsViewModel CollectorsVM => GetCollectorsViewModel();
        public ComfortZonesViewModel ComfortZonesVM => GetComfortZonesViewModel();
        public KTypeViewModel KTypeVM => GetKTypeViewModel();
        public WaterBoilerViewModel WaterBoilerVM => GetWaterBoilerViewModel();
        public WoodBoilerViewModel WoodBoilerVM => GetWoodBoilerViewModel();
        #endregion

        #region Private declarations
        private AvailableChannels channelList = null;
        private AvailableTempSensorNamesList tsnames = null;
        private AvailableTempTriggerNamesList ttrgnames = null;
        private AvailableCircPumpNamesList cpnames = null;
        private AvailableCollectorNamesList colnames = null;
        private AvailableKTypeNamesList ktpnames = null;
        private AvailableWaterBoilerNamesList wtbnames = null;

        private TSensorsViewModel tsVM = null;
        private RelayModulesViewModel rmVM = null;
        private TempTriggersViewModel ttVM = null;
        private CircPumpsViewModel cpVM = null;
        private CollectorsViewModel clVM = null;
        private ComfortZonesViewModel czVM = null;
        private KTypeViewModel ktVM = null;
        private WaterBoilerViewModel wtrVM = null;
        private WoodBoilerViewModel wdbVM = null;
        #endregion

        protected DataCollections()
        {
        }

        private AvailableTempSensorNamesList GetTempSensorNameList()
        {
            if (tsnames == null)
            {
                tsnames = new AvailableTempSensorNamesList();
                tsnames.SetFixedItem(new NameClass("- - - - -") { FakeName = true});

                foreach (var item in TSensorsVM)
                {
                    item.PropertyChanging += TempSensorName_Changing;
                    if (!string.IsNullOrEmpty(item?.Name))
                    {
                        tsnames.Add(new NameClass(item.Name));
                    }
                }
            }
            return tsnames;
        }

        private void TempSensorName_Changing(object sender, PropertyChangingEventArgs args)
        {
            if (args.PropertyName == "Name")
            {
                var item = tsnames.FindItemOrDefault((string)args.OldValue);
                if ((bool)!item?.FakeName)
                {
                    tsnames.Remove(item);                    
                }
                if (!string.IsNullOrEmpty((string)args.NewValue))
                {
                    tsnames.Add(new NameClass((string)args.NewValue));
                }
            }
        }

        private AvailableChannels GetChannelList()
        {
            if(channelList == null)
            {
                channelList = new AvailableChannels();
                ChannelData cd;

                byte idx = 1;                
                foreach(var item in RelayModulesVM)
                {
                    //TODO Handle channel count on Enable/Disable RelayModule
                    if (item != null && (bool)item.Enabled)
                    {
                        for (int j = 0; j < RelayModule.CHANNELS_PER_MODULE; j++)
                        {
                            cd = new ChannelData();
                            cd.Channel = idx++;
                            channelList.Add(cd);
                        }
                    }
                }                
            }
            return channelList;
        }

        private AvailableTempTriggerNamesList GetTempTriggerNameList()
        {
            if (ttrgnames == null)
            {
                ttrgnames = new AvailableTempTriggerNamesList();
                ttrgnames.SetFixedItem(new NameClass("- - - - -") { FakeName = true });

                foreach (var item in TempTriggersVM)
                {
                    item.PropertyChanging += TempTriggerName_Changing;
                    if (!string.IsNullOrEmpty(item?.Name))
                    {
                        ttrgnames.Add(new NameClass(item.Name));
                    }
                }
            }
            return ttrgnames;
        }

        private void TempTriggerName_Changing(object sender, PropertyChangingEventArgs args)
        {
            if (args.PropertyName == "Name")
            {
                var item = ttrgnames.FindItemOrDefault((string)args.OldValue);
                if ((bool)!item?.FakeName)
                {
                    ttrgnames.Remove(item);                    
                }
                if (!string.IsNullOrEmpty((string)args.NewValue))
                {
                    ttrgnames.Add(new NameClass((string)args.NewValue));
                }
            }
        }

        private AvailableCircPumpNamesList GetCircPumpNamesList()
        {
            if(cpnames == null)
            {
                cpnames = new AvailableCircPumpNamesList();
                cpnames.SetFixedItem(new NameClass("- - - - -") { FakeName = true });

                foreach(var item in CircPumpsVM)
                {
                    item.PropertyChanging += CircPumpName_Changing;
                    if(!string.IsNullOrEmpty(item.Name))
                    {
                        cpnames.Add(new NameClass(item.Name));
                    }
                }
            }
            return cpnames;
        }

        private void CircPumpName_Changing(object sender, PropertyChangingEventArgs args)
        {
            if (args.PropertyName == "Name")
            {
                var item = cpnames.FindItemOrDefault((string)args.OldValue);
                if ((bool)!item?.FakeName)
                {
                    cpnames.Remove(item);                    
                }
                if (!string.IsNullOrEmpty((string)args.NewValue))
                {
                    cpnames.Add(new NameClass((string)args.NewValue));
                }
            }
        }

        private AvailableCollectorNamesList GetCollectorNameList()
        {
            if (colnames == null)
            {
                colnames = new AvailableCollectorNamesList();
                colnames.SetFixedItem(new NameClass("- - - - -") { FakeName = true });

                foreach(var item in CollectorsVM)
                {
                    item.PropertyChanging += CollectorName_Changing;
                    if(!string.IsNullOrEmpty(item?.Name))
                    {
                        colnames.Add(new NameClass(item.Name));
                    }
                }
            }
            return colnames;
        }

        private void CollectorName_Changing(object sender, PropertyChangingEventArgs args)
        {
            if (args.PropertyName == "Name")
            {
                var item = colnames.FindItemOrDefault((string)args.OldValue);
                if ((bool)!item?.FakeName)
                {
                    colnames.Remove(item);                    
                }
                if (!string.IsNullOrEmpty((string)args.NewValue))
                {
                    colnames.Add(new NameClass((string)args.NewValue));
                }
            }
        }

        private AvailableKTypeNamesList GetKTypeNameList()
        {
            if(ktpnames == null)
            {
                ktpnames = new AvailableKTypeNamesList();
                ktpnames.SetFixedItem(new NameClass("- - - - -") { FakeName = true });

                foreach(var item in KTypeVM)
                {
                    item.PropertyChanging += KTypeName_Changing;
                    if (!string.IsNullOrEmpty(item?.Name))
                        ktpnames.Add(new NameClass(item.Name));
                }
            }
            return ktpnames;
        }

        private void KTypeName_Changing(object sender, PropertyChangingEventArgs args)
        {
            if (args.PropertyName == "Name")
            {
                var item = ktpnames.FindItemOrDefault((string)args.OldValue);
                if ((bool)!item?.FakeName)
                {
                    ktpnames.Remove(item);                    
                }
                if (!string.IsNullOrEmpty((string)args.NewValue))
                {
                    ktpnames.Add(new NameClass((string)args.NewValue));
                }
            }
        }

        private AvailableWaterBoilerNamesList GetWaterBoilerNameList()
        {
            if(wtbnames == null)
            {
                wtbnames = new AvailableWaterBoilerNamesList();

                wtbnames.SetFixedItem(new NameClass("- - - - -") { FakeName = true });

                foreach(var item in WaterBoilerVM)
                {
                    item.PropertyChanging += WaterBoilerName_Changing;
                    if (!string.IsNullOrEmpty(item?.Name))
                    {
                        wtbnames.Add(new NameClass(item.Name));
                    }
                }
            }
            return wtbnames;
        }


        private void WaterBoilerName_Changing(object sender, PropertyChangingEventArgs args)
        {
            if (args.PropertyName == "Name")
            {
                var item = wtbnames.FindItemOrDefault((string)args.OldValue);
                if ((bool)!item?.FakeName)
                {
                    wtbnames.Remove(item);                    
                }
                if (!string.IsNullOrEmpty((string)args.NewValue))
                {
                    wtbnames.Add(new NameClass((string)args.NewValue));
                }
            }
        }

        private TSensorsViewModel GetTSensorsViewModel()
        {
            if (tsVM == null)
            {
                tsVM = new TSensorsViewModel();
                FillTSensorsViewModel();
            }
            return tsVM;
        }

        private RelayModulesViewModel GetRelayModulesViewModel()
        {
            if (rmVM == null)
            {
                rmVM = new RelayModulesViewModel();
                FillRelayModulesViewModel();
            }
            return rmVM;
        }

        private TempTriggersViewModel GetTempTriggerViewModel()
        {
            if (ttVM == null)
            {
                ttVM = new TempTriggersViewModel();
                FillTempTriggersViewModel();
            }
            return ttVM;
        }

        private CircPumpsViewModel GetCircPumpsViewModel()
        {
            if(cpVM == null)
            {
                cpVM = new CircPumpsViewModel();
                FillCircPumpsViewModel();
            }
            return cpVM;
        }

        private CollectorsViewModel GetCollectorsViewModel()
        {
            if(clVM == null)
            {
                clVM = new CollectorsViewModel();
                FillCollectorsViewModel();
            }
            return clVM;
        }

        private ComfortZonesViewModel GetComfortZonesViewModel()
        {
            if (czVM == null)
            {
                czVM = new ComfortZonesViewModel();
                FillComfortZonesViewModel();
            }
            return czVM;
        }

        private KTypeViewModel GetKTypeViewModel()
        {
            if(ktVM == null)
            {
                ktVM = new KTypeViewModel();
                FillKTypeViewModel();
            }
            return ktVM;
        }

        private WaterBoilerViewModel GetWaterBoilerViewModel()
        {
            if(wtrVM == null)
            {
                wtrVM = new WaterBoilerViewModel();
                FillWaterBoilerViewModel();
            }
            return wtrVM;
        }

        public WoodBoilerViewModel GetWoodBoilerViewModel()
        {
            if(wdbVM == null)
            {
                wdbVM = new WoodBoilerViewModel();
                FillWoodBoilerViewModel();
            }
            return wdbVM;
        }







        private void FillTSensorsViewModel()
        {
            NSUSys s = NSUSys.Instance;
            var tss = s.GetNSUSysPart(NSUAppShared.NSUSystemParts.PartsTypes.TSensors) as TempSensors;
            for (int i = 0; i < tss.Count; i++)
            {
                var tsd = new TSensorModel(tss[i]);
                tsVM.Add(tsd);
                tsd.Index = tsVM.Count;
            }
        }

        private void FillRelayModulesViewModel()
        {
            NSUSys s = NSUSys.Instance;
            var rms = s.GetNSUSysPart(NSUAppShared.NSUSystemParts.PartsTypes.RelayModules) as RelayModules;
            for (int i = 0; i < RelayModules.MAX_RELAY_MODULES; i++)
            {
                var rmd = new RelayModuleModel(rms[i]);
                rmVM.Add(rmd);//Include null modules - they should have a chance to be enabled
                rmd.Index = rmVM.Count;
            }
        }

        private void FillTempTriggersViewModel()
        {
            NSUSys s = NSUSys.Instance;
            var tts = s.GetNSUSysPart(PartsTypes.TempTriggers) as TempTriggers;
            for(int i=0; i< TempTriggers.MAX_TEMP_TRIGGERS; i++)
            {
                var ttvm = new TempTriggerModel(tts[i]);
                ttVM.Add(ttvm);
                ttvm.Index = ttVM.Count;
            }
        }

        private void FillCircPumpsViewModel()
        {
            NSUSys s = NSUSys.Instance;
            var cps = s.GetNSUSysPart(PartsTypes.CircPumps) as CircPumps;
            for(int i=0; i < CircPumps.MAX_CIRC_PUMPS; i++)
            {
                var cpvm = new CircPumpModel(cps[i], ChannelList, TempTriggerNameList);
                cpVM.Add(cpvm);
                cpvm.Index = cpVM.Count;
            }
        }

        private void FillCollectorsViewModel()
        {
            NSUSys s = NSUSys.Instance;
            var cls = s.GetNSUSysPart(PartsTypes.Collectors) as Collectors;
            for(int i=0; i < Collectors.MAX_COLLECTORS; i++)
            {
                var clvm = new CollectorModel(cls[i]);
                clVM.Add(clvm);
                clvm.Index = clVM.Count;
            }
        }

        private void FillComfortZonesViewModel()
        {
            NSUSys s = NSUSys.Instance;
            var czs = s.GetNSUSysPart(PartsTypes.ComfortZones) as ComfortZones;
            for(int i=0; i < ComfortZones.MAX_COMFORT_ZONES; i++)
            {
                var czvm = new ComfortZoneModel(czs[i]);
                czVM.Add(czvm);
                czvm.Index = czVM.Count;
            }
        }

        private void FillKTypeViewModel()
        {
            NSUSys s = NSUSys.Instance;
            var ktps = s.GetNSUSysPart(PartsTypes.KTypes) as KTypes;
            for(int i=0; i < KTypes.MAX_KTYPES; i++)
            {
                var ktpM = new KTypeModel(ktps[i]);
                ktVM.Add(ktpM);
                ktpM.Index = ktVM.Count;
            }
        }

        private void FillWaterBoilerViewModel()
        {
            NSUSys s = NSUSys.Instance;
            var wtrbs = s.GetNSUSysPart(PartsTypes.WaterBoilers) as WaterBoilers;
            for(int i=0; i < WaterBoilers.MAX_WATER_BOILERS; i++)
            {
                var wtrM = new WaterBoilerModel(wtrbs[i]);
                wtrVM.Add(wtrM);
                wtrM.Index = wtrVM.Count;
            }
        }

        private void FillWoodBoilerViewModel()
        {
            NSUSys s = NSUSys.Instance;
            var wodbs = s.GetNSUSysPart(PartsTypes.WoodBoilers) as WoodBoilers;
            for(int i=0; i < WoodBoilers.MAX_WOOD_BOILERS; i++)
            {
                var wodM = new WoodBoilerModel(wodbs[i], ChannelList, TempSensorNameList, TempTriggerNameList, KTypeNameList, WaterBoilerNameList);
                wdbVM.Add(wodM);
                wodM.Index = wdbVM.Count;
            }
        }
    }
}
