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

namespace Echo.Managers
{
    class NetworkManager
    {
        public static Socket conn;
        public static string userID;
        public static bool receiving;

        public static Dictionary<string, object> serverInfo = new Dictionary<string, object>();

        public static bool Connect(string ip, int port)
        {
            if (ip == "127.0.0.1")
            {
                userID = EncryptionManager.SHA256HAsh(KeyGenerator.GetUniqueKey(16)); // Allows me to run two clients on the same device with the eIDs interfering
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
            }
            


            IPAddress ipAddress = IPAddress.Parse(ip);
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

            NetworkManager.conn = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                conn.Connect(remoteEP);
                receiving = true;
                return true;
            }
            catch (System.Net.Sockets.SocketException)
            {
                return false;
            }
        }

        public static void Disconnect()
        {
            SendMessage("disconnect", "");

            receiving = false;

            NetworkManager.serverInfo.Clear();

            conn.Close();

            VisualManager.Cleanup();
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
                            netQueue.Enqueue(jsonData);
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
                    
                    

                    foreach(string netMessage in netQueue)
                    {
                        List<string> encryptedData = JsonConvert.DeserializeObject<List<string>>(netMessage);

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
                    MessageBox.Show("Error - Connection Lost");
                    NetworkManager.serverInfo.Clear();
                    Disconnect();

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
                List<string> jsonEnc = JsonConvert.DeserializeObject<List<string>> (jsonData);
                string jsonMessage = EncryptionManager.Decrypt(jsonEnc[0], KeyGenerator.SecretKey, jsonEnc[1]);
                message = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonMessage);
            }           

            return message;   
        }
    }
}
