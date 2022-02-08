using NSU.NSU_UWP.Dialogs;
using NSU.NSUSystem;
using NSU.Shared;
using NSU.Shared.NSUSystemPart;
using NSU.Shared.NSUUI;
using NSUAppShared;
using NSUAppShared.NSUSystemParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace NSU.NSU_UWP.UI.Elements
{
    public class UIComfortZone : INSUUIComfortZone
    {
        const string LogTag = "UIComfortZone";
        readonly string degree = " °C";
        const double FontSize = 12;

        SynchronizationContext sc;
        ComfortZone cz = null;
        string czName = string.Empty;

        Border border;
        Grid maingrid, topgrid, btmgrid;
        Image imgFloor, imgSettings, imgGraph;
        TextBlock txtRoomTemp, txtFloorTemp;
        StackPanel panel;
        TranslateTransform tt;

        bool cpOn, valveOpened;

        public UIElement uiElement { get { return border; } }

        public NSUUIClass UIClass { get; } = NSUUIClass.ComfortZone;

        public UIComfortZone()
        {
            sc = SynchronizationContext.Current;
            tt = new TranslateTransform();
            tt.X = 0;
            tt.Y = 0;

            border = new Border();
            border.CornerRadius = new CornerRadius(5);
            border.Width = 72;
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = new SolidColorBrush(Colors.Blue);
            border.RenderTransform = tt;
            //border.Width = 140;
            //border.Height = 113;

            maingrid = new Grid();
            //maingrid.CornerRadius = new CornerRadius(5);
            //maingrid.Width = 80;
            //maingrid.BorderThickness = new Thickness(1);
            //maingrid.BorderBrush = new SolidColorBrush(Colors.Blue);
            //maingrid.RenderTransform = tt;
            //maingrid.Width = 140;
            //maingrid.Height = 113;

            RowDefinitionCollection rdc = maingrid.RowDefinitions;
            rdc.Add(new RowDefinition());
            rdc.Add(new RowDefinition());

            //INNER GRIDS
            ColumnDefinitionCollection cdc;
            ColumnDefinition colDef;

            //TOP GRID
            topgrid = new Grid();            
            cdc = topgrid.ColumnDefinitions;
            colDef = new ColumnDefinition();
            colDef.Width = GridLength.Auto;
            cdc.Add(colDef);

            colDef = new ColumnDefinition();
            colDef.Width = new GridLength(1, GridUnitType.Star);
            cdc.Add(colDef);

            imgFloor = new Image();
            imgFloor.Source = new BitmapImage(new Uri(UIConsts.ImgUICZFLoorHeat));
            //imgFloor.Stretch = Stretch.None;
            imgFloor.Height = 26;
            imgFloor.Width = 27;
            imgFloor.Margin = new Thickness(3);
            topgrid.Children.Add(imgFloor);
            Grid.SetColumn(imgFloor, 0);

            panel = new StackPanel();
            panel.Orientation = Orientation.Vertical;
            panel.HorizontalAlignment = HorizontalAlignment.Center;
            panel.VerticalAlignment = VerticalAlignment.Center;

            txtRoomTemp = new TextBlock();
            txtRoomTemp.FontSize = FontSize;
            txtRoomTemp.Text = "--.-" + degree;
            panel.Children.Add(txtRoomTemp);

            txtFloorTemp = new TextBlock();
            txtFloorTemp.FontSize = FontSize;
            txtFloorTemp.Text = "--.-" + degree;
            panel.Children.Add(txtFloorTemp);

            topgrid.Children.Add(panel);
            Grid.SetColumn(panel, 1);

            maingrid.Children.Add(topgrid);
            Grid.SetRow(topgrid, 0);


            //BOTTOM GRID
            btmgrid = new Grid();
            btmgrid.BorderThickness = new Thickness(0, 1, 0, 0);
            btmgrid.BorderBrush = new SolidColorBrush(Colors.Blue);
            cdc = btmgrid.ColumnDefinitions;
            cdc.Add(new ColumnDefinition());
            cdc.Add(new ColumnDefinition());
            cdc[0].Width = new GridLength(1, GridUnitType.Star);
            cdc[1].Width = new GridLength(1, GridUnitType.Star);
            maingrid.Children.Add(btmgrid);
            Grid.SetRow(btmgrid, 1);

            imgSettings = new Image();
            imgSettings.Source = new BitmapImage(new Uri(UIConsts.ImgUICZBtnSettings));
            imgSettings.Width = 24;
            imgSettings.Height = 22;
            imgSettings.Margin = new Thickness(3);
            imgSettings.Tapped += Settings_Tapped;
            btmgrid.Children.Add(imgSettings);
            Grid.SetColumn(imgSettings, 0);

            imgGraph = new Image();
            imgGraph.Source = new BitmapImage(new Uri(UIConsts.ImgUICZBtnGraph));
            imgGraph.Width = 24;
            imgGraph.Height = 21;
            imgGraph.Margin = new Thickness(3);
            imgGraph.Tapped += Graph_Tapped;
            btmgrid.Children.Add(imgGraph);
            Grid.SetColumn(imgGraph, 1);

            border.Child = maingrid;

            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemReady += OnNSUSystemReadyHandler;
            sys.OnNSUSystemUnavailable += OnNSUSystemUnavailableHandler;

        }

        private async void Graph_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            NSUSys sys = NSUSys.Instance;
            if (sys.NSUNetwork.NetworkAvailable && cz != null)
            {
                SensorChartDialog scd = new SensorChartDialog();
                scd.mainGrid.MinHeight = Window.Current.Bounds.Height * 0.6;
                scd.MaxWidth = Window.Current.Bounds.Width - 60;
                scd.MinWidth = Window.Current.Bounds.Width - 60;
                scd.Title = Shared.NSUUtils.Utils.FirstLetterToUpper(cz.Name.Replace('_', ' '));
                //TODO Navigate to url with Room and Floor sensors graph
                scd.web.Navigate(new Uri($"{NSUConsts.MobileURL}/sensor_chart.php?tsname={cz.RoomSensorName}"));

                await scd.ShowAsync();
                scd = null;
            }
        }

        private void Settings_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            
        }

        private void OnNSUSystemReadyHandler(object sender, EventArgs e)
        {
            NSULog.Debug(LogTag, $"OnNSUSystemReadyHandler(). UIID: {UIID}. Attaching ComfortZone.");
            if (!string.IsNullOrWhiteSpace(czName))
            {
                NSUSys s = NSUSys.Instance;
                ComfortZones czs = s.GetNSUSysPart(PartsTypes.ComfortZones) as ComfortZones;
                if (czs != null)
                {
                    cz = czs.FindComfortZone(czName);
                    if(cz != null)
                    {
                        cz.OnFloorTemperatureChanged += OnFloorTemperatureChanged;
                        cz.OnRoomTemperatureChanged += OnRoomTemperatureChanged;
                        cz.OnActuatorOpenedChanged += OnActuatorOpenedChanged;
                        OnRoomTemperatureChanged(cz, new TempChangedEventArgs(cz.CurrentRoomTemperature));
                        OnFloorTemperatureChanged(cz, new TempChangedEventArgs(cz.CurrentFloorTemperature));
                        OnActuatorOpenedChanged(cz, new ActuatorOpenedEventArgs(cz.ActuatorOpened));

                        if (!string.IsNullOrEmpty(cz.CollectorName))
                        {
                            var cls = s.GetNSUSysPart(PartsTypes.Collectors) as Collectors;
                            var cl = cls.FindCollector(cz.CollectorName);
                            if (cl != null)
                            {
                                var cpumps = s.GetNSUSysPart(PartsTypes.CircPumps) as CircPumps;
                                var cp = cpumps.FindCircPump(cl.CircPumpName);
                                if (cp != null)
                                {
                                    cp.OnStatusChanged += OnCircPumpStatusChangedHandler;
                                    OnCircPumpStatusChangedHandler(cp, new StatusChangedEventArgs(cp.Status));
                                }
                            }
                        }
                    }
                }
            }
        }

        private async void OnNSUSystemUnavailableHandler(object sender, EventArgs e)
        {
            await maingrid.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                NSULog.Debug(LogTag, $"OnNSUSystemUnavailableHandler(). UIID: {UIID}");
                if(!txtRoomTemp.Text.StartsWith("!"))
                {
                    txtRoomTemp.Text = "!" + txtRoomTemp.Text;
                    txtFloorTemp.Text = "!" + txtFloorTemp.Text;
                    imgFloor.Source = null;
                    imgFloor.Source = new BitmapImage(new Uri(UIConsts.ImgUICZFLoorHeatBlack));
                }
            });
        }

        public void Free()
        {
            NSUSys s = NSUSys.Instance;
            s.OnNSUSystemReady -= OnNSUSystemReadyHandler;
            s.OnNSUSystemUnavailable -= OnNSUSystemUnavailableHandler;

            tt = null;
            panel = null;
            txtRoomTemp = null;
            txtFloorTemp = null;
            imgFloor = null;
            imgSettings = null;
            imgGraph = null;
            maingrid = null;
            topgrid = null;
            btmgrid = null;
        }

        private void OnActuatorOpenedChanged(object sender, ActuatorOpenedEventArgs e)
        {
            valveOpened = e.Opened;
            UpdateFloorHeatImage();
        }

        private async void OnRoomTemperatureChanged(object sender, TempChangedEventArgs e)
        {
            await maingrid.Dispatcher.RunAsync( CoreDispatcherPriority.Normal, () => {
                if (e.Temperature != -127)
                {
                    txtRoomTemp.Text = $"{e.Temperature:0.0}{degree}";
                    //TODO Reikia LowTempMode kiekvienai CZ, ir tikrinti, pagal ka nustatyt warningus
                    if (e.Temperature < ((sender as ComfortZone).RoomTempHi - (sender as ComfortZone).Histeresis * 2))
                    {
                        txtRoomTemp.Foreground = new SolidColorBrush(Colors.Blue);
                    }
                    else if (e.Temperature > ((sender as ComfortZone).RoomTempHi + (sender as ComfortZone).Histeresis * 2))
                    {
                        txtRoomTemp.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        txtRoomTemp.Foreground = new SolidColorBrush(Colors.Green);
                    }
                }
                else
                {
                    txtRoomTemp.Text = "-.- °C";
                    txtRoomTemp.Foreground = new SolidColorBrush(Colors.Orange);
                }
            });
        }

        private async void OnFloorTemperatureChanged(object sender, TempChangedEventArgs e)
        {
            await maingrid.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                if (e.Temperature != -127)
                {
                    txtFloorTemp.Text = $"{e.Temperature:0.0}{degree}";
                    //TODO Reikia LowTempMode kiekvienai CZ, ir tikrinti, pagal ka nustatyt warningus
                    if (e.Temperature < ((sender as ComfortZone).RoomTempHi - (sender as ComfortZone).Histeresis * 2))
                    {
                        txtFloorTemp.Foreground = new SolidColorBrush(Colors.Blue);
                    }
                    else if (e.Temperature > ((sender as ComfortZone).RoomTempHi + (sender as ComfortZone).Histeresis * 2))
                    {
                        txtFloorTemp.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        txtFloorTemp.Foreground = new SolidColorBrush(Colors.Green);
                    }
                }
                else
                {
                    txtFloorTemp.Text = "-.- °C";
                    txtFloorTemp.Foreground = new SolidColorBrush(Colors.Orange);
                }
            });
        }

        private void OnCircPumpStatusChangedHandler(object sender, StatusChangedEventArgs e)
        {
            cpOn = true;
            if (e.Status == Shared.NSUSystemPart.Status.DISABLED ||
                e.Status == Shared.NSUSystemPart.Status.DISABLED_OFF ||
                e.Status == Shared.NSUSystemPart.Status.OFF)
            {
                cpOn = false;
            }
            UpdateFloorHeatImage();
        }

        private void UpdateFloorHeatImage()
        {
            imgFloor.Source = null;
            if (valveOpened && cpOn)
            {                
                imgFloor.Source = new BitmapImage(new Uri(UIConsts.ImgUICZFLoorHeatRed));
            }
            else if (valveOpened && !cpOn)
            {
                imgFloor.Source = new BitmapImage(new Uri(UIConsts.ImgUICZFLoorHeatOrange));
            }
            else
                imgFloor.Source = new BitmapImage(new Uri(UIConsts.ImgUICZFLoorHeatBlue));
        }

        public string UIID { get; set; }
        public int Width { get { return (int)maingrid.Width; } set { maingrid.Width = value; } }
        public int Height { get { return (int)maingrid.Height; } set { maingrid.Height = value; } }
        public int Left { get { return (int)tt.X; } set { tt.X = value; } }
        public int Top { get { return (int)tt.Y; } set { tt.Y = value; } }

        public void AttachComfortZone(string name)
        {
            czName = name;
            if (NSUSys.Instance.NSUSystemReady)
                OnNSUSystemReadyHandler(this, null);
        }

        public void AttachedToWindow()
        {
        }

        public void DeatachedFromWindow()
        {
        }

    }
}
