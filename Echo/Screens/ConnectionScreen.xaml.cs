using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Echo.Managers;
using Newtonsoft.Json;

namespace Echo.Screens
{
    /// <summary>
    /// Interaction logic for ConnectionScreen.xaml
    /// </summary>
    public partial class ConnectionScreen : Window
    {
        public ConnectionScreen()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (NetworkManager.receiving == true)
            {
                NetworkManager.Disconnect();
            }
            if (NetworkManager.Connect(txtBoxIPAddr.Text, Convert.ToInt32(txtBoxPort.Text), (bool)chkBoxAnon.IsChecked))
            {
                //lblConnStatus.Content = "connected";

                NetworkManager.SendMessage("serverInfoRequest", "", enc: false);

                Dictionary<string, string> message = NetworkManager.ReceiveMessage(); // Receive serverInfo

                KeyGenerator.SecretKey = KeyGenerator.GetUniqueKey(16);

                NetworkManager.SendMessage("clientSecret", EncryptionManager.RSAEncrypt(KeyGenerator.SecretKey, message["data"].ToString()), enc: false);

                message = NetworkManager.ReceiveMessage(true); // Receive gotSecret

                List<string> connRequest = new List<string> { txtBoxUsername.Text, txtBoxPass.Password };

                string jsonConnReq = JsonConvert.SerializeObject(connRequest);

                NetworkManager.SendMessage("connectionRequest", jsonConnReq);

                message = NetworkManager.ReceiveMessage(true);

                if (message["messagetype"] == "CRAccepted")
                {
                    NetworkManager.ReceiveMessages();
                    VisualManager.ClearUsers();
                    VisualManager.ClearChan();
                    Close();
                }
                else if (message["messagetype"] == "CRDenied")
                {
                    MessageBox.Show("Connection denied - " + message["data"]);
                }
            }
            else
            {
                MessageBox.Show("Connection failed");
            }
        }
    }
}
