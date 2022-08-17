using Echo.Models;
using Echo.ViewModels;
using Echo.Managers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading.Tasks;

namespace Echo.Net
{
    public class channelHistory
    {
        public static void Handle(Server _server, EchoClient _echo, Dictionary<string, string> message)
        {
            List<List<string>> channelHistory = JsonConvert.DeserializeObject<List<List<string>>>(message["data"]);


            App.Current.Dispatcher.Invoke(() =>
            {
                foreach (List<string> m in channelHistory)
                {
                    Client historicalClient;
                    if (_server.GetHistoricalClient(m[0]) == null)
                    {
                        historicalClient = new Client(m[0], "unavailable", m[4]);
                    }
                    else
                    {
                        historicalClient = _server.GetHistoricalClient(m[0]);
                    }
                    DateTime formattedDate = VisualManager.UnixToDateTime(m[5]);
                    Message formattedMessage = new Message(historicalClient, formattedDate, m[3]);
                    _server.currentChannelMessageList.Add(new MessageViewModel(formattedMessage));
                }

                ListBox messageBox = VisualManager.getMessageBox();
                Border border = (Border)VisualTreeHelper.GetChild(messageBox, 0);
                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();

                // jank workaround but I can't solve the issue otherwise
                Task.Delay(500).ContinueWith(_ => {
                    NetworkManager.blockHistory = false;
                });
            });
        }
    }
}
