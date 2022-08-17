using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Media;

namespace Echo.Models
{
    public class Client
    {
        private readonly string _username;

        private readonly string _eID;

        private Channel _currentChannel { get; set; }

        private List<Permission> _permissions { get; set; }

        private SolidColorBrush _colour;

        public Client(string username, string eID, string colour)
        {
            _username = username;
            _eID = eID;
            _permissions = new List<Permission>();
            _colour = (SolidColorBrush)new BrushConverter().ConvertFrom(colour);
        }

        public string GetUsername()
        {
            return _username;
        }

        public string GetEchoID()
        {
            return _eID;
        }

        public SolidColorBrush GetColour()
        {
            return _colour;
        }

        public void SetChannel(string channelname, Server server)
        {
            _currentChannel = server.GetChannel(channelname);
        }
    }
}
