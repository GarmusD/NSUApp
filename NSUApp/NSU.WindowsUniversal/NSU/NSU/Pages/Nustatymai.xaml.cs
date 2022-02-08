// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
// PARTICULAR PURPOSE. 
// 
// Copyright (c) Microsoft Corporation. All rights reserved 

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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace NSU.NSU_UWP.Pages
{
    public class Scenario
    {
        public string Title { get; set; }

        public Type ClassType { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }

    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class PageNustatymai : Page
    {
        public event System.EventHandler ScenarioLoaded;

        List<Scenario> scenarios;
        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        //public ObservableDictionary DefaultViewModel
        //{
        //    get { return this.defaultViewModel; }
        //}


        public PageNustatymai()
        {
            this.InitializeComponent();

            //hiddenFrame = new Frame();
            //hiddenFrame.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //ContentRoot.Children.Add(hiddenFrame);

            scenarios = new List<Scenario>
            {
                new Scenario() { Title = "Temp. Sensoriai", ClassType = typeof(NSUSysItems.TSensorsPage) },
                new Scenario() { Title = "Relės", ClassType = typeof(NSUSysItems.RelayModulesPage) },
                new Scenario() { Title = "Temp. trigeriai", ClassType = typeof(NSUSysItems.TempTriggersPage) },
                new Scenario() { Title = "Circ. siurbliai", ClassType = typeof(NSUSysItems.CircPumpsPage) },
                new Scenario() { Title = "Kolektoriai", ClassType = typeof(NSUSysItems.CollectorsPage) },
                new Scenario() { Title = "Komforto zonos", ClassType = typeof(NSUSysItems.ComfortZonesPage) },
                new Scenario() { Title = "KType", ClassType = typeof(NSUSysItems.KTypePage) },
                new Scenario() { Title = "Karštas vanduo", ClassType = typeof(NSUSysItems.WaterBoilerPage) },
                new Scenario() { Title = "Katilas", ClassType = typeof(NSUSysItems.WoodBoilerPage) },
                new Scenario() { Title = "ScenarioBuilder", ClassType = typeof(Builder.ScenarioBuilderPage) },
                new Scenario() { Title = "BinUploader", ClassType = typeof(NSUSysItems.ConsolePage) }
            };

        }

        private void pageRootSettings_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateScenarios();
        }

        private void LoadScenario(Type scenario)
        {
            //hiddenFrame.Navigate(scenario, this);
            //Page hiddenPage = hiddenFrame.Content as Page;
            //UIElement uielem = hiddenPage.FindName("Output") as UIElement;
            //Panel panel = hiddenPage.FindName("LayoutRoot") as Panel;
            //panel.Children.Remove(uielem);
            contentFrame.Navigate(scenario);
        }

        private void PopulateScenarios()
        {
            System.Collections.ObjectModel.ObservableCollection<object> ScenarioList = new System.Collections.ObjectModel.ObservableCollection<object>();
            int i = 0;

            // Populate the ListBox with the list of scenarios as defined in Constants.cs.
            foreach (Scenario s in scenarios)
            {
                ListBoxItem item = new ListBoxItem();
                s.Title = (++i).ToString() + ") " + s.Title;
                item.Content = s;
                item.Name = s.ClassType.FullName;
                ScenarioList.Add(item);
            }

            // Bind the ListBox to the scenario list.
            Scenarios.ItemsSource = ScenarioList;

            // Starting scenario is the first or based upon a previous selection.
            int startingScenarioIndex = -1;

            //if (SuspensionManager.SessionState.ContainsKey("SelectedScenarioIndex"))
            //{
            //    int selectedScenarioIndex = Convert.ToInt32(SuspensionManager.SessionState["SelectedScenarioIndex"]);
            //    startingScenarioIndex = selectedScenarioIndex;
            //}

            Scenarios.SelectedIndex = startingScenarioIndex != -1 ? startingScenarioIndex : 0;
            Scenarios.ScrollIntoView(Scenarios.SelectedItem);
        }


        private void Scenarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Scenarios.SelectedItem != null)
            {

                ListBoxItem selectedListBoxItem = Scenarios.SelectedItem as ListBoxItem;

                //SuspensionManager.SessionState["SelectedScenarioIndex"] = Scenarios.SelectedIndex;

                Scenario scenario = selectedListBoxItem.Content as Scenario;
                LoadScenario(scenario.ClassType);
                //InvalidateSize();

                // Fire the ScenarioLoaded event since we know that everything is loaded now.
                ScenarioLoaded?.Invoke(this, new EventArgs());
            }
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            //
        }
    }
}
