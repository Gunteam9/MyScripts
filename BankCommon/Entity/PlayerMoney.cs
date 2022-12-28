using System.Collections.Generic;

namespace BankCommon.Entity
{
    public class PlayerMoney
    {
        public PlayerMoney()
        {
        }

        public PlayerMoney(long id, int cash, int currentAccount, int savings, List<Stock> stock, int lifeInsurance)
        {
            Id = id;
            Cash = cash;
            CurrentAccount = currentAccount;
            Savings = savings;
            Stock = stock;
            LifeInsurance = lifeInsurance;
        }

        /// <summary>
        ///     ID du joueur
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     Argent en cash
        /// </summary>
        public float Cash { get; set; }

        /// <summary>
        ///     Argent sur le compte courant
        /// </summary>
        public float CurrentAccount { get; set; }

        /// <summary>
        ///     Épargne sur livret
        /// </summary>
        public float Savings { get; set; }

        /// <summary>
        ///     Actions
        /// </summary>
        public List<Stock> Stock { get; set; }

        /// <summary>
        ///     Épargne en assurance vie
        /// </summary>
        public float LifeInsurance { get; set; }
    }
}