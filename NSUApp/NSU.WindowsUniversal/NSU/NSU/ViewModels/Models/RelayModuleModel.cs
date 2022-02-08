using NSU.Shared.NSUSystemPart;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NSU.NSU_UWP.ViewModels.Models
{
    public class RelayModuleModel : ModelBase
    {
        private RelayModule rm;
        private bool enabled = false;
        private bool activelow = false;
        private bool inverted = false;

        public int Index { get; set; }

        public bool? Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != (bool)value)
                {
                    enabled = (bool)value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool? ActiveLow
        {
            get { return activelow; }
            set
            {
                if (activelow != value)
                {
                    activelow = (bool)value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool? LeftToRight
        {
            get { return inverted; }
            set
            {
                if (inverted != value)
                {
                    inverted = (bool)value;
                    RaisePropertyChanged();
                }
            }
        }

        public RelayModuleModel(RelayModule relaymodule)
        {
            rm = relaymodule;
            if(rm != null)
            {
                enabled = rm.Enabled;
                activelow = rm.ActiveLow;
                inverted = rm.Inverted;
            }
        }

    }
}
