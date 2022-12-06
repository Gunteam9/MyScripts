using BankCommon;
using BankCommon.Entity;
using Common;
using Common.Entity;
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
    public class TransactionCrud : ACrud<Transaction>
    {
        public override Transaction Get(long id)
        {
            Transaction res = null;

            try
            {
                connection.Open();

                const string sql = "SELECT * FROM Transaction WHERE id=@id";

                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                var reader = command.ExecuteReader();
                if (reader.Read())
                    res = new Transaction(
                        Convert.ToInt64(reader["Id"]), 
                        (TransactionType)reader["Type"],
                        Convert.ToInt32(reader["Amount"]),
                        Convert.ToDateTime(reader["Date"]), 
                        Convert.ToInt64(reader["Source"]),
                        Convert.ToInt64(reader["Target"]),
                        (Account)reader["SourceAccount"],
                        (Account)reader["TargetAccount"]);
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
        
        /// <summary>
        /// Renvoi une liste de <paramref name="count"/> éléments triés dans l'ordre du plus récent pour le joueur <paramref name="playerId"/>
        /// </summary>
        /// <param name="playerId">ID du joueur</param>
        /// <param name="count">Nombre d'éléments à renvoyer</param>
        /// <returns></returns>
        public List<Transaction> GetElements(long playerId, int count)
        {
            List<Transaction> res = new List<Transaction>();

            try
            {
                connection.Open();

                const string sql = "SELECT * FROM Transaction WHERE Source=@playerId OR Target=@playerId ORDER BY Date LIMIT @count";

                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@playerId", playerId);
                command.Parameters.AddWithValue("@count", count);

                var reader = command.ExecuteReader();

                while(reader.Read())
                    res.Add(new Transaction(
                        Convert.ToInt64(reader["Id"]), 
                        (TransactionType)reader["Type"], 
                        Convert.ToInt32(reader["Amount"]),
                        Convert.ToDateTime(reader["Date"]),
                        Convert.ToInt64(reader["Source"]),
                        Convert.ToInt64(reader["Target"]),
                        (Account)reader["SourceAccount"], 
                        (Account)reader["TargetAccount"]));
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

        public override Transaction Insert(Transaction objectToSave)
        {
            try
            {
                connection.Open();
                connection.SetExtendedResultCodes(true);

                const string sql = "INSERT INTO Transaction(Type, Amount, Date, Source, Target, SourceAccount, TargetAccount) VALUES (@type, @amount, @date, @source, @target, @sourceAccount, @targetAccount)";

                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@type", objectToSave.Type);
                command.Parameters.AddWithValue("@amount", objectToSave.Amount);
                command.Parameters.AddWithValue("@date", objectToSave.Date);
                command.Parameters.AddWithValue("@source", objectToSave.Source);
                command.Parameters.AddWithValue("@target", objectToSave.Target);
                command.Parameters.AddWithValue("@sourceAccount", objectToSave.SourceAccount);
                command.Parameters.AddWithValue("@targetAccount", objectToSave.TargetAccount);

                var res = command.ExecuteNonQuery();

                if (res > 0)
                {
                    long transactionId = connection.LastInsertRowId;
                    objectToSave.Id = transactionId;

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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool Update(Transaction objectToSave)
        {
            try
            {
                connection.Open();

                const string updateSql = "UPDATE Transaction SET Type=@type, Amount=@amount, Date=@date, Source=@source, Target=@target, SourceAccount=@sourceAccount, TargetAccount=@targetAccount WHERE Id=@id";

                var updateCommand = new SQLiteCommand(updateSql, connection);
                updateCommand.Parameters.AddWithValue("@type", objectToSave.Type);
                updateCommand.Parameters.AddWithValue("@amount", objectToSave.Amount);
                updateCommand.Parameters.AddWithValue("@date", objectToSave.Date);
                updateCommand.Parameters.AddWithValue("@source", objectToSave.Source);
                updateCommand.Parameters.AddWithValue("@target", objectToSave.Target);
                updateCommand.Parameters.AddWithValue("@sourceAccount", objectToSave.SourceAccount);
                updateCommand.Parameters.AddWithValue("@targetAccount", objectToSave.TargetAccount);
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

                const string sql = "DELETE FROM Transaction WHERE id=@id";

                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                int res = command.ExecuteNonQuery();

                return (res > 0);
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
    }
}
