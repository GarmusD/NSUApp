using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using NSU.Shared.NSUUI;
using NSU.NSU_UWP.NSUSysItems.Properties;
using NSU.NSU_UWP.UI.Elements;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NSU.NSU_UWP.Pages.Builder
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScenarioBuilderPage : Page
    {
        Frame propFrame;

        NSU.Shared.NSUUI.Scenario.Builder builder;
        NSU.Shared.NSUUI.Scenario.Builder.ScenarioObject scobj;
        //ItemCollection uiitems;
        UIElement manipobj;

        public ScenarioBuilderPage()
        {
            this.InitializeComponent();
            builder = new NSU.Shared.NSUUI.Scenario.Builder();
            UIList.ItemsSource = builder.UIObjectList;
            propFrame = new Frame();
            propFrame.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            LayoutRoot.Children.Add(propFrame);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UIItemContainer.SelectedIndex = 0;
        }


        private void mainCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("mainCanvas_Tapped");
            Image img = null;

            if (UIItemContainer.SelectedIndex > -1)
            {
                img = UIItemContainer.SelectedItem as Image;
                UIItemContainer.SelectedIndex = 0;
            }
            if (img != null)
            {
                string tag = img.Tag as string;
                NSU.Shared.NSUUI.Scenario.Builder.ScenarioObject obj = null;
                switch (tag)
                {
                    case "pointer":
                        break;
                    case "button":
                        obj = builder.NewObject(NSUUIClass.Button);
                        UIButton btn = new UIButton();
                        mainCanvas.Children.Add(btn.uiElement);
                        obj.Object = btn;
                        obj.UIObject = btn.uiElement;
                        btn.Left = (int)e.GetPosition(mainCanvas).X;
                        btn.Top = (int)e.GetPosition(mainCanvas).Y;
                        obj.Left = (int)e.GetPosition(mainCanvas).X;
                        obj.Top = (int)e.GetPosition(mainCanvas).Y;
                        AddUIHandlers(btn.uiElement);                                                
                        break;
                    case "label":
                        obj = builder.NewObject(NSUUIClass.Label);
                        UILabel lbl = new UILabel();
                        mainCanvas.Children.Add(lbl.uiElement);
                        obj.Object = lbl;
                        obj.UIObject = lbl.uiElement;
                        lbl.Left = (int)e.GetPosition(mainCanvas).X;
                        lbl.Top = (int)e.GetPosition(mainCanvas).Y;
                        obj.Left = (int)e.GetPosition(mainCanvas).X;
                        obj.Top = (int)e.GetPosition(mainCanvas).Y;
                        AddUIHandlers(lbl.uiElement);
                        break;//
                    case "circpump":
                        obj = builder.NewObject(NSUUIClass.CircPump);
                        UICircPump circpump = new UICircPump();
                        mainCanvas.Children.Add(circpump.uiElement);
                        obj.Object = circpump;
                        obj.UIObject = circpump.uiElement;
                        circpump.Left = (int)e.GetPosition(mainCanvas).X;
                        circpump.Top = (int)e.GetPosition(mainCanvas).Y;
                        obj.Left = (int)e.GetPosition(mainCanvas).X;
                        obj.Top = (int)e.GetPosition(mainCanvas).Y;
                        AddUIHandlers(circpump.uiElement);
                        break;
                }
                if (obj != null)
                {
                    UIList.ItemsSource = null;
                    UIList.ItemsSource = builder.UIObjectList;
                    UIList.SelectedIndex = UIList.Items.Count - 1;
                }
            }
        }

        void AddUIHandlers(UIElement elem)
        {
            elem.ManipulationStarted += ManipulationStarted;
            elem.ManipulationMode = ManipulationModes.All;
            elem.ManipulationDelta += ManipulationDelta;
            elem.Tapped += Tapped;
        }

        new void ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ManipulationStarted");
            UIList.SelectedIndex = builder.FindIndexByUIObject(sender);            
        }

        new void ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {

        }

        private new void ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            manipobj = sender as UIElement;
            //TranslateTransform translateTransform = dragableItem.RenderTransform as TranslateTransform;

            var translate = (TranslateTransform)manipobj.RenderTransform;

            //translateTransform.X += e.Delta.Translation.X;
            //translateTransform.Y += e.Delta.Translation.Y;

            var newPosX = Canvas.GetLeft(manipobj) + translate.X + e.Delta.Translation.X;
            var newPosY = Canvas.GetTop(manipobj) + translate.Y + e.Delta.Translation.Y;

            if (!isBoundary(newPosX, mainCanvas.ActualWidth - manipobj.RenderSize.Width, 0))
                translate.X += e.Delta.Translation.X;
            if (!isBoundary(newPosY, mainCanvas.ActualHeight - manipobj.RenderSize.Height, 0))
                translate.Y += e.Delta.Translation.Y;
            if(scobj != null)
            {
                scobj.Left = (int)translate.X;
                scobj.Top = (int)translate.Y;
            }
        }

        
        private new void Tapped(object sender, TappedRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Object Tapped");
            UIList.SelectedIndex = builder.FindIndexByUIObject(sender);
        }

        bool isBoundary(double value, double max, double min)
        {
            return value > max ? true : value < min ? true : false;
        }

        private void UIList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(UIList.SelectedIndex != -1)
            {
                scobj = (NSU.Shared.NSUUI.Scenario.Builder.ScenarioObject)UIList.SelectedItem;
                if (scobj != null)
                {
                    switch(scobj.UIClass)
                    {
                        case NSUUIClass.None:
                            break;
                        case NSUUIClass.Button:
                            LoadPropertiesFrame(typeof(UIBtnProperties), scobj);
                            break;
                        case NSUUIClass.CircPump:
                            LoadPropertiesFrame(typeof(UICircPumpProperties), scobj);
                            break;
                        case NSUUIClass.Graphics:
                            break;
                        case NSUUIClass.Input:
                            break;
                        case NSUUIClass.Label:
                            LoadPropertiesFrame(typeof(UILabelProperties), scobj);
                            break;
                        case NSUUIClass.Ladomat:
                            break;
                        case NSUUIClass.SwitchButton:
                            break;
                        case NSUUIClass.Ventilator:
                            break;
                    }
                }
            }
        }

        private void LoadPropertiesFrame(Type type, NSU.Shared.NSUUI.Scenario.Builder.ScenarioObject scObject)
        {
            PropertiesFrame.Navigate(type);
            UIPropertiesBasePage basepage = PropertiesFrame.Content as UIPropertiesBasePage;
            basepage.SetScenarioObject(scObject);
        }

        
    }

}
