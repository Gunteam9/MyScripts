using BankCommon.Entity;
using BankServer.Crud;
using CitizenFX.Core;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using API = CitizenFX.Core.Native.API;

namespace BankServer.Manager
{
    public class StockManager : BaseScript
    {
        // En minute
        private const float REFRESH_TIMER = 5f;

        private readonly CompanyCrud companyCrud = new CompanyCrud();
        private readonly TrendCrud trendCrud= new TrendCrud();
        private readonly CompanyFarmLogCrud companyFarmLogCrud = new CompanyFarmLogCrud();

        private readonly Random random = new Random();

        [EventHandler(Event.Server.FiveM.OnResourceStart)]
        public void OnServerResourceStart(string resourceName)
        {
            if (resourceName != "bank")
                return;

            var timer5Minutes = new Timer(TimeSpan.FromMinutes(REFRESH_TIMER).Seconds);
            timer5Minutes.Enabled = true;
            timer5Minutes.Elapsed += RefreshStockValueByTimer5;
            
            var timer30Minutes = new Timer(TimeSpan.FromMinutes(30).Seconds);
            timer30Minutes.Enabled = true;
            timer30Minutes.Elapsed += RefreshStockValueByTimer30;

            // On applique les tendances à un timing aléatoire dans la journée
            // Trop gourmand en ressource ? 
            Task.Delay(new TimeSpan(random.Next(0, (int)TimeSpan.FromDays(1).Ticks))).ContinueWith((o) => UpdateStockByTrend());
        }

        /// <summary>
        /// Cette fonction est appelée toutes les 5 minutes
        /// </summary>
        private void RefreshStockValueByTimer5(object sender, ElapsedEventArgs e)
        {
            UpdateStockByRandom();
        } 
        
        /// <summary>
        /// Cette fonction est appelée toutes les 30 minutes
        /// </summary>
        private void RefreshStockValueByTimer30(object sender, ElapsedEventArgs e)
        {
            UpdateStockByFarm();
        }

        /// <summary>
        /// Met à jour la valeur de chaque action lorsqu'un joueur achète ou vend des actions
        /// </summary>
        /// <param name="updatedCompanyId"></param>
        /// <param name="volumeBought"></param>
        [EventHandler(Event.Server.Bank.OnVolumeUpdate)]
        private void UpdateStockByVolume(long updatedCompanyId, int volumeBought)
        {
            List<Company> allCompanies = companyCrud.GetAllCompanies();
            Company updatedCompany = allCompanies.Find(o => o.Id == updatedCompanyId);
            allCompanies.Remove(updatedCompany);

            int totalVolume = allCompanies.Sum(o => o.Volume);

            // Pour l'entreprise où on a acheté des actions, on augmente le prix
            updatedCompany.CurrentPrice += updatedCompany.CurrentPrice * Utils.Clamp(volumeBought / totalVolume * 0.5f, 0f, 0.2f);
            companyCrud.Update(updatedCompany);

            // Pour les autres, on fait baisser le prix
            foreach (var otherCompany in allCompanies)
            {
                otherCompany.CurrentPrice -= otherCompany.CurrentPrice * Utils.Clamp(volumeBought / totalVolume * 0.5f, 0f, 0.2f);
                companyCrud.Update(otherCompany);
            }
        }

