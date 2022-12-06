using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankCommon.Entity
{
    /// <summary>
    /// Tendances boursières
    /// </summary>
    public class Trend
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Durée de la tendance en jour
        /// </summary>
        public int Duration { get; private set; }

        /// <summary>
        /// Importance de la tendance
        /// </summary>
        public int Importance { get; private set; }

        /// <summary>
        /// Secteur d'activité de la tendance
        /// </summary>
        public Sector Sector { get; private set; }

        private readonly Random random = new Random();

        public Trend()
        {

        }

        public Trend(long id, int duration, int importance, Sector sector)
        {
            Id = id;
            Duration = duration;
            Importance = importance;
            Sector = sector;
        }

        /// <summary>
        /// Initialise de manière aléatoire les propriétés de la tendance <br />
        /// L'ID n'est pas initialisé
        /// </summary>
        public void RandomInitialize()
        {
            Duration = random.Next(Const.MIN_TREND_DURATION, Const.MAX_TREND_DURATION);
            Importance = random.Next(Const.MIN_TREND_IMPORTANCE, Const.MAX_TREND_IMPORTANCE);
            Array allSectors = Enum.GetValues(typeof(Sector));
            Sector = (Sector)allSectors.GetValue(random.Next(allSectors.Length));
        }
    }
}
