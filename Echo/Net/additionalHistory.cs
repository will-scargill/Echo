using Echo.Managers;
using Echo.Models;
using Echo.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Echo.Net
{
    public class additionalHistory
    {
        public static void Handle(Server _server, EchoClient _echo, Dictionary<string, string> message)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            List<List<string>> channelHistory = JsonConvert.DeserializeObject<List<List<string>>>(message["data"]);
            Debug.WriteLine(channelHistory.Count.ToString() + " new messages to load");
            int newMessages = channelHistory.Count;

            App.Current.Dispatcher.Invoke(() =>
            {
                ListBox messageBox = VisualManager.getMessageBox();

                object posRetain = messageBox.Items.GetItemAt(0);

                foreach (List<string> m in channelHistory)
                {
                    Client historicalClient = _server.GetHistoricalClient(m[0]);
                    if (historicalClient == null)
                    {
                        Debug.WriteLine("creating client " + m[0]);
                        historicalClient = new Client(m[0], "unavailable", m[4]);
                        _server.AddClient(historicalClient);
                    }
                    DateTime formattedDate = VisualManager.UnixToDateTime(m[5]);
                    Message formattedMessage = new Message(historicalClient, formattedDate, m[3]);
                    _server.currentChannelMessageList.Insert(0, new MessageViewModel(formattedMessage));
                    
                }

                Border border = (Border)VisualTreeHelper.GetChild(messageBox, 0);
                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
                messageBox.ScrollIntoView(posRetain);            
                Mouse.OverrideCursor = null;
                if (newMessages >= 15)
                {
                    NetworkManager.blockHistory = false;
                }      
            });

            watch.Stop();
            Debug.WriteLine("Loaded in " + watch.ElapsedMilliseconds.ToString());
            channelHistory.Clear();
        }
    }
}
