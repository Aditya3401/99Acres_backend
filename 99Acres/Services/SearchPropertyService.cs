using _99Acres.Service.Entities.Search;
using _99Acres.Service.Interface.UserInterface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _99Acres.Services
{
    public class SearchPropertyService : ISearch
    {
        public readonly IConfiguration _configuration;
        public readonly SqlConnection _mySqlConnection;

        public SearchPropertyService(IConfiguration configuration)
        {
            _configuration = configuration;
            _mySqlConnection = new SqlConnection(_configuration.GetConnectionString("MyDBConnection"));
        }

        public List<SearchClass> searchProperty(string searchTerm)
        {
            try
            {
                _mySqlConnection.Open();

                string query = "SELECT Address, State, City FROM PostForm WHERE Address LIKE @SearchTerm OR State LIKE @SearchTerm OR City LIKE @SearchTerm";
                using (SqlCommand command = new SqlCommand(query, _mySqlConnection))
                {
                    command.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");

                    List<SearchClass> properties = new List<SearchClass>();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            properties.Add(new SearchClass
                            {
                                Address = reader.GetString(0),
                                State = reader.GetString(1),
                                City = reader.GetString(2)
                            });
                        }
                    }

                    return properties;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }


}
