using System;
using NSU.Shared.NSUUI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Windows.UI;
using NSU.Shared.NSUSystemPart;
using NSUAppShared.NSUSystemParts;
using NSU.NSUSystem;
using NSU.Shared;
using System.Threading;

namespace NSU.NSU_UWP.UI.Elements
{
    class UICircPump : INSUUICircPump
    {
        const string LogTag = "UICircPump";
        SynchronizationContext sc;
        UIMonoBitmap monoBmp;
        CircPump circPump = null;
        string cpName = string.Empty;

        public UIElement uiElement { get { return monoBmp.uiElement; } }

        public UICircPump(/*Canvas cnv*/)
        {
            sc = SynchronizationContext.Current;
            monoBmp = new UIMonoBitmap(/*canvas*/);
            monoBmp.OnMonoBmpTapped += OnTappedMonoBmp;
            monoBmp.SetResource(NSUBitmapResource.Cirkuliacinis);
            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemReady += OnNSUSystemReadyHandler;
            sys.OnNSUSystemUnavailable += OnNSUSystemUnavailableHandler;
        }

        private void OnNSUSystemUnavailableHandler(object sender, EventArgs e)
        {
            NSULog.Debug(LogTag, $"OnNSUSystemUnavailableHandler(). UIID: {UIID}");
            OnCircPumpStatusChange(null, new StatusChangedEventArgs(Status.UNKNOWN));
        }

        private void OnNSUSystemReadyHandler(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(cpName))
            {
                NSULog.Debug(LogTag, $"AttachPump(pumpName: {cpName})");
                var nsusys = NSUSys.Instance;                
                var cps = nsusys.GetNSUSysPart(PartsTypes.CircPumps) as CircPumps;
                if (cps != null)
                {
                    circPump = cps.FindCircPump(cpName);
                    if (circPump != null)
                    {
                        circPump.OnStatusChanged += OnCircPumpStatusChange;
                        OnCircPumpStatusChange(circPump, new StatusChangedEventArgs(circPump.Status));
                    }
                    else
                    {
                        NSULog.Debug(LogTag, $"AttachPump(pumpName: {cpName}) - not found.");
                    }
                }
            }
            else
            {
                circPump = null;
            }
        }

        private void OnTappedMonoBmp(object sender, EventArgs e)
        {
            if (circPump != null)
            {
                circPump.Clicked();
            }
        }

        public void AttachPump(string pumpName)
        {
            cpName = pumpName;
            if(NSUSys.Instance.NSUSystemReady)
            {
                OnNSUSystemReadyHandler(null, null);
            }
        }

        private void OnCircPumpStatusChange(object sender, StatusChangedEventArgs e)
        {
            sc.Post((o) =>
            {
                switch (e.Status)
                {
                    case Status.OFF:
                        SetColor(NSUColor.OFF);
                        break;
                    case Status.ON:
                        SetColor(NSUColor.ON);
                        break;
                    case Status.MANUAL:
                        SetColor(NSUColor.MANUAL);
                        break;
                    case Status.DISABLED:
                        SetColor(NSUColor.DISABLED);
                        break;
                    default:
                        SetColor(NSUColor.UNKNOWN);
                        break;
                }
            }, null);
        }

        public void SetRotation(int val)
        {
            monoBmp.SetRotation(val);
        }

        public void SetGraphicsBytes(byte[] buf)
        {
            throw new NotImplementedException();
        }

        public void SetVectorBytes(byte[] buf)
        {
            throw new NotImplementedException();
        }

        public void SetColor(NSUColor color)
        {
            if (monoBmp != null)
            {
                monoBmp.SetColor(color);
            }
        }

        public void SetBackColor(NSUColor color)
        {
            throw new NotImplementedException();
        }

        public INSUUIDrawer GetDrawer()
        {
            return null;
        }

        public void Free()
        {
            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemReady -= OnNSUSystemReadyHandler;
            sys.OnNSUSystemUnavailable -= OnNSUSystemUnavailableHandler;
            monoBmp.Free();
            monoBmp = null;
            circPump = null;
        }

        public void AttachedToWindow()
        {
        }

        public void DeatachedFromWindow()
        {
        }

        public NSUUIClass UIClass { get { return NSUUIClass.CircPump; } }

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
