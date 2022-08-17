using Echo.Models;
using Echo.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Echo.Commands
{
    public class ExitCommand : CommandBase
    {
        private readonly EchoClient _echo;
        public ExitCommand(EchoClient echo)
        {
            _echo = echo;
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public override void Execute(object parameter)
        {
            if (_echo.ConnectionStatus == ConnectionStatus.Connected)
            {
                _echo.GetServer()?.Disconnect();
                _echo.ConnectionStatus = ConnectionStatus.Disconnected;
            }
            

            System.Windows.Application.Current.Shutdown();
        }
    }
}
