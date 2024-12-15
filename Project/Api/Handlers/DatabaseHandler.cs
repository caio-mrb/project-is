using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

public static class DatabaseHandler
{
    private static readonly string _connectionString = Api.Properties.Settings.Default.ConnStr;

    private static SqlConnection GetSqlConnection()
    {
        SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();
        return connection;
    }

    public static List<T> ExecuteQuery<T>(string query, List<SqlParameter> parameters, Func<SqlDataReader, T> selector)
    {
        using (SqlConnection connection = GetSqlConnection())
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());
            command.CommandType = CommandType.Text;

            List<T> results = new List<T>();
            try
            {
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(selector(reader));
                }
            }
            finally
            {
                connection.Close();
            }

            return results;
        }
    }

    public static int ExecuteNonQuery(string query, List<SqlParameter> parameters)
    {
        using (SqlConnection connection = GetSqlConnection())
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());
            command.CommandType = CommandType.Text;

            try
            {
                return command.ExecuteNonQuery();
            }
            finally
            {
                connection.Close();
            }
        }
    }



    public static List<Dictionary<string, object>> ExecuteDynamicQuery(string query, List<SqlParameter> parameters, List<string> columns)
    {
        using (SqlConnection connection = GetSqlConnection())
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());
            command.CommandType = CommandType.Text;

            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();
            try
            {
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();
                    foreach (var column in columns)
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal(column)))
                        {
                            row[column] = reader[column];
                        }
                    }
                    results.Add(row);
                }
            }
            finally
            {
                connection.Close();
            }

            return results;
        }
    }
}
