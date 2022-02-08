using NSU.NSUSystem;
using NSU.Shared.NSUUI;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using NSU.Shared.NSUSystemPart;
using System;
using NSUAppShared.NSUSystemParts;
using Windows.UI.Xaml;
using NSU.Shared;
using NSU.NSU_UWP.Dialogs;
using NSUAppShared;

namespace NSU.NSU_UWP.UI.Elements
{
    class UITempLabel : INSUUITempLabel
    {
        const string LogTag = "UITempLabel";
        const double FontSize = 19;
        readonly string degree = " °C";

        //Canvas canvas;
        TextBlock label;
        Border border;
        TranslateTransform tt;
        TempSensor tsi = null;
        string tsname = string.Empty, ktpname = string.Empty;

        public UITempLabel(/*Canvas cnv*/)
        {
            //canvas = cnv;
            label = new TextBlock();
            //UIClass = NSUUIClass.TempLabel;
            UIID = string.Empty;
            label.Text = "0.0" + degree;
            label.Foreground = new SolidColorBrush(Colors.Blue);
            label.FontSize = FontSize;
            label.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
            label.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
            tt = new TranslateTransform();
            tt.X = 0;
            tt.Y = 0;

            border = new Border();
            border.Width = stringWidth("-99.9" + degree, FontSize) + 8;
            border.BorderThickness = new Windows.UI.Xaml.Thickness(1);
            border.BorderBrush = new SolidColorBrush(Colors.Blue);
            border.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
            border.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
            border.RenderTransform = tt;
            border.Child = label;

            label.Tapped += Label_Tapped;

            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemReady += OnNSUSystemReadyHandler;
            sys.OnNSUSystemUnavailable += OnNSUSystemUnavailableHandler;
        }

        private async void Label_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            NSUSys sys = NSUSys.Instance;
            if(sys.NSUNetwork.NetworkAvailable)
            {
                SensorChartDialog scd = new SensorChartDialog();
                scd.Loaded += Scd_Loaded;

                string sname = string.Empty;
                if(!string.IsNullOrEmpty(tsname))
                {
                    scd.Title = tsname;
                    sname = tsname;
                }
                else if(!string.IsNullOrEmpty(ktpname))
                {
                    scd.Title = ktpname;
                    sname = ktpname;
                }
                else
                {
                    scd = null;
                    return;
                }

                //scd.web.Width = scd.ActualWidth;//scd.MinWidth - 60;
                //scd.web.Height = scd.ActualHeight;//scd.MinHeight - 60;
                scd.web.Navigate(new Uri($"{NSUConsts.MobileURL}/sensor_chart.php?tsname={sname}"));

                await scd.ShowAsync();
                scd = null;
            }
        }

        private void Scd_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is SensorChartDialog)
            {
                SensorChartDialog scd = sender as SensorChartDialog;
                scd.mainGrid.MinHeight = Window.Current.Bounds.Height * 0.6;
                double w = Window.Current.Bounds.Width - 60;
                //scd.web.MinWidth = w;
                //scd.mainGrid.MinWidth = w;
                scd.MaxWidth = w;// Window.Current.Bounds.Width;
                scd.MinWidth = w;// Window.Current.Bounds.Width;
                scd.Width = w;// Window.Current.Bounds.Width;
            }
        }

        private void OnNSUSystemReadyHandler(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tsname))
            {
                NSULog.Debug(LogTag, $"OnNSUSystemReadyHandler(). UIID: {UIID}. Attaching TSensor.");
                NSUSys s = NSUSys.Instance;
                TempSensors ts = s.GetNSUSysPart(PartsTypes.TSensors) as TempSensors;
                tsi = ts.FindSensor(tsname);
                if (tsi != null)
                {
                    tsi.OnTempChanged += tsi_OnTempChange;
                    tsi_OnTempChange(tsi, new TempChangedEventArgs(tsi.Temperature));
                }
            }
            else if (!string.IsNullOrWhiteSpace(ktpname))
            {
                NSULog.Debug(LogTag, $"OnNSUSystemReadyHandler(). UIID: {UIID}. Attaching KType.");
                NSUSys s = NSUSys.Instance;
                KTypes ktps = s.GetNSUSysPart(PartsTypes.KTypes) as KTypes;
                var ktp = ktps.FindKType(ktpname);
                if (ktp != null)
                {
                    ktp.OnTempChanged += KType_OnTempChanged;
                    KType_OnTempChanged(ktp, new KTypeTempChangedEventArgs(ktp.Temperature));
                }
            }
        }

        private async void OnNSUSystemUnavailableHandler(object sender, EventArgs e)
        {
            await label.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                NSULog.Debug(LogTag, $"OnNSUSystemUnavailableHandler(). UIID: {UIID}");
                label.Text = "!"+label.Text;
            });
        }

        public UIElement uiElement { get { return border; } }

        private double stringWidth(string s, double fontSize)
        {
            if (s == " ")
                s = "\u00A0";  //this line wasn't required in silverlight but is now

            TextBlock t = new TextBlock()
            {
                FontSize = fontSize,
                Text = s
            };
            t.Measure(new Size(double.MaxValue, double.MaxValue));  //this line wasn't required in silverlight but is now
            return t.ActualWidth;
        }

        public int CenterX
        {
            set
            {
                Left = value - Width / 2;
            }
        }

        public void AttachTempSensor(string tsname)
        {
            this.tsname = tsname;
            if (NSUSys.Instance.NSUSystemReady)
                OnNSUSystemReadyHandler(null, null);
        }

        public void AttachKType(string name)
        {
            ktpname = name;
            if (NSUSys.Instance.NSUSystemReady)
                OnNSUSystemReadyHandler(null, null);
        }

        async void KType_OnTempChanged(object sender, KTypeTempChangedEventArgs e)
        {
            await label.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                NSULog.Debug(LogTag, $"OnNSUSystemReadyHandler(). UIID: {UIID}. Updating KType temp.");
                label.Text = $"{e.Temperature}{degree}";
            });
        }

        async void tsi_OnTempChange(object sender, TempChangedEventArgs e)
        {
            await label.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {
                 NSULog.Debug(LogTag, $"OnNSUSystemReadyHandler(). UIID: {UIID}. Updating temp.");
                 label.Text = $"{e.Temperature:0.0}{degree}";
             });

        }

        public void Free()
        {
            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemReady -= OnNSUSystemReadyHandler;
            sys.OnNSUSystemUnavailable -= OnNSUSystemUnavailableHandler;
            border = null;
            label = null;
        }

        public void AttachedToWindow()
        {
            
        }

        public void DeatachedFromWindow()
        {
            
        }

        public NSUUIClass UIClass { get { return NSUUIClass.TempLabel; } }

        public string UIID { get; set; }

        public int Width
        {
            get
            {
                return (int)border.Width;
            }
            set
            {
                border.Width = value;
            }
        }

        public int Height
        {
            get
            {
                return (int)border.Height;
            }
            set
            {
                border.Height = value;
            }
        }

        public int Left
        {
            get
            {
                return (int)Canvas.GetLeft(border);
            }
            set
            {
                Canvas.SetLeft(border, value);
            }
        }

        public int Top
        {
            get
            {
                return (int)Canvas.GetTop(border);
            }
            set
            {
                Canvas.SetTop(border, value);
            }
        }
    }
}
