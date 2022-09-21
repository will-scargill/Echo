using Echo.Managers;
using Echo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Echo.Commands
{
    public class CopyRSAPublicCommand : CommandBase
    {
        private readonly EchoClient _echo;
        public CopyRSAPublicCommand(EchoClient echo)
        {
            _echo = echo;
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public override void Execute(object parameter)
        {
            string publicKey = EncryptionManager.GetRSAPublicKey();
            Clipboard.SetText(publicKey);
        }
    }
}
