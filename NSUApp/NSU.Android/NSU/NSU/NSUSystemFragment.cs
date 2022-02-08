
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace NSU.Droid
{
    public class NSUSystemFragment : Android.Support.V4.App.Fragment
    {
        private const string logTag = "NSUScreenView";

        private View view;

        public static NSUSystemFragment NewInstance(int viewID, string title)
        {
            NSUSystemFragment f = new NSUSystemFragment(){Arguments = new Bundle()};
            f.Arguments.PutInt("viewID", viewID);
            f.Arguments.PutString("title", title);
            return f;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            if (container == null)
            {
                return null;
            }

            view = inflater.Inflate(Arguments.GetInt("viewID"), null);
            return view;
        }

    }
}

