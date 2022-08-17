using Echo.Models;
using Echo.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Echo.Commands
{
    public class UserContextCommand : CommandBase
    {
        private readonly ChatViewModel _chatViewModel;
        private readonly EchoClient _echo;

        public UserContextCommand(ChatViewModel vm, EchoClient echo)
        {
            _chatViewModel = vm;
            _echo = echo;

            _chatViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ChatViewModel.SelectedClient))
            {
                OnCanExecuteChanged();
            }
        }

        public override bool CanExecute(object parameter)
        {
            if (_chatViewModel.SelectedClient is null)
            {
                return false;
            } else
            {
                return true;
            }
        }
        public override void Execute(object parameter)
        {
            string msg = "/" + parameter.ToString().ToLower() + " " + _chatViewModel.SelectedClient.ClientName;
            Debug.WriteLine(msg);
            _echo.GetServer().SendMessageToServer("userMessage", msg);
        }
    }
}
