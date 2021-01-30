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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;

using Echo.Managers;
using Echo.Screens;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Echo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow main;
        public MainWindow()
        {
            InitializeComponent();
            main = this;       
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Debug.WriteLine("Height: " + MainWindow.main.Height + " | Width: " + MainWindow.main.Width);
        }

        private void menuConnect_Click(object sender, RoutedEventArgs e)
        {
            ConnectionScreen cscreen = new ConnectionScreen();

            cscreen.Show();
        }
        private void menuDisconn_Click(object sender, RoutedEventArgs e)
        {
            if (NetworkManager.receiving)
            {
                NetworkManager.Disconnect();
            }    
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (NetworkManager.receiving)
            {
                NetworkManager.Disconnect();
            }        
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (NetworkManager.receiving && txtBoxInput.Text.Trim().Length > 0)
            {
                NetworkManager.SendMessage(messagetype: "userMessage", data: txtBoxInput.Text);
                txtBoxInput.Text = "";
            }
            else
            {
                txtBoxInput.Text = "";
            }
        }

        private void txtBoxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (NetworkManager.receiving && txtBoxInput.Text.Trim().Length > 0)
                {
                    NetworkManager.SendMessage(messagetype: "userMessage", data: txtBoxInput.Text);
                    txtBoxInput.Text = "";
                }
                else
                {
                    txtBoxInput.Text = "";
                }
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e) // Focus onto textbox when key pressed
        {
            txtBoxInput.Focus();
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            if (NetworkManager.receiving)
            {
                NetworkManager.Disconnect();
            }
            Close();
        }

        private void listBoxChannels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxChannels.SelectedItem != null)
            {
                NetworkManager.SendMessage("changeChannel", listBoxChannels.SelectedItem.ToString());
                VisualManager.ClearMessages();
            }
        }
    }
}
