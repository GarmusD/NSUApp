using NSU.NSUSystem;
using NSU.Shared;
using NSU.Shared.NSUUI;
using NSUAppShared.NSUSystemParts;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using NSU.Shared.NSUSystemPart;

namespace NSU.NSU_UWP.UI.Elements
{
    class UIExhaustFan : INSUUIExhaustFan
    {
        const string LogTag = "UIExhaustFan";
        SynchronizationContext sc;
        UIMonoBitmap monoBmp;
        string woodBoilerName;
        WoodBoiler woodBoiler = null;

        public UIElement uiElement { get { return monoBmp.uiElement; } }

        public UIExhaustFan(/*Canvas cnv*/)
        {
            sc = SynchronizationContext.Current;
            monoBmp = new UIMonoBitmap(/*canvas*/);
            monoBmp.SetResource(NSUBitmapResource.Ventiliatorius);
            monoBmp.OnMonoBmpTapped += MonoBmp_OnMonoBmpTapped;
            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemUnavailable += OnNSUSystemUnavailableHandler;
            sys.OnNSUSystemReady += OnNSUSystemReadyHandler;
        }

        private void MonoBmp_OnMonoBmpTapped(object sender, EventArgs e)
        {
            if (woodBoiler != null)
            {
                woodBoiler.SwitchState(SwitchTarget.ExhaustFan);
            }
        }

        private void OnNSUSystemReadyHandler(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(woodBoilerName))
            {
                var nsusys = NSUSys.Instance;
                var wbs = nsusys.GetNSUSysPart(NSUAppShared.NSUSystemParts.PartsTypes.WoodBoilers) as WoodBoilers;
                var wb = wbs.FindWoodBoiler(woodBoilerName);
                if (wb != null)
                {
                    wb.OnExhaustFanStatusChange += WoodBoilerOnExhaustFanStatusChangeHandler;
                    WoodBoilerOnExhaustFanStatusChangeHandler(wb, wb.ExhaustFanStatus);
                    woodBoiler = wb;
                }
            }
        }

        private void OnNSUSystemUnavailableHandler(object sender, System.EventArgs e)
        {
            NSULog.Debug(LogTag, $"OnNSUSystemUnavailableHandler(). UIID: {UIID}");
            WoodBoilerOnExhaustFanStatusChangeHandler(null, Shared.NSUSystemPart.ExhaustFanStatus.UNKNOWN);
        }

        public void AttachSmokeFan(string name)
        {
            woodBoilerName = name;
            if (NSUSys.Instance.NSUSystemReady)
                OnNSUSystemReadyHandler(null, null);
        }

        private void WoodBoilerOnExhaustFanStatusChangeHandler(Shared.NSUSystemPart.WoodBoiler sender, Shared.NSUSystemPart.ExhaustFanStatus status)
        {
            sc.Post((o) =>
            {
                switch (status)
                {
                    case Shared.NSUSystemPart.ExhaustFanStatus.OFF:
                        SetColor(NSUColor.OFF);
                        break;
                    case Shared.NSUSystemPart.ExhaustFanStatus.ON:
                        SetColor(NSUColor.ON);
                        break;
                    case Shared.NSUSystemPart.ExhaustFanStatus.MANUAL:
                        SetColor(NSUColor.MANUAL);
                        break;
                    default:
                        SetColor(NSUColor.UNKNOWN);
                        break;
                }
            }, null);

        }

        public void SetColor(NSUColor color)
        {
            if (monoBmp != null)
            {
                monoBmp.SetColor(color);
            }
        }

        public void Free()
        {
            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemUnavailable -= OnNSUSystemUnavailableHandler;
            sys.OnNSUSystemReady -= OnNSUSystemReadyHandler;
            monoBmp.Free();
            monoBmp = null;
        }

        public void AttachedToWindow()
        {
            
        }

        public void DeatachedFromWindow()
        {
            
        }

        public NSUUIClass UIClass { get { return NSUUIClass.Ventilator; } }

        public string UIID { get; set; }

        public int Width
        {
            get
            {
                return monoBmp.Width;
            }
            set
            {
                monoBmp.Width = value;
            }
        }

        public int Height
        {
            get
            {
                return monoBmp.Height;
            }
            set
            {
                monoBmp.Height = value;
            }
        }

        public int Left
        {
            get
            {
                return monoBmp.Left;
            }
            set
            {
                monoBmp.Left = value;
            }
        }

        public int Top
        {
            get
            {
                return monoBmp.Top;
            }
            set
            {
                monoBmp.Top = value;
            }
        }
    }
}
