using Echo.Commands;
using Echo.Managers;
using Echo.Net;
using Echo.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using static Echo.Managers.EncryptionManager;

namespace Echo.Models
{
    public class Server
    {
        private readonly EchoClient _echo;

        private readonly IPAddress _ipAddress;

        private readonly int _port;

        private string _serverPassword { get; set; }

        private Socket _conn { get; set; }

        private bool _receiving { get; set; }

        private bool _anon { get; set; }

        private readonly User _user;

        private List<Channel> _channelList { get; set; }
        public ObservableCollection<ChannelViewModel> channelList { get; set; }

        private List<Client> _clientList { get; set; }
        public ObservableCollection<ChannelMemberViewModel> currentChannelClientList { get; set; }
        public ObservableCollection<MessageViewModel> currentChannelMessageList { get; set; }

        private List<Permission> _allPermissions { get; set; }

        private string _serverName { get; set; }

        private string _serverMOTD { get; set; }

        public Server(string ipAddress, int port, string password, User user, EchoClient echo)
        {
            try
            {
                _ipAddress = IPAddress.Parse(ipAddress);
            }
            catch (System.FormatException)
            {

                throw new InvalidOperationException();
            }
            _serverPassword = password;
            _port = port;
            _receiving = false;
            _user = user;
            _channelList = new List<Channel>();
            channelList = new ObservableCollection<ChannelViewModel>();
            _clientList = new List<Client>();
            currentChannelClientList = new ObservableCollection<ChannelMemberViewModel>();
            currentChannelMessageList = new ObservableCollection<MessageViewModel>();
            _echo = echo;

            _clientList.Add(new Client("Server", "NO_EID_SERVER", "#0000FF"));
        }

        public void AddChannel(string name)
        {
            Channel newChannel = new Channel(name);
            _channelList.Add(newChannel);
            channelList.Add(new ChannelViewModel(newChannel));
        }

        public Channel GetChannel(string name)
        {
            foreach (Channel c in _channelList)
            {
                if (c.GetName() == name)
                {
                    return c;
                }
            }
            return null;
        }

        public Channel GetCurrentChannel()
        {
            return _echo.GetCurrentChannel();
        }

        public void AddClient(Client c)
        {
            _clientList.Add(c);
        }

        public void AddClientVisible(Client c)
        {
            currentChannelClientList.Add(new ChannelMemberViewModel(c));
        }

        public void RemoveClient(string name)
        {
            foreach (Client c in _clientList)
            {
                if (c.GetUsername() == name)
                {
                    foreach (Channel chan in _channelList)
                    {
                        chan.RemoveUser(name);
                    }
                    _clientList.Remove(c);
                    break;
                }
            }
        }

        public void RemoveClientVisible(string name)
        {
            foreach (ChannelMemberViewModel c in currentChannelClientList)
            {
                if (c.ClientName == name)
                {
                    currentChannelClientList.Remove(c);
                    break;
                }
            }
        }

        public Client GetClientByName(string name)
        {
            foreach (Client c in _clientList)
            {
                if (c.GetUsername() == name && c.GetEchoID() != "unavailable")
                {
                    return c;
                }
            }
            return null;
        }

        public Client GetClientByID(string eID)
        {
            throw new NotImplementedException();
        }

