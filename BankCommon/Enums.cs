using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankCommon
{
    /// <summary>
    /// Secteur des entreprises
    /// </summary>
    public enum Sector
    {
        /// <summary>
        /// Communication
        /// </summary>
        Communication = 0,

        /// <summary>
        /// Service aux consommateurs
        /// </summary>
        Consumer = 1,

        /// <summary>
        /// Energie
        /// </summary>
        Energy = 2, 

        /// <summary>
        /// Financier
        /// </summary>
        Financial = 3,

        /// <summary>
        /// Médical
        /// </summary>
        Healthcare = 4,

        /// <summary>
        /// Industrie
        /// </summary>
        Industrials = 5,

        /// <summary>
        /// Matière première
        /// </summary>
        Materials = 6,
        
        /// <summary>
        /// Services
        /// </summary>
        Utilities = 7
    }

    /// <summary>
    /// Type de transaction
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// Dépôt
        /// </summary>
        Deposit,

        /// <summary>
        /// Retrait
        /// </summary>
        Withdrawal,

        /// <summary>
        /// Virement d'argent vers un autre joueur
        /// </summary>
        TransfertToPlayer,

        /// <summary>
        /// Transfert vers un autre compte du même joueur
        /// </summary>
        Transfert
    }

    /// <summary>
    /// Liste des différents type de compte
    /// </summary>
    public enum Account
    {
        /// <summary>
        /// Cash
        /// </summary>
        Cash = 0,

        /// <summary>
        /// Compte courant
        /// </summary>
        CurrentAccount = 1, 

        /// <summary>
        /// Épargne sur livret
        /// </summary>
        Savings = 2,

        /// <summary>
        /// Actions
        /// </summary>
        Stock = 3,
        
        /// <summary>
        /// Assurance vie
        /// </summary>
        LifeInsurance = 4
    }
}
