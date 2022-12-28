using CitizenFX.Core;
using Common.Entity;

namespace CommonServer
{
    /// <summary>
    ///     Classe d'extension de type singleton permettant de récupérer des informations facilement (eqls = GiveMe)
    /// </summary>
    public sealed class CommonExtension
    {
        private readonly PlayerInfoCrud playerInfoCrud = new PlayerInfoCrud();

        private CommonExtension()
        {
        }

        public static CommonExtension Instance { get; } = new CommonExtension();

        /// <summary>
        ///     Renvoi les informations complementaires sur le joueur
        /// </summary>
        public PlayerInfo GetPlayerInfo(Player player)
        {
            return playerInfoCrud.Get(player.Identifiers["steam"]);
        }
    }
}