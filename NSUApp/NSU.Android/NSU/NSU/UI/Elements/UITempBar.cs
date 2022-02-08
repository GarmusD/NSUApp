using System;
using NSU.Shared.NSUUI;
using Android.Widget;
using Android.Content;
using Android.Graphics;
using NSU.NSUSystem;
using Android.App;
using NSU.Shared.NSUSystemPart;
using NSU.Shared;
using NSUAppShared.NSUSystemParts;

namespace NSU.Droid.UI.Elements
{
    public class UITempBar : ImageView, INSUUITempBar
    {
        const string LogTag = "UITempBar";

        const int MAX_BAR_TEMP = 70;
        const int MIN_BAR_TEMP = 30;

        byte r1, r2, r3, b1, b2, b3;
        float t1, t2, t3;
        TempSensor tsi1, tsi2, tsi3;
        string tsn1, tsn2, tsn3;

        Bitmap bmp;
        Canvas canvas;
        Activity act;
        float scale;
        bool disposed = false;

        public UITempBar(Context context, float scale)
            : base(context)
        {
            act = (Activity)context;
            this.scale = scale;
            Bitmap.Config conf = Bitmap.Config.Argb8888; // see other conf types
            bmp = Bitmap.CreateBitmap(35, 225, conf); // this creates a MUTABLE bitmap
            LayoutParameters = new Android.Views.ViewGroup.LayoutParams(Utilities.ConvertDpToPixel(35, scale), Utilities.ConvertDpToPixel(225, scale));            
            canvas = new Canvas(bmp);
            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemReady += OnNSUSystemReadyHandler;
            sys.OnNSUSystemUnavailable += OnNSUSystemUnavailableHandler;
        }

        private void OnNSUSystemReadyHandler(object sender, EventArgs e)
        {
            NSUSys sys = NSUSys.Instance;
            TempSensors s = sys.GetNSUSysPart(PartsTypes.TSensors) as TempSensors;
            if (!string.IsNullOrEmpty(tsn1))
            {
                tsi1 = s.FindSensor(tsn1);
                if (tsi1 != null)
                {
                    tsi1.OnTempChanged += tsi1_OnTempChange;
                    tsi1_OnTempChange(tsi1, new TempChangedEventArgs(tsi1.Temperature));
                }
            }
            if (!string.IsNullOrEmpty(tsn2))
            {
                tsi2 = s.FindSensor(tsn2);
                if (tsi2 != null)
                {
                    tsi2.OnTempChanged += tsi2_OnTempChange;
                    tsi2_OnTempChange(tsi2, new TempChangedEventArgs(tsi2.Temperature));
                }
            }
            if (!string.IsNullOrEmpty(tsn3))
            {
                tsi3 = s.FindSensor(tsn3);
                if (tsi3 != null)
                {
                    tsi3.OnTempChanged += tsi3_OnTempChange;
                    tsi3_OnTempChange(tsi3, new TempChangedEventArgs( tsi3.Temperature));
                }
            }
        }

        private void OnNSUSystemUnavailableHandler(object sender, EventArgs e)
        {
            NSULog.Debug(LogTag, $"OnNSUSystemUnavailableHandler(). UIID: {UIID}");
            t1 = t2 = t3 = 0.0f;
            Calculate();
        }

        byte MapToB(float value, float fromSource, float toSource, byte fromTarget, byte toTarget)
        {
            var b = (byte)((value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget);
            return b;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposed)
                return;
            Free();
            disposed = true;
        }

        public void Free()
        {
            NSULog.Debug(string.Format("UITempBar [UIID - '{0}']", UIID), "Free() is called.");
            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemReady -= OnNSUSystemReadyHandler;
            sys.OnNSUSystemUnavailable -= OnNSUSystemUnavailableHandler;
            if (tsi1 != null)
            {
                tsi1.OnTempChanged -= tsi1_OnTempChange;
                tsn1 = string.Empty;
                tsi1 = null;
            }

            if (tsi2 != null)
            {
                tsi2.OnTempChanged -= tsi2_OnTempChange;
                tsn2 = string.Empty;
                tsi2 = null;
            }

            if (tsi3 != null)
            {
                tsi3.OnTempChanged -= tsi3_OnTempChange;
                tsn3 = string.Empty;
                tsi3 = null;
            }
            bmp = null;
            canvas = null;
        }

