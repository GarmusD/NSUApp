using System;
using Android.Content;
using Android.Content.Res;
using Android.Util;

namespace NSU.Droid
{
    public static class Utilities
    {
        /**
        * This method converts dp unit to equivalent pixels, depending on device density. 
        * 
        * @param dp A value in dp (density independent pixels) unit. Which we need to convert into pixels
        * @param context Context to get resources and device specific display metrics
        * @return A float value to represent px equivalent to dp depending on device density
        */
        /*
        public static float ConvertDpToPixel(float dp, Context context)
        {
            Resources resources = context.Resources;
            DisplayMetrics metrics = resources.DisplayMetrics;
            float px = dp * ((float)metrics.DensityDpi / 160f);
            return px;
        }
        */
        public static int ConvertDpToPixel(int dp, float scale /*Context context*/)
        {
            return (int)(dp * scale);
        }

        /**
        * This method converts device specific pixels to density independent pixels.
        * 
        * @param px A value in px (pixels) unit. Which we need to convert into db
        * @param context Context to get resources and device specific display metrics
        * @return A float value to represent dp equivalent to px value
        */
        /*
        public static float ConvertPixelsToDp(float px, Context context)
        {
            return TypedValue.ApplyDimension(ComplexUnitType.Dip, px, context.Resources.DisplayMetrics);

            Resources resources = context.Resources;
            DisplayMetrics metrics = resources.DisplayMetrics;
            float dp = px / ((float)metrics.DensityDpi / 160f);
            return dp;
        }
        */

        public static int ConvertPixelsToDp(int px, float scale /*Context context*/)
        {
            return (int)(px / scale);
        }

    }
}

