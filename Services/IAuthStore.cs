using System;
using System.Collections.Generic;
using System.Text;

namespace SWGANH_MasterServer.Services
{
    interface IAuthStore
    {
        int this[Guid guid] { get; }
        void Add(Guid guid, int id);
        bool Remove(Guid guid, int id);
    }
}
