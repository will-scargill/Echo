using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Models
{
    class Message
    {
        private readonly Client _sender;

        private readonly DateTime _timestamp;

        private readonly string _content;

        public Message(Client sender, DateTime timestamp, string content)
        {
            _sender = sender;
            _timestamp = timestamp;
            _content = content;
        }
    }
}
