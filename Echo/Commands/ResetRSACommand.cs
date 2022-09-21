using Echo.Models;
using Echo.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Echo.Commands
{
    public class ResetRSACommand : CommandBase
    {
        private readonly EchoClient _echo;
        public ResetRSACommand(EchoClient echo)
        {
            _echo = echo;
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public override void Execute(object parameter)
        {
            if (MessageBox.Show("Reset your RSA keys? This will cause all currently held permissions to be lost on ALL servers.", "RSA Reset", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                //do no stuff
            }
            else
            {
                EncryptionManager.RegenerateRSAPair();
                MessageBox.Show("Your RSA keys have been reset.");
            }
        }
    }
}
