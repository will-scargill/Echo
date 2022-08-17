using Echo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Echo.Net
{
    public class serverData
    {
        public static void Handle(Server _server, Dictionary<string, string> message)
        {
            List<string> serverData = JsonConvert.DeserializeObject<List<string>>(message["data"]);
            List<string> channels = JsonConvert.DeserializeObject<List<string>>(serverData[0]);
            List<List<string>> userChannels = JsonConvert.DeserializeObject<List<List<string>>>(serverData[2]);
            
            string motd = serverData[1];

            foreach (string name in channels)
            {
                App.Current.Dispatcher.Invoke(() => {
                    _server.AddChannel(name);
                });          
            }

            foreach (List<string> user in userChannels)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    Client c = new Client(user[0], user[1], user[3]);
                    _server.GetChannel(user[2])?.AddUser(c);
                    _server.AddClient(c);
                });
            }
        }
    }
}
