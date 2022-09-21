using Echo.Managers;
using Echo.Stores;
using Echo.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security;
using System.Text;

namespace Echo.Models
{
    public enum ConnectionStatus
    {
        Idle,
        Connecting,
        Connected,
        Disconnected,
        Reconnecting,
        Terminated,
        Error
    }

    public class EchoClient
    {
        private Server currentServer { get; set; }

        private ConnectionStatus _connectionStatus;
        public ConnectionStatus ConnectionStatus
        {
            get => _connectionStatus;
            set
            {
                _connectionStatus = value;
                OnConnectionStatusChanged();
            }
        }

        public event Action ConnectionStatusChanged;

        private void OnConnectionStatusChanged()
        {
            ConnectionStatusChanged?.Invoke();
        }

        public string connectionContext { get; set; }

        public ObservableCollection<ServerPresetViewModel> serverPresets { get; set; }
        private Channel currentChannel { get; set; }

        private User user { get; set; }

        private NavigationStore navigationStore { get; set; }

        public EchoClient(NavigationStore ns)
        {
            serverPresets = new ObservableCollection<ServerPresetViewModel>();

            if (ConfigManager.ReadPresetFile() is not null)
            {
                Dictionary<string, List<object>> presets = ConfigManager.ReadPresetFile();

                foreach (KeyValuePair<string, List<object>> entry in presets)
                {
                    AddPreset(
                        entry.Value[0].ToString(),
                        Convert.ToInt32(entry.Value[1]),
                        entry.Key,
                        entry.Value[2].ToString()
                    );
                }
            }           

            ConnectionStatus = ConnectionStatus.Idle;
            navigationStore = ns;
        }

        public string GetEchoID()
        {
            var macAddr =
            (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();

            string userID = EncryptionManager.SHA256HAsh(macAddr);

            return userID;
        }

        public void CreateUser(string username, bool anon)
        {
            string userID = null;
            if (anon)
            {
                userID = EncryptionManager.SHA256HAsh(KeyGenerator.GetUniqueKey(32));
            }
            else
            {
                userID = GetEchoID();
            }

            user = new User(username, userID, anon);
        }

        public bool CreateServer(string _ipAddress, int _port, string _password)
        {
            try
            {
                currentServer = new Server(_ipAddress, _port, _password, user, this);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public Server GetServer()
        {
            return currentServer;
        }

        public void AddPreset(string ipAddress, int port, string serverName, string password)
        {
            serverPresets.Add(new ServerPresetViewModel(
                new ServerPreset(
                    ipAddress,
                    port,
                    serverName,
                    password
                )));
        }

        public void DeletePreset(ServerPresetViewModel vm)
        {
            serverPresets.Remove(vm);
        }

        public ObservableCollection<ServerPresetViewModel> GetServerPresets()
        {
            return serverPresets;
        }

        public void SetChannel(Channel c)
        {
            currentChannel = c;
            GetServer().SendMessageToServer("changeChannel", c?.GetName());
        }

        public Channel GetCurrentChannel()
        {
            return currentChannel;
        }

        public NavigationStore GetNavStore()
        {
            return navigationStore;
        }
    }
}
