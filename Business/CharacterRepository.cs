using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using SWGANH_MasterServer.Models.User;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SWGANH_MasterServer.Business
{
    class CharacterRepository : ICharacterRepositiory
    {
        private readonly IConfigurationRoot Configuration;
        private readonly string _connectionString;
        private IDbConnection _connection { get { return new MySqlConnection(_connectionString); } }

        private readonly ILogger<UserRepository> logger;

        public CharacterRepository(ILogger<UserRepository> logger, IConfigurationRoot configuration)
        {
            this.Configuration = Configuration;
            this.logger = logger;
            _connectionString = Configuration.GetConnectionString("MySQL");
        }

        public bool CharExists(string charname)
        {
            using (IDbConnection dbConnection = _connection)
            {
                const string query = @"SELECT COUNT(*) FROM characters WHERE Charname=@charname";
                dbConnection.Open();

                var Character = dbConnection.Query<CharacterModel>(query, new { UserName = charname }).FirstOrDefault();

                if (Character == null || Character.CharName != charname)
                {
                    logger.LogInformation("CharacterName doesn't exist");
                    return false;
                }
                else
                {
                    logger.LogInformation("CharacterName exists");
                    return true;
                }
            }
        }

        public void SaveCharacterToDb(int _userId, string charName, string _UmaData)
        {
            using (IDbConnection dbConnection = _connection)
            {
                const string query = @"INSERT INTO characters (UserId, Charname, UmaData) VALUES ( @UserId, @charname, @UmaData)";

                var result = dbConnection.Execute(query, new { UserID = _userId, Charname = charName, UmaData = _UmaData });
            }
        }

        public void UpdateCharacter(int UserId, string charName)
        {
            throw new NotImplementedException();
        }
    }
}
