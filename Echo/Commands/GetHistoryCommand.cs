using Echo.ViewModels;
using Echo.Models;
using Echo.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Echo.Commands
{
    public class GetHistoryCommand : CommandBase
    {
        private readonly ChatViewModel _chatViewModel;
        private readonly EchoClient _echo;
        private readonly NavigationStore _navigationStore;

        public GetHistoryCommand(ChatViewModel chatViewModel, EchoClient echo, NavigationStore navigationStore)
        {
            _chatViewModel = chatViewModel;
            _echo = echo;
            _navigationStore = navigationStore;

        }

        public override void Execute(object parameter)
        {
            Debug.WriteLine("test");
        }
    }
}
