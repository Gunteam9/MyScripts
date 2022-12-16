using CitizenFX.Core;
using Common;
using Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API = CitizenFX.Core.Native.API;

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

            PlayerInfo playerInfo = CommonExtension.Instance.GetPlayerInfo(player);

            if (playerInfo == null)
            {
                playerInfo = playerInfoCrud.Insert(new PlayerInfo(0, player.Identifiers["steam"]));
                TriggerEvent(Event.Server.Bank.CreateEmptyPlayerMoney, playerInfo.Id);
            }

            #endregion
        }
    }
}
