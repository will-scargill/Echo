using Echo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Net
{
    public class userlistUpdate
    {
        public static void Handle(Server _server, EchoClient _echo, Dictionary<string, string> message)
        {
            List<string> update = JsonConvert.DeserializeObject<List<string>>(message["data"]);

            if (update[3] == "connected")
            {
                App.Current.Dispatcher.Invoke(() => {
                    Client newClient = new Client(update[0], update[1], update[2]);
                    _server.AddClient(newClient);
                });            
            } 
            else if (update[3] == "disconnected") 
            {
                _server.RemoveClient(update[0]);
                App.Current.Dispatcher.Invoke(() => {
                    _server.RemoveClientVisible(update[0]);
                });            
            }            
        }
    }
}
