using BankCommon;
using BankCommon.Entity;
using BankServer.Crud;
using CitizenFX.Core;
using Common;
using Common.Entity;
using Common.Errors;
using CommonServer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankServer.Manager
{
    public class EventHandler : BaseScript
    {
        private List<Company> companies;
        private readonly CompanyCrud companyCrud = new CompanyCrud();
        private readonly TransactionCrud transactionCrud = new TransactionCrud();
        private readonly MoneyCrud moneyCrud = new MoneyCrud();

        public IReadOnlyCollection<Company> Companies { get => companies.AsReadOnly(); }

        public EventHandler()
        {
            Initialize();
        }

        private void Initialize()
        {
            companies = new List<Company>()
            {
                new Company(0, "Ambeer", "ABR", new List<Sector>() {Sector.Consumer, Sector.Industrials, Sector.Materials}, 50, 50),
                new Company(0, "Benny's", "BNYS", new List<Sector>() {Sector.Consumer, Sector.Utilities}, 50, 50),
                new Company(0, "BlackCat", "BC", new List<Sector>() {Sector.Communication, Sector.Consumer, Sector.Utilities}, 50, 50),
                new Company(0, "Chilliad Valley Farm", "CVF", new List<Sector>() {Sector.Industrials, Sector.Materials}, 50, 50),
                new Company(0, "Darnell Bros", "DB", new List<Sector>() {Sector.Industrials, Sector.Materials}, 50, 50),
                new Company(0, "Giggling Squid", "GS", new List<Sector>() {Sector.Industrials, Sector.Materials}, 50, 50),
                new Company(0, "Gruppe 6", "GRP", new List<Sector>() {Sector.Consumer, Sector.Financial, Sector.Utilities}, 50, 50),
                new Company(0, "Humane Labs", "HL", new List<Sector>() {Sector.Consumer, Sector.Healthcare}, 50, 50),
                new Company(0, "I-Volt", "VOLT", new List<Sector>() {Sector.Energy, Sector.Industrials}, 50, 50),
                new Company(0, "Los Santos Taxi", "LST", new List<Sector>() {Sector.Communication, Sector.Utilities}, 50, 50),
                new Company(0, "Lumber Yard", "LMBR", new List<Sector>() {Sector.Consumer, Sector.Industrials, Sector.Materials}, 50, 50),
                new Company(0, "Marlow Vinyard", "MRLW", new List<Sector>() {Sector.Consumer, Sector.Industrials, Sector.Materials}, 50, 50),
                new Company(0, "Pacific Bank", "PCF", new List<Sector>() {Sector.Financial, Sector.Utilities}, 50, 50),
                new Company(0, "Roger's", "RGRS", new List<Sector>() {Sector.Consumer, Sector.Utilities}, 50, 50),
                new Company(0, "Ron Petroleum", "RON", new List<Sector>() {Sector.Energy, Sector.Industrials}, 50, 50),
                new Company(0, "Weazel News", "WZL", new List<Sector>() {Sector.Communication, Sector.Consumer, Sector.Utilities}, 50, 50),
                new Company(0, "Yellow Jack", "YJ", new List<Sector>() {Sector.Communication, Sector.Consumer, Sector.Utilities}, 50, 50),
                new Company(0, "Zombie Car", "ZC", new List<Sector>() {Sector.Consumer, Sector.Utilities}, 50, 50)
            };

            for (int i = 0; i < companies.Count; i++)
            {
                companies[i] = companyCrud.Insert(companies[i]);
            }
        }

        [EventHandler(Event.Server.Bank.CreateEmptyPlayerMoney)]
        public void CreateEmptyPlayerMoney(long playerId)
        {
            PlayerMoney playerMoney = new PlayerMoney(0, 100, 500, 0, new List<Stock>(), 0);

            moneyCrud.Insert(playerMoney);
        }

        [EventHandler(Event.Ctos.Bank.GetLast10Transactions)]
        public void GetLast10Transactions([FromSource] Player player)
        {
            PlayerInfo playerInfo = CommonExtension.Instance.GetPlayerInfo(player);

            var res = transactionCrud.GetElements(playerInfo.Id, 10);

            player.TriggerEvent(Event.Stoc.Bank.GetLast10Transactions, res);
        }

        [EventHandler(Event.Ctos.Bank.GetCompanyCurrentActionValue)]
        public void GetCompanyCurrentActionValue([FromSource] Player player)
        {
            PlayerInfo playerInfo = CommonExtension.Instance.GetPlayerInfo(player);

            var res = companyCrud.Get(playerInfo.Id);

            player.TriggerEvent(Event.Stoc.Bank.GetCompanyCurrentActionValue, res);
        }
        
        [EventHandler(Event.Ctos.Bank.GetCompaniesCurrentActionValue)]
        public void GetCompaniesCurrentActionValue([FromSource] Player player)
        {
            PlayerInfo playerInfo = CommonExtension.Instance.GetPlayerInfo(player);

            var res = companyCrud.GetAllCompanies();

            player.TriggerEvent(Event.Stoc.Bank.GetCompaniesCurrentActionValue, JsonConvert.SerializeObject(res));
        }

        [EventHandler(Event.Ctos.Bank.GetPlayerMoney)]
        public void GetPlayerMoney([FromSource] Player player)
        {
            PlayerInfo playerInfo = CommonExtension.Instance.GetPlayerInfo(player);

            var res = moneyCrud.Get(playerInfo.Id);

            player.TriggerEvent(Event.Stoc.Bank.GetPlayerMoney, JsonConvert.SerializeObject(res));
        }

        [EventHandler(Event.Ctos.Bank.GetPlayerMoneyAndCompanies)]
        public void GetPlayerMoneyAndCompanies([FromSource] Player player)
        {
            PlayerInfo playerInfo = CommonExtension.Instance.GetPlayerInfo(player);

            var playerMoney = moneyCrud.Get(playerInfo.Id);
            var companies = companyCrud.GetAllCompanies();

            player.TriggerEvent(Event.Stoc.Bank.GetPlayerMoneyAndCompanies, JsonConvert.SerializeObject(playerMoney), JsonConvert.SerializeObject(companies));
        }

        [EventHandler(Event.Ctos.Bank.DoCashTransaction)]
        public void DoCashTransaction([FromSource] Player player, TransactionType type, int amount)
        {
            if (type != TransactionType.Deposit && type != TransactionType.Withdrawal)
                throw new RssException(ErrorCodes.BAD_TRANSACTION_TYPE, null);

            PlayerInfo playerInfo = CommonExtension.Instance.GetPlayerInfo(player);
            PlayerMoney playerMoney = moneyCrud.Get(playerInfo.Id);
            Account source = type == TransactionType.Deposit ? Account.Cash : Account.CurrentAccount;
            Account target = type == TransactionType.Deposit ? Account.CurrentAccount : Account.Cash;
            
            if (type == TransactionType.Deposit)
            {
                if (playerMoney.Cash >= amount)
                {
                    playerMoney.Cash -= amount;
                    playerMoney.CurrentAccount += amount;
                }
            }
            else
            {
                if (playerMoney.CurrentAccount >= amount)
                {
                    playerMoney.Cash += amount;
                    playerMoney.CurrentAccount -= amount;
                }
            }

            Transaction transaction = new Transaction(0, type, amount, DateTime.Now, playerInfo.Id, playerInfo.Id, source, target);

            var updateMoneyRes = moneyCrud.Update(playerMoney);
            var transactionRes = transactionCrud.Insert(transaction);

            if (updateMoneyRes && transactionRes != null)
                player.TriggerEvent(Event.Shared.Common.ShowNotification, $"Transaction de {amount} effectuée");
        }

        [EventHandler(Event.Ctos.Bank.DoPlayerTransfert)]
        public void DoPlayerTransfert([FromSource] Player player, Player target, int amount)
        {
            PlayerInfo playerInfo = CommonExtension.Instance.GetPlayerInfo(player);
            PlayerInfo targetInfo = CommonExtension.Instance.GetPlayerInfo(target);
            PlayerMoney playerMoney = moneyCrud.Get(playerInfo.Id);
            PlayerMoney targetMoney = moneyCrud.Get(targetInfo.Id);
            
            if (playerMoney.CurrentAccount >= amount)
            {
                playerMoney.CurrentAccount -= amount;
                targetMoney.CurrentAccount += amount;
            }

            Transaction transaction = new Transaction(0, TransactionType.TransfertToPlayer, amount, DateTime.Now, playerInfo.Id, targetInfo.Id, Account.CurrentAccount, Account.CurrentAccount);

            var updateSourceRes = moneyCrud.Update(playerMoney);
            var updateTargetRes = moneyCrud.Update(targetMoney);
            var res = transactionCrud.Insert(transaction);

            if (updateSourceRes && updateSourceRes && res != null)
                player.TriggerEvent(Event.Shared.Common.ShowNotification, $"Transaction de {amount} effectuée");
        }

        [EventHandler(Event.Ctos.Bank.DoTransfert)]
        public void DoTransfert([FromSource] Player player, int amount, Account source, Account target)
        {
            PlayerInfo playerInfo = CommonExtension.Instance.GetPlayerInfo(player);
            PlayerMoney playerMoney = moneyCrud.Get(playerInfo.Id);

            bool canTransfert = false;

            switch (source)
            {
                case Account.CurrentAccount:
                    if (playerMoney.CurrentAccount >= amount)
                    {
                        playerMoney.CurrentAccount -= amount;
                        canTransfert = true;
                    }
                    break;
                case Account.Savings:
                    if (playerMoney.Savings >= amount)
                    {
                        playerMoney.Savings -= amount;
                        canTransfert = true;
                    }
                    break;
                case Account.LifeInsurance:
                    if (playerMoney.LifeInsurance >= amount)
                    {
                        playerMoney.LifeInsurance -= amount;
                        canTransfert = true;
                    }
                    break;
            }

            if (canTransfert)
            {
                switch (target)
                {
                    case Account.CurrentAccount:
                        playerMoney.CurrentAccount += amount;
                        break;
                    case Account.Savings:
                        playerMoney.Savings += amount;
                        break;
                    case Account.LifeInsurance:
                        playerMoney.LifeInsurance += amount;
                        break;
                }
            }

            Transaction transaction = new Transaction(0, TransactionType.Transfert, amount, DateTime.Now, playerInfo.Id, playerInfo.Id, source, target);

            var updateSourceRes = moneyCrud.Update(playerMoney);
            var res = transactionCrud.Insert(transaction);

            if (updateSourceRes && res != null)
                player.TriggerEvent(Event.Shared.Common.ShowNotification, $"Transaction de {amount} effectuée");
        }

        [EventHandler(Event.Ctos.Bank.BuyAction)]
        public void BuyAction([FromSource] Player player, long companyId, int amount)
        {
            try
            {
                PlayerInfo playerInfo = CommonExtension.Instance.GetPlayerInfo(player);
                Company company = companyCrud.Get(companyId);
                PlayerMoney playerMoney = moneyCrud.Get(playerInfo.Id);
                Stock stock = playerMoney.Stock.Find(o => o.CompanyId == companyId);

                int transactionCost = amount * company.CurrentPrice;
                int rest = playerMoney.CurrentAccount - transactionCost;

                if (rest > 0)
                {
                    if (stock == default(Stock))
                    {
                        stock = new Stock(0, playerInfo.Id, companyId, amount);
                        playerMoney.Stock.Add(stock);
                    }
                    else
                        stock.Amount += amount;

                    playerMoney.CurrentAccount -= transactionCost;
                    Transaction transaction = new Transaction(0, TransactionType.Transfert, -transactionCost, DateTime.Now, playerInfo.Id, playerInfo.Id, Account.CurrentAccount, Account.Stock);

                    bool updateSourceRes = moneyCrud.Update(playerMoney);
                    var res = transactionCrud.Insert(transaction);

                    if (updateSourceRes && res != null)
                        player.TriggerEvent(Event.Shared.Common.ShowNotification, $"Transaction de {amount} effectuée");
                }
                else
                    player.TriggerEvent(Event.Shared.Common.ShowNotification, "Vous n'avez pas assez d'argent");
            } 
            catch (Exception e)
            {
                Debug.WriteLine("Exception : " + e);
            }
        }

        [EventHandler(Event.Ctos.Bank.SellAction)]
        public void SellAction([FromSource] Player player, long companyId, int amount)
        {
            PlayerInfo playerInfo = CommonExtension.Instance.GetPlayerInfo(player);
            Company company = companyCrud.Get(companyId);
            PlayerMoney playerMoney = moneyCrud.Get(playerInfo.Id);
            Stock stock = playerMoney.Stock.Find(o => o.CompanyId == companyId);

            if (stock == default(Stock))
                return;

            int transactionGain = amount * company.CurrentPrice;
            stock.Amount -= amount;

            if (stock.Amount == 0)
                playerMoney.Stock.Remove(stock);

            if (stock.Amount >= 0)
            {
                playerMoney.CurrentAccount += transactionGain;
                if (moneyCrud.Update(playerMoney))
                {
                    Transaction transaction = new Transaction(0, TransactionType.Transfert, transactionGain, DateTime.Now, playerInfo.Id, playerInfo.Id, Account.CurrentAccount, Account.Stock);

                    var res = transactionCrud.Insert(transaction);

                    if (res != null)
                        player.TriggerEvent(Event.Shared.Common.ShowNotification, $"Transaction de {amount} effectuée");
                }
            }
        }
    }
}
