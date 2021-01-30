using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Echo.Managers;

namespace Echo.Net
{
    class channelUpdate
    {
        public static void Handle(Dictionary<string, string> message)
        {
            VisualManager.ClearUsers();

            List<string> userList = JsonConvert.DeserializeObject<List<string>>(message["data"]);

            MainWindow.main.Dispatcher.Invoke(()=> { 
                foreach (string user in userList)
                {
                    MainWindow.main.listBoxUsers.Items.Add(user);
                }
            });
        }
    }
}
