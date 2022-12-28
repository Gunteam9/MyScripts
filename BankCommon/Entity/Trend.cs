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
        /// Date de début de la tendance
        /// </summary>
        public DateTime Start { get; private set; }

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

        /// <summary>
        /// Valeur total du modificateur 
        /// </summary>
        public float TotalValue { get => Importance * 10; }

        /// <summary>
        /// Valeur restante du modificateur
        /// </summary>
        public float RemainingValue { get; set; }


        public Trend()
        {

        }

        public Trend(long id, DateTime start, int duration, int importance, Sector sector, float remainingValue)
        {
            Id = id;
            Start = start;
            Duration = duration;
            Importance = importance;
            Sector = sector;
            RemainingValue = remainingValue;
        }



        /// <summary>
        /// Initialise de manière aléatoire les propriétés de la tendance <br />
        /// L'ID n'est pas initialisé
        /// </summary>
        public static Trend RandomInitialize()
        {
            Random random = new Random();
            Array allSectors = Enum.GetValues(typeof(Sector));

            int importance = random.Next(Const.MIN_TREND_IMPORTANCE, Const.MAX_TREND_IMPORTANCE);
            if (importance == 0) 
                importance = random.Next(0, 2) == 0 ? -1 : 1;


            return new Trend()
            {
                Start = DateTime.Now,
                Duration = random.Next(Const.MIN_TREND_DURATION, Const.MAX_TREND_DURATION),
                Importance = importance,
                Sector = (Sector)allSectors.GetValue(random.Next(allSectors.Length))
            };
        }
    }
}
