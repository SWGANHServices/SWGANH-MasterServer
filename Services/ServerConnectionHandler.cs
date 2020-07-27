using Microsoft.Extensions.Logging;
using SWGANH_Core;
using SWGANH_Core.PackageParser;
using SWGANH_Core.PackageParser.PackageImplimentations;
using SWGANH_MasterServer.Business;
using SWGANH_MasterServer.Service.ServiceModels;

namespace SWGANH_MasterServer.Services
{
    public class ServerConnectionHandler : ConnectionHandlerBase<ClientConnection>
    {

        private readonly ILogger<ServerConnectionHandler> logger;
        private readonly IPackageParser packageParser;
        private readonly IUserRepository UserRepo;

        public ServerConnectionHandler(ILogger<ServerConnectionHandler> logger, IPackageParser packageParser,
            IUserRepository UserRepo)
        {
            this.logger = logger;
            this.packageParser = packageParser;
            this.UserRepo = UserRepo;
        }


        protected override void HandleUnknownPackage(ClientConnection connection, object ParsedData, uint type)
        {
            throw new System.NotImplementedException();
        }

        [PackageHandler(CommunicationPackage.LOGIN_REQUEST)]
        public void HandleLogin(ClientConnection connection, LoginRequestPackage parsedObjectData)
        {
            logger.LogInformation("Login Request");
            logger.LogInformation($"Login DATA User: {parsedObjectData.Username} PW: {parsedObjectData.Password}");

            if(!UserRepo.UserExists(parsedObjectData.Username))
            {
                return;
            }
            (var passwordOK, int userId) = UserRepo.PasswordOK(parsedObjectData.Username, parsedObjectData.Password);
            if(!passwordOK)
            {
                logger.LogInformation("Wrong Password");
                return;
            }
            else
            {
                packageParser.ParserPackageToStream(new LoginResponsePackage
                {
                    IsValid = true

                }, connection.Writer);
            }
        }

        [PackageHandler(CommunicationPackage.REALM_REQUEST)]
        public void HandleRaceSelection(ClientConnection connection, RealmRequestPackage parsedObjectData)
        {
            logger.LogInformation("Race Request: ");
            logger.LogInformation($"Race Selected: {parsedObjectData.RealmID}");

            packageParser.ParserPackageToStream(new RealmResponsePackage
            {
                RealmID = (int)parsedObjectData.RealmID,
            }, connection.Writer);
        }
    }
}