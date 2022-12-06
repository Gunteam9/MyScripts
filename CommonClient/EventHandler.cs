using CitizenFX.Core;
using CitizenFX.Core.UI;
using Common;
using API = CitizenFX.Core.Native.API;

namespace CommonClient
{
    public class EventHandler : BaseScript
    {
        [EventHandler(Event.Shared.Common.ShowNotification)]
        public void ShowNotification(string message)
        {
            Screen.ShowNotification(message);
        }
    }
}
