using NSU.NSUSystem;
using NSU.Shared.NSUSystemPart;
using NSU.Shared.NSUUI;
using NSUAppShared.NSUSystemParts;
using System;
using System.Threading;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace NSU.NSU_UWP.UI.Elements
{
    public class UILabel : INSUUILabel
    {
        const string LogTag = "UILabel";
        NSUUIActions action = NSUUIActions.NoAction;
        string actParam;
        TextBlock label;
        //Canvas canvas;
        TranslateTransform tt;
        SynchronizationContext sc;

        public UILabel(/*Canvas cnv*/)
        {
            sc = SynchronizationContext.Current;
            //canvas = cnv;
            label = new TextBlock();
            UIID = String.Empty;
            label.Text = "";
            label.Foreground = new SolidColorBrush(Colors.Blue);
            label.FontSize = 22;
            tt = new TranslateTransform();
            tt.X = 0;
            tt.Y = 0;
            label.RenderTransform = tt;
            //canvas.Children.Add(label);
        }

        public UIElement uiElement { get { return label; } }
        public NSUUIClass UIClass { get { return NSUUIClass.Label; } }

        public string UIID { get; set; }

        public string Caption
        {
            get
            {
                return label.Text;
            }
            set
            {
                sc.Post((o)=> {
                    label.Text = value;
                }, null);
                //label.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { label.Text = value; });                
            }
        }

        public NSUTextAlign TextAlign
        {
            get
            {
                switch (label.HorizontalAlignment)
                {
                    case HorizontalAlignment.Center:
                        return NSUTextAlign.AlignCenter;
                    case HorizontalAlignment.Left:
                        return NSUTextAlign.AlignLeft;
                    case HorizontalAlignment.Right:
                        return NSUTextAlign.AlignRight;
                }
                return NSUTextAlign.AlignLeft;
            }
            set
            {
                switch (value)
                {
                    case NSUTextAlign.AlignCenter:
                        label.HorizontalAlignment = HorizontalAlignment.Center;
                        break;
                    case NSUTextAlign.AlignLeft:
                        label.HorizontalAlignment = HorizontalAlignment.Left;
                        break;
                    case NSUTextAlign.AlignRight:
                        label.HorizontalAlignment = HorizontalAlignment.Right;
                        break;
                }
            }
        }


        public int Width
        {
            get
            {
                return (int)label.Width;
            }
            set
            {
                label.Width = value;
            }
        }

        public int Height
        {
            get
            {
                return (int)label.Height;
            }
            set
            {
                label.Height = value;
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

        public void SetAction(NSUUIActions action, string param)
        {
            WoodBoilers wbs;
            WoodBoiler wb;
            var oldAction = this.action;

            this.action = action;
            actParam = param;

            switch (action)
            {
                case NSUUIActions.NoAction:
                    if(oldAction == NSUUIActions.ShowWoodBoilerStatus)
                    {
                        wbs = NSUSystem.NSUSys.Instance.GetNSUSysPart(NSUAppShared.NSUSystemParts.PartsTypes.WoodBoilers) as WoodBoilers;
                        wb = wbs.FindWoodBoiler(param);
                        if (wb != null)
                        {
                            wb.OnStatusChange -= WoodBoilerOnStatusChangeHandler;
                        }
                    }
                    break;
                case NSUUIActions.ShowWoodBoilerStatus:
                    NSUSys nsusys = NSUSys.Instance;
                    if (nsusys.NSUSystemReady)
                    {
                        wbs = nsusys.GetNSUSysPart(NSUAppShared.NSUSystemParts.PartsTypes.WoodBoilers) as WoodBoilers;
                        wb = wbs.FindWoodBoiler(param);
                        if (wb != null)
                        {
                            wb.OnStatusChange += WoodBoilerOnStatusChangeHandler;
                            WoodBoilerOnStatusChangeHandler(wb, wb.Status);
                        }
                    }
                    else
                    {
                        nsusys.OnNSUSystemReady += OnNSUPartsReadyHandler;
                    }
                    break;
                case NSUUIActions.KatiloIkurimas:
                    break;
                default:
                    break;
            }
        }

        private void OnNSUPartsReadyHandler(object sender, EventArgs e)
        {
            if(action != NSUUIActions.NoAction)
            {
                NSUSys.Instance.OnNSUSystemReady -= OnNSUPartsReadyHandler;
                SetAction(action, actParam);
            }
        }

        private void WoodBoilerOnStatusChangeHandler(Shared.NSUSystemPart.WoodBoiler sender, Shared.NSUSystemPart.WoodBoilerStatus status)
        {
            switch (status)
            {
                case Shared.NSUSystemPart.WoodBoilerStatus.UNKNOWN:
                    Caption = "Nežinoma būsena";
                    break;
                case Shared.NSUSystemPart.WoodBoilerStatus.UZGESES:
                    Caption = "Užgesęs";
                    break;
                case Shared.NSUSystemPart.WoodBoilerStatus.IKURIAMAS:
                    Caption = "Įkuriamas";
                    break;
                case Shared.NSUSystemPart.WoodBoilerStatus.KURENASI:
                    Caption = "Kūrenasi";
                    break;
                case Shared.NSUSystemPart.WoodBoilerStatus.GESTA:
                    Caption = "Gęsta";
                    break;
                default:
                    Caption = "Nežinoma būsena";
                    break;
            }
        }

        public void Free()
        {
            //canvas.Children.Remove(label);
            label = null;
            tt = null;
        }

        public void AttachedToWindow()
        {
            
        }

        public void DeatachedFromWindow()
        {
            
        }
    }
}
