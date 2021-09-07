﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows;

using Echo.Managers;

namespace Echo.Net
{
    class connectionTerminated
    {
        public static void Handle(Dictionary<string, string> message)
        {
            NetworkManager.serverInfo.Clear();
            NetworkManager.receiving = false;
            VisualManager.Cleanup();

            if (message["subtype"] == "kick")
            {
                VisualManager.SystemMessage("You have been kicked from the server");
            }
            else if (message["subtype"] == "ban")
            {
                VisualManager.SystemMessage("You have been banned from the server");
            }
            else if (message["subtype"] == "shutdown")
            {
                VisualManager.SystemMessage("Connection terminated by the server");
            }
            else
            {
                VisualManager.SystemMessage("Connection terminated by the server");
            }
            VisualManager.SystemMessage(message["data"]);
        }
    }
}
