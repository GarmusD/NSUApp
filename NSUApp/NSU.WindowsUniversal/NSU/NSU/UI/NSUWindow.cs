using NSU.NSU_UWP.UI.Elements;
using NSU.Shared.NSUUI;
using NSU.Shared.NSUUI.NSUWindows;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace NSU.NSU_UWP.UI
{
    class NSUWindow
    {
        Canvas mainWindow;
        StackPanel sideWindow;

        public NSUWindow(Viewbox parent)//Panel
        {
            Grid grd = new Grid();
            var cd1 = new ColumnDefinition();
            cd1.Width = new GridLength(714);
            var cd2 = new ColumnDefinition();
            cd2.Width = new GridLength(86);
            grd.ColumnDefinitions.Add(cd1);
            grd.ColumnDefinitions.Add(cd2);

            //Column 1
            Border bdr = new Border();
            bdr.Height = 480;
            bdr.Width = 714;
            bdr.BorderBrush = new SolidColorBrush(Colors.Blue);
            bdr.BorderThickness = new Thickness(1.0);
            bdr.CornerRadius = new CornerRadius(5);

            Border bdr2 = new Border();
            bdr2.BorderBrush = new SolidColorBrush(Colors.Blue);
            bdr2.BorderThickness = new Thickness(1.0);
            bdr2.CornerRadius = new CornerRadius(5);
            bdr2.Margin = new Thickness(1);

            bdr.Child = bdr2;

            mainWindow = new Canvas();
            mainWindow.Margin = new Thickness(1);
            mainWindow.Background = new SolidColorBrush(Colors.White);

            bdr2.Child = mainWindow;
            Grid.SetColumn(bdr, 0);
            grd.Children.Add(bdr);

            //Column 2
            bdr = new Border();
            bdr.Height = 480;
            bdr.Width = 86;
            bdr.BorderBrush = new SolidColorBrush(Colors.Blue);
            bdr.BorderThickness = new Thickness(1.0);
            bdr.CornerRadius = new CornerRadius(5);

            bdr2 = new Border();
            bdr2.BorderBrush = new SolidColorBrush(Colors.Blue);
            bdr2.BorderThickness = new Thickness(1.0);
            bdr2.CornerRadius = new CornerRadius(5);
            bdr2.Margin = new Thickness(1);

            bdr.Child = bdr2;

            sideWindow = new StackPanel();
            sideWindow.Orientation = Orientation.Vertical;
            sideWindow.Background = new SolidColorBrush(Color.FromArgb(255, 75, 150, 190));
            bdr2.Child = sideWindow;
            //mainCanvas = new Canvas();
            //mainCanvas.Margin = new Thickness(1);
            //mainCanvas.Background = new SolidColorBrush(Colors.White);

            //bdr2.Child = mainCanvas;
            Grid.SetColumn(bdr, 1);
            grd.Children.Add(bdr);

            //Add grid to control
            parent.Child = grd;
            //parent.Children.Add(bdr);
        }

        public void AttachNSUWindow(INSUWindow wnd)
        {
            mainWindow.Children.Clear();
            if (wnd == null) return;
            for (int i = 0; i < wnd.Count; i++)
            {
                var item = wnd[i];
                switch (item.UIClass)
                {
                    case NSUUIClass.None:
                        break;
                    case NSUUIClass.MonoBitmap:
                        var mb = item as UIMonoBitmap;
                        mainWindow.Children.Add(mb.uiElement);
                        break;
                    case NSUUIClass.Button:
                        var btn = item as UIButton;
                        mainWindow.Children.Add(btn.uiElement);
                        break;
                    case NSUUIClass.Input:
                        break;
                    case NSUUIClass.Label:
                        var lbl = item as UILabel;
                        mainWindow.Children.Add(lbl.uiElement);
                        break;
                    case NSUUIClass.TempLabel:
                        var t = item as UITempLabel;
                        mainWindow.Children.Add(t.uiElement);
                        break;
                    case NSUUIClass.Graphics:
                        var g = item as UIGraphics;
                        mainWindow.Children.Add(g.uiElement);
                        break;
                    case NSUUIClass.SwitchButton:
                        break;
                    case NSUUIClass.Ladomat:
                        var l = item as UILadomatas;
                        mainWindow.Children.Add(l.uiElement);
                        break;
                    case NSUUIClass.Ventilator:
                        var v = item as UIExhaustFan;
                        mainWindow.Children.Add(v.uiElement);
                        break;
                    case NSUUIClass.CircPump:
                        var c = item as UICircPump;
                        mainWindow.Children.Add(c.uiElement);
                        break;
                    case NSUUIClass.TempBar:
                        var tb = item as UITempBar;
                        mainWindow.Children.Add(tb.uiElement);
                        break;
                    case NSUUIClass.ComfortZone:
                        var cz = item as UIComfortZone;
                        mainWindow.Children.Add(cz.uiElement);
                        break;
                    case NSUUIClass.WeatherInfo:
                        var wi = item as UIWeatherInfo;
                        mainWindow.Children.Add(wi.uiElement);
                        break;
                }
            }
        }

        public void AttachNSUSideWindow(INSUSideWindow swnd)
        {
            sideWindow.Children.Clear();
            if (swnd == null) return;
            for (int i = 0; i < swnd.Count; i++)
            {
                var item = swnd[i];
                sideWindow.Children.Add(item.uiElement);
                /*
                switch (item.UIClass)
                {
                    case NSUUISideElementClass.Switch:
                        var swt = item as UISideSwitch;
                        sideWindow.Children.Add(swt.uiElement);
                        break;
                    case NSUUISideElementClass.WindowSwitch:
                        var wc = item as UISideWindowSwitch;
                        sideWindow.Children.Add(wc.uiElement);
                        break;
                    case NSUUISideElementClass.GroupSwitch:
                        var gs = item as UISideGroupSwitch;
                        sideWindow.Children.Add
                        break;
                }
                */
            }
        }
    }
}
