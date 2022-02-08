using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NSU.NSU_UWP.ViewModels.Models
{
    public class PropertyChangingEventArgs
    {
        public string PropertyName { get; }
        public object OldValue { get; }
        public object NewValue { get; }

        public PropertyChangingEventArgs(string propertyName, object oldValue, object newValue)
        {
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public class ModelBase : INotifyPropertyChanged
    {
        public event EventHandler<PropertyChangingEventArgs> PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsModified { get; set; } = false;

        public void RaisePropertyChanged([CallerMemberName] string propertyChanged = null)
        {
            IsModified = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));
        }

        public void RaisePropertyChanging(object oldValue, object newValue, [CallerMemberName] string propertyName=null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName, oldValue, newValue));
        }
    }
}
