using NSU.NSU_UWP.ViewModels;
using NSU.NSU_UWP.ViewModels.Models;
using NSU.NSUSystem;
using NSU.Shared.NSUSystemPart;
using NSUAppShared.NSUSystemParts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace NSU.NSU_UWP.Pages.NSUSysItems
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TSensorsPage : Page
    {
        public TSensorsViewModel TSensorsData { get; private set; }
        
        public TSensorsPage()
        {
            System.Diagnostics.Debug.WriteLine($"Creating {this.GetType().ToString()}");
            this.InitializeComponent();
            System.Diagnostics.Debug.WriteLine($"{this.GetType().ToString()} Components initialized. Getting data from DataCollections.");
            TSensorsData = DataCollections.Current.TSensorsVM;
        }

        private void MenuFlyoutItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Sender type: " + sender.GetType().ToString() + ". e.OriginalSource: " + e.OriginalSource.GetType().ToString());
            if(sender is MenuFlyoutItem)
            {
                var fitem = sender as MenuFlyoutItem;
                if (fitem.DataContext != null && fitem.DataContext is TSensorModel)
                {
                    var tsvm = fitem.DataContext as TSensorModel;
                    var dataPackage = new DataPackage();
                    dataPackage.SetText(tsvm.Address);
                    Clipboard.SetContent(dataPackage);
                }
            }
        }

        private void TextBlock_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var s = (FrameworkElement)sender;

            if (s != null)
            {
                FlyoutBase f = FlyoutBase.GetAttachedFlyout(s);
                if (f != null)
                {
                    f.ShowAt(s);
                }
                else { System.Diagnostics.Debug.WriteLine("No f value"); }
            }
            else { System.Diagnostics.Debug.WriteLine("No s value"); }
        }
    }
}
