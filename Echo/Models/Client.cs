using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Models
{
    class Client
    {
        private readonly string _username;

        private readonly string _eID;

        private Channel _currentChannel { get; set; }

        private List<Permission> _permissions { get; set; }

        public Client(string username, string eID)
        {
            _username = username;
            _eID = eID;
            _permissions = new List<Permission>();
        }
    }
}
