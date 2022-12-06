using BankClient.Dto;
using BankCommon.Entity;
using CitizenFX.Core;
using Common;
using CommonClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API = CitizenFX.Core.Native.API;

namespace BankClient
{
    public class PhoneManager : BaseScript
    {
        private bool isFirstTickPassed = false;
        private bool isPhoneVisible = false;
        private static bool isBuyingAction = false;
        private static bool isSellingAction = false;
        private static long currentCompanyId = -1;

        private readonly Action<IDictionary<string, object>, CallbackDelegate> getPlayerMoney = (data, cb) =>
        {
            TriggerServerEvent(Event.Ctos.Bank.GetPlayerMoney);
            cb();
        };

        private readonly Action<IDictionary<string, object>, CallbackDelegate> getPlayerMoneyAndCompanies = (data, cb) =>
        {
            TriggerServerEvent(Event.Ctos.Bank.GetPlayerMoneyAndCompanies);
            cb();
        };

        private readonly Action<IDictionary<string, object>, CallbackDelegate> requestBuyAction = (data, cb) =>
        {
            API.DisplayOnscreenKeyboard(0, "Nombre d'action à acheter", "p2", string.Empty, string.Empty, string.Empty, string.Empty, 20);
            isBuyingAction = true;
            currentCompanyId = Convert.ToInt64(data["companyId"]);
            API.SetNuiFocus(false, false);
            cb();
        };

        private readonly Action<IDictionary<string, object>, CallbackDelegate> requestSellAction = (data, cb) =>
        {
            API.DisplayOnscreenKeyboard(0, "Nombre d'action à vendre", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 20);
            isSellingAction = true;
            currentCompanyId = Convert.ToInt64(data["companyId"]);
            cb();
        };


        public PhoneManager() : base()
        {

        }

        [EventHandler(Event.Client.FiveM.OnClientResourceStart)]
        private void OnClientResourceStart(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName)
                return;

            API.RegisterNuiCallbackType("getPlayerMoney");
            API.RegisterNuiCallbackType("getPlayerMoneyAndCompanies");
            API.RegisterNuiCallbackType("requestBuyAction");
            API.RegisterNuiCallbackType("requestSellAction");
            EventHandlers["__cfx_nui:getPlayerMoney"] += getPlayerMoney;
            EventHandlers["__cfx_nui:getPlayerMoneyAndCompanies"] += getPlayerMoneyAndCompanies;
            EventHandlers["__cfx_nui:requestBuyAction"] += requestBuyAction;
            EventHandlers["__cfx_nui:requestSellAction"] += requestSellAction;
        } 

        private void OnLoad()
        {
            API.SetTextChatEnabled(false);
            API.SetNuiFocus(true, false);
            API.SetNuiFocusKeepInput(true);
        }

        [Tick]
        private async Task OnTick()
        {
            if (!isFirstTickPassed)
            {
                OnLoad();
                isFirstTickPassed = true;
            }

            // Touche L
            // Ouvrir le téléphone uniquement
            if (API.IsControlJustPressed(0, Const.Controls.PRESS_L))
            {
                isPhoneVisible = !isPhoneVisible;
                Debug.WriteLine($"Is phone visible : {isPhoneVisible}");

                var message = new
                {
                    messageType = "ACTIVATE_PHONE",
                    isPhoneVisible = isPhoneVisible
                };
                API.SendNuiMessage(JsonConvert.SerializeObject(message));
            }

            // Si le joueur est entrain d'entrer un nombre d'action à acheter
            if (isBuyingAction)
            {
                // On désactive les contrôles
                API.DisableAllControlActions(0);

                // Quand il valide, on give l'arme
                if (API.UpdateOnscreenKeyboard() == 1)
                {
                    API.SetNuiFocus(true, false);
                    int amount = Convert.ToInt32(API.GetOnscreenKeyboardResult());
                    TriggerServerEvent(Event.Ctos.Bank.BuyAction, currentCompanyId, amount);
                    API.EnableAllControlActions(0);
                    isBuyingAction = false;
                }
                else if (API.UpdateOnscreenKeyboard() == 2)
                {
                    API.EnableAllControlActions(0);
                    isBuyingAction = false;
                }
            }
        }

        [EventHandler(Event.Stoc.Bank.GetPlayerMoney)]
        private void GetPlayerMoney(string playerMoneyString)
        {
            PlayerMoney playerMoney = JsonConvert.DeserializeObject<PlayerMoney>(playerMoneyString);
            int totalStock = 0;
            foreach (var stock in playerMoney.Stock)
                totalStock += stock.Amount;

            var message = new
            {
                messageType = "GetPlayerMoney",
                currentAccount = playerMoney.CurrentAccount,
                savings = playerMoney.Savings,
                totalStock = totalStock,
                lifeInsurance = playerMoney.LifeInsurance
            };
            API.SendNuiMessage(JsonConvert.SerializeObject(message));
        }

        [EventHandler(Event.Stoc.Bank.GetPlayerMoneyAndCompanies)]
        private void GetPlayerMoneyAndCompanies(string playerMoneyString, string listCompanyString)
        {
            PlayerMoney playerMoney = JsonConvert.DeserializeObject<PlayerMoney>(playerMoneyString);
            List<Company> companies = JsonConvert.DeserializeObject<List<Company>>(listCompanyString);

            List<PhoneBankPlayerStockDto> dtoOut = new List<PhoneBankPlayerStockDto>();

            foreach (var company in companies)
            {
                PhoneBankPlayerStockDto dto = new PhoneBankPlayerStockDto()
                {
                    CompanyId = company.Id,
                    CompanyName = company.Name,
                    CompanyStockValue = company.CurrentPrice,
                    CompanyStockValueEvolution = (company.CurrentPrice - company.PriceAtOpening) / company.PriceAtOpening * 100
                };

                foreach (var stock in playerMoney.Stock)
                {
                    if (stock.CompanyId == company.Id)
                    {
                        dto.StockAmount = stock.Amount;
                        dto.TotalStockValue = stock.Amount * company.CurrentPrice;
                    }
                }

                dtoOut.Add(dto);
            }

            var message = new
            {
                messageType = "GetPlayerMoneyAndCompanies",
                playerStockDto = dtoOut
            };
            API.SendNuiMessage(JsonConvert.SerializeObject(message));
        }

    }
}