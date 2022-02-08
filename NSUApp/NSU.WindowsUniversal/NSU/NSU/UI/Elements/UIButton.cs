using System;
using NSU.Shared.NSUUI;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using System.Threading;
using NSU.NSUSystem;
using NSU.Shared;
using NSUAppShared.NSUSystemParts;
using Windows.UI.Popups;
using NSU.Shared.NSUSystemPart;

namespace NSU.NSU_UWP.UI.Elements
{
    class UIButton : INSUUIButton
    {
        const string LogTag = "UIButton";
        //Canvas canvas;
        Button button;
        TranslateTransform tt;
        SynchronizationContext sc;

        public UIElement uiElement { get { return button; } }

        public UIButton(/*Canvas cnv*/)
        {
            button = new Button();
            button.Width = 100;
            button.Height = 44;
            button.FontSize = 15;
            button.Content = "Mygtukas";
            button.Background = new SolidColorBrush(Colors.White);
            button.BorderBrush = new SolidColorBrush(Colors.Blue);
            button.Foreground = new SolidColorBrush(Colors.Blue);
            button.Tapped += Button_Tapped;
            //button.
            tt = new TranslateTransform();
            tt.X = 0;
            tt.Y = 0;
            button.RenderTransform = tt;
            sc = SynchronizationContext.Current;
            NSUSys sys = NSUSys.Instance;
            sys.OnNSUSystemReady += OnNSUSystemReadyHandler;
            sys.OnNSUSystemUnavailable += OnNSUSystemUnavailableHandler;
            SetEnabled(sys.NSUSystemReady);
        }

        private async void Button_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            //do action
            NSULog.Debug(LogTag, "UIButton Click event.");
            if (Action == NSUUIActions.KatiloIkurimas)
            {
                if (NSUSys.Instance.GetNSUSysPart(PartsTypes.WoodBoilers) is WoodBoilers wbs)
                {
                    var wb = wbs.FindWoodBoiler(0);
                    if (wb != null)
                    {
                        // Create the message dialog and set its content
                        var messageDialog = new MessageDialog("Ar tikrai norite įkūrti?");

                        // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
                        messageDialog.Commands.Add(new UICommand(
                            "Taip",
                            new UICommandInvokedHandler(this.CommandInvokedHandler), wb));
                        messageDialog.Commands.Add(new UICommand(
                            "Ne",
                            new UICommandInvokedHandler(this.CommandInvokedHandler), null));

                        // Set the command that will be invoked by default
                        messageDialog.DefaultCommandIndex = 0;

                        // Set the command to be invoked when escape is pressed
                        messageDialog.CancelCommandIndex = 1;

                        // Show the message dialog
                        await messageDialog.ShowAsync();
                    }
                }
            }
        }

        private void CommandInvokedHandler(IUICommand command)
        {
            if(command.Id != null && command.Id is WoodBoiler)
            {
                (command.Id as WoodBoiler).Action(NSUAction.WoodBoilerIkurimas);
            }
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

        public void SetEnabled(bool value)
        {
            sc.Post((o) => {
                button.IsEnabled = value;
            }, null);            
        }

        public void Free()
        { 
            button = null;
            tt = null;
        }

        public void AttachedToWindow()
        {
            
        }

        public void DeatachedFromWindow()
        {
            
        }

        public byte FontHeight
        {
            get
            {
                return (byte)button.FontSize;
            }
            set
            {
                button.FontSize = value;
            }
        }

        public string Caption
        {
            get
            {
                return (string)button.Content;
            }
            set
            {
                sc.Post((o) => { button.Content = value; }, null);                
            }
        }

        public NSUUIClass UIClass { get { return NSUUIClass.Button; }  }

        public string UIID { get; set;  }        

        public int Width
        {
            get
            {
                return (int)button.Width;
            }
            set
            {
                button.Width = value;
            }
        }

        public int Height
        {
            get
            {
                return (int)button.Height;
            }
            set
            {
                button.Height = value;
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

        public NSUUIActions Action { get; set; }

    }
}
