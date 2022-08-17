using System;
using System.Collections.Generic;
using System.Text;
using Echo.Models;
using Newtonsoft.Json;
using Echo.Managers;
using Echo.ViewModels;

namespace Echo.Net
{
    public class outboundMessage
    {
        public static void Handle(Server _server, EchoClient _echo, Dictionary<string, string> message)
        {
            string messageContent = message["data"];

            List<string> metadata = JsonConvert.DeserializeObject<List<string>>(message["metadata"]);
          
            App.Current.Dispatcher.Invoke(() => {
                Message newMessage = new Message(_server.GetClientByName(metadata[0]), VisualManager.UnixToDateTime(metadata[2]), messageContent);
                _server.currentChannelMessageList.Add(new MessageViewModel(newMessage));
            });
        }
    }
}
