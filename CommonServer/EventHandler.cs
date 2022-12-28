using CitizenFX.Core;
using CitizenFX.Core.Native;
using Common;
using Common.Entity;

namespace CommonServer
{
    public class EventHandler : BaseScript
    {
        private readonly PlayerInfoCrud playerInfoCrud = new PlayerInfoCrud();

        [EventHandler(Event.Server.FiveM.OnPlayerJoining)]
        public void OnPlayerJoining([FromSource] Player player, string oldId)
        {
            if (API.GetCurrentResourceName() != "common")
                return;

            Debug.WriteLine($"Player {player.Name} joining");

            #region Génération du PlayerInfo à la première connexion au serveur

            var playerInfo = CommonExtension.Instance.GetPlayerInfo(player);

            if (playerInfo == null)
            {
                playerInfo = playerInfoCrud.Insert(new PlayerInfo(0, player.Identifiers["steam"]));
                TriggerEvent(Event.Server.Bank.CreateEmptyPlayerMoney, playerInfo.Id);
            }

            #endregion
        }
    }
}