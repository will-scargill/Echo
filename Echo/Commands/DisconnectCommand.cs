using Echo.Models;
using Echo.Stores;
using Echo.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Echo.Commands
{
    public class DisconnectCommand : CommandBase
    {
        private readonly EchoClient _echo;
        private readonly MainViewModel _mainViewModel;
        private readonly NavigationStore _navigationStore;

        public DisconnectCommand(EchoClient echo, MainViewModel mainViewModel, NavigationStore navigationStore)
        {
            _echo = echo;
            _mainViewModel = mainViewModel;
            _navigationStore = navigationStore;

            _mainViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_mainViewModel.Connected))
            {
                OnCanExecuteChanged();
            }
        }
        public override bool CanExecute(object parameter)
        {
            if (_mainViewModel.Connected)
            {
                return true;
            } else
            {
                return false;
            }
            
        }
        public override void Execute(object parameter)
        {
            _echo.GetServer()?.Disconnect();
            _echo.ConnectionStatus = ConnectionStatus.Disconnected;

            _navigationStore.CurrentViewModel = new ConnectionViewModel(_echo, _navigationStore);
        }
    }
}
