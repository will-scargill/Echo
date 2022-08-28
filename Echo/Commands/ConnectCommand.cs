using Echo.Models;
using Echo.Stores;
using Echo.ViewModels;
using Echo.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Echo.Commands
{
    public class ConnectCommand : CommandBase
    {
        private readonly ConnectionViewModel _connectionViewModel;
        private readonly EchoClient _echo;
        private readonly NavigationStore _navigationStore;

        public ConnectCommand(ConnectionViewModel vm, EchoClient echo, NavigationStore navigationStore)
        {
            _connectionViewModel = vm;
            _echo = echo;
            _navigationStore = navigationStore;

            _connectionViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ConnectionViewModel.IPAddress) ||
                e.PropertyName == nameof(ConnectionViewModel.Port) ||
                e.PropertyName == nameof(ConnectionViewModel.Username))
            {
                OnCanExecuteChanged();
            }
        }
        public override bool CanExecute(object parameter)
        {
            Regex regexIP = new Regex(@"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.$");

            Match matchIP = regexIP.Match(_connectionViewModel.IPAddress);

            Regex regexURL = new Regex(@"^(?:\S.*\.)?\S.*\.\S.*$");

            Match matchURL = regexURL.Match(_connectionViewModel.IPAddress);

            bool isPortValid;
            try
            {
                int portNum = Convert.ToInt32(_connectionViewModel.Port);
                if (0 >= portNum || portNum > 65535)
                {
                    isPortValid = false;
                }
                else
                {
                    isPortValid = true;
                }              
            }
            catch (System.FormatException)
            {
                isPortValid = false;
            }

            return (!string.IsNullOrEmpty(_connectionViewModel.IPAddress) &&
                !string.IsNullOrEmpty(_connectionViewModel.Username) &&
                isPortValid &&
                (matchIP.Success || matchURL.Success) &&
                base.CanExecute(parameter));
        }
        public override void Execute(object parameter)
        {
            ConfigManager.UpdateSetting("last_used_username", _connectionViewModel.Username);
            ConfigManager.UpdateSetting("last_used_ip", _connectionViewModel.IPAddress);
            ConfigManager.UpdateSetting("last_used_port", _connectionViewModel.Port);

            _echo.ConnectionStatus = ConnectionStatus.Connecting;

            _echo.CreateUser(_connectionViewModel.Username, _connectionViewModel.Anonymous);

            if (_echo.CreateServer(_connectionViewModel.IPAddress, Convert.ToInt32(_connectionViewModel.Port), _connectionViewModel.Password))
            {
                Connect();
            } else
            {
                _echo.connectionContext = "Unable to create Server object";
                _echo.ConnectionStatus = ConnectionStatus.Error;                
            }    
        }

        private async void Connect()
        {
            bool connected = await _echo.GetServer().Handshake();

            if (connected)
            {
                _echo.GetServer().ReceiveMessages();
                _echo.ConnectionStatus = ConnectionStatus.Connected;
                _navigationStore.CurrentViewModel = new ChatViewModel(_echo, _navigationStore);
                _echo.GetServer().SendMessageToServer("requestInfo", "");
            }
            else
            {
                _echo.ConnectionStatus = ConnectionStatus.Error;
            }
        }
    }
}
