using NSU.Shared.NSUSystemPart;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSU.NSU_UWP.ViewModels.Models
{
    public class KTypeModel : ModelBase
    {        
        public int Index { get; set; }

        public bool? Enabled
        {
            get { return enabled; }
            set
            {
                if(value != enabled)
                {
                    enabled = (bool)value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if(name != value)
                {
                    name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int Interval
        {
            get { return interval; }
            set
            {
                if(interval != value)
                {
                    interval = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool enabled = false;
        private string name = string.Empty;
        private int interval = 0;

        private KType ktype;

        public KTypeModel(KType kType)
        {
            ktype = kType;
            if(ktype != null)
            {
                enabled = ktype.Enabled;
                name = ktype.Name;
                interval = ktype.Interval;
            }
        }
    }
}