        public void SetTempSensors(string s1, string s2, string s3)
        {
            tsn1 = s1;
            tsn2 = s2;
            tsn3 = s3;
            if (NSUSys.Instance.NSUSystemReady)
                OnNSUSystemReadyHandler(null, null);
        }

        void sys_OnNSUPartsReady(object sender, EventArgs e)
        {
            SetTempSensors(tsn1, tsn2, tsn3);
            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemReady -= sys_OnNSUPartsReady;
        }

		void tsi3_OnTempChange(object sender, TempChangedEventArgs e)
        {
            t3 = e.Temperature;
            Calculate();
        }

        void tsi2_OnTempChange(object sender, TempChangedEventArgs e)
        {
            t2 = e.Temperature;
            Calculate();
        }

        void tsi1_OnTempChange(object sender, TempChangedEventArgs e)
        {
            t1 = e.Temperature;
            Calculate();
        }

        float constrain(float value, float min, float max)
        {
            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            return value;
        }

        void CalculateUI()
        {
            if(canvas == null)
            {
                NSULog.Error(LogTag, "CalculateUI() - canvas is NULL. (Object destroyed?)");
                return;
            }
            r1 = MapToB(constrain(t1, MIN_BAR_TEMP, MAX_BAR_TEMP), MIN_BAR_TEMP, MAX_BAR_TEMP, 0, 255);
            r2 = MapToB(constrain(t2, MIN_BAR_TEMP, MAX_BAR_TEMP), MIN_BAR_TEMP, MAX_BAR_TEMP, 0, 255);
            r3 = MapToB(constrain(t3, MIN_BAR_TEMP, MAX_BAR_TEMP), MIN_BAR_TEMP, MAX_BAR_TEMP, 0, 255);
            b1 = MapToB(constrain(t1, MIN_BAR_TEMP, MAX_BAR_TEMP), MIN_BAR_TEMP, MAX_BAR_TEMP, 255, 0);
            b2 = MapToB(constrain(t2, MIN_BAR_TEMP, MAX_BAR_TEMP), MIN_BAR_TEMP, MAX_BAR_TEMP, 255, 0);
            b3 = MapToB(constrain(t3, MIN_BAR_TEMP, MAX_BAR_TEMP), MIN_BAR_TEMP, MAX_BAR_TEMP, 255, 0);

            var gclrs = new int[]
            {
                Color.Argb(255, r1, 0, b1).ToArgb(), 
                Color.Argb(255, r2, 0, b2).ToArgb(),
                Color.Argb(255, r3, 0, b3).ToArgb()
            };

            Shader shader = new LinearGradient(0, 0, 0, 225, gclrs, null, Shader.TileMode.Clamp);
            var paint = new Paint();
            paint.SetShader(shader);
            canvas.DrawRect(new Rect(0, 0, 35, 225), paint);

            SetImageBitmap(null);
            SetImageBitmap(bmp);
        }

        void Calculate()
        {
            act.RunOnUiThread(() =>
                {
                    CalculateUI();
                });

        }

        public NSUUIClass UIClass { get { return NSUUIClass.TempBar; } }

        public string UIID{ get; set; }

        public new int Width
        {
            get { return Utilities.ConvertPixelsToDp(LayoutParameters.Width, scale); }
            set
            {
                this.LayoutParameters.Width = Utilities.ConvertDpToPixel(value, scale);// (int)((float)value * scale);
            }
        }

        public new int Height
        {
            get { return Utilities.ConvertPixelsToDp(LayoutParameters.Height, scale); }
            set
            {
                LayoutParameters.Height = Utilities.ConvertDpToPixel(value, scale);// (int)((float)value * scale);
            }
        }

        public new int Left
        { 
            get { return Utilities.ConvertPixelsToDp((int)GetX(), scale); }
            set { SetX(Utilities.ConvertDpToPixel(value, scale)); } 
        }

        public new int Top
        { 
            get { return Utilities.ConvertPixelsToDp((int)GetY(), scale); }
            set { SetY(Utilities.ConvertDpToPixel(value, scale)); } 
        }

        public void AttachedToWindow()
        {

        }

        public void DeatachedFromWindow()
        {

        }
    }
}

