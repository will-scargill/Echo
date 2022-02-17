using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Net.NetworkInformation;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using System.Threading;
using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Diagnostics;

using Echo.Net;
using Echo.Screens;
using System.Threading.Tasks;

namespace Echo.Managers
{
    class NetworkManager
    {
        public static Socket conn;
        public static string userID;
        public static bool receiving;

        public static Dictionary<string, object> serverInfo = new Dictionary<string, object>();
        private static Dictionary<string, object> handshakeInfo = new Dictionary<string, object>();

        public static bool Connect(string ip, int port, bool anon)
        {
            if (anon)
            {
                userID = EncryptionManager.SHA256HAsh(KeyGenerator.GetUniqueKey(32));
                VisualManager.SystemMessage("Connecting in anonymous mode...");
                VisualManager.SystemMessage("eID is " + userID);
                //userID = "testid";
            }
            else
            {
                string macAddr =
                (
                    from nic in NetworkInterface.GetAllNetworkInterfaces()
                    where nic.OperationalStatus == OperationalStatus.Up
                    select nic.GetPhysicalAddress().ToString()
                ).FirstOrDefault();

                userID = EncryptionManager.SHA256HAsh(macAddr);

                VisualManager.SystemMessage("Connecting...");
            }

            IPAddress ipAddress = IPAddress.Parse(ip);
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

            NetworkManager.conn = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                conn.Connect(remoteEP);
                receiving = true;
                VisualManager.SystemMessage("Connected to server");
                return true;
            }
            catch (System.Net.Sockets.SocketException)
            {
                VisualManager.SystemMessage("Failed to connect to server");
                return false;
            }
        }

        public static void Disconnect()
        {
            SendMessage("disconnect", "");

            receiving = false;

            NetworkManager.serverInfo.Clear();

            VisualManager.Cleanup();

            conn.Close();
        }

        public static void SendMessage(string messagetype, string data, string subtype = "", List<string> metadata = null, bool enc = true)
        {
            Dictionary<string, string> message = new Dictionary<string, string>();

            message["userid"] = userID;
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

            if (receiving)
            {
                try
                {
                    int bytesSent = conn.Send(msg);
                }
                catch
                {

                }
            }
        }

        public static void RecvLoop()
        {
            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] bytes = new byte[20480];
            try
            {
                Queue netQueue = new Queue();

                while (receiving == true)
                {
                    int bytesRec = conn.Receive(bytes);
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
                                    if (splitMessages[i][splitMessages[i].Length-1] != ']')
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
                                bytesRec = conn.Receive(bytes);
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
                                    Net.serverInfo.Handle(message);
                                }
                                break;
                            case "outboundMessage":
                                {
                                    Net.outboundMessage.Handle(message);
                                }
                                break;
                            case "channelUpdate":
                                {
                                    Net.channelUpdate.Handle(message);
                                }
                                break;
                            case "userlistUpdate":
                                {
                                    // Placeholder
                                }
                                break;
                            case "channelHistory":
                                {
                                    Net.channelHistory.Handle(message);
                                }
                                break;
                            case "additionalHistory":
                                {
                                    Net.additionalHistory.Handle(message);
                                }
                                break;
                            case "commandData":
                                {
                                    Net.commandData.Handle(message);
                                }
                                break;
                            case "connectionTerminated":
                                {
                                    Net.connectionTerminated.Handle(message);
                                }
                                break;
                            case "errorOccured":
                                {
                                    MessageBox.Show("Error - " + message["data"]);
                                }
                                break;
                            default:
                                {
                                    MessageBox.Show("Error - Unknown messagetype received");
                                }
                                break;
                           
                        }
                    }
                }
            }
            catch (System.Net.Sockets.SocketException)
            {
                if (receiving == true)
                {               
                    Disconnect();
                    VisualManager.SystemMessage("Error - Connection Lost");
                    VisualManager.SystemMessage("Trying to reconnect");
                    for (int reconnCounter = 0; reconnCounter < 3; reconnCounter++)
                    {
                        bool reconnSuccess = NetworkManager.Handshake((string)handshakeInfo["ip"], (int)handshakeInfo["port"], (bool)handshakeInfo["anon"], (string)handshakeInfo["username"], (string)handshakeInfo["password"]);
                        if (reconnSuccess)
                        {
                            VisualManager.SystemMessage("Reconnect successful");

                            //SendMessage("requestInfo", "");

                            // Task.Delay(500).ContinueWith(t => ServerInfoCheck());

                            break;
                        }
                    }              
                }
            }
        }


        public static void ReceiveMessages()
        {
            ThreadStart childThreadRef = new ThreadStart(RecvLoop);
            Thread childThread = new Thread(childThreadRef);
            childThread.Start();
        }

        public static Dictionary<string, string> ReceiveMessage(bool enc = false)
        {   
            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] bytes = new byte[20480];
            int bytesRec = conn.Receive(bytes);
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

        public static bool Handshake(string IPAddr, int port, bool anon, string username, string password, ConnectionScreen connSc = null)
        {
            if (NetworkManager.receiving == true)
            {
                NetworkManager.Disconnect();
                return true;
            }
            if (NetworkManager.Connect(IPAddr, port, (bool)anon))
            {
                try
                {
                    NetworkManager.SendMessage("serverInfoRequest", "", enc: false);

                    Dictionary<string, string> message = NetworkManager.ReceiveMessage(); // Receive serverInfo

                    KeyGenerator.SecretKey = KeyGenerator.GetUniqueKey(16);

                    NetworkManager.SendMessage("clientSecret", EncryptionManager.RSAEncrypt(KeyGenerator.SecretKey, message["data"].ToString()), enc: false);

                    message = NetworkManager.ReceiveMessage(true); // Receive gotSecret

                    string version = ConfigManager.GetSetting("version");

                    List<string> connRequest = new List<string> { username, password, version };

                    string jsonConnReq = JsonConvert.SerializeObject(connRequest);

                    NetworkManager.SendMessage("connectionRequest", jsonConnReq);

                    message = NetworkManager.ReceiveMessage(true);

                    if (message["messagetype"] == "CRAccepted")
                    {
                        NetworkManager.ReceiveMessages();
                        VisualManager.ClearUsers();
                        VisualManager.ClearChan();
                        VisualManager.SystemMessage("Handshake complete");
                        if (connSc != null)
                        {
                            connSc.Close();
                        }
                        handshakeInfo["ip"] = IPAddr;
                        handshakeInfo["port"] = port;
                        handshakeInfo["username"] = username;
                        handshakeInfo["password"] = password;
                        handshakeInfo["anon"] = anon;

                        SendMessage("requestInfo", "");

                        return true;
                    }
                    else if (message["messagetype"] == "CRDenied")
                    {
                        VisualManager.SystemMessage("Connection denied - " + message["data"]);
                        if (connSc != null)
                        {
                            connSc.Close();
                        }
                        return false;
                    }
                    return false;
                }
                catch (System.Net.Sockets.SocketException)
                {

                    VisualManager.SystemMessage("Connection was lost during handshake");
                    if (connSc != null)
                    {
                        connSc.Close();
                    }
                    return false;
                }
            }
            else
            {
                VisualManager.SystemMessage("Connection failed");
                return false;
            }
        }
    }
}

