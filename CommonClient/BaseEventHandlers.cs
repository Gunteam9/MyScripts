using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace CommonClient
{
    public class BaseEventHandlers : BaseScript
    {
        public BaseEventHandlers()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
            EventHandlers["onClientResourceStop"] += new Action<string>(OnClientResourceStop);
            EventHandlers["onResourceStart"] += new Action<string>(OnResourceStart);
            EventHandlers["onResourceStarting"] += new Action<string>(OnResourceStarting);
            EventHandlers["onResourceStop"] += new Action<string>(OnResourceStop);
            EventHandlers["gameEventTriggered"] += new Action<string>(GameEventTriggered);
            EventHandlers["populationPedCreating"] += new Action<string>(PopulationPedCreating);
        }

        protected virtual void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
        }

        protected virtual void OnClientResourceStop(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
        }

        protected virtual void OnResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
        }

        protected virtual void OnResourceStarting(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
        }

        protected virtual void OnResourceStop(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
        }

        protected virtual void GameEventTriggered(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
        }

        protected virtual void PopulationPedCreating(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
        }
    }
}
