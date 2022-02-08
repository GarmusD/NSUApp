using System;
using NSU.Shared.NSUUI;
using Android.Widget;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using NSU.NSUSystem;
using NSU.Shared;

namespace NSU.Droid.UI.Elements
{
    public class UIMonoBitmap : ImageView, INSUUIMonoBmp
    {
        public event EventHandler OnMonoBitmapClicked;
        float scale;
        NSUColor nsuclr = NSUColor.RGB(0, 0, 0);
        NSUBitmapResource br;
        Bitmap mbmp;
        int pxW, pxH;

        public UIMonoBitmap(Context context, float scale):base(context)
        {
            this.scale = scale;
            //SetScaleType(ScaleType.FitXy);
            LayoutParameters = new Android.Views.ViewGroup.LayoutParams(0, 0);
            Clickable = true;
            Click += UIMonoBitmapClickHandler;
        }

        private void UIMonoBitmapClickHandler(object sender, EventArgs e)
        {
            OnMonoBitmapClicked?.Invoke(this, null);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Free();
        }

        #region INSUUIMonoBmp implementation

        public void Free()
        {
            NSULog.Debug(String.Format("UIMonoBitmap [UIID - '{0}']", UIID), "Free() is called.");
            mbmp = null;
        }

        public void SetColor(NSUColor clr)
        {
            nsuclr = clr;
            ReColor(mbmp);
        }

        private void ReColor(Bitmap bmp)
        {
            if (bmp != null)
            {
                mbmp = bmp.Copy(Bitmap.Config.Argb8888, true);
                int w = mbmp.Width;
                int h = mbmp.Height;
                int[] pixels = new int[w * h];

                mbmp.GetPixels(pixels, 0, w, 0, 0, w, h);
                int clrWhite = Color.Argb(255, 255, 255, 255); ;// Color.White;
                int newC = Color.Argb(255, nsuclr.R, nsuclr.G, nsuclr.B);

                for(int i=0; i < w*h; i++)
                {
                    if (pixels[i] != clrWhite)
                    {
                        pixels[i] = newC;
                    }
                }
                mbmp.SetPixels(pixels, 0, w, 0, 0, w, h);
                SetImageBitmap(mbmp);
                Width = pxW;
                Height = pxH;

                /*
                mbmp = bmp.Copy(Bitmap.Config.Argb8888, true);
                if (mbmp != null)
                {
                    Color newC = Color.Rgb(nsuclr.R, nsuclr.G, nsuclr.B);
                    for (int i = 0; i < mbmp.Width; i++)
                    {
                        for (int j = 0; j < mbmp.Height; j++)
                        {
                            int cint = mbmp.GetPixel(i, j);
                            Color c = Color.Argb(Color.GetAlphaComponent(cint), Color.GetRedComponent(cint), Color.GetGreenComponent(cint), Color.GetBlueComponent(cint));
                            if (!c.Equals(Color.White))
                            {
                                mbmp.SetPixel(i, j, newC);
                            }
                        }
                    }
                    SetImageBitmap(mbmp);
                    Width = pxW;
                    Height = pxH;
                }
                */
            }
        }

        public void SetSize(int w, int h)
        {
            Width = w;
            Height = h;
        }

