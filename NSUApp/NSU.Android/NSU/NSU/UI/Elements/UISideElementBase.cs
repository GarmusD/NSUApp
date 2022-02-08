using System;
using Android.Content;
using NSU.Shared.NSUUI;
using Android.Graphics;
using NSU.NSUSystem;
using NSUAppShared.NSUSystemParts;
using Android.Widget;
using static Android.Views.ViewGroup;

namespace NSU.Droid.UI.Elements
{
    public class UISideElementBase : ImageView, INSUUISideElementBase
    {
        public event EventHandler OnImageClicked;
        Context context;
        float scale;
        
        protected Bitmap bmpOn, bmpOff = null;
        //protected ImageView img;

        public NSUUISideElementClass UIClass
        {
            get
            {
                return NSUUISideElementClass.Unknown;
            }
        }

        public UISideElementBase(Context cntx, float scale):base(cntx)
        {
            context = cntx;
            this.scale = scale;
            Clickable = true;
            Click += ImgageClickedHandler;
            SetScaleType(ScaleType.FitXy);
            LayoutParameters = new LayoutParams(Utilities.ConvertDpToPixel(82, scale), Utilities.ConvertDpToPixel(70, scale));
        }

        public void ShowOnImage()
        {
            SetImageBitmap(null);
            SetImageBitmap(bmpOn);
        }

        public void ShowOffImage()
        {
            SetImageBitmap(null);
            SetImageBitmap(bmpOff);
        }

        private void ImgageClickedHandler(object sender, EventArgs e)
        {
            OnImageClicked?.Invoke(this, e);
        }

        Color From565To888(ushort value)
        {            
            byte R5 = (byte)((value & 0xf800) >> 11);
            byte G6 = (byte)((value & 0x07e0) >> 5);
            byte B5 = (byte)(value & 0x001f);

            return Color.Argb(255, (byte)((R5 * 527 + 23) >> 6), (byte)((G6 * 259 + 33) >> 6), (byte)((B5 * 527 + 23) >> 6));
        }

        public void SetOffBMPBytes(ushort[] data)
        {
            if (data != null)
            {
                int idx = 0;
                ushort w = data[idx++];
                ushort h = data[idx++];
                Bitmap.Config bc = Bitmap.Config.Argb8888;
                bmpOff = Bitmap.CreateBitmap(w, h, bc);                
                for (int j = 0; j < h; j++)
                    for (int i = 0; i < w; i++)
                        bmpOff.SetPixel(i, j, From565To888(data[idx++]));
            }
        }

        public void SetOnBMPBytes(ushort[] data)
        {
            if (data != null)
            {
                int idx = 0;
                ushort w = data[idx++];
                ushort h = data[idx++];
                Bitmap.Config bc = Bitmap.Config.Argb8888;
                bmpOn = Bitmap.CreateBitmap(w, h, bc);                
                for (int j = 0; j < h; j++)
                    for (int i = 0; i < w; i++)
                        bmpOn.SetPixel(i, j, From565To888(data[idx++]));
                SetImageBitmap(bmpOn);
            }
        }

        public void Free()
        {
            bmpOn?.Dispose();
            bmpOff?.Dispose();
            Dispose();
        }
    }
}