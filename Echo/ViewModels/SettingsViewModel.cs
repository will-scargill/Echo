using Echo.Models;
using Echo.Stores;
using Echo.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Echo.Managers;

namespace Echo.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly EchoClient _echo;
        private readonly NavigationStore _navigationStore;

        private Visibility _eIDHidden;
        public Visibility eIDHidden
        {
            get
            {
                return _eIDHidden;
            }
            set
            {
                _eIDHidden = value;
                OnPropertyChanged(nameof(eIDHidden));
            }
        }

        private string _EchoID;
        public string EchoID
        {
            get
            {
                return _EchoID;
            }
            set
            {
                _EchoID = value;
                OnPropertyChanged(nameof(EchoID));
            }
        }

        private string _EchoVersion;
        public string EchoVersion
        {
            get
            {
                return _EchoVersion;
            }
            set
            {
                _EchoVersion = value;
                OnPropertyChanged(nameof(EchoVersion));
            }
        }

        private bool _DisplayImages;
        public bool DisplayImages
        {
            get
            {
                return _DisplayImages;
            }
            set
            {
                _DisplayImages = value;
                OnPropertyChanged(nameof(DisplayImages));
            }
        }

        public ICommand ViewSettingsCommand { get; }
        public ICommand ToggleEchoIDCommand { get; }
        public ICommand ResetRSACommand { get; }
        public ICommand InstallThemeCommand { get; }
        public SettingsViewModel(EchoClient echo, NavigationStore navigationStore)
        {
            _echo = echo;
            _navigationStore = navigationStore;

            ViewSettingsCommand = new ViewSettingsCommand(echo, navigationStore);
            ToggleEchoIDCommand = new ToggleEchoIDCommand(echo, this);
            ResetRSACommand = new ResetRSACommand(echo);
            InstallThemeCommand = new InstallThemeCommand(echo);

            _EchoID = _echo.GetEchoID();
            _EchoVersion = ConfigManager.GetSetting("version");
        }
    }
}
