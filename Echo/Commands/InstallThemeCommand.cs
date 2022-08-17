﻿using Echo.Models;
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
            return false;
        }
        public override void Execute(object parameter)
        {
            
        }
    }
}
