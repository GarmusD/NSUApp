using System;
using System.Collections.Generic;
using System.Text;

namespace NSUAppShared
{
    public class Credentials
    {
        public enum Field
        {
            Host,
            Port,
            UserName,
            Password,
            Hash,
            DeviceID
        }

        private static Credentials instance = null;

        public delegate void InfoChanged(Field infoField);
        public event InfoChanged OnInfoChanged;

        private string host = string.Empty;
        private int port = 5152;
        private string userName = string.Empty;
        private string password = string.Empty;
        private string deviceID = string.Empty;
        private string hash = string.Empty;

        
        public static Credentials Current
        {
            get
            {
                if (instance == null)
                    instance = new Credentials();
                return instance;
            }
        }
        

        private Credentials(){}

        public string Host
        {
            get { return host; }
            set {
                if (value != host)
                {
                    host = value;
                    RaiseInfoChanged(Field.Host);
                }
                }
        }

        public int Port
        {
            get { return port; }
            set
            {
                if(value != port)
                {
                    port = value;
                    RaiseInfoChanged(Field.Port);
                }
            }
        }

        public string UserName
        {
            get { return userName; }
            set
            {
                if(userName != value)
                {
                    userName = value;
                    RaiseInfoChanged(Field.UserName);
                }
            }
        }

        public string Password
        {
            get { return password; }
            set
            {
                if (password != value)
                {
                    password = value;
                    RaiseInfoChanged(Field.Password);
                }
            }
        }

        public string DeviceID
        {
            get { return deviceID; }
            set
            {
                if(deviceID != value)
                {
                    deviceID = value;
                    RaiseInfoChanged(Field.DeviceID);
                }
            }
        }

        public string Hash
        {
            get { return hash; }
            set
            {
                if(hash != value)
                {
                    hash = value;
                    RaiseInfoChanged(Field.Hash);
                }
            }
        }

        public bool HostOk
        {
            get
            {
                return
                    !string.IsNullOrEmpty(Host) &&
                    Port != 0;
            }
        }

        public bool CredentialsOk
        {
            get
            {
                return
                    !string.IsNullOrEmpty(UserName) &&
                    !string.IsNullOrEmpty(Password);
            }
        }

        public bool UseHashForLogin
        {
            get
            {
                return
                    !string.IsNullOrEmpty(Hash) &&
                    !string.IsNullOrEmpty(DeviceID);
            }
        }

        private void RaiseInfoChanged(Field infoField)
        {
            OnInfoChanged?.Invoke(infoField);
        }
    }
}
