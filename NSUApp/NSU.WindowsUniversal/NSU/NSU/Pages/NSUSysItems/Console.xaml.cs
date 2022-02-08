using Newtonsoft.Json.Linq;
using NSU.NSUSystem;
using NSU.Shared.NSUTypes;
using NSUAppShared.NSUSystemParts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Console = NSUAppShared.NSUSystemParts.Console;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace NSU.NSU_UWP.Pages.NSUSysItems
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConsolePage : Page
    {
        private BinUploader binUploader;
        private Console console;
        private string fileName = string.Empty;
        private bool logOutScrollEnabled = true;
        private bool uploaderScrollEnabled = true;
        public ConsolePage()
        {
            this.InitializeComponent();

            NSUSys sys = NSUSys.Instance;
            //sys.OnNSUSystemUnavailable += OnNSUSystemUnavailable;
            //sys.OnNSUSystemReady += OnNSUSystemReady;
            binUploader = sys.GetNSUSysPart(PartsTypes.BinUploader) as BinUploader;
            if (binUploader != null)
            {
                binUploader.OnError += BinUploader_OnError;
                binUploader.OnUploadStart += BinUploader_OnUploadStart;
                binUploader.OnUploadProgress += BinUploader_OnUploadProgress;
                binUploader.OnUploadFinish += BinUploader_OnUploadFinish;

                binUploader.OnWriteStart += BinUploader_OnFlashStart;
                binUploader.OnWriteProgress += BinUploader_OnFlashProgress;
                binUploader.OnWriteFinish += BinUploader_OnFlashFinish;

                binUploader.OnVerifyStart += BinUploader_OnVerifyStart;
                binUploader.OnVerifyProgress += BinUploader_OnVerifyProgress;
                binUploader.OnVerifyFinish += BinUploader_OnVerifyFinish;
                
                binUploader.OnFlashSuccess += BinUploader_OnFlashSuccess;

                binUploader.OnAborted += BinUploader_OnAborted;

                binUploader.OnInfoText += BinUploader_OnInfoText;
            }

            console = sys.GetNSUSysPart(PartsTypes.Console) as Console;
            if(console != null)
            {
                console.ConsoleOutput += Console_ConsoleOutput;
                console.Start();
            }
            

        }

        private void OnNSUSystemReady(object sender, EventArgs e)
        {
            btnPickFile.IsEnabled = true;
        }

        private void OnNSUSystemUnavailable(object sender, EventArgs e)
        {
            if (binUploader.CurrentStage != BinUploader.Stage.None)
            {
                AddInfoTextLine("Disconnected from server.");
                BinUploader_OnAborted(null, null);
            }
        }

        private void Console_ConsoleOutput(object sender, ConsoleOutEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbConsoleOut.Text))
                tbConsoleOut.Text += Environment.NewLine;
            tbConsoleOut.Text += e.Message;
            //Scroll to end
            if(logOutScrollEnabled)
                svConsoleOut.ChangeView(null, svConsoleOut.ScrollableHeight+1000, null);
        }    

        private async void btnPickFile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.ComputerFolder;
            picker.FileTypeFilter.Add(".bin");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {                
                this.tbFileName.Text = file.Name;

                pbUpload.Value = 0;
                pbWrite.Value = 0;
                pbVerify.Value = 0;

                binUploader.StartUploadFile(file);
            }
        }

        private void AddInfoTextLine(string text)
        {
            if (!string.IsNullOrEmpty(tbInfo.Text))
                tbInfo.Text += Environment.NewLine;
            tbInfo.Text += text;
            if(uploaderScrollEnabled)
                svUploaderInfo.ChangeView(null, svUploaderInfo.ScrollableHeight+1000, null);
        }

        private void btnAbort_Tapped(object sender, TappedRoutedEventArgs e)
        {
            binUploader.Abort();
        }

        private async void BinUploader_OnError(object sender, Shared.NSUSystemPart.BinUploaderTextMsgEventArgs e)
        {
            await mainGrid.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => 
            {
                if (!string.IsNullOrEmpty(tbInfo.Text))
                    tbInfo.Text += Environment.NewLine;
                tbInfo.Text += e.Message;

                btnAbort.IsEnabled = false;
                btnPickFile.IsEnabled = true;
            });
            
        }

        private async void BinUploader_OnInfoText(object sender, Shared.NSUSystemPart.BinUploaderTextMsgEventArgs e)
        {
            await mainGrid.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                AddInfoTextLine(e.Message);
            });
        }

        private async void BinUploader_OnFlashSuccess(object sender, EventArgs e)
        {
            await mainGrid.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                btnPickFile.IsEnabled = true;
                AddInfoTextLine(" Flash success.");
            });
        }

        private async void BinUploader_OnAborted(object sender, EventArgs e)
        {
            await mainGrid.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                btnPickFile.IsEnabled = true;
                btnAbort.IsEnabled = false;
                AddInfoTextLine("Aborted.");
            });
        }

        private async void BinUploader_OnUploadStart(object sender, Shared.NSUSystemPart.BinUploaderProgressEventArgs e)
        {
            await mainGrid.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => 
            {
                pbUpload.Minimum = 0;
                pbUpload.Value = 0;
                pbUpload.Maximum = e.Value;
                tbInfo.Text = string.Empty;
                btnAbort.IsEnabled = true;
                btnPickFile.IsEnabled = false;
                AddInfoTextLine("Upload started.");
            });
        }

        private async void BinUploader_OnUploadProgress(object sender, Shared.NSUSystemPart.BinUploaderProgressEventArgs e)
        {
            await mainGrid.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                pbUpload.Value = e.Value;
            });
        }

        private async void BinUploader_OnUploadFinish(object sender, EventArgs e)
        {
            await mainGrid.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                btnAbort.IsEnabled = false;
                AddInfoTextLine("Upload finished.");
            });
        }

        private async void BinUploader_OnFlashStart(object sender, Shared.NSUSystemPart.BinUploaderProgressEventArgs e)
        {
            await mainGrid.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                pbWrite.Maximum = e.Value;
                AddInfoTextLine("Write started.");
            });
        }

        private async void BinUploader_OnFlashProgress(object sender, Shared.NSUSystemPart.BinUploaderProgressEventArgs e)
        {
            await mainGrid.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                pbWrite.Value = e.Value;
            });
        }

        private async void BinUploader_OnFlashFinish(object sender, EventArgs e)
        {
            await mainGrid.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                AddInfoTextLine("Writed finished");
            });
        }

        private async void BinUploader_OnVerifyStart(object sender, Shared.NSUSystemPart.BinUploaderProgressEventArgs e)
        {
            await mainGrid.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                pbVerify.Maximum = e.Value;
                AddInfoTextLine("Verify started.");
            });
        }

        private async void BinUploader_OnVerifyProgress(object sender, Shared.NSUSystemPart.BinUploaderProgressEventArgs e)
        {
            await mainGrid.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                pbVerify.Value = e.Value;
            });
        }

        private async void BinUploader_OnVerifyFinish(object sender, EventArgs e)
        {
            await mainGrid.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                AddInfoTextLine("Verify finish.");
            });
        }

        private void ChangeVisibility_BinUploader(object sender, TappedRoutedEventArgs e)
        {
            switch (gridBinUploader.Visibility)
            {
                case Visibility.Visible:
                    gridBinUploader.Visibility = Visibility.Collapsed;
                    break;
                case Visibility.Collapsed:
                    gridBinUploader.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }            
        }

        private void ChangeVisibility_Console(object sender, TappedRoutedEventArgs e)
        {
            switch (gridConsole.Visibility)
            {
                case Visibility.Visible:
                    gridConsole.Visibility = Visibility.Collapsed;
                    break;
                case Visibility.Collapsed:
                    gridConsole.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private void svConsoleOut_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            logOutScrollEnabled = svConsoleOut.VerticalOffset >= svConsoleOut.ScrollableHeight - 100;
            System.Diagnostics.Debug.WriteLine($"svConsoleOut_ViewChanged():logOutScrollEnabled({logOutScrollEnabled}) = svConsoleOut.HorizontalOffset({svConsoleOut.VerticalOffset}) >= (svConsoleOut.ScrollableHeight - 100)({svConsoleOut.ScrollableHeight - 100});");
        }

        private void svUploaderInfo_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            uploaderScrollEnabled = svUploaderInfo.VerticalOffset >= svUploaderInfo.ScrollableHeight - 100;
        }

        private void btnSendCommand_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string inp = tbCommandInput.Text;
            if(!string.IsNullOrEmpty(inp))
            {
                tbCommandInput.Text = string.Empty;
                JObject jo = new JObject()
                {
                    [JKeys.Generic.Target] = JKeys.Console.TargetName,
                    [JKeys.Generic.Action] = JKeys.Console.ManualCommand,
                    [JKeys.Generic.Value] = inp
                };
                NSUSys nsusys = NSUSys.Instance;
                if (nsusys != null)
                {
                    if (nsusys.NSUNetwork.Connected())
                    {
                        nsusys.NSUNetwork.SendCommand(jo);
                    }
                }
            }
        }
    }
}
