using NSU.Shared.NSUUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace NSU.NSU_UWP.UI.Elements
{
    public class UISideElementBase : INSUUISideElementBase
    {
        public event EventHandler OnImageClicked;

        protected WriteableBitmap bmpOn, bmpOff = null;
        Image image;
        //protected ImageView img;

        public UIElement uiElement { get { return image; } }

        public NSUUISideElementClass UIClass
        {
            get
            {
                return NSUUISideElementClass.Unknown;
            }
        }

        public UISideElementBase()
        {
            image = new Image();
            image.Tapped += Image_Tapped;
        }

        private void Image_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            OnImageClicked?.Invoke(this, new EventArgs());
        }

        public async void ShowOnImage()
        {
            await image.Dispatcher.RunAsync( Windows.UI.Core.CoreDispatcherPriority.Normal, ()=> {
                image.Source = null;
                image.Source = bmpOn;
            });
        }

        public async void ShowOffImage()
        {
            await image.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                image.Source = null;
                image.Source = bmpOff;
            });
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

            return Color.FromArgb(255, (byte)((R5 * 527 + 23) >> 6), (byte)((G6 * 259 + 33) >> 6), (byte)((B5 * 527 + 23) >> 6));
        }

        public void SetOffBMPBytes(ushort[] data)
        {
            if (data != null)
            {
                int idx = 0;
                ushort w = data[idx++];
                ushort h = data[idx++];
                bmpOff = BitmapFactory.New(w, h);
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
                bmpOn = BitmapFactory.New(w, h);
                for (int j = 0; j < h; j++)
                    for (int i = 0; i < w; i++)
                        bmpOn.SetPixel(i, j, From565To888(data[idx++]));
                ShowOnImage();
            }
        }

        public void Free()
        {
            throw new NotImplementedException();
        }
    }
}
