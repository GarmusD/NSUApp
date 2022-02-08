using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NSU.NSU_UWP.ViewModels.Helpers
{
    public class ChannelData : INotifyPropertyChanged
    {
        public byte Channel
        {
            get { return ch; }
            set
            {
                if(ch != value)
                {
                    ch = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string UsedBy
        {
            get { return ownr; }
            set
            {
                if(ownr != value)
                {
                    ownr = value;
                    RaisePropertyChanged("Value");
                }
            }
        }

        public string Value
        {
            get { return ToString(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private byte ch = 255;
        private string ownr = string.Empty;

        public ChannelData() { }

        public ChannelData(byte channel)
        {
            ch = channel;
        }

        public ChannelData(byte channel, string usedBy)
        {
            ch = channel;
            ownr = usedBy;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(ownr) ? ch.ToString() : (ch == 0xFF ? ownr : $"{ch} ({ownr})");
        }

        public void SetUsedBy(string owner)
        {
            if (ch != 0xFF)
            {
                UsedBy = owner;
            }
        }

        public static void ParseChannel(ref ChannelData channelData, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var parts = value.Split(' ');
                byte val = 255;
                byte.TryParse(parts[0], out val);
                channelData.Channel = val;
            }
        }

        public static byte ParseChannel(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var parts = value.Split(' ');
                byte val = 255;
                byte.TryParse(parts[0], out val);
                return val;
            }
            return 255;
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
