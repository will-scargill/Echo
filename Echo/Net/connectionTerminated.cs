using Echo.Models;
using Echo.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Echo.ViewModels;
using System.Diagnostics;

namespace Echo.Net
{
    public class connectionTerminated
    {
        public static void Handle(Server _server, EchoClient _echo, Dictionary<string, string> message)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                _echo.GetServer()?.Disconnect();
                _echo.connectionContext = message["data"];
                _echo.ConnectionStatus = ConnectionStatus.Terminated;
                _echo.GetNavStore().CurrentViewModel = new ConnectionViewModel(_echo, _echo.GetNavStore());          
            });
        }
    }
}
