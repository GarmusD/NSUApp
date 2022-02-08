using System;
using Android.Preferences;
using Android.Content;
using NSUAppShared;

namespace NSU.Droid
{
    public class Preferences
    {
        const string KEY_USER = "usrname";
        const string KEY_PASSWORD = "usrpsw";
        const string KEY_HOST = "nsuhost";
        const string KEY_PORT = "nsuport";
        const string KEY_HASH = "hash";
        const string KEY_DEVICEID = "deviceid";

        private ISharedPreferences sp;
        private ISharedPreferencesEditor spe;
        private Credentials c;

        public Preferences(Context cntx)
        {
            sp = PreferenceManager.GetDefaultSharedPreferences(cntx);
            spe = sp.Edit();
            c = Credentials.Current;
            c.UserName = UserName;
            c.Password = Password;
            c.Host = Host;
            c.Port = Port;
            c.Hash = Hash;
            c.DeviceID = DeviceID;
            c.OnInfoChanged += Credentials_OnInfoChanged;
        }

        private void Credentials_OnInfoChanged(Credentials.Field infoField)
        {
            switch (infoField)
            {
                case Credentials.Field.Host:
                    Host = c.Host;
                    break;
                case Credentials.Field.Port:
                    Port = c.Port;
                    break;
                case Credentials.Field.UserName:
                    UserName = c.UserName;
                    break;
                case Credentials.Field.Password:
                    Password = c.Password;
                    break;
                case Credentials.Field.Hash:
                    Hash = c.Hash;
                    break;
                case Credentials.Field.DeviceID:
                    DeviceID = c.DeviceID;
                    break;
                default:
                    break;
            }
        }

        public bool UseHashForLogin
        {
            get
            {
                return 
                    CredentialsOk &&
                    !string.IsNullOrEmpty(Hash) &&
                    !string.IsNullOrEmpty(DeviceID);
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

        public bool HostOk
        {
            get
            {
                return
                    !string.IsNullOrEmpty(Host) &&
                    Port != 0;
            }
        }

        public string UserName
        {
            get { return sp.GetString(KEY_USER, /*String.Empty*/"admin"); }
            set { spe.PutString(KEY_USER, value); spe.Apply(); }
        }

        public string Password
        {
            get { return sp.GetString(KEY_PASSWORD, /*String.Empty*/"admin"); }
            set { spe.PutString(KEY_PASSWORD, value); spe.Apply(); }
        }

        public string Host
        {
            get { return sp.GetString(KEY_HOST, "nsu.dgs.lt"/*"89.117.236.213"*/); }
            set { spe.PutString(KEY_HOST, value); spe.Apply(); }
        }

        public int Port
        {
            get { return sp.GetInt(KEY_PORT, 5152); }
            set { spe.PutInt(KEY_PORT, value); spe.Apply(); }
        }

        public string Hash
        {
            get { return sp.GetString(KEY_HASH, string.Empty); }
            set { spe.PutString(KEY_HASH, value); spe.Apply(); }
        }

        public string DeviceID
        {
            get { return sp.GetString(KEY_DEVICEID, string.Empty); }
            set { spe.PutString(KEY_DEVICEID, value); spe.Apply(); }
        }

    }
}

