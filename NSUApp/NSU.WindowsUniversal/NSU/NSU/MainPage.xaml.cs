using NSU.NSU_UWP.Pages;
using NSU.NSUSystem;
using System;
using System.Linq;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using NSU.NSU_UWP.Dialogs;
using NSUAppShared;
using NSU.NSU_UWP.Net;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NSU.NSU_UWP
{
    public enum NotifyType
    {
        StatusMessage,
        ErrorMessage
    };

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NSUSys nsusys = null;

        private PageGrafikai pageGrafikai = null;
        private PageKatiline pageKatiline = null;
        private PageNustatymai pageNustatymai = null;

        public MainPage()
        {
            this.InitializeComponent();

            //TODO Load credentials from app preferences
            Credentials.Current.Host = NSUConsts.Host;// "nsu.dgs.lt";//"89.117.236.213";
            Credentials.Current.Port = 5152;
            Credentials.Current.UserName = "admin";
            Credentials.Current.Password = "admin";
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (nsusys == null)
            {
                //NotifyUser("Atsijungta...", NotifyType.ErrorMessage);
                StatusLabel.Text = "Atsijungta...";
                nsusys = NSUSys.Instance;
                nsusys.AddNSUNetworkSocket(new NSUSocket());
                nsusys.NSUNetwork.NetChangeNotifier = new NetNotifier();
                nsusys.NSUNetwork.Connect(Credentials.Current.Host, Credentials.Current.Port);
                PivotMain.SelectedIndex = 1;
            }
            nsusys.OnNSUSystemReady += OnNSUSystemReadyHandler;
            nsusys.OnNSUSystemUnavailable += OnNSUSystemUnavailableHandler;
            nsusys.NSUNetwork.OnConnectStarted += OnConnectStartedHandler;
            nsusys.NSUNetwork.OnConnectToServerTimeout += OnConnectToServerTimeoutHandler;
            nsusys.NSUNetwork.OnConnectFailure += OnConnectFailureHandler;
            nsusys.NSUNetwork.OnAttemptRecconect += OnAttemptRecconectHandler;
        }

        private void OnAttemptRecconectHandler(object sender, Shared.NSUNet.NetAttemptReconnectEventArgs args)
        {
            UpdateStatusText("Bandoma jungtis..." + args.ReconnectCount);
        }

        private void OnConnectFailureHandler(object sender, EventArgs e)
        {
            UpdateStatusText("Serveris nepasiekiamas...");
        }

        private void OnConnectToServerTimeoutHandler(object sender, EventArgs e)
        {
            UpdateStatusText("Baigėsi prisijungimui skirtas laikas...");
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (nsusys != null)
            {
                nsusys.OnNSUSystemReady -= OnNSUSystemReadyHandler;
                nsusys.OnNSUSystemUnavailable -= OnNSUSystemUnavailableHandler;
                nsusys.NSUNetwork.OnConnectStarted -= OnConnectStartedHandler;
                nsusys.NSUNetwork.OnConnectToServerTimeout -= OnConnectToServerTimeoutHandler;
                nsusys.NSUNetwork.OnConnectFailure -= OnConnectFailureHandler;
                nsusys.NSUNetwork.OnAttemptRecconect -= OnAttemptRecconectHandler;
            }
        }
        private void OnNSUSystemUnavailableHandler(object sender, EventArgs e)
        {
            UpdateStatusText("Atsijungta...");
            //NotifyUser("Atsijungta...", NotifyType.ErrorMessage);
        }

        private void OnConnectStartedHandler(object sender, EventArgs e)
        {
            ShowPB();
            UpdateStatusText("Jungiamasi...");
        }

        private void OnNSUSystemReadyHandler(object sender, EventArgs e)
        {
            HidePB();
            //NotifyUser("Prisijungta", NotifyType.StatusMessage);
            UpdateStatusText("Prisijungta...");
        }

        private async void ShowPB()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                ProgressBarTop.Opacity = 1.0;
            });
        }

        private async void HidePB()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ProgressBarTop.Opacity = 0.0;
            });
        }

        private async void UpdateStatusText(string msg)
        {
            await Dispatcher.RunAsync( Windows.UI.Core.CoreDispatcherPriority.Normal, ()=> {
                StatusLabel.Text = msg;
            });
        }
        /// <summary>
        /// Used to display messages to the user
        /// </summary>
        /// <param name="strMessage"></param>
        /// <param name="type"></param>
        public void NotifyUser(string strMessage, NotifyType type)
        {
            switch (type)
            {
                case NotifyType.StatusMessage:
                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                    break;
                case NotifyType.ErrorMessage:
                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                    break;
            }
            StatusBlock.Text = strMessage;

            // Collapse the StatusBlock if it has no text to conserve real estate.
            StatusBorder.Visibility = (StatusBlock.Text != String.Empty) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Splitter.IsPaneOpen = (Splitter.IsPaneOpen == true) ? false : true;
            StatusBorder.Visibility = Visibility.Collapsed;
        }

        private void PivotItemGrafikai_Loaded(object sender, RoutedEventArgs e)
        {
            if (pageGrafikai == null)
            {
                pageGrafikai = new PageGrafikai();
            }
            PivotItemGrafikai.Content = pageGrafikai;
        }

        private void PivotItemKatiline_Loaded(object sender, RoutedEventArgs e)
        {
            if (pageKatiline == null)
            {
                pageKatiline = new PageKatiline();
            }
            PivotItemKatiline.Content = pageKatiline;
        }

        private void PivotItemSettings_Loaded(object sender, RoutedEventArgs e)
        {
            if (pageNustatymai == null)
            {
                pageNustatymai = new PageNustatymai();
            }
            PivotItemSettings.Content = pageNustatymai;
        }

        private void ShowToast(string text)
        {
            var toastNotifier = ToastNotificationManager.CreateToastNotifier();
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            var textNodes = toastXml.GetElementsByTagName("text");
            textNodes.First().AppendChild(toastXml.CreateTextNode(text));
            var toastNotification = new ToastNotification(toastXml);
            toastNotifier.Show(new ToastNotification(toastXml));
        }

        private async void MenuListBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Splitter.IsPaneOpen = false;
            var obj = sender as ListBoxItem;
            if (obj != null)
            {
                if (obj.Tag.Equals("connect"))
                {
                    ConnectDialog cdlg = new ConnectDialog();
                    var res = await cdlg.ShowAsync();
                    if (res == ContentDialogResult.Primary)
                    {
                        Credentials.Current.UserName = cdlg.userNameTextBox.Text;
                        Credentials.Current.Password = cdlg.passwordTextBox.Password;
                        if (nsusys != null)
                        {
                            nsusys.NSUNetwork.Connect(NSUConsts.Host, 5152);
                        }
                    }
                    cdlg = null;
                }
            }
        }
    }
}
