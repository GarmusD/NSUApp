using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using NSU.NSUSystem;
using NSU.Shared;
using NSU.Shared.NSUTypes;
using NSU.Shared.NSUSystemPart;
using System.Threading.Tasks;
using System.Threading;

#if WINDOWS_UWP
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.Storage.Streams;
#endif

namespace NSUAppShared.NSUSystemParts
{
    public class BinUploader : NSUSysPartsBase
    {
        public enum Stage
        {
            None,
            Upload,
            Flash,
            Verify
        };

        private const string LogTag = "BinUploader";
        private const int DATA_LENGTH = 550;

        public event EventHandler<BinUploaderProgressEventArgs> OnUploadStart;
        public event EventHandler<BinUploaderProgressEventArgs> OnUploadProgress;
        public event EventHandler<EventArgs> OnUploadFinish;

        public event EventHandler<BinUploaderProgressEventArgs> OnWriteStart;
        public event EventHandler<BinUploaderProgressEventArgs> OnWriteProgress;
        public event EventHandler<EventArgs> OnWriteFinish;

        public event EventHandler<BinUploaderProgressEventArgs> OnVerifyStart;
        public event EventHandler<BinUploaderProgressEventArgs> OnVerifyProgress;
        public event EventHandler<EventArgs> OnVerifyFinish;

        public event EventHandler<EventArgs> OnFlashSuccess;

        public event EventHandler<BinUploaderTextMsgEventArgs> OnInfoText;
        public event EventHandler<BinUploaderTextMsgEventArgs> OnError;

        public event EventHandler<EventArgs> OnAborted;

        public Stage CurrentStage { get { return stage; } }

#if WINDOWS_UWP
        private StorageFile fileToUpload;
#endif
        private byte[] buffer;
        private int totalParts;
        private int currentPart;
        private string hash;
        private Stage stage = Stage.None;
        private bool isAborted = false;

        public BinUploader(NSUSys sys, PartsTypes part) : base(sys, part)
        {
            //sys.OnNSUSystemUnavailable += OnNSUSystemUnavailable;
        }

        private void OnNSUSystemUnavailable(object sender, EventArgs e)
        {
            if (stage != Stage.None)
            {
                Abort();
                CleanUp();
            }
        }

        public override string[] RegisterTargets()
        {
            return new string[] { JKeys.BinUploader.TargetName };
        }

        public override void Clear()
        {
            //
        }
        public override async void ParseNetworkData(JObject data)
        {
            if (data.Property(JKeys.Generic.Action) != null)
            {
                string action = (string)data[JKeys.Generic.Action];
                switch (action)
                {
                    case JKeys.BinUploader.Abort:
                        if ((string)data[JKeys.Generic.Result] == JKeys.Result.Ok)
                        {
                            CleanUp();
                            OnAborted?.Invoke(this, new EventArgs());
                        }
                        break;

                    case JKeys.BinUploader.StartUpload:                        
                        if ((string)data[JKeys.Generic.Result] == JKeys.Result.Ok)
                        {
                            if (await PrepareAsync())
                            {
                                stage = Stage.Upload;
                                OnUploadStart?.Invoke(this, new BinUploaderProgressEventArgs(totalParts));
                                UploadBinFileAsync();
                            }
                        }
                        else if ((string)data[JKeys.Generic.Result] == JKeys.Result.Error)
                        {
                            OnError?.Invoke(this, new BinUploaderTextMsgEventArgs((string)data[JKeys.Generic.Message]));
                        }
                        break;

                    case JKeys.BinUploader.Data:
                        if ((string)data[JKeys.Generic.Result] == JKeys.Result.Ok)
                        {
                            //if (!isAborted)
                            //{
                            //Upload next part
                            //    UploadNextPart();
                            //}
                            OnUploadProgress?.Invoke(this, new BinUploaderProgressEventArgs((int)data[JKeys.Generic.Status]));
                        }
                        else if ((string)data[JKeys.Generic.Result] == JKeys.Result.Error)
                        {
                            isAborted = true;
                            OnError?.Invoke(this, new BinUploaderTextMsgEventArgs((string)data[JKeys.Generic.Message]));
                        }
                        break;

                    case JKeys.BinUploader.DataDone:
                        if((string)data[JKeys.Generic.Result] == JKeys.Result.Ok)
                        {
                            OnUploadFinish?.Invoke(this, new EventArgs());
                            if( ((string)data[JKeys.Generic.Value]).Equals(hash, StringComparison.OrdinalIgnoreCase))
                            {
                                //Start flashing
                                JObject jo = new JObject
                                {
                                    [JKeys.Generic.Target] = JKeys.BinUploader.TargetName,
                                    [JKeys.Generic.Action] = JKeys.BinUploader.StartFlash
                                };
                                SendCommand(jo);
                            }
                        }
                        else if((string)data[JKeys.Generic.Result] == JKeys.Result.Error)
                        {
                            OnError?.Invoke(this, new BinUploaderTextMsgEventArgs((string)data[JKeys.Generic.Message]));
                        }
                        //Nothing to do from here...
                        CleanUp();
                        break;

                    case JKeys.BinUploader.StartFlash:
                        if ((string)data[JKeys.Generic.Result] == JKeys.Result.Error)
                        {
                            if((string)data[JKeys.Generic.ErrCode] == JKeys.ErrCodes.BinUploader.BossacError)
                            OnError?.Invoke(this, new BinUploaderTextMsgEventArgs("Bossac-udoo error. Exit code "+(string)data[JKeys.Generic.Value]));
                        }
                        break;

                    case JKeys.BinUploader.FlashStarted:
                        stage = Stage.Flash;
                        OnWriteStart?.Invoke(this, new BinUploaderProgressEventArgs(100));
                        break;

                    case JKeys.BinUploader.VerifyStarted:
                        OnWriteFinish?.Invoke(this, new EventArgs());
                        stage = Stage.Verify;
                        OnVerifyStart?.Invoke(this, new BinUploaderProgressEventArgs(100));
                        break;

                    case JKeys.BinUploader.Progress:
                        switch (stage)
                        {
                            case Stage.None:
                                break;
                            case Stage.Upload:
                                break;
                            case Stage.Flash:
                                OnWriteProgress?.Invoke(this, new BinUploaderProgressEventArgs((int)data[JKeys.Generic.Value]));
                                break;
                            case Stage.Verify:
                                OnVerifyProgress?.Invoke(this, new BinUploaderProgressEventArgs((int)data[JKeys.Generic.Value]));
                                break;
                            default:
                                break;
                        }
                        break;
                    case JKeys.BinUploader.FlashDone:
                        OnVerifyFinish?.Invoke(this, new EventArgs());
                        CleanUp();
                        OnFlashSuccess?.Invoke(this, new EventArgs());
                        break;

                    case JKeys.BinUploader.InfoText:
                        OnInfoText?.Invoke(this, new BinUploaderTextMsgEventArgs((string)data[JKeys.Generic.Value]));
                        break;

                    default:
                        break;

                }
            }
        }

