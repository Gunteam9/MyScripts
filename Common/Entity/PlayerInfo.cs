using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    /// <summary>
    /// Informations sur le joueur
    /// </summary>
    public class PlayerInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// License FiveM du joueur (identifiant)
        /// </summary>
        public string PlayerLicense { get; set; }

        public PlayerInfo()
        {

        }

        public PlayerInfo(long id, string playerLicense)
        {
            Id = id;
            PlayerLicense = playerLicense;
        }
    }
}
