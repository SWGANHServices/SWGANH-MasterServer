using System;
using System.Collections.Generic;
using System.Text;

namespace SWGANH_MasterServer.Business
{
    public interface ICharacterRepositiory
    {
        bool CharExists(string charname);
        void SaveCharacterToDb(int UserId, string charName, string UmaData);
        void UpdateCharacter(int UserId, string charName);
    }
}
