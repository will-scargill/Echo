using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Models
{
    class User
    {
        public string Username;
        public string eID;
        public bool anonymous;

        public User(string username, string eID, bool anonymous)
        {
            Username = username;
            this.eID = eID;
            this.anonymous = anonymous;
        }
    }
}
