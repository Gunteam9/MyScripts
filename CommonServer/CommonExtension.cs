using CitizenFX.Core;
using Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonServer
{
    /// <summary>
    /// Classe d'extension de type singleton permettant de récupérer des informations facilement (eqls = GiveMe)
    /// </summary>
    public sealed class CommonExtension
    {
        private static readonly CommonExtension instance = new CommonExtension();
        public static CommonExtension Instance { get => instance; }

        private readonly PlayerInfoCrud playerInfoCrud = new PlayerInfoCrud();

        private CommonExtension()
        {

        }

        /// <summary>
        /// Renvoi les informations complementaires sur le joueur
        /// </summary>
        public PlayerInfo GetPlayerInfo(Player player)
        {
            return playerInfoCrud.Get(player.Identifiers["steam"]);
        }
    }
}
