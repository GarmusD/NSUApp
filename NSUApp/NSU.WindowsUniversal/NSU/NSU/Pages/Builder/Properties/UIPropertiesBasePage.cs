using Windows.UI.Xaml.Controls;
using NSU.Shared.NSUUI;

namespace NSU.NSU_UWP.NSUSysItems.Properties
{
    public class UIPropertiesBasePage : Page
    {
        public delegate void ScenarioObjectAdded();
        public event ScenarioObjectAdded OnScenarioObjectAdded;

        protected Scenario.Builder.ScenarioObject scObj;

        public UIPropertiesBasePage()
        { }

        public void SetScenarioObject(Scenario.Builder.ScenarioObject obj)
        {
            scObj = obj;
            OnScenarioObjectAdded?.Invoke();
        }
    }
}