        /// <summary>
        /// Met à jour la valeur des actions en fonction de l'activité de farm de l'entreprise
        /// Toutes les 30 minutes à heure fixe (12h00, 12h30, ...)
        /// </summary>
        private void UpdateStockByFarm()
        {
            List<Company> allCompanies = companyCrud.GetAllCompanies();

            // On recherche la demi heure précise du jour précédent à laquelle on retire 10 demi-heure.
            // Ex: Si on est le 20/12 à 18:10, on cherche le 19/12 à 13:00
            DateTime start = DateTime.Now.AddDays(-1).AddMinutes(-30 * 10);
            start.AddMinutes(start.Minute < 30 ? -start.Minute : -start.Minute + 30);
            DateTime end = start.AddMinutes(30 * 10);

            foreach (var company in allCompanies)
            {
                // On prend tous les enregistrements dans cette période
                List<CompanyFarmLog> companyFarmLogsYesterday = companyFarmLogCrud.GetBetween(company.Id, start, end);

                // On modifie les dates de début et de fin pour éviter trop de calcul
                start = start.AddDays(1);
                end = end.AddDays(1);
                List<CompanyFarmLog> companyFarmLogsToday = companyFarmLogCrud.GetBetween(company.Id, start, end);

                // On organise les données en tranche de 30 minutes
                Dictionary<DateTime, float> organizedFarmYesterday = new Dictionary<DateTime, float>();
                foreach (var farmLog in companyFarmLogsYesterday)
                {
                    if (farmLog.FarmDate.Minute < 30)
                    {
                        if (organizedFarmYesterday.TryGetValue(new DateTime(farmLog.FarmDate.Year, farmLog.FarmDate.Month, farmLog.FarmDate.Day, farmLog.FarmDate.Hour, 0, 0), out float value))
                            value += farmLog.FarmCaValue;
                        else
                            organizedFarmYesterday.Add(new DateTime(farmLog.FarmDate.Year, farmLog.FarmDate.Month, farmLog.FarmDate.Day, farmLog.FarmDate.Hour, 0, 0), farmLog.FarmCaValue);
                    }
                    else
                    {
                        if (organizedFarmYesterday.TryGetValue(new DateTime(farmLog.FarmDate.Year, farmLog.FarmDate.Month, farmLog.FarmDate.Day, farmLog.FarmDate.Hour, 30, 0), out float value))
                            value += farmLog.FarmCaValue;
                        else
                            organizedFarmYesterday.Add(new DateTime(farmLog.FarmDate.Year, farmLog.FarmDate.Month, farmLog.FarmDate.Day, farmLog.FarmDate.Hour, 30, 0), farmLog.FarmCaValue);
                    }
                }

                Dictionary<DateTime, float> organizedFarmToday = new Dictionary<DateTime, float>();
                foreach (var farmLog in companyFarmLogsToday)
                {
                    if (farmLog.FarmDate.Minute < 30)
                    {
                        if (organizedFarmToday.TryGetValue(new DateTime(farmLog.FarmDate.Year, farmLog.FarmDate.Month, farmLog.FarmDate.Day, farmLog.FarmDate.Hour, 0, 0), out float value))
                            value += farmLog.FarmCaValue;
                        else
                            organizedFarmToday.Add(new DateTime(farmLog.FarmDate.Year, farmLog.FarmDate.Month, farmLog.FarmDate.Day, farmLog.FarmDate.Hour, 0, 0), farmLog.FarmCaValue);
                    }
                    else
                    {
                        if (organizedFarmToday.TryGetValue(new DateTime(farmLog.FarmDate.Year, farmLog.FarmDate.Month, farmLog.FarmDate.Day, farmLog.FarmDate.Hour, 30, 0), out float value))
                            value += farmLog.FarmCaValue;
                        else
                            organizedFarmToday.Add(new DateTime(farmLog.FarmDate.Year, farmLog.FarmDate.Month, farmLog.FarmDate.Day, farmLog.FarmDate.Hour, 30, 0), farmLog.FarmCaValue);
                    }
                }

                float totalCaYesterday = organizedFarmYesterday.Sum(o => o.Value);
                float totalCaToday = organizedFarmToday.Sum(o => o.Value);

                float evolution = Utils.Median(new List<float>() { totalCaYesterday / totalCaToday, 0.95f, 1.05f });

                company.CurrentPrice *= company.CurrentPrice * evolution;
                companyCrud.Update(company);
            }
        }

        /// <summary>
        /// Met à jour la valeur des entreprises en fonction des tendances boursières
        /// Tous les jours à heure aléatoire
        /// </summary>
        private void UpdateStockByTrend()
        {
            List<Company> allCompanies = companyCrud.GetAllCompanies();
            List<Trend> allTrends = trendCrud.GetAllTrends();

            foreach (var trend in allTrends)
            {
                int daysBetweenStart = (DateTime.Now - trend.Start).Days;
                int randomNumber = random.Next(0, 100) * trend.Duration - daysBetweenStart;
                float randomEvolution;

                if (randomNumber < 40)
                    randomEvolution = random.Next(0, (int)(trend.RemainingValue / trend.Duration * 100)) / 100f;
                else if (randomNumber < 80)
                    randomEvolution = random.Next((int)(trend.RemainingValue / trend.Duration * 100), (int)(trend.RemainingValue / trend.Duration * 100 * 2)) / 100f;
                else
                    randomEvolution = random.Next((int)(trend.RemainingValue / trend.Duration * 100 * 2), (int)(trend.RemainingValue / trend.Duration * 100 * 3)) / 100f;

                trend.RemainingValue -= randomEvolution;

                foreach (var company in allCompanies)
                {
                    if (company.Sectors.Contains(trend.Sector))
                    {
                        company.CurrentPrice *= randomEvolution / 100f;
                        companyCrud.Update(company);
                    }
                }

                if (daysBetweenStart >= trend.Duration)
                    trendCrud.Remove(trend.Id);
                else
                    trendCrud.Update(trend);
            }
        }

        /// <summary>
        /// Met à jour la valeur des entreprises en fonction de l'aléatoire
        /// Toutes les <see cref="REFRESH_TIMER"/> minutes
        /// </summary>
        private void UpdateStockByRandom()
        {
            List<Company> allCompanies = new List<Company>();

            foreach (var company in allCompanies)
            {
                float randomEvolution = random.Next(99, 102) / 100f;
                company.CurrentPrice *= randomEvolution;
                companyCrud.Update(company);
            }
        }
    }
}