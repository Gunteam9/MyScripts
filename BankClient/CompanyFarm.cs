using CitizenFX.Core;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API = CitizenFX.Core.Native.API;

namespace BankClient
{
    public class CompanyFarm : BaseScript
    {
        private const float MAX_HARVEST_DISTANCE = 5f;

        /// <summary>
        /// Supérette Mirror Park Nord 7302
        /// </summary>
        private Vector3 sup7302 = new Vector3(1160.33f, -316.22f, 68.35f);

        private bool isHarvesting = false;
        private bool IsHarvesting
        {
            get => isHarvesting;
            set
            {
                isHarvesting = value;
                if (value)
                    ProgressBar.OnProgressBarStart(2000, ProgressBar.Reason.CompanyFarmEndHarvesting);
            }
        }

        [Tick]
        public async Task OnTick()
        {
            API.DrawMarker(27, sup7302.X, sup7302.Y, sup7302.Z, 0, 0, 0, 0, 0, 0, 1.5f, 1.5f, 1.5f, 255, 255, 255, 255, false, false, 2, false, null, null, false);

            // Pickup = E
            if (API.IsControlJustPressed(0, (int)Control.Pickup))
            {
                if (IsHarvesting)
                {
                    IsHarvesting = false;
                }
                else
                {
                    if (Game.PlayerPed.Position.DistanceToSquared(sup7302) < MAX_HARVEST_DISTANCE)
                    {
                        IsHarvesting = true;
                    }
                }
            }

            if (IsHarvesting && Game.PlayerPed.Position.DistanceToSquared(sup7302) > MAX_HARVEST_DISTANCE)
            {
                IsHarvesting = false;
            }
        }

        [EventHandler(Event.Client.Common.OnProgressBarEnd)]
        public void OnProgressBarEnd(ProgressBar.Reason reason)
        {
            Debug.WriteLine("ProgressBarEnd");
            if (reason == ProgressBar.Reason.CompanyFarmEndHarvesting)
            {
                IsHarvesting = false;
                // On se fiche de grouper les event de farm dans cette situation même si l'opti = nul /20
                TriggerServerEvent(Event.Ctos.Bank.FarmHarvest);
                Debug.WriteLine("Vente terminé");
            }
        }
    }
}
