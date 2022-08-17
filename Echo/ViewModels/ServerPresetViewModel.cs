using Echo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.ViewModels
{
    public class ServerPresetViewModel : ViewModelBase
    {
        private readonly ServerPreset _preset;
        public string ServerName => _preset._serverName;

        public ServerPresetViewModel(ServerPreset preset)
        {
            _preset = preset;
        }

        public ServerPreset GetPreset()
        {
            return _preset;
        }
    }
}
