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
using Echo.Objects;

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

        private void menuLeaveChan_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxChannels.SelectedItem != null)
            {
                NetworkManager.SendMessage("leaveChannel", "");
                VisualManager.ClearMessages();
                VisualManager.ClearUsers();
                listBoxChannels.UnselectAll();
            }
        }

        private void listBoxChannels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxChannels.SelectedItem != null)
            {
                NetworkManager.SendMessage("changeChannel", listBoxChannels.SelectedItem.ToString());
                VisualManager.ClearMessages();
            }
        }

        private void listBoxMessages_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (NetworkManager.receiving == true && listBoxMessages.Items.Count >= 50)
            {
                Border border = (Border)VisualTreeHelper.GetChild(listBoxMessages, 0);
                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);

                if (scrollViewer.VerticalOffset == 0)
                {
                    NetworkManager.SendMessage("historyRequest", "");
                }
            }           
        }

        private void UsersContext_whois(object sender, RoutedEventArgs e)
        {
            if (listBoxUsers.SelectedItem != null)
            {
                NetworkManager.SendMessage("userMessage", "/whois " + listBoxUsers.SelectedItem.ToString());
            }
        }
        private void UsersContext_kick(object sender, RoutedEventArgs e)
        {
            if (listBoxUsers.SelectedItem != null)
            {
                NetworkManager.SendMessage("userMessage", "/kick " + listBoxUsers.SelectedItem.ToString());
            }
        }
        private void UsersContext_ban(object sender, RoutedEventArgs e)
        {
            if (listBoxUsers.SelectedItem != null)
            {
                NetworkManager.SendMessage("userMessage", "/ban " + listBoxUsers.SelectedItem.ToString());
            }
        }
        private void UsersContext_mute(object sender, RoutedEventArgs e)
        {
            if (listBoxUsers.SelectedItem != null)
            {
                NetworkManager.SendMessage("userMessage", "/mute " + listBoxUsers.SelectedItem.ToString());
            }
        }
        private void UsersContext_unmute(object sender, RoutedEventArgs e)
        {
            if (listBoxUsers.SelectedItem != null)
            {
                NetworkManager.SendMessage("userMessage", "/unmute " + listBoxUsers.SelectedItem.ToString());
            }
        }
        private void MessagesContext_copy(object sender, RoutedEventArgs e)
        {
            if (listBoxMessages.SelectedItem != null)
            {
                Message selectedMessage = (Message)listBoxMessages.SelectedItem;
                Clipboard.SetText(selectedMessage.Content);
            }
        }
    }
}
