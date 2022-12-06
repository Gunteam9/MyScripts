using BankCommon;
using BankCommon.Entity;
using Common;
using Common.Errors;
using CommonServer;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankServer.Crud
{
    public class CompanyCrud : ACrud<Company>
    {
        public override Company Get(long id)
        {
            try
            {
                connection.Open();

                const string sql = "SELECT * FROM Company WHERE id=@id";

                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    long companyId = Convert.ToInt64(reader["Id"]);
                    List<Sector> sectors = this.GetSectors(companyId);
                    return new Company(
                        companyId,
                        Convert.ToString(reader["Name"]), 
                        Convert.ToString(reader["Acronym"]),
                        sectors,
                        Convert.ToInt32(reader["PriceAtOpening"]),
                        Convert.ToInt32(reader["CurrentPrice"]));
                }

                return null;
            }
            catch (SQLiteException e)
            {
                throw new RssException(ErrorCodes.SQL_ERROR, e);
            }
            finally
            {
                connection.Close();
            }
        }
        public List<Company> GetAllCompanies()
        {
            try
            {
                connection.OpenAsync();

                const string sql = "SELECT * FROM Company";

                var command = new SQLiteCommand(sql, connection);
                var reader = command.ExecuteReader();

                List<Company> companies = new List<Company>();

                while (reader.Read())
                {
                    long companyId = Convert.ToInt64(reader["Id"]);
                    List<Sector> sectors = this.GetSectors(companyId);
                    companies.Add(new Company(
                        companyId,
                        Convert.ToString(reader["Name"]), 
                        Convert.ToString(reader["Acronym"]),
                        sectors,
                        Convert.ToInt32(reader["PriceAtOpening"]),
                        Convert.ToInt32(reader["CurrentPrice"])));
                }

                return companies;
            }
            catch (SQLiteException e)
            {
                throw new RssException(ErrorCodes.SQL_ERROR, e);
            }
            finally
            {
                connection.Close();
            }
        }

        public override Company Insert(Company objectToSave)
        {
            try
            {
                connection.Open();
                connection.SetExtendedResultCodes(true);

                const string sql = "INSERT INTO Company(Name, Acronym, PriceAtOpening, CurrentPrice) VALUES (@name, @acronym, @priceAtOpening, @currentPrice)";

                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@name", objectToSave.Name);
                command.Parameters.AddWithValue("@acronym", objectToSave.Acronym);
                command.Parameters.AddWithValue("@priceAtOpening", objectToSave.PriceAtOpening);
                command.Parameters.AddWithValue("@currentPrice", objectToSave.CurrentPrice);

                var res = command.ExecuteNonQuery();

                if (res > 0)
                {
                    long companyId = connection.LastInsertRowId;
                    foreach (var sector in objectToSave.Sectors)
                    {
                        if (!this.AddSector(companyId, (int)sector))
                            return null;
                    }

                    objectToSave.Id = companyId;
                    return objectToSave;
                }

                return null;
            }
            catch (SQLiteException e)
            {
                if (e.ResultCode == SQLiteErrorCode.Constraint_Unique)
                    return null;

                throw new RssException(ErrorCodes.SQL_ERROR, e);
            }
            finally
            {
                connection.Close();
            }
        }

        public override bool Update(Company objectToSave)
        {
            try
            {
                connection.Open();

                // On recherche les secteurs d'activité de l'entreprise
                const string sectorSql = "SELECT * FROM CompanySector WHERE CompanyId=@id";

                var sectorCommand = new SQLiteCommand(sectorSql, connection);
                sectorCommand.Parameters.AddWithValue("@id", objectToSave.Id);

                var sectorReader = sectorCommand.ExecuteReader();

                List<Sector> savedSectors = new List<Sector>();

                while (sectorReader.Read())
                    savedSectors.Add((Sector)sectorReader["SectorId"]);

                // Ensuite on en ajoute ou en supprime en fonction de la nouvauté
                var differentsSector = savedSectors.Except(objectToSave.Sectors);
                foreach (var sector in differentsSector)
                {
                    if (objectToSave.Sectors.Contains(sector))
                    {
                        if (!this.AddSector(objectToSave.Id, (int)sector))
                            return false;
                    }
                    else
                    {
                        if (!this.RemoveSector(objectToSave.Id, (int)sector))
                            return false;
                    }
                }

                // Puis on met à jour le reste des informations
                const string sql = "UPDATE Company SET Name=@name, Acronym=@acronym, PriceAtOpening=@priceAtOpening, CurrentPrice=@currentPrice WHERE Id=@id";

                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@name", objectToSave.Name);
                command.Parameters.AddWithValue("@acronym", objectToSave.Acronym);
                command.Parameters.AddWithValue("@priceAtOpening", objectToSave.PriceAtOpening);
                command.Parameters.AddWithValue("@currentPrice", objectToSave.CurrentPrice);
                command.Parameters.AddWithValue("@id", objectToSave.Id);

                var res = command.ExecuteNonQuery();

                return res > 0;
            }
            catch (SQLiteException e)
            {
                throw new RssException(ErrorCodes.SQL_ERROR, e);
            }
            finally
            {
                connection.Close();
            }
        }

        public override bool Remove(long id)
        {
            try
            {
                connection.Open();

                const string sql = "DELETE FROM Company WHERE CompanyId=@id";

                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                var res = command.ExecuteNonQuery();

                if (res > 0)
                {
                    List<Sector> sectorToDel = (this.Get(id)).Sectors;

                    foreach (var item in sectorToDel)
                    {
                        if (!this.RemoveSector(id, (int)item))
                            return false;
                    }

                    return true;
                }
                else
                    return false;
            }
            catch (SQLiteException e)
            {
                throw new RssException(ErrorCodes.SQL_ERROR, e);
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Récupère les secteurs d'activité de l'entreprise
        /// </summary>
        /// <param name="companyId">Id de l'entreprise</param>
        /// <exception cref="RssException"></exception>
        private List<Sector> GetSectors(long companyId)
        {
            const string sql = "SELECT SectorId FROM CompanySector WHERE CompanyId=@companyId";

            var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@companyId", companyId);

            var reader = command.ExecuteReader();

            List<Sector> res = new List<Sector>();

            while (reader.Read())
                res.Add((Sector)Convert.ToInt32(reader["SectorId"]));

            return res;
        }

        /// <summary>
        /// Ajoute un secteur d'activité à une entreprise
        /// </summary>
        /// <param name="companyId">Id de l'entreprise</param>
        /// <param name="sectorId">Id du secteur d'activité (voir <see cref="Sector"/>)</param>
        /// <exception cref="RssException"></exception>
        /// <returns>
        /// <see langword="true"/> : Si un objet a été inséré <br />
        /// <see langword="false"/> : Si aucun objet a été inséré
        /// </returns>
        private bool AddSector(long companyId, int sectorId)
        {
            const string sql = "INSERT INTO CompanySector(CompanyId, SectorId) VALUES(@companyId, @sectorId)";

            var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@companyId", companyId);
            command.Parameters.AddWithValue("@sectorId", sectorId);

            var res = command.ExecuteNonQuery();

            return res > 0;
        }

        /// <summary>
        /// Supprime un secteur d'activité pour l'entreprise
        /// </summary>
        /// <param name="companyId">Id de l'entreprise</param>
        /// <param name="sectorId">id du secteur d'activité (voir <see cref="Sector"/>)</param>
        /// <exception cref="RssException"></exception>
        /// <returns>
        /// <see langword="true"/> : Si un objet a été supprimé <br />
        /// <see langword="false"/> : Si aucun objet a été supprimé
        /// </returns>
        private bool RemoveSector(long companyId, int sectorId)
        {
            const string sql = "DELETE FROM CompanySector WHERE CompanyId=@companyId AND SectorId=@sectorId";

            var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@companyId", companyId);
            command.Parameters.AddWithValue("@sectorId", sectorId);

            var res = command.ExecuteNonQuery();

            return res > 0;
        }
    }
}
