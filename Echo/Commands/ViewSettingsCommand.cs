using Echo.Models;
using Echo.Stores;
using Echo.ViewModels;
using Echo.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;

namespace Echo.Commands
{
    public class ViewSettingsCommand : CommandBase
    {
        private readonly EchoClient _echo;
        private readonly NavigationStore _navigationStore;

        public ViewSettingsCommand(EchoClient echo, NavigationStore navigationStore)
        {
            _echo = echo;
            _navigationStore = navigationStore;

        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public override void Execute(object parameter)
        {
            if (_navigationStore.CurrentViewModel.GetType() == typeof(SettingsViewModel))
            {
                if (_echo.ConnectionStatus == ConnectionStatus.Connected)
                {
                    _navigationStore.CurrentViewModel = new ChatViewModel(_echo, _navigationStore);         
                    
                    // jank workaround but I can't solve the issue otherwise
                    Task.Delay(1000).ContinueWith(_ =>
                    {
                        NetworkManager.blockHistory = false;
                    });
                }
                else
                {
                    _navigationStore.CurrentViewModel = new ConnectionViewModel(_echo, _navigationStore);
                }
                
            }
            else
            {
                if (_navigationStore.CurrentViewModel.GetType() == typeof(ChatViewModel) && _echo.GetCurrentChannel() is not null)
                {
                    ((ChatViewModel)_navigationStore.CurrentViewModel).SelectedChannel = null;
                }
                _navigationStore.CurrentViewModel = new SettingsViewModel(_echo, _navigationStore);
                NetworkManager.blockHistory = true;
            }
        }
    }
}
