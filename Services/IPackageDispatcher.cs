using SWGANH_Core.PackageParser;
using SWGANH_MasterServer.Service.ServiceModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWGANH_MasterServer.Services
{
    public interface IPackageDispatcher
    {
        bool Running { get; }

        void DispatchPackage(PackageBase package, ClientConnection connection);
        public void Start()
        {

        }
    }
}
