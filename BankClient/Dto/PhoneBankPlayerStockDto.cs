using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankClient.Dto
{
    /// <summary>
    /// Ce DTO regroupe les informations nécessaires pour la vue "Action" sur le téléphone du joueur
    /// </summary>
    public class PhoneBankPlayerStockDto
    {
        /// <summary>
        /// Id de l'entreprise
        /// </summary>
        public long CompanyId { get; set; }

        /// <summary>
        /// Nom de l'entreprise
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Nombre d'action détenue par le joueur
        /// </summary>
        public int StockAmount { get; set; }

        /// <summary>
        /// Valeur d'une action de l'entreprise
        /// </summary>
        public int CompanyStockValue { get; set; }

        /// <summary>
        /// Évolution du prix de l'action de l'entreprise depuis l'ouverture
        /// </summary>
        public float CompanyStockValueEvolution { get; set; }

        /// <summary>
        /// Valeur total des actions détenues par le joueur pour l'entreprise
        /// </summary>
        public int TotalStockValue { get; set; }

    }
}
