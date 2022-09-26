using Echo.Managers;
using Echo.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Echo.Commands
{
    public class InstallThemeCommand : CommandBase
    {
        private readonly EchoClient _echo;
        public InstallThemeCommand(EchoClient echo)
        {
            _echo = echo;
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public override void Execute(object parameter)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                // decode json
                // VisualManager.Themes.InstallTheme();
            }
        }
    }
}
