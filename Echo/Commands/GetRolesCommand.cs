﻿using Echo.Models;
using Echo.Stores;
using Echo.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Echo.Commands
{
    public class GetRolesCommand : CommandBase
    {
        private readonly EchoClient _echo;
        private readonly MainViewModel _mainViewModel;
        public GetRolesCommand(EchoClient echo, MainViewModel mainViewModel)
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
            } else
            {
                return false;
            }
        }

        public override void Execute(object parameter)
        {
            _echo.GetServer().SendMessageToServer("userMessage", "/getroles");
        }
    }
}
