using System;
using System.Collections.Generic;
using System.Text;

namespace SWGANH_MasterServer.Models.User
{
    public class UserModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }

        public List<CharacterModel> Characters { get; set; }


    }

}
