using NSU.NSU_UWP.ViewModels;
using NSU.NSU_UWP.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class ComfortZonesPage : Page
    {
        public ComfortZonesViewModel ComfortZonesData { get; private set; }
        public ComfortZonesPage()
        {
            System.Diagnostics.Debug.WriteLine($"Creating {this.GetType().ToString()}");
            this.InitializeComponent();
            System.Diagnostics.Debug.WriteLine($"{this.GetType().ToString()} Components initialized. Getting data from DataCollections.");
            ComfortZonesData = DataCollections.Current.ComfortZonesVM;
            this.DataContext = ComfortZonesData;
        }

    }
}
