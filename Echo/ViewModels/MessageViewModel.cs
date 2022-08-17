using Echo.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Media;

namespace Echo.ViewModels
{
    public class MessageViewModel : ViewModelBase
    {
        private readonly Message _message;

        public string Username => _message.GetSender().GetUsername();
        public string Timestamp => _message.GetVariableTimestamp();
        public string TimestampFull => _message.GetTimestamp();
        public string Content => _message.GetContent();
        public SolidColorBrush Colour => _message.GetSender().GetColour();

        public MessageViewModel(Message message)
        {
            _message = message;
        }
    }
}
