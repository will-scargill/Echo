using Echo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Media;

namespace Echo.ViewModels
{
    public class ChannelMemberViewModel : ViewModelBase
    {
        private readonly Client _client;

        public string ClientName => _client.GetUsername();

        public SolidColorBrush Colour => _client.GetColour();

        public ChannelMemberViewModel(Client client)
        {
            _client = client;   
        }
    }
}
