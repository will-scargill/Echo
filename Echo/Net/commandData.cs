using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Echo.Objects;

namespace Echo.Net
{
    class commandData
    {
        public static void Handle(Dictionary<string, string> message)
        {
            if (message["subtype"] == "multiLine")
            {
                List<string> commandData = JsonConvert.DeserializeObject<List<string>>(message["data"]);

                List<string> metadata = JsonConvert.DeserializeObject<List<string>>(message["metadata"]);

                MainWindow.main.Dispatcher.Invoke(() => { 
                    foreach (string line in commandData)
                    {
                        Message newMessage = new Message(metadata[0], line, metadata[2], metadata[1]);
                        MainWindow.main.listBoxMessages.Items.Add(newMessage);
                    }
                });
            }
        }
    }
}
