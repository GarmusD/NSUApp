using NSU.NSUSystem;
using NSU.Shared;
using NSU.Shared.NSUSystemPart;
using NSU.Shared.NSUUI;
using NSUAppShared.NSUSystemParts;
using System;
using System.Threading;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace NSU.NSU_UWP.UI.Elements
{
    class UITempBar : INSUUITempBar
    {
        const string LogTag = "UITempBar";
        const int MAX_BAR_TEMP = 70;
        const int MIN_BAR_TEMP = 32;

        SynchronizationContext sc;
        //Canvas canvas;
        Rectangle r;
        LinearGradientBrush lgb;
        GradientStop gs1, gs2, gs3;
        TranslateTransform tt;
        byte r1, r2, r3, b1, b2, b3;
        float t1, t2, t3;
        TempSensor tsi1, tsi2, tsi3;
        string tsn1, tsn2, tsn3;

        public UITempBar(/*Canvas cnv*/)
        {
            sc = SynchronizationContext.Current;
            t1 = t2 = t3 = 20.0f;
            tsn1 = tsn2 = tsn3 = String.Empty;
            //UIClass = NSUUIClass.TempBar;
            //canvas = cnv;
            r = new Rectangle();

            r.Stroke = new SolidColorBrush(Colors.Blue);
            r.StrokeThickness = 2;

            lgb = new LinearGradientBrush();
            r.Fill = lgb;

            gs1 = new GradientStop();
            gs1.Offset = 0;
            gs1.Color = Colors.Blue;

            gs2 = new GradientStop();
            gs2.Offset = 0.5;
            gs2.Color = Colors.Red;

            gs3 = new GradientStop();
            gs3.Offset = 1;
            gs3.Color = Colors.Blue;

            lgb.GradientStops.Add(gs1);
            lgb.GradientStops.Add(gs2);
            lgb.GradientStops.Add(gs3);

            r.Width = 35;
            r.Height = 225;
            tt = new TranslateTransform();
            r.RenderTransform = tt;
            //canvas.Children.Add(r);

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
                    tsi3_OnTempChange(tsi3, new TempChangedEventArgs(tsi3.Temperature));
                }
            }
        }

        private void OnNSUSystemUnavailableHandler(object sender, EventArgs e)
        {
            NSULog.Debug(LogTag, $"OnNSUSystemUnavailableHandler(). UIID: {UIID}");
            t1 = t2 = t3 = 0.0f;
            Calculate();
        }

        public UIElement uiElement { get { return r; } }

        private byte MapToB(float value, float fromSource, float toSource, byte fromTarget, byte toTarget)
        {
            byte b = (byte)((value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget);
            return b;
        }

        public void SetTempSensors(string s1, string s2, string s3)
        {
            tsn1 = s1;
            tsn2 = s2;
            tsn3 = s3;
            if (NSUSys.Instance.NSUSystemReady)
                OnNSUSystemReadyHandler(null, null);
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
            r1 = MapToB(constrain(t1, MIN_BAR_TEMP, MAX_BAR_TEMP), MIN_BAR_TEMP, MAX_BAR_TEMP, 0, 255);
            r2 = MapToB(constrain(t2, MIN_BAR_TEMP, MAX_BAR_TEMP), MIN_BAR_TEMP, MAX_BAR_TEMP, 0, 255);
            r3 = MapToB(constrain(t3, MIN_BAR_TEMP, MAX_BAR_TEMP), MIN_BAR_TEMP, MAX_BAR_TEMP, 0, 255);
            b1 = MapToB(constrain(t1, MIN_BAR_TEMP, MAX_BAR_TEMP), MIN_BAR_TEMP, MAX_BAR_TEMP, 255, 0);
            b2 = MapToB(constrain(t2, MIN_BAR_TEMP, MAX_BAR_TEMP), MIN_BAR_TEMP, MAX_BAR_TEMP, 255, 0);
            b3 = MapToB(constrain(t3, MIN_BAR_TEMP, MAX_BAR_TEMP), MIN_BAR_TEMP, MAX_BAR_TEMP, 255, 0);

            gs1.Color = Color.FromArgb(255, r1, 0, b1);
            gs2.Color = Color.FromArgb(255, r2, 0, b2);
            gs3.Color = Color.FromArgb(255, r3, 0, b3);
        }

        async void Calculate()
        {
            await r.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {
                 CalculateUI();
             });

        }

        public void Free()
        {
            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemReady -= OnNSUSystemReadyHandler;
            sys.OnNSUSystemUnavailable -= OnNSUSystemUnavailableHandler;
            r = null;
        }

        public void AttachedToWindow()
        {
            
        }

        public void DeatachedFromWindow()
        {
            
        }

        public NSUUIClass UIClass { get { return NSUUIClass.TempBar; } }

        public string UIID { get; set; }

        public int Width
        {
            get
            {
                return (int)r.Width;
            }
            set
            {
                r.Width = value;
            }
        }

        public int Height
        {
            get
            {
                return (int)r.Height;
            }
            set
            {
                r.Height = value;
            }
        }

        public int Left
        {
            get
            {
                return (int)tt.X;
            }
            set
            {
                tt.X = value;
            }
        }

        public int Top
        {
            get
            {
                return (int)tt.Y;
            }
            set
            {
                tt.Y = value;
            }
        }
    }
}
