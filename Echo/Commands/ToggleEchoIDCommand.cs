using Echo.Models;
using Echo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Echo.Commands
{
    public class ToggleEchoIDCommand : CommandBase
    {
        private readonly EchoClient _echo;
        private readonly SettingsViewModel _viewmodel;

        public ToggleEchoIDCommand(EchoClient echo, SettingsViewModel viewmodel)
        {
            _echo = echo;
            _viewmodel = viewmodel;

        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public override void Execute(object parameter)
        {
            if (_viewmodel.eIDHidden == System.Windows.Visibility.Visible)
            {
                _viewmodel.eIDHidden = System.Windows.Visibility.Hidden;
            } else
            {
                _viewmodel.eIDHidden = System.Windows.Visibility.Visible;
            }
        }
    }
}
