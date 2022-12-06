using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankCommon.Entity
{
    /// <summary>
    /// Propriétés boursières des entreprises
    /// </summary>
    public class Company
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Nom
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Sigle (Nom sur 4 caractères maximum)
        /// </summary>
        public string Acronym { get; set; }

        /// <summary>
        /// Secteurs d'activités
        /// </summary>
        public List<Sector> Sectors { get; set; }

        /// <summary>
        /// Prix à l'ouverture (03h00)
        /// </summary>
        public int PriceAtOpening { get; set; }

        /// <summary>
        /// Valeur actuel de l'action
        /// </summary>
        public int CurrentPrice { get; set; }

        public Company()
        {

        }

        public Company(long id, string name, string acronym, List<Sector> sectors, int priceAtOpening, int currentPrice)
        {
            Id = id;
            Name = name;
            Acronym = acronym;
            Sectors = sectors;
            PriceAtOpening = priceAtOpening;
            CurrentPrice = currentPrice;
        }
    }
}
