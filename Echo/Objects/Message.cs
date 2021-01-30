using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Echo.Objects
{
    class Message
    {
        public string Username { get; set; }

        public string Content { get; set; }

        public string Datetime { get; set; }

        public string Colour { get; set; }

        public Message(string _username, string _content, string _datetime, string _colour)
        {
            Username = _username;
            Content = _content;
            Datetime = _datetime;
            Colour = _colour;
        }
    }
}
