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
using Android.Webkit;

namespace NSU.Droid.UI.Elements
{
    public class WebViewHelper : WebViewClient
    {
        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {            
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                view.Settings.MixedContentMode = MixedContentHandling.AlwaysAllow;
            }
            view.LoadUrl(request.Url.ToString());
            return false;
        }
    }
}