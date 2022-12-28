using BankClient.Dto;
using BankCommon;
using BankCommon.Entity;
using CitizenFX.Core;
using Common;
using CommonClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API = CitizenFX.Core.Native.API;

namespace BankClient
{
    public class PhoneManager : BaseScript
    {
        private enum InputType
        {
            BuyAction,
            SellAction,
            TransferToSavings,
            transfertToCurrentAccount
        }

        private bool isFirstTickPassed = false;
        private bool isPhoneVisible = false;
        private static bool isEnteringSomething = false;
        private static InputType enteringInputType;
        private static long currentCompanyId = -1;
        private static StockType currentStockType;

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
            isEnteringSomething = true;
            enteringInputType = InputType.BuyAction;

            currentCompanyId = Convert.ToInt64(data["companyId"]);
            currentStockType = (StockType)Convert.ToInt32(data["stockType"]);
            API.SetNuiFocus(false, false);
            cb();
        };

        private readonly Action<IDictionary<string, object>, CallbackDelegate> requestSellAction = (data, cb) =>
        {
            API.DisplayOnscreenKeyboard(0, "Nombre d'action à vendre", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 20);
            isEnteringSomething = true;
            enteringInputType = InputType.SellAction;

            currentCompanyId = Convert.ToInt64(data["companyId"]);
            currentStockType = (StockType)Convert.ToInt32(data["stockType"]);
            API.SetNuiFocus(false, false);
            cb();
        };
        
        private readonly Action<IDictionary<string, object>, CallbackDelegate> transfertToSavings = (data, cb) =>
        {
            API.DisplayOnscreenKeyboard(0, "Montant à transférer", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 20);
            isEnteringSomething = true;
            enteringInputType = InputType.TransferToSavings;

            API.SetNuiFocus(false, false);
            cb();
        };
        
        private readonly Action<IDictionary<string, object>, CallbackDelegate> transfertToCurrentAccount = (data, cb) =>
        {
            API.DisplayOnscreenKeyboard(0, "Montant à transférer", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 20);
            isEnteringSomething = true;
            enteringInputType = InputType.transfertToCurrentAccount;

            API.SetNuiFocus(false, false);
            cb();
        };
        
