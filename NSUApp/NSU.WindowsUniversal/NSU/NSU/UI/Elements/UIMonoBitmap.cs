using NSU.Shared;
using NSU.Shared.NSUUI;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace NSU.NSU_UWP.UI.Elements
{
    class UIMonoBitmap : INSUUIMonoBmp
    {
        const string LogTag = "UIMonoBitmap";
        public event EventHandler OnMonoBmpTapped;

        //Canvas canvas;
        WriteableBitmap bmp = null;
        Color c;
        Image image;
        NSUBitmapResource res;
        TranslateTransform tt;
        //object lck;
        bool loading, reload;
        Uri loadUri;
        SynchronizationContext sc;

        public UIElement uiElement { get { return image; } }

        public UIMonoBitmap(/*Canvas cnv*/)
        {
            //lck = new object();
            //canvas = cnv;
            c = Colors.Black;
            image = new Image();
            tt = new TranslateTransform();
            tt.X = 0;
            tt.Y = 0;
            image.RenderTransform = tt;
            res = NSUBitmapResource.UnknownResource;
            //SetResource(res);
            image.Tapped += OnImageTapped;
            //canvas.Children.Add(image);
            sc = SynchronizationContext.Current;
        }

        private void OnImageTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            OnMonoBmpTapped?.Invoke(this, null);
        }

        async Task LoadAsync(Uri uri)
        {
            if (loading)
                return;
            loading = true;
            reload = false;
            bmp = await BitmapFactory.New(1, 1).FromContent(uri);

            if (!c.Equals(Colors.Black))
            {
                Recolor();
            }
            UpdateSource();
            loading = false;
            if (reload)
                await LoadAsync(loadUri);
        }

        private void UpdateSource()
        {
            sc.Post((o) => {
                image.Source = null;
                image.Source = bmp;
            }, null);
        }

        private void SetLoadUri(Uri uri)
        {
            loadUri = uri;
            if (loading)
            {
                reload = true; 
            }
            else
            {
                LoadAsync(loadUri);
            }
        }

        public void SetColor(Color clr)
        {
            NSULog.Debug(LogTag, $"SetColor(a:{clr.A}, r:{clr.R}, g:{clr.G}, b:{clr.B})");
            c = clr;
            Recolor();
        }

        void Recolor()
        {
            //lock (lck)
            //{
            try
            {
                if (bmp != null)
                {
                    for (int i = 0; i < bmp.PixelWidth; i++)
                    {
                        for (int j = 0; j < bmp.PixelHeight; j++)
                        {
                            Color pixc = bmp.GetPixel(i, j);
                            if (!pixc.Equals(Colors.White))
                            {
                                bmp.SetPixel(i, j, c);
                            }
                        }
                    }
                    UpdateSource();
                }
            }
            catch(Exception ex)
            {
                NSULog.Exception(LogTag, "Recolor() - " + ex);
            }
            //}
        }


        public void SetColor(NSUColor clr)
        {
            SetColor(Color.FromArgb(255, clr.R, clr.G, clr.B));
        }

        public void SetSize(int w, int h)
        {
            //
        }

        public void SetResource(NSUBitmapResource r)
        {
            res = r;
            string sres;
            switch (r)
            {
                case NSUBitmapResource.UnknownResource:
                    sres = UIConsts.ImgUIUnknown;
                    break;
                case NSUBitmapResource.Akumuliacine:
                    sres = UIConsts.ImgUIAkumuliacine;
                    break;
                case NSUBitmapResource.Boileris:
                    sres = UIConsts.ImgUIBoileris;
                    break;
                case NSUBitmapResource.Cirkuliacinis:
                    sres = UIConsts.ImgUICircPump1;
                    break;
                case NSUBitmapResource.Grindys:
                    sres = UIConsts.ImgUIGrindys;
                    break;
                case NSUBitmapResource.Kaminas:
                    sres = UIConsts.ImgUIKaminas;
                    break;
                case NSUBitmapResource.Katilas:
                    sres = UIConsts.ImgUIKatilas;
                    break;
                case NSUBitmapResource.Kolektorius:
                    sres = UIConsts.ImgUIKolektorius;
                    break;
                case NSUBitmapResource.Ladomatas:
                    sres = UIConsts.ImgUILadomatas;
                    break;
                case NSUBitmapResource.Radiatorius:
                    sres = UIConsts.ImgUIRadiatorius;
                    break;
                case NSUBitmapResource.Trisakis:
                    sres = UIConsts.ImgUITrisakis1;
                    break;
                case NSUBitmapResource.Ventiliatorius:
                    sres = UIConsts.ImgUIVentiliatorius;
                    break;
                default:
                    sres = UIConsts.ImgUIUnknown;
                    break;
            }
            //lock (lck)
            //{
            SetLoadUri(new Uri(sres));
            //}
        }

        public void SetRotation(int rot)
        {
            //lock (lck)
            //{
            switch (res)
            {
                case NSUBitmapResource.Cirkuliacinis:
                    switch (rot)
                    {
                        case 0:
                            SetLoadUri(new Uri(UIConsts.ImgUICircPump1));
                            break;
                        case 1:
                            SetLoadUri(new Uri(UIConsts.ImgUICircPump2));
                            break;
                        case 2:
                            SetLoadUri(new Uri(UIConsts.ImgUICircPump3));
                            break;
                        case 3:
                            SetLoadUri(new Uri(UIConsts.ImgUICircPump4));
                            break;
                    }
                    break;
                case NSUBitmapResource.Trisakis:
                    switch (rot)
                    {
                        case 0:
                            SetLoadUri(new Uri(UIConsts.ImgUITrisakis1));
                            break;
                        case 1:
                            SetLoadUri(new Uri(UIConsts.ImgUITrisakis2));
                            break;
                        case 2:
                            SetLoadUri(new Uri(UIConsts.ImgUITrisakis3));
                            break;
                        case 3:
                            SetLoadUri(new Uri(UIConsts.ImgUITrisakis4));
                            break;
                    }
                    break;
            }
            //}
        }

        public void Free()
        {
            //canvas.Children.Remove(image);
            bmp = null;
            image = null;
        }

        public void AttachedToWindow()
        {
            
        }

        public void DeatachedFromWindow()
        {
            
        }

        public INSUUIDrawer Drawer
        {
            get
            {
                return null;
            }
        }

        public NSUUIClass UIClass { get { return NSUUIClass.MonoBitmap; } }

        public string UIID { get; set; }

        public int Width
        {
            get
            {
                return (int)image.Width;
            }
            set
            {
                image.Width = value;
            }
        }

        public int Height
        {
            get
            {
                return (int)image.Height;
            }
            set
            {
                image.Height = value;
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
