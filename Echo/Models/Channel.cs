using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Models
{
    class Channel
    {
        private readonly string _name;

        private List<Client> _channelMembers { get; set; }

        public Channel(string name)
        {
            _name = name;
            _channelMembers = new List<Client>();
        }
    }
}
