using NSU.NSUSystem;
using NSU.Shared;
using NSU.Shared.NSUUI;
using NSUAppShared.NSUSystemParts;
using System;
using System.Threading;
using Windows.UI.Xaml;

namespace NSU.NSU_UWP.UI.Elements
{
    class UILadomatas : INSUUILadomat
    {
        const string LogTag = "UILadomatas";
        SynchronizationContext sc;
        //Canvas canvas;
        UIMonoBitmap monoBmp;
        string woodBoilerName;

        public UIElement uiElement { get { return monoBmp.uiElement; } }

        public UILadomatas(/*Canvas cnv*/)
        {
            sc = SynchronizationContext.Current;
            monoBmp = new UIMonoBitmap(/*canvas*/);
            monoBmp.SetResource(NSUBitmapResource.Ladomatas);
            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemReady += OnNSUSystemReadyHandler;
            sys.OnNSUSystemUnavailable += OnNSUSystemUnavailableHandler;
        }

        private void OnNSUSystemReadyHandler(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(woodBoilerName))
            {
                var nsusys = NSUSystem.NSUSys.Instance;
                var wbs = nsusys.GetNSUSysPart(NSUAppShared.NSUSystemParts.PartsTypes.WoodBoilers) as WoodBoilers;
                var wb = wbs.FindWoodBoiler(woodBoilerName);
                if (wb != null)
                {
                    wb.OnLadomatStatusChange += WoodBoilerOnLadomatStatusChangeHandler;
                    WoodBoilerOnLadomatStatusChangeHandler(wb, wb.LadomStatus);
                }
            }
        }

        private void OnNSUSystemUnavailableHandler(object sender, EventArgs e)
        {
            NSULog.Debug(LogTag, $"OnNSUSystemUnavailableHandler(). UIID: {UIID}");
            WoodBoilerOnLadomatStatusChangeHandler(null, Shared.NSUSystemPart.LadomatStatus.UNKNOWN);
        }

        public void AttachLadomat(string name)
        {
            woodBoilerName = name;
            if (NSUSys.Instance.NSUSystemReady)
                OnNSUSystemReadyHandler(null, null);
        }

        private void WoodBoilerOnLadomatStatusChangeHandler(Shared.NSUSystemPart.WoodBoiler sender, Shared.NSUSystemPart.LadomatStatus status)
        {
            sc.Post((o) =>
            {
                switch (status)
                {
                    case Shared.NSUSystemPart.LadomatStatus.OFF:
                        SetColor(NSUColor.OFF);
                        break;
                    case Shared.NSUSystemPart.LadomatStatus.ON:
                        SetColor(NSUColor.ON);
                        break;
                    case Shared.NSUSystemPart.LadomatStatus.MANUAL:
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
            sys.OnNSUSystemReady -= OnNSUSystemReadyHandler;
            sys.OnNSUSystemUnavailable -= OnNSUSystemUnavailableHandler;
            monoBmp.Free();
            monoBmp = null;
        }

        public void AttachedToWindow()
        {
            
        }

        public void DeatachedFromWindow()
        {
            
        }

        public NSUUIClass UIClass { get { return NSUUIClass.Ladomat; } }

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
