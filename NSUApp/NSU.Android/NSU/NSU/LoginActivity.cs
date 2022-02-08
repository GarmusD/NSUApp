
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace NSU.Droid
{
    [Activity(Label = "Login")]			
    public class LoginActivity : Activity
    {
        TextView usrname;
        TextView password;
        TextView host;
        TextView port;
        Button btnOk;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.Login);

            usrname = (TextView)FindViewById(Resource.Id.reg_name);
            password = (TextView)FindViewById(Resource.Id.reg_password);
            host = (TextView)FindViewById(Resource.Id.reg_host);
            port = (TextView)FindViewById(Resource.Id.reg_port);

            usrname.TextChanged += HandleUsrTextChange;
            password.TextChanged += HandleUsrTextChange;
            host.TextChanged += HandleUsrTextChange;
            port.TextChanged += HandleUsrTextChange;

            btnOk = (Button)FindViewById(Resource.Id.btnRegister);
            btnOk.Enabled = false;
            btnOk.Click += btnOkClick;

            Preferences p = App.Current.Prefs;
            usrname.Text = p.UserName;
            password.Text = p.Password;
            host.Text = p.Host;
            port.Text = p.Port.ToString();
        }

        void HandleUsrTextChange (object sender, Android.Text.TextChangedEventArgs e)
        {
            btnOk.Enabled = !string.IsNullOrWhiteSpace(usrname.Text) && 
                            !string.IsNullOrWhiteSpace(password.Text) && 
                            !string.IsNullOrWhiteSpace(host.Text) &&
                            !string.IsNullOrWhiteSpace(port.Text);
        }

        void btnOkClick (object sender, EventArgs e)
        {
            NSUAppShared.Credentials c = NSUAppShared.Credentials.Current;
            c.UserName = usrname.Text;
            c.Password = password.Text;
            c.Host = host.Text;
            int ires;
            var res = int.TryParse(port.Text, out ires);
            if (res)
            {
                c.Port = ires;
                SetResult(Result.Ok);
                Finish();
            }
            else
            {
                //TODO Show error message
            }
        }
    }
}

