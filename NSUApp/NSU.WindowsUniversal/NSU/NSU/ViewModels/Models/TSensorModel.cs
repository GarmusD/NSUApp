using NSU.Shared.NSUSystemPart;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NSU.NSU_UWP.ViewModels.Models
{
    public class TSensorModel : ModelBase
    {
        private TempSensor ts;

        public TSensorModel(TempSensor tempSensor)
        {
            ts = tempSensor;
            if(ts != null)
            {
                enabled = ts.Enabled;
                cfgpos = ts.ConfigPos;
                Address = TempSensor.AddrToString(ts.SensorID);
                fName = ts.Name;
                interval = ts.Interval;
                notfound = ts.NotFound;
                ReadErrors = ts.ReadErrors;
            }
        }

        public TSensorModel()
        {
        }

        public int Index { get; set; }

        public int ConfigPos
        {
            get { return cfgpos; }
            set {  }
        }private int cfgpos;

        public bool? Enabled
        {
            get { return enabled; }
            set {  }
        }private bool enabled;

        public string Address { get; set; }

        public string Name
        {
            get { return fName; }
            set
            {
                if(fName != value)
                {
                    RaisePropertyChanging(fName, value);
                    fName = value;
                    RaisePropertyChanged();
                }
            }
        }private string fName;

        public int Interval
        {
            get { return interval; }
            set
            {
                if(interval != value)
                {
                    interval = value;
                    RaisePropertyChanged();
                    System.Diagnostics.Debug.WriteLine($"New value: '{interval}'.");
                }
            }
        }private int interval;

        public bool? NotFound
        {
            get { return notfound; }
            set {  }
        }private bool notfound;

        public int ReadErrors
        { get; set; }
    }
}
