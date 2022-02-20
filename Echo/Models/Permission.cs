using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Models
{
    class Permission
    {
        private readonly string _name;
        private readonly string _flag;

        public Permission(string name, string flag)
        {
            _name = name;
            _flag = flag;
        }
    }
}
