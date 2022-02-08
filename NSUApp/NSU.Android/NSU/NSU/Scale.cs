using System;
using Android.Views;
using NSU.Shared;

namespace NSU.Droid
{
    public class Scale
    {
        public static void scaleContents(View rootView, View container)
        {
            Scale.scaleContents(rootView, container, rootView.Width, rootView.Height);
        }

        // Scales the contents of the given view so that it completely fills the given
        // container on one axis (that is, we're scaling isotropically).
        public static void scaleContents(View rootView, View container, int width, int height)
        {
            NSULog.Debug("notcloud.scale", "Scale::scaleContents: container: " + container.Width + "x" + container.Height + ".");

            // Compute the scaling ratio
            float xScale = (float)container.Width / width;
            float yScale = (float)container.Height / height;
            float scale = Math.Min(xScale, yScale);

            // Scale our contents
            NSULog.Debug("notcloud.scale", "Scale::scaleContents: scale=" + scale + ", width=" + width + ", height=" + height + ".");
            scaleViewAndChildren(rootView, scale, 0);
        }

        // Scale the given view, its contents, and all of its children by the given factor.
        public static void scaleViewAndChildren(View root, float scale, int canary)
        {
            // Retrieve the view's layout information
            ViewGroup.LayoutParams layoutParams = root.LayoutParameters;

            // Scale the View itself
            if (layoutParams.Width != ViewGroup.LayoutParams.MatchParent && layoutParams.Width != ViewGroup.LayoutParams.WrapContent)
            {
                layoutParams.Width = (int)(layoutParams.Width * scale);
            }
            if (layoutParams.Height != ViewGroup.LayoutParams.MatchParent && layoutParams.Height != ViewGroup.LayoutParams.WrapContent)
            {
                layoutParams.Height = (int)(layoutParams.Height * scale);
            }

            // If the View has margins, scale those too
            if (layoutParams.GetType() == typeof(ViewGroup.MarginLayoutParams))
            {
                ViewGroup.MarginLayoutParams marginParams = (ViewGroup.MarginLayoutParams)layoutParams;
                marginParams.LeftMargin = (int)(marginParams.LeftMargin * scale);
                marginParams.TopMargin = (int)(marginParams.TopMargin * scale);
                marginParams.RightMargin = (int)(marginParams.RightMargin * scale);
                marginParams.BottomMargin = (int)(marginParams.BottomMargin * scale);
            }
            root.LayoutParameters = layoutParams;

            // Same treatment for padding
            root.SetPadding(
                (int)(root.PaddingLeft * scale),
                (int)(root.PaddingTop * scale),
                (int)(root.PaddingRight * scale),
                (int)(root.PaddingBottom * scale)
            );

            // If it's a TextView, scale the font size
            /*
            if(root instanceof TextView) {
                TextView tv = (TextView)root;
                tv.setTextSize(tv.getTextSize() * scale); //< We do NOT want to do this.
            }
            */

            // If it's a ViewGroup, recurse!
            if (root.GetType() == typeof(ViewGroup))
            {
                ViewGroup vg = (ViewGroup)root;
                for (int i = 0; i < vg.ChildCount; i++)
                {
                    scaleViewAndChildren(vg.GetChildAt(i), scale, canary + 1);
                }
            }
        }
    }
}