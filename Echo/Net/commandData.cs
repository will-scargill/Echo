using Echo.Managers;
using Echo.Models;
using Echo.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Echo.Net
{
    public class commandData
    {
        public static void Handle(Server server, EchoClient echo, Dictionary<string, string> message)
        {
            List<string> commandData = JsonConvert.DeserializeObject<List<string>>(message["data"]);

            List<string> metadata = JsonConvert.DeserializeObject<List<string>>(message["metadata"]);

            App.Current.Dispatcher.Invoke(() => {
                foreach (string line in commandData)
                {
                    DateTime formattedDate = VisualManager.UnixToDateTime(metadata[2]);
                    Message formattedMessage = new Message(server.GetClientByName("Server"), formattedDate, line);
                    server.currentChannelMessageList.Add(new MessageViewModel(formattedMessage));
                }               
            });
        }
    }
}
