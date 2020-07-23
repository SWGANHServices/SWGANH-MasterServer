using SWGANH_MasterServer.Models.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWGANH_MasterServer.Business
{
    public interface IUserRepository
    {
        UserModel GetUser(int id);
        List<UserModel> GetAllUsers();
        void AddUser(string user, string password, string email);
        void DeleteUser(int id);
        void UpdateUser(UserModel User);

        (bool sucess, IEnumerable<CharacterModel> charlist) GetCharacterList(int userId);
        (bool, int) PasswordOK(string username, string password);
        bool UserExists(string username);
    }
}
