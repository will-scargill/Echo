using Echo.Models;
using Echo.ViewModels;
using Echo.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Echo.Commands
{
    public class NewPresetCommand : CommandBase
    {
        private readonly ConnectionViewModel _connectionViewModel;
        private readonly EchoClient _echo;
        
        public NewPresetCommand(ConnectionViewModel vm, EchoClient echo)
        {
            _connectionViewModel = vm;
            _echo = echo;

            _connectionViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ConnectionViewModel.IPAddress) ||
                e.PropertyName == nameof(ConnectionViewModel.PresetName))
            {
                OnCanExecuteChanged();
            }
        }

        public override bool CanExecute(object parameter)
        {
            return (!string.IsNullOrEmpty(_connectionViewModel.IPAddress) &&
                !string.IsNullOrEmpty(_connectionViewModel.PresetName) &&
                base.CanExecute(parameter));
        }
        public override void Execute(object parameter)
        {
            _echo.AddPreset(_connectionViewModel.IPAddress, Convert.ToInt32(_connectionViewModel.Port),
                _connectionViewModel.PresetName, _connectionViewModel.Password);
            ConfigManager.AddPreset(_connectionViewModel.IPAddress,
                Convert.ToInt32(_connectionViewModel.Port),
                _connectionViewModel.PresetName,
                _connectionViewModel.Password);
        }
    }
}
