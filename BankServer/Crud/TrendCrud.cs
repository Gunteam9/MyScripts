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
    public class TrendCrud : ACrud<Trend>
    {
        public override Trend Get(long id)
        {
            Trend res = null;

            try
            {
                connection.Open();

                const string sql = "SELECT * FROM Trend WHERE id=@id";

                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                var reader = command.ExecuteReader();
                if (reader.Read())
                    res = new Trend(
                        Convert.ToInt64(reader["Id"]),
                        Convert.ToInt32(reader["Duration"]),
                        Convert.ToInt32(reader["Importance"]),
                        (Sector)reader["Sector"]);
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

        public override Trend Insert(Trend objectToSave)
        {
            try
            {
                connection.Open();
                connection.SetExtendedResultCodes(true);

                const string sql = "INSERT INTO Trend(Duration, Importance, Sector) VALUES (@duration, @importance, @sector)";

                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@duration", objectToSave.Duration);
                command.Parameters.AddWithValue("@importance", objectToSave.Importance);
                command.Parameters.AddWithValue("@sector", objectToSave.Sector);

                var res = command.ExecuteNonQuery();

                if (res > 0)
                {
                    long trendId = connection.LastInsertRowId;
                    objectToSave.Id = trendId;

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
        public override bool Update(Trend objectToSave)
        {
            try
            {
                connection.Open();

                const string updateSql = "UPDATE Trend SET Duration=@duration, Importance=@importance, Sector=@sector WHERE Id=@id";

                var updateCommand = new SQLiteCommand(updateSql, connection);
                updateCommand.Parameters.AddWithValue("@duration", objectToSave.Duration);
                updateCommand.Parameters.AddWithValue("@importance", objectToSave.Importance);
                updateCommand.Parameters.AddWithValue("@sector", objectToSave.Sector);
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

                const string sql = "DELETE FROM Trend WHERE id=@id";

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
