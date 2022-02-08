using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.V4.App;
using Android.Support.V4.View;

namespace NSU.Droid
{
    public class GenericViewPagerFragment : Android.Support.V4.App.Fragment
    {
        private View view;

        public static GenericViewPagerFragment NewInstance(int viewID, string Title)
        {
            GenericViewPagerFragment f = new GenericViewPagerFragment(){Arguments = new Bundle()};
            f.Arguments.PutInt("viewID", viewID);
            f.Arguments.PutString("title", Title);
            return f;
        }
        //public GenericViewPagerFragment()
        //{
        //}
        //public GenericViewPagerFragment(Func<LayoutInflater, ViewGroup, Bundle, View> view)
        //{
        //    _view = view;
        //}

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