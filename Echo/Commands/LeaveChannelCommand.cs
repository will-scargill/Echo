using Echo.Models;
using Echo.ViewModels;
using Echo.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Input;

namespace Echo.Commands
{
    public class LeaveChannelCommand : CommandBase
    {
        private readonly EchoClient _echo;
        private readonly MainViewModel _mainViewModel;

        public LeaveChannelCommand(EchoClient echo, MainViewModel mainViewModel)
        {
            _echo = echo;
            _mainViewModel = mainViewModel;

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
            }
            else
            {
                return false;
            }
        }

        public override void Execute(object parameter)
        {
            if (_echo.GetCurrentChannel() is not null)
            {
                ((ChatViewModel)_mainViewModel.CurrentViewModel).SelectedChannel = null;
            }        
        }
    }
}
