using NSU.Shared.NSUUI;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NSU.NSU_UWP.NSUSysItems.Properties
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UIBtnProperties : UIPropertiesBasePage
    {
        private bool propchanging = false;
        private List<string> ButtonActions;

        public UIBtnProperties()
        {
            this.InitializeComponent();
            this.OnScenarioObjectAdded += UIBtnProperties_OnScenarioObjectAdded;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (scObj != null)
            {
                scObj.PropertyChanged -= scObj_PropertyChanged;
            }
            base.OnNavigatedFrom(e);
        }

        void UIBtnProperties_OnScenarioObjectAdded()
        {
            if(scObj != null)
            {
                UIName.Text = scObj.Name;
                UIPosLeft.Text = scObj.Left.ToString();
                UIPosTop.Text = scObj.Top.ToString();

                scObj.PropertyChanged += scObj_PropertyChanged;
            }
        }

        void scObj_PropertyChanged(string property, string value)
        {
            if (!propchanging)
            {
                if (property.Equals(Scenario.Builder.ScenarioObject.PropChangeLeft))
                {
                    UIPosLeft.Text = value;
                }
                else if (property.Equals(Scenario.Builder.ScenarioObject.PropChangeTop))
                {
                    UIPosTop.Text = value;
                }

            }
        }

        private void UIName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(scObj != null)
            {
                scObj.Text = UIName.Text;
            }
        }
    }
}
