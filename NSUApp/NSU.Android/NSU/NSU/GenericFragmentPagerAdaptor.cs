using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.V4.View;
using Android.Support.V4.App;

namespace NSU.Droid
{
    public class GenericFragmentPagerAdaptor : FragmentPagerAdapter
    {
        const string argTitle = "title";

        private List<Android.Support.V4.App.Fragment> _fragmentList = new List<Android.Support.V4.App.Fragment>();

        public GenericFragmentPagerAdaptor(Android.Support.V4.App.FragmentManager fm)
            : base(fm) {}

        public override int Count
        {
            get { return _fragmentList.Count; }
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return _fragmentList[position];
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(_fragmentList[position].Arguments.GetString(argTitle).ToUpper());
        }

        public void AddFragment(Android.Support.V4.App.Fragment fragment)
        {
            _fragmentList.Add(fragment);
        }

        //public void AddFragmentView(Func<LayoutInflater, ViewGroup, Bundle, View> view)
        //{
        //    _fragmentList.Add(new GenericViewPagerFragment(view));
        //}
    }

    /*
    public class ViewPageListenerForActionBar : ViewPager.SimpleOnPageChangeListener
    {
        private ActionBar _bar;
        public ViewPageListenerForActionBar(ActionBar bar)
        {
            _bar = bar;
        }
        public override void OnPageSelected(int position)
        {
            _bar.SetSelectedNavigationItem(position);
        }
    }

    public static class ViewPagerExtensions
    {
        public static ActionBar.Tab GetViewPageTab(this ViewPager viewPager, ActionBar actionBar, string name)
        {
            var tab = actionBar.NewTab();
            tab.SetText(name);
            tab.TabSelected += (o, e) =>
                {
                    viewPager.SetCurrentItem(actionBar.SelectedNavigationIndex, false);
                };
            return tab;
        }
    }
    */
}

