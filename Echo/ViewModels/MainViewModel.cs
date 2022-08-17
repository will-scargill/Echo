using Echo.Commands;
using Echo.Managers;
using Echo.Models;
using Echo.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;

namespace Echo.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly EchoClient _echo;
        private readonly NavigationStore _navigationStore;
        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;

        private SolidColorBrush _ConnStatusColour;
        public SolidColorBrush ConnStatusColour
        {
            get
            {
                return _ConnStatusColour;
            }
            set
            {
                _ConnStatusColour = value;
                OnPropertyChanged(nameof(ConnStatusColour));
            }
        }

        private string _StatusContextText;
        public string StatusContextText
        {
            get
            {
                return _StatusContextText;
            }
            set
            {
                _StatusContextText = value;
                OnPropertyChanged(nameof(StatusContextText));
            }
        }

        private string _ConnStatusText;
        public string ConnStatusText
        {
            get
            {
                return _ConnStatusText;
            }
            set
            {
                _ConnStatusText = value;
                OnPropertyChanged(nameof(ConnStatusText));
            }
        }

        private bool _Connected;
        public bool Connected
        {
            get
            {
                return _Connected;
            }
            set
            {
                _Connected = value;
                OnPropertyChanged(nameof(Connected));
            }
        }
        public ICommand DisconnCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand GetRolesCommand { get; }
        public ICommand GetUsersCommand { get; }
        public ICommand LeaveChannelCommand { get; }
        public ICommand ViewSettingsCommand { get; }
        public ICommand JoinLastCommand { get; }
        public ICommand ViewConnectionsCommand { get; }

        public MainViewModel(EchoClient echo, NavigationStore navigationStore)
        {
            _echo = echo;
            _navigationStore = navigationStore;

            DisconnCommand = new DisconnectCommand(_echo, this, _navigationStore);
            ExitCommand = new ExitCommand(_echo);
            GetRolesCommand = new GetRolesCommand(_echo, this);
            GetUsersCommand = new GetUsersCommand(_echo, this);
            LeaveChannelCommand = new LeaveChannelCommand(_echo, this);
            ViewSettingsCommand = new ViewSettingsCommand(_echo, _navigationStore);
            JoinLastCommand = new JoinLastCommand(_echo);
            ViewConnectionsCommand = new ViewConnectionsCommand(_echo);

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _echo.ConnectionStatusChanged += OnConnectionStatusChanged;

            ConnStatusColour = VisualManager.ConnStatusToBrush(_echo.ConnectionStatus);
            ConnStatusText = VisualManager.ConnStatusToText(_echo.ConnectionStatus);
        }

        private void OnConnectionStatusChanged()
        {
            ConnStatusColour = VisualManager.ConnStatusToBrush(_echo.ConnectionStatus);
            OnPropertyChanged(nameof(ConnStatusColour));
            ConnStatusText = VisualManager.ConnStatusToText(_echo.ConnectionStatus, _echo.connectionContext);
            OnPropertyChanged(nameof(ConnStatusText));
            StatusContextText = _echo.connectionContext;
            OnPropertyChanged(nameof(StatusContextText));

            if (_echo.ConnectionStatus == ConnectionStatus.Connected)
            {
                Connected = true;
            } else
            {
                Connected = false;
            }
            OnPropertyChanged(nameof(Connected));
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
