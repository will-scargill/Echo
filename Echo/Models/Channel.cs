using Echo.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace Echo.Models
{
    public class Channel
    {
        private readonly string _name;

        private List<Client> _channelMembers;
        public bool allHistoryLoaded { get; set; }
        public ObservableCollection<ChannelMemberViewModel> channelMembers { get; set; }

        public Channel(string name)
        {
            _name = name;
            _channelMembers = new List<Client>();
            channelMembers = new ObservableCollection<ChannelMemberViewModel>();

            allHistoryLoaded = false;
        }

        public string GetName()
        {
            return _name;
        }

        public void AddUser(Client c)
        {
            _channelMembers.Add(c);
            channelMembers.Add(new ChannelMemberViewModel(c));
        }

        public Client GetUser(string name)
        {
            foreach (Client c in _channelMembers)
            {
                if (c.GetUsername() == name)
                {
                    return c;
                }
            }
            return null;
        }

        public List<Client> GetUsers()
        {
            return _channelMembers;
        }

        public ObservableCollection<ChannelMemberViewModel> GetUserViewModels()
        {
            ObservableCollection<ChannelMemberViewModel> result = new ObservableCollection<ChannelMemberViewModel>();

            foreach (Client c in _channelMembers)
            {
                result.Add(new ChannelMemberViewModel(c));
            }

            return result;
        }

        public void RemoveUser(string name)
        {
            foreach (Client c in _channelMembers)
            {
                if (c?.GetUsername() == name)
                {
                    _channelMembers.Remove(c);
                    break;
                }
            }

            foreach (ChannelMemberViewModel c in channelMembers)
            {
                if (c?.ClientName == name)
                {
                    channelMembers.Remove(c);
                    break;
                }
            }
        }
    }
}
