using BankCommon.Entity;
using Common.Errors;
using CommonServer;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankServer.Crud
{
    public class CompanyFarmLogCrud : ACrud<CompanyFarmLog>
    {
        public override CompanyFarmLog Get(long id)
        {
            throw new NotImplementedException();
        }

        public List<CompanyFarmLog> GetBetween(long companyId, DateTime start, DateTime end) 
        {
            List<CompanyFarmLog> result = new List<CompanyFarmLog>()
                ;
            try
            {
                connection.Open();

                const string sql = "SELECT * FROM CompanyFarmLog WHERE FarmDate BETWEEN @start AND @end";

                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@start", start);
                command.Parameters.AddWithValue("@end", end);

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new CompanyFarmLog(
                        Convert.ToInt64(reader["Id"]),
                        Convert.ToInt64(reader["CompanyId"]),
                        Convert.ToDateTime(reader["FarmDate"]),
                        Convert.ToSingle(reader["FarmCaValue"])));
                }
            }
            catch (MySqlException)
            {
                return null;
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        public override CompanyFarmLog Insert(CompanyFarmLog objectToSave)
        {
            try
            {
                connection.Open();

                const string sql = "INSERT INTO CompanyFarmLog(CompanyId, FarmDate, FarmCaValue) VALUES (@companyId, @farmDate, @farmCaValue)";

                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@companyId", objectToSave.CompanyId);
                command.Parameters.AddWithValue("@farmDate", objectToSave.FarmDate);
                command.Parameters.AddWithValue("@farmCaValue", objectToSave.FarmCaValue);

                var res = command.ExecuteNonQuery();

                if (res > 0)
                {
                    objectToSave.Id =  command.LastInsertedId;

                    return objectToSave;
                }

                return null;
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
            throw new NotImplementedException();
        }

        public override bool Update(CompanyFarmLog objectToSave)
        {
            throw new NotImplementedException();
        }
    }
}
