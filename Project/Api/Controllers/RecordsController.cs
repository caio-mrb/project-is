using Api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace Api.Controllers
{
    [RoutePrefix("api/somiod/records")]
    public class RecordsController : ApiController
    {
        string connectionString = Api.Properties.Settings.Default.ConnStr;

        [Route("")]
        public IEnumerable<Record> GetAllRecords()
        {
            List<Record> records = new List<Record>();

            SqlConnection connection = new SqlConnection(connectionString);



            const string queryString = "SELECT * FROM dbo.Records";
            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand(queryString, connection);

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Record record = new Record
                    {
                        Id = (int)reader["id"],
                        Name = (string)reader["name"],
                        Content= (string)reader["content"],
                        CreationDatetime = (DateTime)(reader["creation_datetime"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["creation_datetime"]),
                        Parent = (int)reader["parent"],
                    };
                    records.Add(record);
                }
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
                Console.WriteLine(ex.Message);
            }
            return records;
        }
        /*[Route("{id:int}")]
        public IHttpActionResult GetProduct(int id)
        {

            Product p = null;
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();

                SqlCommand command = new SqlCommand(); //USING PARAMETERS -- best approach
                command.CommandText = "SELECT * FROM Prods where Id = @idprod";
                command.Parameters.AddWithValue("@idprod", id);
                command.CommandType = System.Data.CommandType.Text;
                command.Connection = conn;

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    p = new Product();
                    p.Id = (int)reader["id"];
                    p.Name = (string)reader["Name"];
                    p.Category = (string)reader["Category"];
                    p.Price = Convert.ToDecimal(reader["Price"]);//(decimal)reader["Price"];                                        
                }
                reader.Close();
                return Ok(p);

            }
            catch (Exception)
            {
                return NotFound();
            }
            finally
            {
                try
                {
                    conn.Close();
                }
                catch
                {

                }
            }
        }

        [HttpGet]
        [Route("{category}")]
        public IEnumerable<Product> GetProductByCategory(string category)
        {
            SqlConnection conn = new SqlConnection(connectionString);

            SqlCommand command = new SqlCommand();
            SqlDataReader reader = command.ExecuteReader();
            //var productList = products.FindAll(p => p.Category.ToLower() == category.ToLower());

            //return productList;
            return null;

        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult PostProduct([FromBody] Product p)
        {
            SqlConnection connection = new SqlConnection(connectionString);


            try
            {
                connection.Open();

                const string queryString = "INSERT INTO dbo.Prods values(@id, @name, @category, @price)";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@id", p.Id);
                command.Parameters.AddWithValue("@name", p.Name);
                command.Parameters.AddWithValue("@category", p.Category);
                command.Parameters.AddWithValue("@price", p.Price);

                SqlDataReader reader = command.ExecuteReader();

                connection.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
                Console.WriteLine(ex.Message);
            }
            return Ok("Produto criado com sucesso!");
        }*/
    }
}
