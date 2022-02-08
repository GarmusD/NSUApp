using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSU.NSU_UWP.ViewModels.Helpers
{
    public class NameClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get { return name; }
            set
            {
                if(name != value)
                {
                    name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }

        public bool FakeName { get; set; }

        private string name;

        public NameClass(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
