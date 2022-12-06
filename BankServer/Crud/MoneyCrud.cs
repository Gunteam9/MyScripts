using BankCommon.Entity;
using CitizenFX.Core;
using Common;
using Common.Entity;
using Common.Errors;
using CommonServer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankServer.Crud
{
    public class MoneyCrud : ACrud<PlayerMoney>
    {
        public override PlayerMoney Get(long id)
        {
            PlayerMoney res = null;

            try
            {
                connection.Open();

                List<Stock> stocks = this.GetStock(id);

                const string sql = "SELECT * FROM PlayerMoney WHERE id=@id";

                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                var reader = command.ExecuteReader();
                if (reader.Read())
                    res = new PlayerMoney(
                        Convert.ToInt64(reader["Id"]), 
                        Convert.ToInt32(reader["Cash"]), 
                        Convert.ToInt32(reader["CurrentAccount"]), 
                        Convert.ToInt32(reader["Savings"]), 
                        stocks, 
                        Convert.ToInt32(reader["LifeInsurance"]));
            }
            catch (SQLiteException)
            {
                return null;
            }
            finally
            {
                connection.Close();
            }

            return res;
        }

        public override PlayerMoney Insert(PlayerMoney objectToSave)
        {
            try
            {
                connection.Open();
                connection.SetExtendedResultCodes(true);

                const string sql = "INSERT INTO PlayerMoney(Cash, CurrentAccount, Savings, LifeInsurance) VALUES (@cash, @currentAccount, @savings, @lifeInsurance)";

                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@cash", objectToSave.Cash);
                command.Parameters.AddWithValue("@currentAccount", objectToSave.CurrentAccount);
                command.Parameters.AddWithValue("@savings", objectToSave.Savings);
                command.Parameters.AddWithValue("@lifeInsurance", objectToSave.LifeInsurance);

                var res = command.ExecuteNonQuery();

                if (res > 0)
                {
                    long companyId = connection.LastInsertRowId;
                    objectToSave.Id = companyId;

                    for (int i = 0; i < objectToSave.Stock.Count; i++)
                    {
                        long stockId = this.AddStock(objectToSave.Stock[i]);
                        objectToSave.Stock[i].Id = stockId;
                    }

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

        public override bool Update(PlayerMoney objectToSave)
        {
            try
            {
                connection.Open();

                // On récupère d'abord les stocks pour les mettre à jour
                const string selectSql = "SELECT * FROM Stock WHERE PlayerId=@id";

                var selectCommand = new SQLiteCommand(selectSql, connection);
                selectCommand.Parameters.AddWithValue("@id", objectToSave.Id);

                var selectReader = selectCommand.ExecuteReader();

                List<Stock> savedStocks = new List<Stock>();

                while (selectReader.Read())
                    savedStocks.Add(new Stock(
                        Convert.ToInt64(selectReader["Id"]),
                        Convert.ToInt64(selectReader["PlayerId"]), 
                        Convert.ToInt64(selectReader["CompanyId"]),
                        Convert.ToInt32(selectReader["Amount"])));

                // Ensuite on en ajoute ou en supprime en fonction de la nouvauté
                // Si l'objet n'existe pas en base on j'aojoute
                foreach (var stockToSave in objectToSave.Stock)
                {
                    if (!savedStocks.Select(o => o.Id).Contains(stockToSave.Id))
                    {
                        if (this.AddStock(stockToSave) == -1)
                            return false;
                    }
                }

                // Si l'objet existe en base mais qu'il doit être supprimé
                foreach (var savedStock in savedStocks)
                {
                    if (!objectToSave.Stock.Select(o => o.Id).Contains(savedStock.Id))
                    {
                        if (this.RemoveStock(savedStock.Id))
                            return false;
                    }
                }

                // Puis on met à jour le reste des informations
                const string updateSql = "UPDATE PlayerMoney SET Cash=@cash, CurrentAccount=@currentAccount, Savings=@savings, LifeInsurance=@lifeInsurance WHERE Id=@id";

                Debug.WriteLine(JsonConvert.SerializeObject(objectToSave));

                var updateCommand = new SQLiteCommand(updateSql, connection);
                updateCommand.Parameters.AddWithValue("@cash", objectToSave.Cash);
                updateCommand.Parameters.AddWithValue("@currentAccount", objectToSave.CurrentAccount);
                updateCommand.Parameters.AddWithValue("@savings", objectToSave.Savings);
                updateCommand.Parameters.AddWithValue("@lifeInsurance", objectToSave.LifeInsurance);
                updateCommand.Parameters.AddWithValue("@id", objectToSave.Id);

                var res = updateCommand.ExecuteNonQuery();

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

                const string sql = "DELETE FROM PlayerMoney WHERE id=@id";

                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                int res = command.ExecuteNonQuery();

                if (res > 0)
                {
                    List<Stock> stockToDel = (this.Get(id)).Stock;

                    foreach (var item in stockToDel)
                    {
                        if (!this.RemoveStock(id))
                            return false;
                    }

                    return true;
                }
                else
                    return false;
            }
            catch (SQLiteException)
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
        /// <param name="companyId">Id de l'entreprise</param>
        /// <exception cref="RssException"></exception>
        private List<Stock> GetStock(long playerId)
        {
            const string sql = "SELECT * FROM Stock WHERE PlayerId=@playerId";

            var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@playerId", playerId);

            var reader = command.ExecuteReader();

            List<Stock> res = new List<Stock>();

            while (reader.Read())
                res.Add(new Stock(
                    Convert.ToInt64(reader["Id"]),
                    Convert.ToInt64(reader["PlayerId"]),
                    Convert.ToInt64(reader["CompanyId"]),
                    Convert.ToInt32(reader["Amount"])));

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
            const string sql = "INSERT INTO Stock(PlayerId, CompanyId, Amount) VALUES(@playerId, @companyId, @amount)";

            var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@playerId", stock.PlayerId);
            command.Parameters.AddWithValue("@companyId", stock.CompanyId);
            command.Parameters.AddWithValue("@amount", stock.Amount);

            var res = command.ExecuteNonQuery();

            if (res > 0)
                return connection.LastInsertRowId;
            else
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
            const string sql = "DELETE FROM Stock WHERE Id=@stockId";

            var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@stockId", stockId);

            var res = command.ExecuteNonQuery();

            return res > 0;
        }
    }
}
