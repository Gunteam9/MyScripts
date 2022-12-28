using System.Collections.Generic;

namespace BankCommon.Entity
{
    /// <summary>
    ///     Propriétés boursières des entreprises
    /// </summary>
    public class Company
    {
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

        /// <summary>
        ///     ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     Nom
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Sigle (Nom sur 4 caractères maximum)
        /// </summary>
        public string Acronym { get; set; }

        /// <summary>
        ///     Secteurs d'activités
        /// </summary>
        public List<Sector> Sectors { get; set; }

        /// <summary>
        ///     Prix à l'ouverture (03h00)
        /// </summary>
        public float PriceAtOpening { get; set; }

        /// <summary>
        ///     Valeur actuel de l'action
        /// </summary>
        public float CurrentPrice { get; set; }

        /// <summary>
        ///     Volume d'action échangé aujourd'hui (depuis 03h00) <br />
        ///     Ce champ n'est pas stocké en base mais calculé
        /// </summary>
        public int Volume { get; set; }

        /// <summary>
        ///     Évolution du prix depuis l'ouverture
        /// </summary>
        public float Evolution => CurrentPrice - PriceAtOpening;

        /// <summary>
        ///     Évolution du prix depuis l'ouverture en pourcentage
        /// </summary>
        public float EvolutionAsPercent => (CurrentPrice - PriceAtOpening) / PriceAtOpening * 100;
    }
}