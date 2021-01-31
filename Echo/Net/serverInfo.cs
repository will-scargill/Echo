using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Echo.Managers;
using Echo.Objects;

namespace Echo.Net
{
    class serverInfo
    {
        public static void Handle(Dictionary<string, string> message)
        {
            List<string> serverData = JsonConvert.DeserializeObject<List<string>>(message["data"]);
            List<string> channels = JsonConvert.DeserializeObject<List<string>>(serverData[0]);
            NetworkManager.serverInfo["channel"] = channels;
            NetworkManager.serverInfo["motd"] = serverData[1];

            MainWindow.main.Dispatcher.Invoke(() => {

                Message motdMessage = new Message("Server", NetworkManager.serverInfo["motd"].ToString(), DateTime.Now.ToString(), "#0000FF");

                MainWindow.main.listBoxMessages.Items.Add(motdMessage);

                foreach (string channel in channels)
                {
                    MainWindow.main.listBoxChannels.Items.Add(channel);
                }
            });
        }
    }
}
