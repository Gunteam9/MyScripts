using System;
using System.Collections.Generic;
using BankCommon;
using BankCommon.Entity;
using Common.Errors;
using CommonServer;
using MySql.Data.MySqlClient;

namespace BankServer.Crud
{
    public class TransactionCrud : ACrud<Transaction>
    {
        public override Transaction Get(long id)
        {
            try
            {
                connection.Open();

                var command =
                    new MySqlCommand(
                        $"SELECT * FROM {TransactionDatabase.TABLE_NAME} WHERE {TransactionDatabase.ID_FIELD}=@id",
                        connection
                    );
                command.Parameters.AddWithValue("@id", id);

                var reader = command.ExecuteReader();

                if (reader.Read())
                    return new Transaction(
                        Convert.ToInt64(reader[TransactionDatabase.ID_FIELD]),
                        (TransactionType)reader[TransactionDatabase.TYPE_FIELD],
                        Convert.ToInt32(reader[TransactionDatabase.AMOUNT_FIELD]),
                        Convert.ToDateTime(reader[TransactionDatabase.DATE_FIELD]),
                        Convert.ToInt64(reader[TransactionDatabase.SOURCE_FIELD]),
                        Convert.ToInt64(reader[TransactionDatabase.TARGET_FIELD]),
                        (Account)reader[TransactionDatabase.SOURCEACCOUNT_FIELD],
                        (Account)reader[TransactionDatabase.TARGETACCOUNT_FIELD]
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

        /// <summary>
        /// Renvoi une liste de <paramref name="count"/> éléments triés dans l'ordre du plus récent pour le joueur <paramref name="playerId"/>
        /// </summary>
        /// <param name="playerId">ID du joueur</param>
        /// <param name="count">Nombre d'éléments à renvoyer</param>
        /// <returns></returns>
        public List<Transaction> GetElements(long playerId, int count)
        {
            var res = new List<Transaction>();

            try
            {
                connection.Open();

                var command =
                    new MySqlCommand(
                        $"SELECT * FROM {TransactionDatabase.TABLE_NAME} WHERE {TransactionDatabase.SOURCE_FIELD}=@playerId OR {TransactionDatabase.TARGET_FIELD}=@playerId ORDER BY {TransactionDatabase.DATE_FIELD} LIMIT @count",
                        connection
                    );
                command.Parameters.AddWithValue("@playerId", playerId);
                command.Parameters.AddWithValue("@count", count);

                var reader = command.ExecuteReader();

                while (reader.Read())
                    res.Add(new Transaction(
                        Convert.ToInt64(reader[TransactionDatabase.ID_FIELD]),
                        (TransactionType)reader[TransactionDatabase.TYPE_FIELD],
                        Convert.ToInt32(reader[TransactionDatabase.AMOUNT_FIELD]),
                        Convert.ToDateTime(reader[TransactionDatabase.DATE_FIELD]),
                        Convert.ToInt64(reader[TransactionDatabase.SOURCE_FIELD]),
                        Convert.ToInt64(reader[TransactionDatabase.TARGET_FIELD]),
                        (Account)reader[TransactionDatabase.SOURCEACCOUNT_FIELD],
                        (Account)reader[TransactionDatabase.TARGETACCOUNT_FIELD]));
            }
            catch (MySqlException)
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
                // connection.SetExtendedResultCodes(true);

                var command =
                    new MySqlCommand(
                        $"INSERT INTO {TransactionDatabase.TABLE_NAME}({TransactionDatabase.TYPE_FIELD}, {TransactionDatabase.AMOUNT_FIELD}, {TransactionDatabase.DATE_FIELD}, {TransactionDatabase.SOURCE_FIELD}, {TransactionDatabase.TARGET_FIELD}, {TransactionDatabase.SOURCEACCOUNT_FIELD}, {TransactionDatabase.TARGETACCOUNT_FIELD}) VALUES (@type, @amount, @date, @source, @target, @sourceAccount, @targetAccount)",
                        connection
                    );
                command.Parameters.AddWithValue("@type", objectToSave.Type);
                command.Parameters.AddWithValue("@amount", objectToSave.Amount);
                command.Parameters.AddWithValue("@date", objectToSave.Date);
                command.Parameters.AddWithValue("@source", objectToSave.Source);
                command.Parameters.AddWithValue("@target", objectToSave.Target);
                command.Parameters.AddWithValue("@sourceAccount", objectToSave.SourceAccount);
                command.Parameters.AddWithValue("@targetAccount", objectToSave.TargetAccount);

                if (command.ExecuteNonQuery() <= 0) return null;

                objectToSave.Id = command.LastInsertedId;
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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool Update(Transaction objectToSave)
        {
            try
            {
                connection.Open();

                var updateCommand =
                    new MySqlCommand(
                        $"UPDATE {TransactionDatabase.TABLE_NAME} SET {TransactionDatabase.TYPE_FIELD}=@type, {TransactionDatabase.AMOUNT_FIELD}=@amount, {TransactionDatabase.DATE_FIELD}=@date, {TransactionDatabase.SOURCE_FIELD}=@source, {TransactionDatabase.TARGET_FIELD}=@target, {TransactionDatabase.SOURCEACCOUNT_FIELD}=@sourceAccount, {TransactionDatabase.TARGETACCOUNT_FIELD}=@targetAccount WHERE {TransactionDatabase.ID_FIELD}=@id",
                        connection
                    );
                updateCommand.Parameters.AddWithValue("@type", objectToSave.Type);
                updateCommand.Parameters.AddWithValue("@amount", objectToSave.Amount);
                updateCommand.Parameters.AddWithValue("@date", objectToSave.Date);
                updateCommand.Parameters.AddWithValue("@source", objectToSave.Source);
                updateCommand.Parameters.AddWithValue("@target", objectToSave.Target);
                updateCommand.Parameters.AddWithValue("@sourceAccount", objectToSave.SourceAccount);
                updateCommand.Parameters.AddWithValue("@targetAccount", objectToSave.TargetAccount);
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
                        $"DELETE FROM {TransactionDatabase.TABLE_NAME} WHERE {TransactionDatabase.ID_FIELD}=@id",
                        connection
                    );
                command.Parameters.AddWithValue("@id", id);

                return command.ExecuteNonQuery() > 0;
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
    }
}