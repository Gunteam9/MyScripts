using Common;
using Common.Entity;
using Common.Errors;
using CommonServer;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonServer
{
    public class PlayerInfoCrud : ACrud<PlayerInfo>
    {


        //public override PlayerInfo Get(long id)
        //{
        //    PlayerInfo res = null;

        //    try
        //    {
        //        connection.Open();

        //        const string sql = "SELECT * FROM PlayerInfo WHERE id=@id";

        //        var command = new MySqlCommand(sql, connection);
        //        command.Parameters.AddWithValue("@id", id);

        //        var reader = command.ExecuteReader();
        //        if (reader.Read())
        //            res = new PlayerInfo(
        //                Convert.ToInt64(reader["Id"]),
        //                Convert.ToString(reader["PlayerLicense"]));
        //    }
        //    catch (MySqlException)
        //    {
        //        return null;
        //    }
        //    finally
        //    {
        //        connection.Close();
        //    }

        //    return res;
        //}

        //public PlayerInfo Get(string license)
        //{
        //    PlayerInfo res = null;

        //    try
        //    {
        //        connection.Open();

        //        const string sql = "SELECT * FROM PlayerInfo WHERE PlayerLicense=@license";

        //        var command = new MySqlCommand(sql, connection);
        //        command.Parameters.AddWithValue("license", license);

        //        var reader = command.ExecuteReader();
        //        if (reader.Read())
        //            res = new PlayerInfo(
        //                Convert.ToInt64(reader["Id"]),
        //                Convert.ToString(reader["PlayerLicense"]));
        //    }
        //    catch (MySqlException)
        //    {
        //        return null;
        //    }
        //    finally
        //    {
        //        connection.Close();
        //    }

        //    return res;
        //}

        //public override PlayerInfo Insert(PlayerInfo objectToSave)
        //{
        //    try
        //    {
        //        connection.Open();
        //        connection.SetExtendedResultCodes(true);

        //        const string sql = "INSERT INTO PlayerInfo(PlayerLicense) VALUES (@playerLicense)";

        //        var command = new SQLiteCommand(sql, connection);
        //        command.Parameters.AddWithValue("@playerLicense", objectToSave.PlayerLicense);

        //        var res = command.ExecuteNonQuery();

        //        if (res > 0)
        //        {
        //            long companyId = connection.LastInsertRowId;
        //            objectToSave.Id = companyId;

        //            return objectToSave;
        //        }

        //        return null;
        //    }
        //    catch (SQLiteException e)
        //    {
        //        if (e.ResultCode == SQLiteErrorCode.Constraint_Unique)
        //            return null;

        //        throw new RssException(ErrorCodes.SQL_ERROR, e);
        //    }
        //    finally
        //    {
        //        connection.Close();
        //    }
        //}

        ///// <summary>
        ///// <inheritdoc/>
        ///// </summary>
        //public override bool Update(PlayerInfo objectToSave)
        //{
        //    try
        //    {
        //        connection.Open();

        //        // Puis on met à jour le reste des informations
        //        const string updateSql = "UPDATE PlayerInfo SET PlayerLicense=@playerLicense WHERE Id=@id";

        //        var updateCommand = new SQLiteCommand(updateSql, connection);
        //        updateCommand.Parameters.AddWithValue("@playerLicense", objectToSave.PlayerLicense);
        //        updateCommand.Parameters.AddWithValue("@id", objectToSave.Id);

        //        var res = updateCommand.ExecuteNonQuery();

        //        return res > 0;
        //    }
        //    catch (SQLiteException e)
        //    {
        //        throw new RssException(ErrorCodes.SQL_ERROR, e);
        //    }
        //    finally
        //    {
        //        connection.Close();
        //    }
        //}

        //public override bool Remove(long id)
        //{
        //    try
        //    {
        //        connection.Open();

        //        const string sql = "DELETE FROM PlayerInfo WHERE id=@id";

        //        var command = new SQLiteCommand(sql, connection);
        //        command.Parameters.AddWithValue("@id", id);

        //        int res = command.ExecuteNonQuery();

        //        return (res > 0);
        //    }
        //    catch (SQLiteException)
        //    {
        //        return false;
        //    }
        //    finally
        //    {
        //        connection.Close();
        //    }
        //}
        public override PlayerInfo Get(long id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Renvoi le PlayerInfo correspond au steamId passé en paramètre. Le SteamId est un ID unique
        /// </summary>
        public PlayerInfo Get(string steamId)
        {
            throw new NotImplementedException();
        }

        public override PlayerInfo Insert(PlayerInfo objectToSave)
        {
            throw new NotImplementedException();
        }

        public override bool Remove(long id)
        {
            throw new NotImplementedException();
        }

        public override bool Update(PlayerInfo objectToSave)
        {
            throw new NotImplementedException();
        }
    }
}
