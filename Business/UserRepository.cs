using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using SWGANH_Core;
using SWGANH_MasterServer.Models.User;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SWGANH_MasterServer.Business
{
    class UserRepository : IUserRepository
    {
        private readonly IConfigurationRoot Configuration;
        private readonly string _connectionString;
        private IDbConnection _connection { get { return new MySqlConnection(_connectionString); } }

        private readonly ILogger<UserRepository> logger;

        public UserRepository(ILogger<UserRepository> logger, IConfigurationRoot configuration)
        {
            this.Configuration = Configuration;
            this.logger = logger;
            _connectionString = Configuration.GetConnectionString("MySQL");
        }

        public void AddUser(string user, string password, string email)
        {
            using (IDbConnection dbConnection = _connection)
            {
                const string query = @"INSERT INTO Users (User, password, email) VALUES ( @UserName, @Password, @EmailAddress)";

                var result = dbConnection.Execute(query, new { UserName = user, Password = Encryption.HashPassword(password), EmailAddress = email });
            }
        }

        public void DeleteUser(int id)
        {
            throw new NotImplementedException();
        }

        public List<UserModel> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public (bool sucess, IEnumerable<CharacterModel> charlist) GetCharacterList(int userId)
        {
            throw new NotImplementedException();
        }

        public UserModel GetUser(int id)
        {
            throw new NotImplementedException();
        }

        public (bool, int) PasswordOK(string username, string password)
        {
            using (IDbConnection dbConnection = _connection)
            {
                const string query = @"SELECT * FROM User WHERE username=@username AND active='1'";
                dbConnection.Open();

                var User = dbConnection.Query<UserModel>(query, new { UserName = username }).FirstOrDefault();

                bool validPassword = Encryption.ValidatePassword(password, User.Password);

                if(validPassword)
                {
                    logger.LogInformation("Password is correct.");
                    return (true, User.Id);
                }
                else
                {
                    logger.LogInformation("Password is incorrect!");
                    return (false, -1);
                }
            }
        }

        public void UpdateUser(UserModel User)
        {
            throw new NotImplementedException();
        }

        public bool UserExists(string username)
        {
            using (IDbConnection dbConnection = _connection)
            {
                const string query = @"SELECT * FROM User WHERE username=@username AND active='1'";
                dbConnection.Open();

                var User = dbConnection.Query<UserModel>(query, new { UserName = username }).FirstOrDefault();

                if(User.UserName == null)
                {
                    logger.LogInformation("UserName does not exist");
                    return false;
                }
                else
                {
                    logger.LogInformation("UserName exists");
                    return true;
                }
            }
        }
    }
}
