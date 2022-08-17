using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Models
{
    public class User
    {
        public string Username { get; set; }
        public string eID { get; set; }
        public bool anonymous { get; set; }
        public List<Permission> permissions { get; set; }

        public User(string username, string eID, bool anonymous)
        {
            this.Username = username;
            this.eID = eID;
            this.anonymous = anonymous;
            this.permissions = new List<Permission>();
        }
    }
}
