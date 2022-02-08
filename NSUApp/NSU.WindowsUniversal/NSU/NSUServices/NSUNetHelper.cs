using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NSU.NSUServices
{
    public enum DataType
    {
        DT_Unknown,
        DT_Binary,
        DT_String
    };   

    class NSUNetHelper
    {
        public class NetworkDataReceivedEventArgs
        {
            readonly string logTag = "NetworkDataReceivedEventArgs";
            private DataType datatype = DataType.DT_Unknown;
            private byte[] buff = null;
            private string str = String.Empty;
            private int count = 0;
            private int totalCount = 0;

            public DataType Datatype { get { return datatype; } }
            public int Count { get { return count; } }
            public int DataCount { get { return totalCount; } }

            public string GetAsString()
            {
                return str;
            }

            public byte[] GetAsBinary()
            {
                return buff;
            }

            public NetworkDataReceivedEventArgs(string value)
            {
                Debug.WriteLine(logTag + String.Format(". Creating NetworkDataReceivedArgs('{0}')", value));
                datatype = DataType.DT_String;
                str = new String(value.ToCharArray());
            }

            public NetworkDataReceivedEventArgs(byte[] data, int start, int length)
            {

                if (data[start] <= 2)
                {
                    datatype = (DataType)data[start];

                    if (datatype == DataType.DT_String)
                    {
                        count = length - 1;
                        totalCount = length;
                        str = Encoding.UTF8.GetString(data, start + 1, count);
                    }
                    else if (datatype == DataType.DT_Binary)
                    {
                        count = data[start + 1];
                        totalCount = count + 2;
                        buff = new byte[count];
                        Array.Copy(data, start + 2, buff, 0, count);
                    }
                }
            }
        }

        private class ArgBuilder
        {
            private readonly string logTag = "NSUNetHelper.ArgBuilder";
            byte[] buf;
            string remainer = String.Empty;
            int start, end;

            public bool DataAvailable { get { return start < end; } }

            public void Init(byte[] buf, int start, int end)
            {
                this.buf = buf;
                this.start = start;
                this.end = end;
            }

            public void Clear()
            {
                Debug.WriteLine("ArgBuilder.Clear()");
                buf = null;
                remainer = String.Empty;
                start = 0;
                end = 0;
            }

            public NetworkDataReceivedEventArgs BuildArgs()
            {
                if (buf == null || start >= buf.Length) return null;

                if (remainer.Length > 0 || (DataType)buf[start] == DataType.DT_String)
                {
                    StringBuilder sb = new StringBuilder();
                    bool eol = false;
                    sb.Clear();
                    if (remainer.Length > 0)
                    {
                        sb.Append(remainer);
                        remainer = String.Empty;
                    }
                    else
                    {
                        start++;
                    }
                    int i;
                    for (i = start; i < end; i++)
                    {
                        if (buf[i] != 10)
                        {
                            if (buf[i] >= 32)
                            {
                                sb.Append(Convert.ToChar(buf[i]));
                            }
                        }
                        else
                        {
                            eol = true;
                            break;
                        }
                    }
                    start = i + 1;
                    Debug.WriteLine(logTag + String.Format(". ArgBuilder. Start: {0}, End: {1}, DataAvailable: {2}", start, end, DataAvailable));
                    if (sb.Length > 0)
                    {
                        if (eol)
                        {
                            string new_str = remainer + sb.ToString();
                            remainer = String.Empty;
                            return new NetworkDataReceivedEventArgs(new_str);
                        }
                        else
                        {
                            remainer = sb.ToString();
                            Debug.WriteLine(logTag + String.Format(". Remainer is left: '{0}'", remainer));
                            return null;
                        }
                    }
                    else if (remainer.Length > 0)
                    {
                        return new NetworkDataReceivedEventArgs(remainer);
                    }
                    return null;
                }
                else
                {
                    return null;
                }
            }
        }
        /*
        private void ProccessNetworkData(byte[] buf, int count)
        {
            if (count > 0)
            {
                try
                {
                    NSULog.Debug("ProccessNetworkData", String.Format("Gautas buferis: [{0}];", GetReadableNetworkData(buf, count)));
                    //reset pingTimer;
                    ResetPinger();

                    argbuilder.Init(buf, 0, count);
                    while (argbuilder.DataAvailable)
                    {
                        NetworkDataReceivedEventArgs args = argbuilder.BuildArgs();

                        if (args != null)
                        {
                            if (args.Datatype == DataType.DT_String)
                            {
                                string resp_str = args.GetAsString().Trim();
                                if (!String.IsNullOrWhiteSpace(resp_str))
                                {
                                    NSULog.Debug("ProccessNetworkData", String.Format("NetStringReceived: '{0}'", resp_str));
                                    if (first_response)
                                    {
                                        first_response = false;
                                        if (resp_str.StartsWith("NSUServer"))
                                        {
                                            //Check version
                                            loginTimer.Reset();
                                            PrintOkResponse();
                                            //Compatibility
                                            if (resp_str.Contains("login:"))
                                            {
                                                SendString(usr_name);
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("Not a NSUServer or incompatible version.");
                                        }
                                    }
                                    else if (resp_str.Equals("login:"))
                                    {
                                        loginTimer.Reset();
                                        SendString(usr_name);
                                    }
                                    else if (resp_str.Equals("password:"))
                                    {
                                        loginTimer.Reset();
                                        SendString(password);
                                    }
                                    else if (resp_str.Equals("ready"))
                                    {
                                        loginTimer.Stop();
                                        PrintOkResponse();
                                        RaiseOnClientServerReady();
                                    }
                                    else if (resp_str.Equals("hello"))
                                    {
                                        loginTimer.Reset();
                                        PrintOkResponse();
                                        RaiseOnClientLoginSuccess();
                                    }
                                    else if (resp_str.StartsWith("param "))
                                    {
                                        loginTimer.Reset();
                                        string param = resp_str.Remove(0, "param ".Length);
                                        string[] parts = param.Split(':');
                                        RaiseOnClientParameterReceived(parts[0], parts[1]);
                                        PrintOkResponse();
                                    }
                                    else if (resp_str.Equals("goodbye"))
                                    {
                                        RaiseOnClientLoginFailure();
                                    }
                                    else if (resp_str.Equals("create admin"))
                                    {
                                        OnClientCreateAdmin();
                                    }
                                    else if (resp_str.Equals("PING"))
                                    {
                                        SendString("PONG");
                                    }
                                    else if (resp_str.Equals("DISCONNECT"))
                                    {
                                        socket.Disconnect();
                                        RaiseOnClientDisconnected();
                                    }
                                    else
                                    {
                                        NSULog.Debug(logTag, "Raising OnClientDataReceived event.");
                                        //RaiseOnClientDataReceived(new NetworkDataReceivedEventArgs(resp_str));
                                        //send to queue, to check

                                        OnClientDataReceivedString(new String(resp_str.ToCharArray()));
                                        queue.ResponseReceived(resp_str);
                                        //if netcmd in queue available, sent it to server
                                    }
                                }
                                resp_str = String.Empty;
                            }
                            else if (buf[0] == (byte)DataType.DT_Binary)
                            {
                                if (first_response)
                                {
                                    throw new Exception("Not a NSUServer.");
                                }
                            }
                            else
                            {
                                throw new Exception("Not a NSUServer.");
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    //ivyko fignia kazkokia
                    NSULog.Debug(logTag, "In Network runner: " + e.Message);
                    Disconnect();
                    autoReconnect.StartReconnect();
                }
            }
        }
        */
    }
}
