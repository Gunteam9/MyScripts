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

        /// <summary>
        /// Crée une entité correspondante à l'argent du joueur par défaut en base (première connexion)
        /// </summary>
        [EventHandler(Event.Server.Bank.CreateEmptyPlayerMoney)]
        public void CreateEmptyPlayerMoney(long playerId)
        {
            PlayerMoney playerMoney = new PlayerMoney(0, 100, 500, 0, new List<Stock>(), 0);

            moneyCrud.Insert(playerMoney);
        }

        /// <summary>
        /// Renvoi les X dernières transaction bancaires du joueur
        /// </summary>
        /// <param name="transactionCount">Nombre de transaction</param>
        [EventHandler(Event.Ctos.Bank.GetLastXTransactions)]
        public void GetLastXTransactions([FromSource] Player player, int transactionCount)
        {
            PlayerInfo playerInfo = CommonExtension.Instance.GetPlayerInfo(player);

            var res = transactionCrud.GetLastXTransactionForPlayer(playerInfo.Id, transactionCount);

            player.TriggerEvent(Event.Stoc.Bank.GetLastXTransactions, JsonConvert.SerializeObject(res));
        }

        /// <summary>
        /// Renvoi l'argent du joueur
        /// </summary>
        [EventHandler(Event.Ctos.Bank.GetPlayerMoney)]
        public void GetPlayerMoney([FromSource] Player player)
        {
            PlayerInfo playerInfo = CommonExtension.Instance.GetPlayerInfo(player);

            var res = moneyCrud.Get(playerInfo.Id);

            player.TriggerEvent(Event.Stoc.Bank.GetPlayerMoney, JsonConvert.SerializeObject(res));
        }

        /// <summary>
        /// Renvoi les informations bancaires du joueur ainsi que les informations bancaires de toutes les entreprises
        /// </summary>
        [EventHandler(Event.Ctos.Bank.GetPlayerMoneyAndCompanies)]
        public void GetPlayerMoneyAndCompanies([FromSource] Player player)
        {
            PlayerInfo playerInfo = CommonExtension.Instance.GetPlayerInfo(player);

            var playerMoney = moneyCrud.Get(playerInfo.Id);
            var companies = companyCrud.GetAllCompanies();

            player.TriggerEvent(Event.Stoc.Bank.GetPlayerMoneyAndCompanies, JsonConvert.SerializeObject(playerMoney), JsonConvert.SerializeObject(companies));
        }

        /// <summary>
        /// Renvoi les informations bancaires de l'entreprise ainsi que celles du joueur
        /// </summary>
        /// <param name="companyId">Id de l'entreprise</param>
        [EventHandler(Event.Ctos.Bank.GetCompanyInfoAndPlayerMoney)]
        public void GetCompanyInfoAndPlayerMoney([FromSource] Player player, long companyId)
        {
            PlayerInfo playerInfo = CommonExtension.Instance.GetPlayerInfo(player);

            var company = companyCrud.Get(companyId);
            var playerMoney = moneyCrud.Get(playerInfo.Id);

            player.TriggerEvent(Event.Stoc.Bank.GetCompanyInfoAndPlayerMoney, JsonConvert.SerializeObject(company), JsonConvert.SerializeObject(playerMoney));
        }

        /// <summary>
        /// Effectue un transfert d'argent entre compte courant et cash ou inversement
        /// </summary>
        /// <param name="type">Type de transfert (Dépot / Retrait)</param>
        /// <param name="amount">Montant de la transaction</param>
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
                else
                    player.TriggerEvent(Event.Shared.Common.ShowNotification, $"Tu n'as pas assez d'argent pour déposer cette somme");
            }
            else
            {
                if (playerMoney.CurrentAccount >= amount)
                {
                    playerMoney.Cash += amount;
                    playerMoney.CurrentAccount -= amount;
                }
                else
                    player.TriggerEvent(Event.Shared.Common.ShowNotification, $"Tu n'as pas assez d'argent sur ton compte courant pour retirer cette somme");
            }

            Transaction transaction = new Transaction(0, type, amount, DateTime.Now, playerInfo.Id, playerInfo.Id, source, target);

            var updateMoneyRes = moneyCrud.Update(playerMoney);
            var transactionRes = transactionCrud.Insert(transaction);

            if (updateMoneyRes && transactionRes != null)
                player.TriggerEvent(Event.Shared.Common.ShowNotification, $"Transaction de {amount} effectuée");
            else
                player.TriggerEvent(Event.Shared.Common.ShowNotification, "La transaction n'a pas pu être effectuée");
        }

        /// <summary>
        /// Effectue un transfert d'argent vers un autre joueur. <br />
        /// Les transferts s'effectue toujours de compte courant à compte courant
        /// </summary>
        /// <param name="target">Joueur cible</param>
        /// <param name="amount">Montant de la transaction</param>
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
            else
                player.TriggerEvent(Event.Shared.Common.ShowNotification, $"Tu n'as pas assez d'éargent pour faire ça");

            Transaction transaction = new Transaction(0, TransactionType.TransfertToPlayer, amount, DateTime.Now, playerInfo.Id, targetInfo.Id, Account.CurrentAccount, Account.CurrentAccount);

            var updateSourceRes = moneyCrud.Update(playerMoney);
            var updateTargetRes = moneyCrud.Update(targetMoney);
            var res = transactionCrud.Insert(transaction);

            if (updateSourceRes && updateTargetRes && res != null)
                player.TriggerEvent(Event.Shared.Common.ShowNotification, $"Transaction de {amount} effectuée");
            else
                player.TriggerEvent(Event.Shared.Common.ShowNotification, $"La transaction n'a pas pu être effectuée");
        }

        /// <summary>
        /// Permet de transférer de l'argent entre les comptes du joueur
        /// </summary>
        /// <param name="amount">Montant de la transaction</param>
        /// <param name="source">Compte source</param>
        /// <param name="target">Compte cible</param>
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
            else
                player.TriggerEvent("Tu n'as pas assez d'argent pour effectuer ce transfert");

            Transaction transaction = new Transaction(0, TransactionType.Transfert, amount, DateTime.Now, playerInfo.Id, playerInfo.Id, source, target);

            var updateSourceRes = moneyCrud.Update(playerMoney);
            var res = transactionCrud.Insert(transaction);

            if (updateSourceRes && res != null)
                player.TriggerEvent(Event.Shared.Common.ShowNotification, $"Transaction de {amount} effectuée");
            else
                player.TriggerEvent(Event.Shared.Common.ShowNotification, "La transaction n'a pas pu être effectuée");
        }

        /// <summary>
        /// Permet au joueur d'acheter des actions à une entreprise
        /// </summary>
        /// <param name="companyId">Identifiant de l'entreprise</param>
        /// <param name="stockType">Type d'action à acheter (Normal, VAD, ...)</param>
        /// <param name="leverage">Levier de l'achat</param>
        /// <param name="amountToBuy">Nombre d'action à acheter</param>
        [EventHandler(Event.Ctos.Bank.BuyAction)]
        public void BuyAction([FromSource] Player player, long companyId, StockType stockType, int leverage, int amountToBuy)
        {
            try
            {
                PlayerInfo playerInfo = CommonExtension.Instance.GetPlayerInfo(player);
                Company company = companyCrud.Get(companyId);
                PlayerMoney playerMoney = moneyCrud.Get(playerInfo.Id);
                Stock stock = playerMoney.Stock.Find(o => o.CompanyId == companyId);

                float transactionCost = amountToBuy * company.CurrentPrice;
                float rest = playerMoney.CurrentAccount - transactionCost;

                if (rest > 0)
                {
                    if (stock == default(Stock))
                    {
                        stock = new Stock(0, playerInfo.Id, company.Id, stockType, amountToBuy, amountToBuy * leverage, company.CurrentPrice);
                        playerMoney.Stock.Add(stock);
                    }
                    else
                    {
                        stock.AverageBuyValue = (stock.Amount * stock.AverageBuyValue + amountToBuy * company.CurrentPrice) / (stock.Amount + amountToBuy);
                        stock.Amount += amountToBuy;
                        stock.SimulatedAmount += amountToBuy * leverage;
                    }

                    playerMoney.CurrentAccount -= transactionCost;
                    Transaction transaction = new Transaction(0, TransactionType.Transfert, transactionCost, DateTime.Now, playerInfo.Id, playerInfo.Id, Account.CurrentAccount, Account.Stock);

                    bool updateSourceRes = moneyCrud.Update(playerMoney);
                    var res = transactionCrud.Insert(transaction);

                    if (updateSourceRes && res != null)
                        player.TriggerEvent(Event.Shared.Common.ShowNotification, $"Transaction de {amountToBuy} actions effectuée");
                    else
                        player.TriggerEvent(Event.Shared.Common.ShowNotification, "La transaction n'a pas pu être effectuée");
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
        public void SellAction([FromSource] Player player, long companyId, StockType stockType, int leverage, int amountToSell)
        {
            // On récupère les infos
            PlayerInfo playerInfo = CommonExtension.Instance.GetPlayerInfo(player);
            Company company = companyCrud.Get(companyId);
            PlayerMoney playerMoney = moneyCrud.Get(playerInfo.Id);
            Stock stock = playerMoney.Stock.Find(o => o.CompanyId == companyId);

            // On doit posséder les actions
            if (stock == default(Stock) || stock.Amount < amountToSell)
                player.TriggerEvent(Event.Shared.Common.ShowNotification, "Tu veux vendre des actions que tu ne possède pas ?");
            else
            {
                // On calcul l'effet de levier moyen
                float averageLeverage = stock.SimulatedAmount / stock.Amount;

                // On calcul aussi le gain (nombre d'action * valeur + différence * effet de levier)
                float transactionGain = 0;
                if (stockType == StockType.Normal)
                    transactionGain = amountToSell * company.CurrentPrice + (company.CurrentPrice - stock.AverageBuyValue) * (amountToSell * (averageLeverage - 1));
                else if (stockType == StockType.VAD)
                    transactionGain = amountToSell * company.CurrentPrice + (stock.AverageBuyValue - company.CurrentPrice) * (amountToSell * (averageLeverage + 1));

                // Enfin, on enlève les actions
                stock.Amount -= amountToSell;
                playerMoney.CurrentAccount += transactionGain;

                // Si on tombe à 0, on supprime le stock, sinon on l'update
                bool isCorrectlyUpdated = false;
                if (stock.Amount == 0)
                    isCorrectlyUpdated = playerMoney.Stock.Remove(stock);
                else // stock.Amount >= 0
                    isCorrectlyUpdated = moneyCrud.Update(playerMoney);

                Transaction transaction = new Transaction(0, TransactionType.Transfert, transactionGain, DateTime.Now, playerInfo.Id, playerInfo.Id, Account.CurrentAccount, Account.Stock);
                var transactionRes = transactionCrud.Insert(transaction);

                if (isCorrectlyUpdated && transactionRes != null)
                    player.TriggerEvent(Event.Shared.Common.ShowNotification, $"Transaction de {amountToSell} actions effectuée");
                else
                    player.TriggerEvent(Event.Shared.Common.ShowNotification, "La transaction a échouée");
            }
        }
    }
}
