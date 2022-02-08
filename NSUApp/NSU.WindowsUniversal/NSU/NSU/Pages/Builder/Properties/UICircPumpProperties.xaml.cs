using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using NSU.Shared.NSUUI;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NSU.NSU_UWP.NSUSysItems.Properties
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UICircPumpProperties : UIPropertiesBasePage
    {
        bool propchanging = false;

        public UICircPumpProperties()
        {
            this.InitializeComponent();
            this.OnScenarioObjectAdded += UICircPumpProperties_OnScenarioObjectAdded;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if(scObj != null)
            {
                scObj.PropertyChanged -= scObj_PropertyChanged;
            }
            base.OnNavigatedFrom(e);
        }

        void UICircPumpProperties_OnScenarioObjectAdded()
        {
            if (scObj != null)
            {
                UIName.Text = scObj.Name;
                UICircPumpName.SelectedValue = scObj.ControlObjectName;
                UIPosLeft.Text = scObj.Left.ToString();
                UIPosTop.Text = scObj.Top.ToString();
                UIRotation.SelectedIndex = scObj.Rotation;

                UIPosLeft.Text = scObj.Left.ToString();
                UIPosTop.Text = scObj.Top.ToString();
                scObj.PropertyChanged += scObj_PropertyChanged;
            }
        }

        void scObj_PropertyChanged(string property, string value)
        {
            if(!propchanging)
            {
                if(property.Equals(Scenario.Builder.ScenarioObject.PropChangeLeft))
                {
                    UIPosLeft.Text = value;
                }
                else if(property.Equals(Scenario.Builder.ScenarioObject.PropChangeTop))
                {
                    UIPosTop.Text = value;
                }
                
            }
        }

        private void UIName_TextChanged(object sender, TextChangedEventArgs e)
        {
            propchanging = true;
            if (scObj != null)
            {
                scObj.Name = UIName.Text;
            }
            propchanging = false;
        }

        private void UICircPumpName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            propchanging = true;
            if (scObj != null)
            {
                scObj.ControlObjectName = (string)UICircPumpName.SelectedValue;
            }
            propchanging = false;
        }

        private void UIRotation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            propchanging = true;
            if (scObj != null && UIRotation.SelectedIndex > -1)
            {
                scObj.Rotation = UIRotation.SelectedIndex;
            }
            propchanging = false;
        }
    }
}
