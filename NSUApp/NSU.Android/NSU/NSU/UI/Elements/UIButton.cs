using System;
using Android.Widget;
using Android.Content;
using Android.Graphics;
using Android.Util;
using NSU.Shared.NSUUI;
using NSU.NSUSystem;
using NSU.Shared;
using Android.App;
using NSUAppShared.NSUSystemParts;

namespace NSU.Droid.UI.Elements
{
    public class UIButton : Button, INSUUIButton
    {
        private const string LogTag = "UIButton";

        private float scale = 1.0f;
        private bool disposed;
        private Context context;
        private Activity activity;
        private AlertDialog dialog;

        public UIButton(Context context, float scale)
            : base(context)
        {
            this.context = context;
            activity = (Activity)context;
            this.scale = scale;
            UIID = string.Empty;
			SetBackgroundResource(Resource.Drawable.button_shape);
            SetPadding(Utilities.ConvertDpToPixel(5, scale), PaddingTop, Utilities.ConvertDpToPixel(5, scale), PaddingBottom);
            LayoutParameters = new Android.Views.ViewGroup.LayoutParams(Utilities.ConvertDpToPixel(100, scale), Utilities.ConvertDpToPixel(40, scale));

            SetTextColor(Color.Blue);
            SetTextSize(Android.Util.ComplexUnitType.Px, Utilities.ConvertDpToPixel(15, scale));

            SetMinimumHeight(0);
            SetMinimumWidth(0);
            SetPadding(2, 2, 2, 2);
            Click += HandleButtonClick;
            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemReady += OnNSUSystemReadyHandler;
            sys.OnNSUSystemUnavailable += OnNSUSystemUnavailableHandler;
            this.Enabled = sys.NSUSystemReady;
        }

        private void OnNSUSystemUnavailableHandler(object sender, EventArgs e)
        {
            NSULog.Debug(LogTag, $"OnNSUSystemUnavailableHandler(). UIID: {UIID}");
            SetEnabled(false);
        }

        private void OnNSUSystemReadyHandler(object sender, EventArgs e)
        {
            SetEnabled(true);
        }

        void HandleButtonClick(object sender, EventArgs e)
        {
            //do action
            Log.Debug(LogTag, "UIButton Click event.");
            if (Action == NSUUIActions.KatiloIkurimas)
            {
                if (NSUSys.Instance.GetNSUSysPart(PartsTypes.WoodBoilers) is WoodBoilers wbs)
                {
                    var wb = wbs.FindWoodBoiler(0);
                    if (wb != null)
                    {
                        AlertDialog.Builder b = new AlertDialog.Builder(Context);
                        b.SetTitle("Įkūrimas");
                        b.SetMessage("Ar tikrai norite įkurti?");
                        b.SetNegativeButton(Android.Resource.String.No, (s, ea) =>
                        {
                            dialog.Dismiss();
                        });
                        b.SetPositiveButton(Android.Resource.String.Yes, (s, ea) =>
                        {
                            dialog.Dismiss();
                            wb.Action(Shared.NSUSystemPart.NSUAction.WoodBoilerIkurimas);
                        });
                        dialog = b.Show();
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Free();
        }

        #region INSUUIButton implementation

        public void Free()
        {
            NSULog.Debug(string.Format("UIButton [UIID - '{0}']", UIID), "Free() is called.");
            if (!disposed)
            {
                NSUSys sys = NSUSys.Instance;
                sys.OnNSUSystemReady += OnNSUSystemReadyHandler;
                sys.OnNSUSystemUnavailable += OnNSUSystemUnavailableHandler;
                Click -= HandleButtonClick;
                disposed = true;
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
            get { return Utilities.ConvertPixelsToDp(LayoutParameters.Width, scale); }
            set { LayoutParameters.Width = Utilities.ConvertDpToPixel(value, scale); }
        }

        public new int Height
        {
            get { return Utilities.ConvertPixelsToDp(LayoutParameters.Height, scale); }
            set { LayoutParameters.Height = Utilities.ConvertDpToPixel(value, scale); }
        }

        public byte FontHeight
        {
            get
            {
                return 0;
            }
            set
            {
                //throw new NotImplementedException();
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

        public NSUUIActions Action{ get; set; }

        public void SetCaption(string str)
        {
            activity.RunOnUiThread(() => {
                Text = str;
            });            
        }

        public void SetColor(NSUColor fclr, NSUColor bclr, NSUColor brdrclr)
        {
            throw new NotImplementedException();
        }

        public void SetEnabled(bool value)
        {
            activity.RunOnUiThread(() =>
            {
                Enabled = value;
            });
        }

        public void AttachedToWindow()
        {
            
        }

        public void DeatachedFromWindow()
        {
            
        }

        public NSUUIClass UIClass { get { return NSUUIClass.Button; } }

        public string UIID { get; set; }

        #endregion
    }
}

