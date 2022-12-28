using CitizenFX.Core;
using Common;
using CommonClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API = CitizenFX.Core.Native.API;

namespace BankClient
{
    public class ProgressBar : BaseScript
    {
        public enum Reason
        {
            /// <summary>
            /// Inconnu
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// Fin de récolte pour le farm de l'entreprise
            /// </summary>
            CompanyFarmEndHarvesting = 1
        }

        /// <summary>
        /// Permet de créer une barre de progression
        /// </summary>
        /// <param name="duration">Temps en ms</param>
        /// <param name="reason">Motif de vie</param>
        public static void OnProgressBarStart(float duration, ProgressBar.Reason reason)
        {
            var message = new
            {
                messageType = "ACTIVATE_PROGRESS_BAR",
                isProgressBarVisible = true,
                duration = duration,
            };
            API.SendNuiMessage(JsonConvert.SerializeObject(message));

            StopProgressBar(duration, reason);
        }

        /// <summary>
        /// Fonction appelé au début de la barre de progression puis qui attend sa fin. <br />
        /// Invoque l'event <see cref="Event.Client.Common.OnProgressBarEnd"/> une fois la progression terminée
        /// </summary>
        /// <param name="duration">Temps en ms</param>
        /// <param name="reason">Motif de vie</param>
        private static async void StopProgressBar(float duration, ProgressBar.Reason reason)
        {
            // On ajoute 100ms pour gérer les éventuelles lags
            await BaseScript.Delay((int)duration + 100);

            var message = new
            {
                messageType = "ACTIVATE_PROGRESS_BAR",
                isProgressBarVisible = false
            };
            API.SendNuiMessage(JsonConvert.SerializeObject(message));

            TriggerEvent(Event.Client.Common.OnProgressBarEnd, reason);
        }
    }
}
