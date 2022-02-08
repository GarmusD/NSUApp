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
using NSU.Shared;
using Android.Util;
using Android.Graphics;

namespace NSU.Droid
{
    public class ScalingLinearLayout : LinearLayout
    {
        int baseWidth;
        int baseHeight;
        bool alreadyScaled;
        float scale;
        int expectedWidth;
        int expectedHeight;

        public ScalingLinearLayout(Context context) : base(context)
        {
            NSULog.Debug("notcloud.view", "ScalingLinearLayout: width=" + this.Width + ", height=" + this.Height);
            this.alreadyScaled = false;
        }

        public ScalingLinearLayout(Context context, IAttributeSet attributes) : base(context, attributes)
        {
            NSULog.Debug("notcloud.view", "ScalingLinearLayout: width=" + this.Width + ", height=" + this.Height);
            this.alreadyScaled = false;
        }

        protected override void OnFinishInflate()
        {
            //return;
            NSULog.Debug("notcloud.view", "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
            NSULog.Debug("notcloud.view", "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            NSULog.Debug("notcloud.view", "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
            NSULog.Debug("notcloud.view", "ScalingLinearLayout::onFinishInflate: 1 width=" + this.Width + ", height=" + this.Height);

            // Do an initial measurement of this layout with no major restrictions on size.
            // This will allow us to figure out what the original desired width and height are.
            this.Measure(1000, 1000); // Adjust this up if necessary.
            this.baseWidth = this.MeasuredWidth;
            this.baseHeight = this.MeasuredHeight;
            NSULog.Debug("notcloud.view", "ScalingLinearLayout::onFinishInflate: 2 width=" + this.Width + ", height=" + this.Height);

            NSULog.Debug("notcloud.view", "ScalingLinearLayout::onFinishInflate: alreadyScaled=" + this.alreadyScaled);
            NSULog.Debug("notcloud.view", "ScalingLinearLayout::onFinishInflate: scale=" + this.scale);
            if (this.alreadyScaled)
            {
                Scale.scaleViewAndChildren((LinearLayout)this, this.scale, 0);
            }
        }

        public override void Draw(Canvas canvas)
        {
            //base.Draw(canvas);
            //return;

            // Get the current width and height.
            int width = this.Width;
            int height = this.Height;

            // Figure out if we need to scale the layout.
            // We may need to scale if:
            //    1. We haven't scaled it before.
            //    2. The width has changed.
            //    3. The height has changed.
            if (!this.alreadyScaled || width != this.expectedWidth || height != this.expectedHeight)
            {
                // Figure out the x-scaling.
                float xScale = (float)width / this.baseWidth;
                if (this.alreadyScaled && width != this.expectedWidth)
                {
                    xScale = (float)width / this.expectedWidth;
                }
                // Figure out the y-scaling.
                float yScale = (float)height / this.baseHeight;
                if (this.alreadyScaled && height != this.expectedHeight)
                {
                    yScale = (float)height / this.expectedHeight;
                }

                // Scale the layout.
                this.scale = Math.Min(xScale, yScale);
                if (scale == 0) scale = 1.0f;
                NSULog.Debug("notcloud.view", "ScalingLinearLayout::onLayout: Scaling!");
                Scale.scaleViewAndChildren((LinearLayout)this, this.scale, 0);

                // Mark that we've already scaled this layout, and what
                // the width and height were when we did so.
                this.alreadyScaled = true;
                this.expectedWidth = width;
                this.expectedHeight = height;

                // Finally, return.
                return;
            }

            base.Draw(canvas);
        }
    }
}