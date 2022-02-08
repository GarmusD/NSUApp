using NSU.NSU_UWP.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace NSU.NSU_UWP.ViewModels.Converters
{
    public class BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool res = (bool)value;
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ChannelDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if(targetType == typeof(ChannelData))
            {
                return value as ChannelData;
            }
            return null;
        }
    }

    public class NameClassConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                System.Diagnostics.Debug.WriteLine($"NameClassConverter.Convert(value: 'NULL'/NULL)");

            }
            else
                System.Diagnostics.Debug.WriteLine($"NameClassConverter.Convert(value: '{value}'/{value.GetType().ToString()}, targetType: '{targetType}')");
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                System.Diagnostics.Debug.WriteLine($"NameClassConverter.Convert(value: 'NULL'/NULL)");
                return null;
            }
            System.Diagnostics.Debug.WriteLine($"NameClassConverter.ConvertBack(value: '{value}', targetType: '{targetType}')");
            return (NameClass)value;
        }
    }

    public class GenericConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return 0;
            return (int)value;
        }
    }
}
