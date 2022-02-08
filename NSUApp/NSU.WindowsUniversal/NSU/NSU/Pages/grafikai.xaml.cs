using System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using NSU.NSUSystem;
using System.Collections.Generic;
using NSUAppShared.NSUSystemParts;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace NSU.NSU_UWP.Pages
{
    public sealed partial class PageGrafikai : Page
    {
        readonly string logTag = "NSUPlotFragment";
        private int period = 1;
        private string myurl = string.Empty;
        string tsname = string.Empty;

        private NSUSys nsusys = null;

        public PageGrafikai()
        {
            this.InitializeComponent();

            this.Loaded += Grafikai_Loaded;

            if (nsusys == null)
            {
                nsusys = NSUSys.Instance;
            }
            if (!nsusys.NSUSystemReady)
            {
                nsusys.OnNSUSystemReady += OnNSUPartsReady;
            }
        }

        private void OnNSUPartsReady(object sender, System.EventArgs e)
        {
            var tsensors = nsusys.GetNSUSysPart( PartsTypes.TSensors) as TempSensors;
            PopulateListTSensorNames(tsensors.GetTSensorsNames());
        }

        private void Grafikai_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (nsusys.NSUSystemReady)
            {
                this.webView.Source = new Uri(BuildURL());
            }
        }

        string BuildURL()
        {
            return "http://nsu.dgs.lt/mobile/mobile.php" + $"?data={DateTime.Now.ToString("yyyy-MM-dd")}&period={period}&tsname={tsname}";
        }

        void UpdateWebView()
        {

        }

        async void PopulateListTSensorNames(List<string> list)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => 
            {
                //Log.Debug(logTag, "Populating TSensor names list.");
                list.Sort();
                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = list[i].Replace('_', ' ');
                }
                TSensorListBox.ItemsSource = null;
                TSensorListBox.ItemsSource = list;


                if (TSensorListBox.Items.Count > 0)
                {
                    if (!String.IsNullOrEmpty(tsname))
                    {
                        TSensorListBox.SelectedItem = tsname;
                    }
                    else
                    {
                        tsname = (TSensorListBox.Items[0] as string).Replace(' ', '_');
                        TSensorListBox.SelectedIndex = 0;
                        webView.Source = new Uri(BuildURL());
                    }
                }
            });
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (Int32.TryParse(btn.Tag.ToString(), out period))
            {
                webView.Source = new Uri(BuildURL());
            }
            else
            {
                webView.Refresh();
            }
        }

        private void TSensorListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TSensorListBox.Items != null && TSensorListBox.Items.Count > 0)
            {
                tsname = (TSensorListBox.SelectedItem as string).Replace(' ', '_');
                webView.Source = new Uri(BuildURL());
            }
        }
    }
}
