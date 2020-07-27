using System;
using System.Collections.Generic;
using System.Text;

namespace SWGANH_MasterServer.Models.User
{
    public class CharacterModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public string CharName { get; set; }
        public string UmaData { get; set; }

        //public string Class { get; set; }
    }
}
