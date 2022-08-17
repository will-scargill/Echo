using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Echo.Commands;
using Echo.Models;
using Echo.Stores;

namespace Echo.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        private readonly EchoClient _echo;

        private string _UserMessage;
        public string UserMessage
        {
            get
            {
                return _UserMessage;
            }
            set
            {
                _UserMessage = value;
                OnPropertyChanged(nameof(UserMessage));
            }
        }

        private ChannelViewModel _SelectedChannel;
        public ChannelViewModel SelectedChannel
        {
            get
            {
                return _SelectedChannel;
            }
            set
            {
                _Messages.Clear();
                
                _SelectedChannel = value;
                _echo.SetChannel(_echo.GetServer().GetChannel(_SelectedChannel?.ChannelName));
                _echo.GetServer().currentChannelClientList.Clear();
                _echo.GetServer().currentChannelMessageList.Clear();
                if (_echo.GetCurrentChannel() is not null)
                {
                    _echo.GetServer().GetCurrentChannel().allHistoryLoaded = false;

                    foreach (ChannelMemberViewModel c in _echo.GetCurrentChannel().GetUserViewModels())
                    {
                        _echo.GetServer().currentChannelClientList.Add(c);
                    }
                }                        
                OnPropertyChanged(nameof(SelectedChannel));
            }
        }

        private MessageViewModel _SelectedMessage;
        public MessageViewModel SelectedMessage
        {
            get
            {
                return _SelectedMessage;
            }
            set
            {
                _SelectedMessage = value;
                OnPropertyChanged(nameof(SelectedMessage));
            }
        }

        private ChannelMemberViewModel _SelectedClient;
        public ChannelMemberViewModel SelectedClient
        {
            get
            {
                return _SelectedClient;
            }
            set
            {
                _SelectedClient = value;
                OnPropertyChanged(nameof(SelectedClient));
            }
        }

        private int _ScrollOffset;

        public int ScrollOffset
        {
            get
            {
                return _ScrollOffset;
            }
            set 
            { 
                _ScrollOffset = value; 
                OnPropertyChanged(nameof(ScrollOffset)); 
            }
        }

        private readonly ObservableCollection<ChannelViewModel> _Channels;

        public IEnumerable<ChannelViewModel> Channels => _Channels;

        private readonly ObservableCollection<ChannelMemberViewModel> _ChannelMembers;

        public IEnumerable<ChannelMemberViewModel> ChannelMembers => _ChannelMembers;

        private readonly ObservableCollection<MessageViewModel> _Messages;

        public IEnumerable<MessageViewModel> Messages => _Messages;

        public ICommand SendMessageCommand { get; }
        public ICommand UserContextCommand { get; }
        public ICommand MessageContextCommand { get; }

        public ChatViewModel(EchoClient echo, NavigationStore navigationStore)
        {
            _echo = echo;

            _Channels = echo.GetServer().channelList;
            _ChannelMembers = echo.GetServer().currentChannelClientList;
            _Messages = echo.GetServer().currentChannelMessageList;

            SendMessageCommand = new SendMessageCommand(this, _echo, navigationStore);
            UserContextCommand = new UserContextCommand(this, _echo);
            MessageContextCommand = new MessageContextCommand(this, _echo);
        }
    }
}
