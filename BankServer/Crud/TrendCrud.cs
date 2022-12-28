using System;
using BankCommon;
using BankCommon.Entity;
using Common.Errors;
using CommonServer;
using MySql.Data.MySqlClient;

namespace BankServer.Crud
{
    public class TrendCrud : ACrud<Trend>
    {
        public override Trend Get(long id)
        {
            try
            {
                connection.Open();

                var command =
                    new MySqlCommand(
                        $"SELECT * FROM {TrendDatabase.TABLE_NAME} WHERE {TrendDatabase.ID_FIELD}=@id",
                        connection
                    );
                command.Parameters.AddWithValue("@id", id);

                var reader = command.ExecuteReader();
                if (reader.Read())
                    return new Trend(
                        Convert.ToInt64(reader[TrendDatabase.ID_FIELD]),
                        Convert.ToInt32(reader[TrendDatabase.DURATION_FIELD]),
                        Convert.ToInt32(reader[TrendDatabase.IMPORTANCE_FIELD]),
                        (Sector)reader[TrendDatabase.SECTOR_FIELD]
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

        public override Trend Insert(Trend objectToSave)
        {
            try
            {
                connection.Open();
                // connection.SetExtendedResultCodes(true);

                var command =
                    new MySqlCommand(
                        $"INSERT INTO {TrendDatabase.TABLE_NAME}({TrendDatabase.DURATION_FIELD}, {TrendDatabase.IMPORTANCE_FIELD}, {TrendDatabase.SECTOR_FIELD}) VALUES (@duration, @importance, @sector)",
                        connection
                    );
                command.Parameters.AddWithValue("@duration", objectToSave.Duration);
                command.Parameters.AddWithValue("@importance", objectToSave.Importance);
                command.Parameters.AddWithValue("@sector", objectToSave.Sector);


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
        public override bool Update(Trend objectToSave)
        {
            try
            {
                connection.Open();

                var updateCommand =
                    new MySqlCommand(
                        $"UPDATE {TrendDatabase.TABLE_NAME} SET {TrendDatabase.DURATION_FIELD}=@duration, {TrendDatabase.IMPORTANCE_FIELD}=@importance, {TrendDatabase.SECTOR_FIELD}=@sector WHERE {TrendDatabase.ID_FIELD}=@id",
                        connection
                    );
                updateCommand.Parameters.AddWithValue("@duration", objectToSave.Duration);
                updateCommand.Parameters.AddWithValue("@importance", objectToSave.Importance);
                updateCommand.Parameters.AddWithValue("@sector", objectToSave.Sector);
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
                        $"DELETE FROM {TrendDatabase.TABLE_NAME} WHERE {TrendDatabase.ID_FIELD}=@id",
                        connection
                    );
                command.Parameters.AddWithValue("@id", id);

                return (command.ExecuteNonQuery() > 0);
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