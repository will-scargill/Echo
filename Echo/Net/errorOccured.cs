using Echo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Echo.Net
{
    public class errorOccured
    {
        public static void Handle(Server _server, EchoClient _echo, Dictionary<string, string> message)
        {
            MessageBox.Show(message["data"], "Error");
        }
    }
}
