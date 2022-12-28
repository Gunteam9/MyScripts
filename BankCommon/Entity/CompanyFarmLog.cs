using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankCommon.Entity
{
    /// <summary>
    /// Entité de log pour le farm des entreprises. A chaque vente groupée, le CA généré est enregistrer avec la date correspondante.
    /// </summary>
    public class CompanyFarmLog
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// ID de l'entreprise correspond à l'entité
        /// </summary>
        public long CompanyId { get; set; }

        /// <summary>
        /// Date d'enregistrement
        /// </summary>
        public DateTime FarmDate { get; set; }

        /// <summary>
        /// Chiffre d'affaire généré sur l'enregistrement
        /// </summary>
        public float FarmCaValue { get; set; }

        public CompanyFarmLog()
        {

        }

        public CompanyFarmLog(long id, long companyId, DateTime farmDate, float farmCAValue)
        {
            Id = id;
            CompanyId = companyId;
            FarmDate = farmDate;
            FarmCaValue = farmCAValue;
        }

    }
}