        public Client GetHistoricalClient(string name)
        {
            foreach (Client c in _clientList)
            {
                if (c.GetUsername() == name && c.GetEchoID() == "unavailable")
                {
                    return c;
                }
            }
            return null;
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

        public async Task<bool> Handshake(bool anon)
        {
            return await Task.Run(() => {
                _anon = anon;
                if (_receiving)
                {
                    Disconnect();
                    _receiving = false;
                }
                if (Connect())
                {
                    try
                    {
                        string publicKey;
                        string privateKey;
                        if (anon)
                        {
                            RSACryptoServiceProvider rsa = EncryptionManager.GetTempRSAProvider();
                            TextWriter tw = new StringWriter();

                            EncryptionManager.PemKeyUtils.ExportPublicKey(rsa, tw);

                            publicKey = tw.ToString();

                            tw = new StringWriter();

                            EncryptionManager.PemKeyUtils.ExportPrivateKey(rsa, tw);

                            privateKey = tw.ToString();
                        } else
                        {
                            publicKey = EncryptionManager.GetRSAPublicKey();
                            privateKey = null;
                        }
                        Dictionary<string, string> message;
                        SendMessageToServer("serverInfoRequest", publicKey, enc: false);

                        message = ReceiveMessageFromServer(); // Receive serverInfo

                        KeyGenerator.SecretKey = KeyGenerator.GetUniqueKey(16);

                        SendMessageToServer("clientSecret", EncryptionManager.RSAEncryptWithPem(KeyGenerator.SecretKey, message["data"].ToString()), enc: false);

                        message = ReceiveMessageFromServer(true); // Receive gotSecret

                        string tokenEncrypted = message["data"];
                        string token;
                        if (anon)
                        {
                            token = EncryptionManager.RSADecryptWithPem(tokenEncrypted, privateKey);
                        } else
                        {
                            token = EncryptionManager.RSADecrypt(tokenEncrypted);
                        }                       

                        string version = ConfigManager.GetSetting("version");

                        List<string> connRequest = new List<string> { _user.Username, _serverPassword, version, token, "" };

                        string jsonConnRequest = JsonConvert.SerializeObject(connRequest);

                        SendMessageToServer("connectionRequest", jsonConnRequest);

                        message = ReceiveMessageFromServer(true);

                        if (message["messagetype"] == "CRAccepted")
                        {
                            NetworkManager.registerServer(this);
                            return true;
                        }
                        else if (message["messagetype"] == "CRDenied")
                        {
                            _receiving = false;
                            _conn?.Close();
                            _echo.connectionContext = message["data"];
                            return false;
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        _receiving = false;
                        _conn?.Close();
                        

                        if (ex is System.Net.Sockets.SocketException) {
                            _echo.connectionContext = "Failed to connect to server - err_socket_dropped";
                        } else if (ex is System.NullReferenceException) {
                            _echo.connectionContext = "Failed to connect to server - err_client_nullref";
                        } else if (ex is CryptographicException) {
                            _echo.connectionContext = "Failed to connect to server - err_key_failed";
                        } else {
                            _echo.connectionContext = "Failed to connect to server - err_unknown_err";
                        }

                        return false;
                    }
                }
                else
                {
                    _echo.connectionContext = "Failed to connect to server - err_no_connection";
                    return false;
                }
            });
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
                                    serverData.Handle(this, message);
                                }
                                break;
                            case "outboundMessage":
                                {
                                    outboundMessage.Handle(this, _echo, message);
                                }
                                break;
                            case "channelUpdate":
                                {
                                    channelUpdate.Handle(this, _echo, message);
                                }
                                break;
                            case "userlistUpdate":
                                {
                                    userlistUpdate.Handle(this, _echo, message);
                                }
                                break;
                            case "channelHistory":
                                {
                                    channelHistory.Handle(this, _echo, message);
                                }
                                break;
                            case "additionalHistory":
                                {
                                    additionalHistory.Handle(this, _echo, message);
                                }
                                break;
                            case "commandData":
                                {
                                    commandData.Handle(this, _echo, message);
                                }
                                break;
                            case "connectionTerminated":
                                {
                                    connectionTerminated.Handle(this, _echo, message);
                                }
                                break;
                            case "errorOccured":
                                {
                                    errorOccured.Handle(this, _echo, message);
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
                    App.Current.Dispatcher.Invoke(() => { _echo.ConnectionStatus = ConnectionStatus.Reconnecting;  });

                    bool reconnSuccess = false;

                    for (int reconnCounter = 0; reconnCounter < 3; reconnCounter++)
                    {                       
                        reconnSuccess = Task.Run(() => Handshake(_anon)).Result;

                        if (reconnSuccess)
                        {
                            
                            App.Current.Dispatcher.Invoke(() => {
                                this.currentChannelClientList.Clear();
                                this._clientList.Clear();
                                _echo.ConnectionStatus = ConnectionStatus.Connected;
                                ReceiveMessages();
                            });

                            //Task.Delay(500).ContinueWith(t => ServerInfoCheck());

                            break;
                        }
                    }
                    if (!reconnSuccess)
                    {                    
                        App.Current.Dispatcher.Invoke(() => {
                            _echo.connectionContext = "Failed to reconnect";
                            _echo.ConnectionStatus = ConnectionStatus.Terminated;
                            _echo.GetNavStore().CurrentViewModel = new ConnectionViewModel(_echo, _echo.GetNavStore());
                        });
                    }                                     
                }
            }
        }
    }
}
