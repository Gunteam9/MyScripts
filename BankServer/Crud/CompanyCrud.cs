using System;
using System.Collections.Generic;
using System.Linq;
using BankCommon;
using BankCommon.Entity;
using Common.Errors;
using CommonServer;
using MySql.Data.MySqlClient;

namespace BankServer.Crud
{
    public class CompanyCrud : ACrud<Company>
    {
        public override Company Get(long id)
        {
            try
            {
                connection.Open();

                var command =
                    new MySqlCommand(
                        $"SELECT * FROM {CompanyDatabase.TABLE_NAME} WHERE {CompanyDatabase.ID_FIELD}=@id",
                        connection
                    );
                command.Parameters.AddWithValue("@id", id);
                var reader = command.ExecuteReader();

                if (!reader.Read()) return null;

                var companyId = Convert.ToInt64(reader[CompanyDatabase.ID_FIELD]);
                return new Company(
                    companyId,
                    Convert.ToString(reader[CompanyDatabase.NAME_FIELD]),
                    Convert.ToString(reader[CompanyDatabase.ACRONYM_FIELD]),
                    GetSectors(companyId),
                    Convert.ToInt32(reader[CompanyDatabase.PRICEATOPENING_FIELD]),
                    Convert.ToInt32(reader[CompanyDatabase.CURRENTPRICE_FIELD])
                );
            }
            catch (MySqlException e)
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

                var command =
                    new MySqlCommand(
                        $"SELECT * FROM {CompanyDatabase.TABLE_NAME}",
                        connection
                    );
                var reader = command.ExecuteReader();

                var companies = new List<Company>();

                while (reader.Read())
                {
                    var companyId = Convert.ToInt64(reader[CompanyDatabase.ID_FIELD]);
                    companies.Add(
                        new Company(
                            companyId,
                            Convert.ToString(reader[CompanyDatabase.NAME_FIELD]),
                            Convert.ToString(reader[CompanyDatabase.ACRONYM_FIELD]),
                            GetSectors(companyId),
                            Convert.ToInt32(reader[CompanyDatabase.PRICEATOPENING_FIELD]),
                            Convert.ToInt32(reader[CompanyDatabase.CURRENTPRICE_FIELD])
                        )
                    );
                }

                return companies;
            }
            catch (MySqlException e)
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
                // connection.SetExtendedResultCodes(true);

                var command =
                    new MySqlCommand(
                        $"INSERT INTO {CompanyDatabase.TABLE_NAME}({CompanyDatabase.NAME_FIELD}, {CompanyDatabase.ACRONYM_FIELD}, {CompanyDatabase.PRICEATOPENING_FIELD}, {CompanyDatabase.CURRENTPRICE_FIELD}) VALUES (@name, @acronym, @priceAtOpening, @currentPrice)",
                        connection
                    );
                command.Parameters.AddWithValue("@name", objectToSave.Name);
                command.Parameters.AddWithValue("@acronym", objectToSave.Acronym);
                command.Parameters.AddWithValue("@priceAtOpening", objectToSave.PriceAtOpening);
                command.Parameters.AddWithValue("@currentPrice", objectToSave.CurrentPrice);

                if (command.ExecuteNonQuery() <= 0) return null;

                var companyId = command.LastInsertedId;
                if (objectToSave.Sectors.Any(sector => !AddSector(companyId, (int)sector))) return null;

                objectToSave.Id = companyId;
                return objectToSave;
            }
            catch (MySqlException e)
            {
                // TODO: Check
                if (e.ErrorCode == (int)MySqlErrorCode.DuplicateUnique) return null;

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
                var sectorCommand =
                    new MySqlCommand(
                        $"SELECT * FROM {CompanySectorDatabase.TABLE_NAME} WHERE {CompanySectorDatabase.COMPANYID_FIELD}=@id",
                        connection
                    );
                sectorCommand.Parameters.AddWithValue("@id", objectToSave.Id);
                var sectorReader = sectorCommand.ExecuteReader();

                var savedSectors = new List<Sector>();

                while (sectorReader.Read())
                    savedSectors.Add((Sector)sectorReader[CompanySectorDatabase.SECTORID_FIELD]);

                // Ensuite on en ajoute ou en supprime en fonction de la nouvauté
                var differentSectors = savedSectors.Except(objectToSave.Sectors);
                foreach (var sector in differentSectors)
                {
                    if (objectToSave.Sectors.Contains(sector) && !AddSector(objectToSave.Id, (int)sector)) return false;
                    else if (!RemoveSector(objectToSave.Id, (int)sector)) return false;
                }

                // Puis on met à jour le reste des informations
                var command =
                    new MySqlCommand(
                        $"UPDATE {CompanyDatabase.TABLE_NAME} SET {CompanyDatabase.NAME_FIELD}=@name, {CompanyDatabase.ACRONYM_FIELD}=@acronym, {CompanyDatabase.PRICEATOPENING_FIELD}=@priceAtOpening, {CompanyDatabase.CURRENTPRICE_FIELD}=@currentPrice WHERE {CompanyDatabase.ID_FIELD}=@id",
                        connection
                    );
                command.Parameters.AddWithValue("@name", objectToSave.Name);
                command.Parameters.AddWithValue("@acronym", objectToSave.Acronym);
                command.Parameters.AddWithValue("@priceAtOpening", objectToSave.PriceAtOpening);
                command.Parameters.AddWithValue("@currentPrice", objectToSave.CurrentPrice);
                command.Parameters.AddWithValue("@id", objectToSave.Id);

                return command.ExecuteNonQuery() > 0;
            }
            catch (MySqlException e)
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

                var command =
                    new MySqlCommand(
                        $"DELETE FROM {CompanyDatabase.TABLE_NAME} WHERE {CompanyDatabase.ID_FIELD}=@id",
                        connection
                    );
                command.Parameters.AddWithValue("@id", id);

                return command.ExecuteNonQuery() > 0 && Get(id).Sectors.All(item => RemoveSector(id, (int)item));
            }
            catch (MySqlException e)
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
            var command =
                new MySqlCommand(
                    $"SELECT {CompanySectorDatabase.SECTORID_FIELD} FROM {CompanySectorDatabase.TABLE_NAME} WHERE {CompanySectorDatabase.COMPANYID_FIELD}=@companyId",
                    connection
                );
            command.Parameters.AddWithValue("@companyId", companyId);
            var reader = command.ExecuteReader();

            var res = new List<Sector>();

            while (reader.Read())
                res.Add((Sector)Convert.ToInt32(reader[CompanySectorDatabase.SECTORID_FIELD]));

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
            var command =
                new MySqlCommand(
                    $"INSERT INTO {CompanySectorDatabase.TABLE_NAME}({CompanySectorDatabase.COMPANYID_FIELD}, {CompanySectorDatabase.SECTORID_FIELD}) VALUES(@companyId, @sectorId)",
                    connection
                );
            command.Parameters.AddWithValue("@companyId", companyId);
            command.Parameters.AddWithValue("@sectorId", sectorId);

            return command.ExecuteNonQuery() > 0;
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
            var command =
                new MySqlCommand(
                    $"DELETE FROM {CompanySectorDatabase.TABLE_NAME} WHERE {CompanySectorDatabase.COMPANYID_FIELD}=@companyId AND {CompanySectorDatabase.SECTORID_FIELD}=@sectorId",
                    connection
                );
            command.Parameters.AddWithValue("@companyId", companyId);
            command.Parameters.AddWithValue("@sectorId", sectorId);

            return command.ExecuteNonQuery() > 0;
        }
    }
}