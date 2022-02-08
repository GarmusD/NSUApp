using System;
using Android.Content;
using Android.App;
using Android.Widget;
using NSU.Shared.NSUUI;
using Android.Views;
using NSU.Shared.NSUSystemPart;
using NSU.Shared;
using NSUAppShared.NSUSystemParts;
using Android.Graphics;

namespace NSU.Droid.UI.Elements
{

	public class UILabel : TextView, INSUUILabel
    {
        const string LogTag = "UILabel";
        Activity activity;
        float scale;
        string uiid = string.Empty;
        NSUColor fClr, bClr;
        NSUUIActions action = NSUUIActions.NoAction;
        string actParam;
        bool disposed;
        WoodBoiler wb;

        public UILabel(Context context, float scale)
            : base(context)
        {
            activity = (Activity)context;
            this.scale = scale;
			Gravity = GravityFlags.Left;
			LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            SetTextColor(Color.ParseColor("#ff0000ff"));
            SetTextSize(Android.Util.ComplexUnitType.Px, Utilities.ConvertDpToPixel(17, scale));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Free();
        }

        public void Free()
        {
            NSULog.Debug(string.Format("UILabel [UIID - '{0}']", UIID), "Free() is called.");
            if (!disposed)
            {
                disposed = true;
                if (action == NSUUIActions.ShowWoodBoilerStatus && wb != null)
                {
                    wb.OnStatusChange -= OnWoodBoilerStatusChangeHandler;
                    wb = null;
                }
            }
        }

        public void SetAction(NSUUIActions act, string param)
        {
            switch (act)
            {
                case NSUUIActions.ShowWoodBoilerStatus:
                    action = act;
                    actParam = param;
                    if (!App.Current.NSUSys.NSUSystemReady)
                    {
                        NSULog.Debug("UILabel.SetAction", "NSUSystem NOT Ready");
                        App.Current.NSUSys.OnNSUSystemReady += HandleOnNSUPartsReady;
                        return;
                    }
                    NSULog.Debug("UILabel.SetAction", "NSUSystem IS Ready");
					var wbs = (WoodBoilers)(App.Current.NSUSys.GetNSUSysPart(PartsTypes.WoodBoilers));
					wb = wbs.FindWoodBoiler(actParam);
                    if (wb != null)
                    {
						wb.OnStatusChange += OnWoodBoilerStatusChangeHandler;
						OnWoodBoilerStatusChangeHandler(wb, wb.Status);
                    }
                    break;
            }
        }

		void OnWoodBoilerStatusChangeHandler (WoodBoiler sender, WoodBoilerStatus status)
        {
            activity.RunOnUiThread(() =>
                {
                    UpdateStatus(status);
                });
        }

		void UpdateStatus(WoodBoilerStatus status)
        {
            NSULog.Debug("UILabel.UpdateStatus", status.ToString());
            switch (status)
            {
				case WoodBoilerStatus.UNKNOWN:
                    SetCaption("Būsena tikrinama");
                    break;
				case WoodBoilerStatus.UZGESES:
                    SetCaption("Užgesęs");
                    break;
				case WoodBoilerStatus.IKURIAMAS:
                    SetCaption("Įkuriamas");
                    break;
				case WoodBoilerStatus.KURENASI:
                    SetCaption("Kūrenasi");
                    break;
				case WoodBoilerStatus.GESTA:
                    SetCaption("Gęsta");
                    break;
            }
        }

        void HandleOnNSUPartsReady (object sender, EventArgs e)
        {
            App.Current.NSUSys.OnNSUSystemReady -= HandleOnNSUPartsReady;
            if (action == NSUUIActions.ShowWoodBoilerStatus)
            {
                SetAction(action, actParam);
            }            
        }

        public string Caption
        {
            get
            {
                return Text;
            }
            set
            {
                SetCaption(value);
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

        public new int Width
        {
            get{ return Utilities.ConvertPixelsToDp(LayoutParameters.Width, scale); }
            set{ LayoutParameters.Width = Utilities.ConvertDpToPixel(value, scale); }
        }

        public new int Height
        {
            get{ return Utilities.ConvertPixelsToDp(LayoutParameters.Height, scale); }
            set{ LayoutParameters.Height = Utilities.ConvertDpToPixel(value, scale); }
        }

        public void SetCaption(string str)
        {
            activity.RunOnUiThread(() => { Text = str; });
        }

        public void SetColor(NSUColor fclr, NSUColor bclr, NSUColor brdrclr)
        {
            //throw new NotImplementedException();
        }

        public void SetEnabled(bool value)
        {
            Enabled = value;
        }

        public void SetColor(NSUColor faceClr, NSUColor backClr)
        {
            fClr = faceClr;
            bClr = backClr;
        }


        public NSUUIClass UIClass { get { return NSUUIClass.Label; } }

        public string UIID{ get; set; }

        public NSUTextAlign TextAlign
        {
            get
            {
                switch (Gravity)
                {
                    case GravityFlags.Left:
                        return NSUTextAlign.AlignLeft;
                    case GravityFlags.Right:
                        return NSUTextAlign.AlignRight;
                    case GravityFlags.Center:
                        return NSUTextAlign.AlignCenter;
                    default:
                        return NSUTextAlign.AlignLeft;
                }

            }
            set
            {
                switch (value)
                {
                    case NSUTextAlign.AlignCenter:
                        Gravity = GravityFlags.Center;
                        break;
                    case NSUTextAlign.AlignLeft:
                        Gravity = GravityFlags.Left;
                        break;
                    case NSUTextAlign.AlignRight:
                        Gravity = GravityFlags.Right;
                        break;
                }
            }
        }

        public void AttachedToWindow()
        {

        }

        public void DeatachedFromWindow()
        {

        }
    }
}

