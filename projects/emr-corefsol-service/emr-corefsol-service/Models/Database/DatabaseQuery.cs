using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace emr_corefsol_service.Models
{
    public class DatabaseQuery
    {
        private static readonly MySqlConnection _connection;

        static DatabaseQuery()
        {
            if(_connection == null)
            {
                var connectionString = WebConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
                _connection = new MySqlConnection(connectionString);
                _connection.Open();
            }
        }

        public static string[] Query(string query)
        {
            var cmd = new MySqlCommand(query, _connection);
            MySqlDataReader rd = cmd.ExecuteReader();

            if (rd.HasRows)
            {
                List<string> data = new List<string>();
                while (rd.Read())
                {
                    data.Add(rd.GetString(1));
                }
                rd.Close();
                return data.ToArray();
            }
            rd.Close();
            return null;
        }
    }
}