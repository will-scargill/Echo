using Echo.Models;
using Echo.ViewModels;
using Echo.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Echo.Commands
{
    public class DeletePresetCommand : CommandBase
    {
        private readonly EchoClient _echo;
        private readonly ConnectionViewModel _connectionViewModel;
        public DeletePresetCommand(EchoClient echo, ConnectionViewModel connectionViewModel)
        {
            _echo = echo;
            _connectionViewModel = connectionViewModel;
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public override void Execute(object parameter)
        {
            ServerPresetViewModel vm = _connectionViewModel.SelectedPreset;
            _echo.DeletePreset(vm);
            ConfigManager.DeletePreset(vm);
        }
    }
}
