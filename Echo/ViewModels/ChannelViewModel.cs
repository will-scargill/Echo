using Echo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.ViewModels
{
    public class ChannelViewModel : ViewModelBase
    {
        private readonly Channel _channel;

        public string ChannelName => _channel.GetName();

        public ChannelViewModel(Channel channel)
        {
            _channel = channel; 
        }
    }
}
