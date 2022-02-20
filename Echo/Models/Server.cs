using Echo.Managers;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Echo.Models
{
    class Server
    {
        private readonly IPAddress _ipAddress;

        private readonly int _port;

        private string _serverPassword { get; set; }

        private Socket _conn { get; set; }

        private bool _receiving { get; set; }

        private readonly User _user;

        private List<Channel> _channelList { get; set; }

        private List<Client> _clientList { get; set; }

        private List<Permission> _allPermissions { get; set; }

        private string _serverName { get; set; }

        private string _serverMOTD { get; set; }

        public Server(string ipAddress, int port, string password, User user)
        {
            try
            {
                _ipAddress = IPAddress.Parse(ipAddress);
            }
            catch (System.FormatException)
            {

                throw;
            }
            _serverPassword = password;
            _port = port;
            _receiving = false;
            _user = user;
            _channelList = new List<Channel>();
            _clientList = new List<Client>();
        }

        public bool Connect()
        {
            IPEndPoint remoteEndpoint = new IPEndPoint(_ipAddress, _port);
            _conn = new Socket(_ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _conn.Connect(remoteEndpoint);
                _receiving = true;
                return true;
            }
            catch (System.Net.Sockets.SocketException)
            {
                return false;
            }
        }

        public bool Handshake()
        {
            if (_receiving)
            {
                Disconnect();
                _receiving = false;
            }
            if (Connect())
            {
                try
                {
                    Dictionary<string, string> message;

                    SendMessageToServer("serverInfoRequest", "", enc: false);

                    message = ReceiveMessageFromServer(); // Receive serverInfo

                    KeyGenerator.SecretKey = KeyGenerator.GetUniqueKey(16);

                    SendMessageToServer("clientSecret", EncryptionManager.RSAEncrypt(KeyGenerator.SecretKey, message["data"].ToString()), enc: false);

                    message = ReceiveMessageFromServer(true); // Receive gotSecret

                    string version = ConfigManager.GetSetting("version");

                    List<string> connRequest = new List<string> { _user.Username, _serverPassword, version };

                    string jsonConnRequest = JsonConvert.SerializeObject(connRequest);

                    SendMessageToServer("connectionRequest", jsonConnRequest);

                    message = ReceiveMessageFromServer(true);

                    if (message["messagetype"] == "CRAccepted")
                    {
                        return true;
                    }
                    else if (message["messagetype"] == "CRDenied")
                    {
                        _receiving = false;
                        _conn?.Close();
                        return false;
                    }
                    return false;
                }
                catch (System.Net.Sockets.SocketException)
                {
                    _receiving = false;
                    _conn?.Close();
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void Disconnect()
        {
            SendMessageToServer("disconnect", "");

            _receiving = false;

            _conn.Close();
        }

        public void SendMessageToServer(string messagetype, string data, string subtype = "", List<string> metadata = null, bool enc = true)
        {
            Dictionary<string, string> message = new Dictionary<string, string>();

            message["userid"] = _user.eID;
            message["messagetype"] = messagetype;
            message["subtype"] = subtype;
            message["data"] = data;
            if (metadata == null)
            {
                message["metadata"] = JsonConvert.SerializeObject(new List<string>());
            }
            else
            {
                message["metadata"] = JsonConvert.SerializeObject(metadata);
            }

            string jsonMessage = JsonConvert.SerializeObject(message);

            byte[] msg;

            if (enc == true)
            {
                List<object> encryptedData = EncryptionManager.Encrypt(jsonMessage);
                string jsonEncrypted = JsonConvert.SerializeObject(encryptedData);

                msg = Encoding.UTF8.GetBytes(jsonEncrypted);
            }
            else
            {
                msg = Encoding.UTF8.GetBytes(jsonMessage);
            }

            if (_receiving)
            {
                try
                {
                    int bytesSent = _conn.Send(msg);
                }
                catch
                {

                }
            }
        }

        public Dictionary<string, string> ReceiveMessageFromServer(bool enc = false)
        {
            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] bytes = new byte[20480];
            int bytesRec = _conn.Receive(bytes);
            string jsonData = Encoding.UTF8.GetString(bytes, 0, bytesRec);

            Dictionary<string, string> message = new Dictionary<string, string>();

            if (enc == false)
            {
                message = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);
            }
            else
            {
                List<string> jsonEnc = JsonConvert.DeserializeObject<List<string>>(jsonData);
                string jsonMessage = EncryptionManager.Decrypt(jsonEnc[0], KeyGenerator.SecretKey, jsonEnc[1]);
                message = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonMessage);
            }

            return message;
        }

        public void ReceiveMessages()
        {
            ThreadStart receiveThreadReference = new ThreadStart(RecvLoop);
            Thread receiveThread = new Thread(receiveThreadReference);
            receiveThread.Start();
        }

        private void RecvLoop()
        {
            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] bytes = new byte[20480];
            try
            {
                Queue netQueue = new Queue();

                while (_receiving == true)
                {
                    int bytesRec = _conn.Receive(bytes);
                    string jsonData = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    netQueue.Clear();

                    if (jsonData.Length > 0)
                    {
                        if (jsonData[0].Equals('[') && jsonData[(jsonData.Length - 1)].Equals(']'))
                        {
                            if (jsonData.Contains("]["))
                            {
                                string[] splitMessages = jsonData.Split(new string[] { "][" }, StringSplitOptions.None);
                                for (int i = 0; i < splitMessages.Length; i++)
                                {
                                    if (splitMessages[i][0] != '[')
                                    {
                                        splitMessages[i] = "[" + splitMessages[i];
                                    }
                                    if (splitMessages[i][splitMessages[i].Length - 1] != ']')
                                    {
                                        splitMessages[i] += "]";
                                    }
                                    netQueue.Enqueue(splitMessages[i]);
                                }
                            }
                            else
                            {
                                netQueue.Enqueue(jsonData);
                            }
                        }
                        else
                        {
                            string incompleteMessage = jsonData;
                            bool messageComplete = false;
                            while (messageComplete == false)
                            {
                                bytesRec = _conn.Receive(bytes);
                                jsonData = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                                if (jsonData[(jsonData.Length - 1)].Equals(']'))
                                {
                                    messageComplete = true;
                                    incompleteMessage += jsonData;

                                    netQueue.Enqueue(incompleteMessage);
                                }
                                else
                                {
                                    incompleteMessage += jsonData;
                                }
                            }
                        }
                    }


                    foreach (string netMessage in netQueue)
                    {
                        List<string> encryptedData = null;
                        try
                        {
                            encryptedData = JsonConvert.DeserializeObject<List<string>>(netMessage);
                        }
                        catch (Newtonsoft.Json.JsonReaderException)
                        {
                            netQueue.Enqueue(jsonData.Substring(0, jsonData.IndexOf("][") + 1));
                            netQueue.Enqueue(jsonData.Substring(jsonData.IndexOf("][") + 1));
                            break;
                        }

                        jsonData = EncryptionManager.Decrypt(encryptedData[0], KeyGenerator.SecretKey, encryptedData[1]);

                        Dictionary<string, string> message = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);


                        switch (message["messagetype"])
                        {
                            case "serverData":
                                {

                                }
                                break;
                            case "outboundMessage":
                                {

                                }
                                break;
                            case "channelUpdate":
                                {

                                }
                                break;
                            case "userlistUpdate":
                                {

                                }
                                break;
                            case "channelHistory":
                                {

                                }
                                break;
                            case "additionalHistory":
                                {

                                }
                                break;
                            case "commandData":
                                {

                                }
                                break;
                            case "connectionTerminated":
                                {

                                }
                                break;
                            case "errorOccured":
                                {

                                }
                                break;
                            default:
                                {

                                }
                                break;

                        }
                    }
                }
            }
            catch (System.Net.Sockets.SocketException)
            {
                if (_receiving == true)
                {
                    Disconnect();
                    //VisualManager.SystemMessage("Error - Connection Lost");
                    //VisualManager.SystemMessage("Trying to reconnect");
                    for (int reconnCounter = 0; reconnCounter < 3; reconnCounter++)
                    {
                        bool reconnSuccess = Handshake();
                        if (reconnSuccess)
                        {
                            //VisualManager.SystemMessage("Reconnect successful");

                            //SendMessage("requestInfo", "");

                            // Task.Delay(500).ContinueWith(t => ServerInfoCheck());

                            break;
                        }
                    }

                }
            }
        }
    }
}