        public void StartUploadFile
    (
#if WINDOWS_UWP
            StorageFile file
#endif
            )
        {
            if (stage == Stage.None)
            {
#if WINDOWS_UWP
                fileToUpload = file;
#endif
                JObject jo = new JObject();
                jo[JKeys.Generic.Target] = JKeys.BinUploader.TargetName;
                jo[JKeys.Generic.Action] = JKeys.BinUploader.StartUpload;
                SendCommand(jo);
                stage = Stage.Upload;
            }
        }

        public void Abort()
        {
            if(stage == Stage.Upload)
            {
                isAborted = true;
                SendAbort();
            }
        }

        private void CleanUp()
        {
            isAborted = false;
            buffer = null;
            totalParts = 0;
            currentPart = 0;
            hash = string.Empty;
            stage = Stage.None;
        }

        private Task<bool> PrepareAsync()
        {
            return Task.Run(async () => 
            { 
                try
                {
#if WINDOWS_UWP
                    var stream = (await fileToUpload.OpenReadAsync()).AsStreamForRead();
                    buffer = new byte[stream.Length];
                    await stream.ReadAsync(buffer, 0, buffer.Length);

                    var hAlgProv = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
                    var cbuff = CryptographicBuffer.CreateFromByteArray(buffer);
                    var ihash = hAlgProv.HashData(cbuff);
                    hash = CryptographicBuffer.EncodeToHexString(ihash);

                    fileToUpload = null;
#endif
                    

                    totalParts = buffer.Length / DATA_LENGTH;
                    if (totalParts * DATA_LENGTH < buffer.Length)
                        totalParts++;
                    currentPart = 0;

                    return true;
                }
                catch (Exception ex)
                {
                    NSULog.Exception(LogTag, "Prepare(): " + ex);
                }
                buffer = null;

                return false;
            });
        }

        private Task UploadBinFileAsync()
        {
            return Task.Factory.StartNew(async () => { while (UploadNextPart() && !isAborted) { await Task.Delay(20); } }, TaskCreationOptions.LongRunning);
        }
        
        private bool UploadNextPart()
        {
            if (currentPart < totalParts)
            {
                int remL = ((currentPart * DATA_LENGTH) + DATA_LENGTH) > buffer.Length ? buffer.Length - (currentPart * DATA_LENGTH) : DATA_LENGTH;
                JObject jo = new JObject
                {
                    [JKeys.Generic.Target] = JKeys.BinUploader.TargetName,
                    [JKeys.Generic.Action] = JKeys.BinUploader.Data,
                    [JKeys.Generic.Value] = Convert.ToBase64String(buffer, currentPart * DATA_LENGTH, remL),
                    [JKeys.Generic.Status] = currentPart
                };
                SendCommand(jo);
                //OnUploadProgress?.Invoke(this, new BinUploaderProgressEventArgs(currentPart));
                currentPart++;
                return true;
            }
            else
            {
                JObject jo = new JObject
                {
                    [JKeys.Generic.Target] = JKeys.BinUploader.TargetName,
                    [JKeys.Generic.Action] = JKeys.BinUploader.DataDone
                };
                SendCommand(jo);
                return false;
            }
        }

        private void SendAbort()
        {
            JObject jo = new JObject
            {
                [JKeys.Generic.Target] = JKeys.BinUploader.TargetName,
                [JKeys.Generic.Action] = JKeys.BinUploader.Abort
            };
            SendCommand(jo);
        }


    }
}
