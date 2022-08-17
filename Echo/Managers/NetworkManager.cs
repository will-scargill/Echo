using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Echo.Models;

namespace Echo.Managers
{
    class NetworkManager
    {
        private static Server _echo;
        public static bool blockHistory;
        public static void registerServer(Server s)
        {
            _echo = s;
            blockHistory = true;
        }

        public static Server getServer()
        {
            return _echo;
        }
    }
}