        private readonly Action<IDictionary<string, object>, CallbackDelegate> getCompanyInfoAndPlayerMoney = (data, cb) =>
        {
            long companyId = Convert.ToInt64(data["companyId"]);
            TriggerServerEvent(Event.Ctos.Bank.GetCompanyInfoAndPlayerMoney, companyId);
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
            API.RegisterNuiCallbackType("transfertToSavings");
            API.RegisterNuiCallbackType("transfertToCurrentAccount");
            API.RegisterNuiCallbackType("GetCompanyInfoAndPlayerMoney");
            EventHandlers["__cfx_nui:getPlayerMoney"] += getPlayerMoney;
            EventHandlers["__cfx_nui:getPlayerMoneyAndCompanies"] += getPlayerMoneyAndCompanies;
            EventHandlers["__cfx_nui:requestBuyAction"] += requestBuyAction;
            EventHandlers["__cfx_nui:requestSellAction"] += requestSellAction;
            EventHandlers["__cfx_nui:transfertToSavings"] += transfertToSavings;
            EventHandlers["__cfx_nui:transfertToCurrentAccount"] += transfertToCurrentAccount;
            EventHandlers["__cfx_nui:GetCompanyInfoAndPlayerMoney"] += getCompanyInfoAndPlayerMoney;
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
            if (API.IsControlJustPressed(0, CommonClient.Const.Controls.PRESS_L))
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

            // Si le joueur est entrain d'entrer quelque chose (native input)
            if (isEnteringSomething)
            {
                // On désactive les contrôles
                API.DisableAllControlActions(0);

                // Quand il valide
                if (API.UpdateOnscreenKeyboard() == 1)
                {
                    API.SetNuiFocus(true, false);
                    switch (enteringInputType)
                    {
                        case InputType.BuyAction:
                            int amount1 = Convert.ToInt32(API.GetOnscreenKeyboardResult());
                            TriggerServerEvent(Event.Ctos.Bank.BuyAction, currentCompanyId, currentStockType, amount1);
                            break;
                        case InputType.SellAction:
                            int amount2 = Convert.ToInt32(API.GetOnscreenKeyboardResult());
                            TriggerServerEvent(Event.Ctos.Bank.SellAction, currentCompanyId, currentStockType, amount2);
                            break;
                        case InputType.TransferToSavings:
                            int amount3 = Convert.ToInt32(API.GetOnscreenKeyboardResult());
                            TriggerServerEvent(Event.Ctos.Bank.DoTransfert, Account.CurrentAccount, Account.Savings, amount3);
                            break;
                        case InputType.transfertToCurrentAccount:
                            int amount4 = Convert.ToInt32(API.GetOnscreenKeyboardResult());
                            TriggerServerEvent(Event.Ctos.Bank.DoTransfert, Account.Savings, Account.CurrentAccount, amount4);
                            break;
                        default:
                            break;
                    }

                    API.EnableAllControlActions(0);
                    isEnteringSomething = false;
                }
                // Quand il annule
                else if (API.UpdateOnscreenKeyboard() == 2)
                {
                    API.EnableAllControlActions(0);
                    isEnteringSomething = false;
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
                    CompanyAcronym = company.Acronym,
                    CompanyStockValue = company.CurrentPrice,
                    CompanyStockValueEvolution = company.Evolution
                };

                foreach (var stock in playerMoney.Stock)
                {
                    if (stock.CompanyId == company.Id)
                    {
                        dto.StockAmount = stock.Amount;
                        dto.StockLeverage = stock.SimulatedAmount / stock.Amount;
                        dto.TotalStockValue = 
                            currentStock.Amount * company.CurrentPrice + (company.CurrentPrice - currentStock.AverageBuyValue) * (currentStock.Amount * (currentStock.SimulatedAmount / currentStock.Amount - 1)) + ;
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
        
        [EventHandler(Event.Stoc.Bank.GetCompanyInfoAndPlayerMoney)]
        private void GetCompanyInfoAndPlayerMoney(string companyInfoString, string playerMoneyString)
        {
            Company company = JsonConvert.DeserializeObject<Company>(companyInfoString);
            PlayerMoney playerMoney = JsonConvert.DeserializeObject<PlayerMoney>(playerMoneyString);

            Stock currentStock = playerMoney.Stock.Find(o => o.CompanyId == company.Id && o.Type == StockType.Normal);
            Stock currentStockVAD = playerMoney.Stock.Find(o => o.CompanyId == company.Id && o.Type == StockType.VAD);

            float stockTotalValue = currentStock.Amount * company.CurrentPrice + (company.CurrentPrice - currentStock.AverageBuyValue) * (currentStock.Amount * (currentStock.SimulatedAmount / currentStock.Amount - 1));
            float stockTotalValueVAD = currentStockVAD.Amount * company.CurrentPrice + (company.CurrentPrice - currentStockVAD.AverageBuyValue) * (currentStockVAD.Amount * (currentStockVAD.SimulatedAmount / currentStockVAD.Amount + 1));

            var message = new
            {
                messageType = "GetCompanyInfoAndPlayerMoney",
                companyName = company.Name,
                companyStockValue = company.CurrentPrice,
                companyStockEvolutionAsPercent = company.EvolutionAsPercent,
                companyVolume = company.Volume,
                stockNumber = currentStock.Amount,
                stockLeverage = currentStock.SimulatedAmount / currentStock.Amount,
                stockTotalValue = stockTotalValue,
                stockEvolutionAsPercent = stockTotalValue / (currentStock.Amount * currentStock.AverageBuyValue),
                stockNumberVAD = currentStockVAD.Amount,
                stockLeverageVAD = currentStockVAD.SimulatedAmount / currentStockVAD.Amount,
                stockTotalValueVAD = stockTotalValueVAD,
                stockEvolutionAsPercentVAD = stockTotalValueVAD / (currentStockVAD.Amount * currentStockVAD.AverageBuyValue),
            };
            API.SendNuiMessage(JsonConvert.SerializeObject(message));
        }

    }
}