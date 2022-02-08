using NSU.NSUSystem;
using NSU.Shared.NSUSystemPart;
using NSU.Shared.NSUUI;
using NSUAppShared;
using NSUAppShared.NSUSystemParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace NSU.NSU_UWP.UI.Elements
{
    public class UIWeatherInfo : INSUUIWeatherInfo
    {
        private readonly string degree = " °C";

        private Grid mainGrid, infoGrid;
        private WebView web;
        private TextBlock txtTitle, txtLTempTitle, txtPTempTitle, txtLTemp, txtPTemp;
        private TempSensor lsensor, psensor;
        private string lsName = string.Empty, psName = string.Empty;
        private TranslateTransform tt;

        public UIElement uiElement { get { return mainGrid; } }
        public NSUUIClass UIClass => NSUUIClass.WeatherInfo;

        public string UIID { get; set; }
        public int Width { get => (int)mainGrid.Width; set => mainGrid.Width = value; }
        public int Height { get => (int)mainGrid.Height; set => mainGrid.Height = value; }
        public int Left { get => (int)tt.X; set => tt.X = value; }
        public int Top { get => (int)tt.Y; set => tt.Y = value; }

        public UIWeatherInfo()
        {
            tt = new TranslateTransform();
            tt.X = 0;
            tt.Y = 0;

            //MAIN GRID
            mainGrid = new Grid();
            mainGrid.RenderTransform = tt;

            RowDefinition rd;
            var rDefs = mainGrid.RowDefinitions;
            //1 row
            rd = new RowDefinition();
            rd.Height = new GridLength(1, GridUnitType.Auto);
            rDefs.Add(rd);
            //2 row
            rd = new RowDefinition();
            rd.Height = new GridLength(1, GridUnitType.Star);
            rDefs.Add(rd);

            //INFO GRID
            infoGrid = new Grid();
            rDefs = infoGrid.RowDefinitions;
            //1 row
            rd = new RowDefinition();
            rDefs.Add(rd);
            //2 row
            rd = new RowDefinition();
            rDefs.Add(rd);
            //3 row
            rd = new RowDefinition();
            rDefs.Add(rd);

            ColumnDefinition cd;
            var cDefs = infoGrid.ColumnDefinitions;
            //1 column
            cd = new ColumnDefinition();
            cDefs.Add(cd);
            //2 column
            cd = new ColumnDefinition();
            cDefs.Add(cd);

            txtTitle = new TextBlock();
            txtTitle.Text = "Temperatūros";
            txtTitle.FontSize = 14;
            txtTitle.FontWeight = Windows.UI.Text.FontWeights.Bold;
            txtTitle.Foreground = new SolidColorBrush(Colors.Blue);
            txtTitle.HorizontalAlignment = HorizontalAlignment.Center;
            infoGrid.Children.Add(txtTitle);
            Grid.SetRow(txtTitle, 0);
            Grid.SetColumn(txtTitle, 0);
            Grid.SetColumnSpan(txtTitle, 2);

            txtLTempTitle = new TextBlock();
            txtLTempTitle.Text = "Lauke: ";
            txtLTempTitle.FontSize = 12;
            txtLTempTitle.Foreground = new SolidColorBrush(Colors.Blue);
            txtLTempTitle.HorizontalAlignment = HorizontalAlignment.Right;
            infoGrid.Children.Add(txtLTempTitle);
            Grid.SetColumn(txtLTempTitle, 0);
            Grid.SetRow(txtLTempTitle, 1);

            txtPTempTitle = new TextBlock();
            txtPTempTitle.Text = "Požeminio oro: ";
            txtPTempTitle.FontSize = 12;
            txtPTempTitle.Foreground = new SolidColorBrush(Colors.Blue);
            txtPTempTitle.HorizontalAlignment = HorizontalAlignment.Right;
            infoGrid.Children.Add(txtPTempTitle);
            Grid.SetColumn(txtPTempTitle, 0);
            Grid.SetRow(txtPTempTitle, 2);

            txtLTemp = new TextBlock();
            txtLTemp.Text = "--.-" + degree;
            txtLTemp.FontSize = 12;
            txtLTemp.Foreground = new SolidColorBrush(Colors.Blue);
            txtLTemp.HorizontalAlignment = HorizontalAlignment.Left;
            infoGrid.Children.Add(txtLTemp);
            Grid.SetColumn(txtLTemp, 1);
            Grid.SetRow(txtLTemp, 1);

            txtPTemp = new TextBlock();
            txtPTemp.Text = "--.-" + degree;
            txtPTemp.FontSize = 12;
            txtPTemp.Foreground = new SolidColorBrush(Colors.Blue);
            txtPTemp.HorizontalAlignment = HorizontalAlignment.Left;
            infoGrid.Children.Add(txtPTemp);
            Grid.SetColumn(txtPTemp, 1);
            Grid.SetRow(txtPTemp, 2);

            mainGrid.Children.Add(infoGrid);
            Grid.SetRow(infoGrid, 0);

            web = new WebView();
            web.Margin = new Thickness(10);
            web.Navigate(new Uri($"{NSUConsts.MobileURL}/weather_info.php"));
            mainGrid.Children.Add(web);
            Grid.SetRow(web, 1);

            NSUSys s = NSUSys.Instance;
            s.OnNSUSystemReady += OnNSUSystemReady;
            s.OnNSUSystemUnavailable += OnNSUSystemUnavailable;
        }        

        private void OnNSUSystemReady(object sender, EventArgs e)
        {
            NSUSys s = NSUSys.Instance;
            var tss = s.GetNSUSysPart(NSUAppShared.NSUSystemParts.PartsTypes.TSensors) as TempSensors;
            if(!string.IsNullOrEmpty(lsName))
            {
                lsensor = tss.FindSensor(lsName);
                if(lsensor != null)
                {
                    lsensor.OnTempChanged += LaukoTempChanged;
                    LaukoTempChanged(lsensor, new TempChangedEventArgs(lsensor.Temperature));
                }
            }
            if(!string.IsNullOrEmpty(psName))
            {
                psensor = tss.FindSensor(psName);
                if(psensor != null)
                {
                    psensor.OnTempChanged += PozemioTempChanged;
                    PozemioTempChanged(psensor, new TempChangedEventArgs(psensor.Temperature));
                }
            }
        }

        private async void PozemioTempChanged(object sender, TempChangedEventArgs e)
        {
            await mainGrid.Dispatcher.RunAsync( Windows.UI.Core.CoreDispatcherPriority.Normal, ()=> 
            {
                txtPTemp.Text = $"{e.Temperature}{degree}";
            });
        }

        private async void LaukoTempChanged(object sender, TempChangedEventArgs e)
        {
            await mainGrid.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                txtLTemp.Text = $"{e.Temperature}{degree}";
            });
        }

        private void OnNSUSystemUnavailable(object sender, EventArgs e)
        {
            
        }

        public void AttachedToWindow()
        {
            
        }

        public void AttachLaukoSensor(string name)
        {
            lsName = name;
            NSUSys s = NSUSys.Instance;
            if (!string.IsNullOrEmpty(lsName) && s.NSUSystemReady)
            {                
                var tss = s.GetNSUSysPart(NSUAppShared.NSUSystemParts.PartsTypes.TSensors) as TempSensors;
                lsensor = tss.FindSensor(lsName);
                if (lsensor != null)
                {
                    lsensor.OnTempChanged += LaukoTempChanged;
                    LaukoTempChanged(lsensor, new TempChangedEventArgs(lsensor.Temperature));
                }
            }
        }

        public void AttachPozemioSensor(string name)
        {
            psName = name;
            NSUSys s = NSUSys.Instance;
            if (!string.IsNullOrEmpty(psName) && s.NSUSystemReady)
            {
                var tss = s.GetNSUSysPart(NSUAppShared.NSUSystemParts.PartsTypes.TSensors) as TempSensors;
                psensor = tss.FindSensor(psName);
                if (psensor != null)
                {
                    psensor.OnTempChanged += LaukoTempChanged;
                    LaukoTempChanged(psensor, new TempChangedEventArgs(psensor.Temperature));
                }
            }
        }

        public void DeatachedFromWindow()
        {
            
        }

        public void Free()
        {
            if (lsensor != null)
                lsensor.OnTempChanged -= LaukoTempChanged;
            if (psensor != null)
                psensor.OnTempChanged -= PozemioTempChanged;
            tt = null;
            txtTitle = null;
            txtLTempTitle = null;
            txtPTempTitle = null;
            txtLTemp = null;
            txtPTemp = null;
            web = null;
            infoGrid = null;
            mainGrid = null;

            NSUSys s = NSUSys.Instance;
            s.OnNSUSystemReady -= OnNSUSystemReady;
            s.OnNSUSystemUnavailable -= OnNSUSystemUnavailable;
        }
    }
}
