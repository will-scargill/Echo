using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
            txtBoxUsername.Text = ConfigManager.GetSetting("username");
            txtBoxIPAddr.Text = ConfigManager.GetSetting("last_ip");
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            ConfigManager.UpdateSetting("username", txtBoxUsername.Text);
            ConfigManager.UpdateSetting("last_ip", txtBoxIPAddr.Text);
            string ip;
            try
            {
                IPAddress.Parse(txtBoxIPAddr.Text); // parse this to check it is valid ip
                ip = txtBoxIPAddr.Text;
            }
            catch (System.FormatException)
            {
                try
                {
                    ip = (Dns.GetHostAddresses(txtBoxIPAddr.Text))[0].ToString();
                }
                catch (System.Net.Sockets.SocketException)
                {
                    ip = "0";
                }
            }

            int port;
            try
            {
                port = Convert.ToInt32(txtBoxPort.Text);
            }
            catch (System.FormatException) // NaN
            {
                port = 0;
            }
            catch (System.OverflowException) // Number too large for int32
            {
                port = 0;
            }

            if (port > 65535)
            {
                port = 0;
            }
            NetworkManager.Handshake(ip, port, (bool)chkBoxAnon.IsChecked, txtBoxUsername.Text, txtBoxPass.Password, connSc: this);
        }
    }
}
