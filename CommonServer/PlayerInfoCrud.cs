using System;
using BankServer;
using Common.Entity;
using Common.Errors;
using MySql.Data.MySqlClient;

namespace CommonServer
{
    public class PlayerInfoCrud : ACrud<PlayerInfo>
    {
        public override PlayerInfo Get(long id)
        {
            try
            {
                connection.Open();

                var command =
                    new MySqlCommand(
                        $"SELECT * FROM {PlayerInfoDatabase.TABLE_NAME} WHERE {PlayerInfoDatabase.ID_FIELD}=@id",
                        connection
                    );
                command.Parameters.AddWithValue("@id", id);

                var reader = command.ExecuteReader();

                if (reader.Read())
                    return new PlayerInfo(
                        Convert.ToInt64(reader[PlayerInfoDatabase.ID_FIELD]),
                        Convert.ToString(reader[PlayerInfoDatabase.STEAMID_FIELD])
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

        public PlayerInfo Get(string steamid)
        {
            try
            {
                connection.Open();

                var command =
                    new MySqlCommand(
                        $"SELECT * FROM {PlayerInfoDatabase.TABLE_NAME} WHERE {PlayerInfoDatabase.STEAMID_FIELD}=@steamid",
                        connection
                    );
                command.Parameters.AddWithValue("@steamid", steamid);

                var reader = command.ExecuteReader();

                if (reader.Read())
                    return new PlayerInfo(
                        Convert.ToInt64(reader[PlayerInfoDatabase.ID_FIELD]),
                        Convert.ToString(reader[PlayerInfoDatabase.STEAMID_FIELD])
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

        public override PlayerInfo Insert(PlayerInfo objectToSave)
        {
            try
            {
                connection.Open();
                // connection.SetExtendedResultCodes(true);

                var command =
                    new MySqlCommand(
                        $"INSERT INTO {PlayerInfoDatabase.TABLE_NAME}({PlayerInfoDatabase.STEAMID_FIELD}) VALUES (@steamid)",
                        connection
                    );
                command.Parameters.AddWithValue("@steamid", objectToSave.SteamId);

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
        public override bool Update(PlayerInfo objectToSave)
        {
            try
            {
                connection.Open();

                // Puis on met à jour le reste des informations
                var updateCommand =
                    new MySqlCommand(
                        $"UPDATE {PlayerInfoDatabase.TABLE_NAME} SET {PlayerInfoDatabase.STEAMID_FIELD}=@steamid WHERE {PlayerInfoDatabase.ID_FIELD}=@id",
                        connection
                    );
                updateCommand.Parameters.AddWithValue("@steamid", objectToSave.SteamId);
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
                        $"DELETE FROM {PlayerInfoDatabase.TABLE_NAME} WHERE {PlayerInfoDatabase.ID_FIELD}=@id",
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