using Echo.Models;
using Echo.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Echo.Net
{
    public class channelUpdate
    {
        public static void Handle(Server _server, EchoClient _echo, Dictionary<string, string> message)
        {
            List<string> update = JsonConvert.DeserializeObject<List<string>>(message["data"]);

            App.Current.Dispatcher.Invoke(() =>
            {
                _server.GetClientByName(update[0])?.SetChannel(update[2], _server);
                _server.GetChannel(update[1])?.RemoveUser(update[0]);
                _server.GetChannel(update[2])?.AddUser(_server.GetClientByName(update[0]));
                if (update[1] == _echo.GetCurrentChannel()?.GetName())
                {
                    _server.RemoveClientVisible(update[0]);
                }
                else if (update[2] == _echo.GetCurrentChannel()?.GetName() && _echo.GetCurrentChannel() is not null)
                {
                    _server.AddClientVisible(_server.GetClientByName(update[0]));
                }

            });
        }
    }
}