        public void SetResource(NSUBitmapResource r)
        {
            int rid = 0;
            br = r;
            switch (r)
            {
                case NSUBitmapResource.Akumuliacine:
                    rid = Resource.Drawable.UIImages_akumuliacine;
                    break;
                case NSUBitmapResource.Boileris:
                    rid = Resource.Drawable.UIImages_boileris;
                    break;
                case NSUBitmapResource.Cirkuliacinis:
                    rid = Resource.Drawable.UIImages_cirkuliacinis1;
                    break;
                case NSUBitmapResource.Grindys:
                    rid = Resource.Drawable.UIImages_grindys;
                    break;
                case NSUBitmapResource.Kaminas:
                    rid = Resource.Drawable.UIImages_kaminas;
                    break;
                case NSUBitmapResource.Katilas:
                    rid = Resource.Drawable.UIImages_katilas;
                    break;
                case NSUBitmapResource.Kolektorius:
                    rid = Resource.Drawable.UIImages_kolektorius;
                    break;
                case NSUBitmapResource.Ladomatas:
                    rid = Resource.Drawable.UIImages_ladomatas;
                    break;
                case NSUBitmapResource.Radiatorius:
                    rid = Resource.Drawable.UIImages_radiatorius;
                    break;
                case NSUBitmapResource.Trisakis:
                    rid = Resource.Drawable.UIImages_trisakis1;
                    break;
                case NSUBitmapResource.UnknownResource:
                    rid = Resource.Drawable.UIImages_unknown;
                    break;
                case NSUBitmapResource.Ventiliatorius:
                    rid = Resource.Drawable.UIImages_ventiliatorius2;
                    break;
            }
            Bitmap bmp = BitmapFactory.DecodeResource(Resources, rid);
            pxW = bmp.Width;
            pxH = bmp.Height;
            ReColor(bmp);
        }

        public void SetRotation(int rot)
        {
            int res = 0;
            switch (br)
            {
                case NSUBitmapResource.Cirkuliacinis:
                    switch (rot)
                    {
                        case 1:
                            res = Resource.Drawable.UIImages_cirkuliacinis1;
                            break;
                        case 2:
                            res = Resource.Drawable.UIImages_cirkuliacinis2;
                            break;
                        case 3:
                            res = Resource.Drawable.UIImages_cirkuliacinis3;
                            break;
                        case 4:
                            res = Resource.Drawable.UIImages_cirkuliacinis4;
                            break;
                        default:
                            res = Resource.Drawable.UIImages_cirkuliacinis1;
                            break;
                    }
                    Bitmap bmp = BitmapFactory.DecodeResource(Resources, res);
                    ReColor(bmp);
                    break;
                case NSUBitmapResource.Trisakis:
                    switch (rot)
                    {
                        case 1:
                            res = Resource.Drawable.UIImages_trisakis1;
                            break;
                        case 2:
                            res = Resource.Drawable.UIImages_trisakis2;
                            break;
                        case 3:
                            res = Resource.Drawable.UIImages_trisakis3;
                            break;
                        case 4:
                            res = Resource.Drawable.UIImages_trisakis4;
                            break;
                        default:
                            res = Resource.Drawable.UIImages_trisakis1;
                            break;
                    }
                    bmp = BitmapFactory.DecodeResource(Resources, res);
                    ReColor(bmp);
                    break;
            }

        }

        public INSUUIDrawer Drawer
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region INSUUIBase implementation

        public NSUUIClass UIClass{ get { return NSUUIClass.MonoBitmap; } }

        public string UIID{ get; set; }

        public new int Width
        {
            get
            {
                return Utilities.ConvertPixelsToDp(LayoutParameters.Width, scale);
            }
            set
            {
                pxW = value;
                LayoutParameters.Width = Utilities.ConvertDpToPixel(value, scale);
            }
        }

        public new int Height
        {
            get
            {
                return (int)Utilities.ConvertPixelsToDp(LayoutParameters.Height, scale);
            }
            set
            {
                pxH = value;
                LayoutParameters.Height = Utilities.ConvertDpToPixel(value, scale);
            }
        }

        public new int Left
        {
            get
            {
                return (int)Utilities.ConvertPixelsToDp((int)GetX(), scale);
            }
            set
            {
                //this.SetX(value);
                SetX((int)Utilities.ConvertDpToPixel(value, scale));
            }
        }

        public new int Top
        {
            get
            {
                return (int)Utilities.ConvertPixelsToDp((int)GetY(), scale);
            }
            set
            {
                //this.SetY(value);
                SetY((int)Utilities.ConvertDpToPixel(value, scale));
            }
        }

        public void AttachedToWindow()
        {

        }

        public void DeatachedFromWindow()
        {

        }
        #endregion
    }
}

