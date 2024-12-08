using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

public class DatabaseHandler
{
    private readonly string _connectionString;

    public DatabaseHandler()
    {
        _connectionString = Api.Properties.Settings.Default.ConnStr;
    }

    private SqlConnection GetSqlConnection()
    {
        SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();
        return connection;
    }

    public List<T> ExecuteQuery<T>(string query, List<SqlParameter> parameters, Func<SqlDataReader, T> selector)
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

    public int ExecuteNonQuery(string query, List<SqlParameter> parameters)
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

    public string GenerateUniqueName(string baseName)
    {
        int counter = 1;

        while (true)
        {
            string candidateName = $"{baseName}{counter}";

            string checkQuery = "SELECT COUNT(1) FROM dbo.applications WHERE name = @name";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@name", candidateName)
            };

            int existingCount = ExecuteQuery(checkQuery,parameters, reader =>
                (int)reader[0]
            ).FirstOrDefault();

            if (existingCount == 0)
            {
                return candidateName;
            }

            counter++;
        }
    }



    public List<Dictionary<string, object>> ExecuteDynamicQuery(string query, List<SqlParameter> parameters, List<string> columns)
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
