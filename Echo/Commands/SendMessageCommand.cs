using Echo.Models;
using Echo.Stores;
using Echo.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Echo.Commands
{
    public class SendMessageCommand : CommandBase
    {
        private readonly ChatViewModel _chatViewModel;
        private readonly EchoClient _echo;
        private readonly NavigationStore _navigationStore;

        public SendMessageCommand(ChatViewModel chatViewModel, EchoClient echo, NavigationStore navigationStore)
        {
            _chatViewModel = chatViewModel;
            _echo = echo;
            _navigationStore = navigationStore;

        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public override void Execute(object parameter)
        {
            _echo.GetServer().SendMessageToServer("userMessage", _chatViewModel.UserMessage);
            _chatViewModel.UserMessage = "";
        }
    }
}
