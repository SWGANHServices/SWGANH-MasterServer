using System;
using System.Collections.Generic;
using System.Text;

namespace SWGANH_MasterServer.Services
{
    class AuthStore : IAuthStore
    {
        private Dictionary<Guid, int> AuthClients = new Dictionary<Guid, int>();

        public void Add(Guid guid, int id) => AuthClients.Add(guid, id);

        public bool Remove(Guid guid, int id) => AuthClients.Remove(guid);

        public int this[Guid guid] => (AuthClients.ContainsKey(guid))?AuthClients[guid]: -1;
 
    }
}
