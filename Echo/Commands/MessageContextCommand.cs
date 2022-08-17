using Echo.Models;
using Echo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace Echo.Commands
{
    public class MessageContextCommand : CommandBase
    {
        private readonly ChatViewModel _chatViewModel;
        private readonly EchoClient _echo;

        public MessageContextCommand(ChatViewModel vm, EchoClient echo)
        {
            _chatViewModel = vm;
            _echo = echo;

            _chatViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ChatViewModel.SelectedMessage))
            {
                OnCanExecuteChanged();
            }
        }

        public override bool CanExecute(object parameter)
        {
            if (_chatViewModel.SelectedMessage is null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public override void Execute(object parameter)
        {
            switch (parameter)
            {
                case "copy_content":
                    Clipboard.SetText(_chatViewModel.SelectedMessage.Content);
                    return;
                case "copy_username":
                    Clipboard.SetText(_chatViewModel.SelectedMessage.Username);
                    return;
                case "copy_timestamp":
                    Clipboard.SetText(_chatViewModel.SelectedMessage.TimestampFull);
                    return;
                case "delete":
                    return;
                default:
                    return;
            }
        }
    }
}
