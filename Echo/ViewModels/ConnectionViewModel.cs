using Echo.Commands;
using Echo.Managers;
using Echo.Models;
using Echo.Stores;
using Echo.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security;
using System.Text;
using System.Windows.Input;

namespace Echo.ViewModels
{
    public class ConnectionViewModel : ViewModelBase
    {
        private string _IPAddress;
        public string IPAddress
        {
            get
            {
                return _IPAddress;
            }
            set
            {
                _IPAddress = value;
                OnPropertyChanged(nameof(IPAddress));
            }
        }

        private string _Port;
        public string Port
        {
            get
            {
                return _Port;
            }
            set
            {
                _Port = value;
                OnPropertyChanged(nameof(Port));
            }
        }

        private string _Username;
        public string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                _Username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string _Password;
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {              
                _Password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        private bool _PassBoxEnabled;
        public bool PassBoxEnabled
        {
            get
            {
                return _PassBoxEnabled;
            }
            set
            {
                _PassBoxEnabled = value;
                OnPropertyChanged(nameof(PassBoxEnabled));
            }
        }

        private bool _Anonymous;
        public bool Anonymous
        {
            get
            {
                return _Anonymous;
            }

            set
            {
                _Anonymous = value;
                OnPropertyChanged(nameof(Anonymous));
            }
        }

        private string _PresetName;
        public string PresetName
        {
            get
            {
                return _PresetName;
            }
            set
            {
                // Logic for switching values here
                _PresetName = value;
                OnPropertyChanged(nameof(PresetName));
            }
        }

        private ServerPresetViewModel _SelectedPreset;
        public ServerPresetViewModel SelectedPreset
        {
            get
            {
                return _SelectedPreset;
            }
            set
            {
                if (value is not null)
                {
                    IPAddress = value.GetPreset()._ipAddress;
                    Port = value.GetPreset()._port.ToString();
                    Password = value.GetPreset()._password;                                      
                }
                _SelectedPreset = value;
                OnPropertyChanged(nameof(SelectedPreset));
            }
        }

        private readonly ObservableCollection<ServerPresetViewModel> _Presets;

        public IEnumerable<ServerPresetViewModel> Presets => _Presets;

        public ICommand ConnectCommand { get; }

        public ICommand NewPresetCommand { get; }
        public ICommand DeletePresetCommand { get; }

        public ConnectionViewModel(EchoClient echo, NavigationStore navigationStore)
        {
            Username = ConfigManager.GetSetting("last_used_username");
            IPAddress = ConfigManager.GetSetting("last_used_ip");
            Port = ConfigManager.GetSetting("last_used_port");
            _Presets = echo.serverPresets;

            NewPresetCommand = new NewPresetCommand(this, echo);
            ConnectCommand = new ConnectCommand(this, echo, navigationStore);
            DeletePresetCommand = new DeletePresetCommand(echo, this);
            
        }
    }
}
