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
    public class MoneyCrud : ACrud<PlayerMoney>
    {
        public override PlayerMoney Get(long id)
        {
            try
            {
                connection.Open();

                var stocks = GetStock(id);

                var command =
                    new MySqlCommand(
                        $"SELECT * FROM {PlayerMoneyDatabase.TABLE_NAME} WHERE {PlayerMoneyDatabase.ID_FIELD}=@id",
                        connection
                    );
                command.Parameters.AddWithValue("@id", id);

                var reader = command.ExecuteReader();
                if (reader.Read())
                    return new PlayerMoney(
                        Convert.ToInt64(reader[PlayerMoneyDatabase.ID_FIELD]),
                        Convert.ToInt32(reader[PlayerMoneyDatabase.CASH_FIELD]),
                        Convert.ToInt32(reader[PlayerMoneyDatabase.CURRENTACCOUNT_FIELD]),
                        Convert.ToInt32(reader[PlayerMoneyDatabase.SAVING_FIELD]),
                        stocks,
                        Convert.ToInt32(reader[PlayerMoneyDatabase.LIFEINSURANCE_FIELD])
                    );

                return null;
            }
            catch (MySqlException)
            {
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

        public override PlayerMoney Insert(PlayerMoney objectToSave)
        {
            try
            {
                connection.Open();
                // connection.SetExtendedResultCodes(true);

                var command =
                    new MySqlCommand(
                        $"INSERT INTO {PlayerMoneyDatabase.TABLE_NAME}({PlayerMoneyDatabase.CASH_FIELD}, {PlayerMoneyDatabase.CURRENTACCOUNT_FIELD}, {PlayerMoneyDatabase.SAVING_FIELD}, {PlayerMoneyDatabase.LIFEINSURANCE_FIELD}) VALUES (@cash, @currentAccount, @savings, @lifeInsurance)",
                        connection
                    );
                command.Parameters.AddWithValue("@cash", objectToSave.Cash);
                command.Parameters.AddWithValue("@currentAccount", objectToSave.CurrentAccount);
                command.Parameters.AddWithValue("@savings", objectToSave.Savings);
                command.Parameters.AddWithValue("@lifeInsurance", objectToSave.LifeInsurance);

                if (command.ExecuteNonQuery() <= 0) return null;

                objectToSave.Id = command.LastInsertedId;

                foreach (var t in objectToSave.Stock) t.Id = AddStock(t);

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

        public override bool Update(PlayerMoney objectToSave)
        {
            try
            {
                connection.Open();

                // On récupère d'abord les stocks pour les mettre à jour
                var selectCommand =
                    new MySqlCommand(
                        $"SELECT * FROM {StockDatabase.TABLE_NAME} WHERE {StockDatabase.PLAYERID_FIELD}=@id",
                        connection
                    );
                selectCommand.Parameters.AddWithValue("@id", objectToSave.Id);
                var selectReader = selectCommand.ExecuteReader();

                var savedStocks = new List<Stock>();

                while (selectReader.Read())
                    savedStocks.Add(new
                        Stock(
                            Convert.ToInt64(selectReader[StockDatabase.ID_FIELD]),
                            Convert.ToInt64(selectReader[StockDatabase.PLAYERID_FIELD]),
                            Convert.ToInt64(selectReader[StockDatabase.COMPANYID_FIELD]),
                            (StockType) Convert.ToInt16(selectReader[StockDatabase.TYPE_FIELD]),
                            Convert.ToInt32(selectReader[StockDatabase.AMOUNT_FIELD]),
                            Convert.ToInt32(selectReader[StockDatabase.SIMULATEDAMOUNT_FIELD]),
                            Convert.ToSingle(selectReader[StockDatabase.AVERAGEBYVALUE_FIELD])
                        )
                    );

                // Ensuite on en ajoute ou en supprime en fonction de la nouvauté
                // Si l'objet n'existe pas en base on j'aojoute
                if (objectToSave.Stock.Where(stockToSave => !savedStocks.Select(o => o.Id).Contains(stockToSave.Id))
                    .Any(stockToSave => AddStock(stockToSave) == -1)) return false;

                // Si l'objet existe en base mais qu'il doit être supprimé
                if (savedStocks.Where(savedStock => !objectToSave.Stock.Select(o => o.Id).Contains(savedStock.Id))
                    .Any(savedStock => RemoveStock(savedStock.Id))) return false;

                // Puis on met à jour le reste des informations
                var updateCommand =
                    new MySqlCommand(
                        $"UPDATE {PlayerMoneyDatabase.TABLE_NAME} SET {PlayerMoneyDatabase.CASH_FIELD}=@cash, {PlayerMoneyDatabase.CURRENTACCOUNT_FIELD}=@currentAccount, {PlayerMoneyDatabase.SAVING_FIELD}=@savings, {PlayerMoneyDatabase.LIFEINSURANCE_FIELD}=@lifeInsurance WHERE {PlayerMoneyDatabase.ID_FIELD}=@id",
                        connection
                    );
                updateCommand.Parameters.AddWithValue("@cash", objectToSave.Cash);
                updateCommand.Parameters.AddWithValue("@currentAccount", objectToSave.CurrentAccount);
                updateCommand.Parameters.AddWithValue("@savings", objectToSave.Savings);
                updateCommand.Parameters.AddWithValue("@lifeInsurance", objectToSave.LifeInsurance);
                updateCommand.Parameters.AddWithValue("@id", objectToSave.Id);

                return updateCommand.ExecuteNonQuery() > 0;
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
                        $"DELETE FROM {PlayerMoneyDatabase.TABLE_NAME} WHERE {PlayerMoneyDatabase.ID_FIELD}=@id",
                        connection
                    );
                command.Parameters.AddWithValue("@id", id);

                return command.ExecuteNonQuery() > 0 && Get(id).Stock.All(item => RemoveStock(item.Id));
            }
            catch (MySqlException)
            {
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Récupère les actions du joueur
        /// </summary>
        /// <param name="playerId">Id du joueur</param>
        /// <exception cref="RssException"></exception>
        private List<Stock> GetStock(long playerId)
        {
            var command =
                new MySqlCommand(
                    $"SELECT * FROM {StockDatabase.TABLE_NAME} WHERE {StockDatabase.PLAYERID_FIELD}=@playerId",
                    connection
                );
            command.Parameters.AddWithValue("@playerId", playerId);
            var reader = command.ExecuteReader();

            var res = new List<Stock>();

            while (reader.Read())
                res.Add(
                    new Stock(
                        Convert.ToInt64(reader[StockDatabase.ID_FIELD]),
                        Convert.ToInt64(reader[StockDatabase.PLAYERID_FIELD]),
                        Convert.ToInt64(reader[StockDatabase.COMPANYID_FIELD]),
                        (StockType) Convert.ToInt16(reader[StockDatabase.TYPE_FIELD]),
                        Convert.ToInt32(reader[StockDatabase.AMOUNT_FIELD]),
                        Convert.ToInt32(reader[StockDatabase.SIMULATEDAMOUNT_FIELD]),
                        Convert.ToSingle(reader[StockDatabase.AVERAGEBYVALUE_FIELD])
                    )
                );

            return res;
        }

        /// <summary>
        /// Sauvegarde l'achat d'action en base
        /// </summary>
        /// <param name="stock">Action. L'ID n'est pas pris en compte</param>
        /// <exception cref="RssException"></exception>
        /// <returns>
        /// OK : l'ID de l'objet inséré <br />
        /// KO : -1
        /// </returns>
        private long AddStock(Stock stock)
        {
            var command =
                new MySqlCommand(
                    $"INSERT INTO {StockDatabase.TABLE_NAME}({StockDatabase.PLAYERID_FIELD}, {StockDatabase.COMPANYID_FIELD}, {StockDatabase.AMOUNT_FIELD}) VALUES(@playerId, @companyId, @amount)",
                    connection
                );
            command.Parameters.AddWithValue("@playerId", stock.PlayerId);
            command.Parameters.AddWithValue("@companyId", stock.CompanyId);
            command.Parameters.AddWithValue("@amount", stock.Amount);

            if (command.ExecuteNonQuery() > 0) return command.LastInsertedId;
            return -1;
        }

        /// <summary>
        /// Supprime l'action possédé par le joueur
        /// </summary>
        /// <param name="stockId">Id le l'action a supprimer</param>
        /// <exception cref="RssException"></exception>
        /// <returns>
        /// <see langword="true"/> : Si un objet a été supprimé <br />
        /// <see langword="false"/> : Si aucun objet a été supprimé
        /// </returns>
        private bool RemoveStock(long stockId)
        {
            var command = new MySqlCommand($"DELETE FROM {StockDatabase.TABLE_NAME} WHERE {StockDatabase.ID_FIELD}=@stockId", connection);
            command.Parameters.AddWithValue("@stockId", stockId);

            return command.ExecuteNonQuery() > 0;
        }
    }
}