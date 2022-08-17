using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Models
{
    public class ServerPreset
    {
        public string _ipAddress { get; set; }
        public int _port { get; set; }
        public string _serverName { get; set; }
        public string _password { get; set; }

        public ServerPreset(string ipAddress, int port, string serverName, string password)
        {
            _ipAddress = ipAddress;
            _port = port;
            _serverName = serverName;
            _password = password;
        }
    }
}
