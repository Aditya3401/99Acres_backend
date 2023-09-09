using _99Acres.Service.Entities.PostProperty;
using _99Acres.Service.Entities.User;
using _99Acres.Service.Entities.UserEntities;
using _99Acres.Service.Interface.UserInterface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace _99Acres.Services
{
    public class PostPropertyService : IPostProperty
    {
        public readonly IConfiguration _configuration;
        public readonly SqlConnection _mySqlConnection;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public PostPropertyService(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _mySqlConnection = new SqlConnection(_configuration.GetConnectionString("MyDBConnection"));
            _webHostEnvironment = webHostEnvironment;

        }

        [HttpPost]
        public async Task<PostPropertyResponse> PostPropertyDetails(PostPropertyRecord request)
        {
            
            PostPropertyResponse response = new PostPropertyResponse();
            response.IsSuccess = true;
            response.Message = "SuccessFul";

            try
            {

                if (_mySqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                string SqlQuery = @"INSERT INTO PostForm 
                                    (PropertyOptions,PropertyType,PropertyArea,Address,State,City,Price,ContactNo,Email,ImageProperty) Values 
                                    (@PropertyOptions, @PropertyType,@PropertyArea, @Address, @State, @City, @Price, @ContactNo, @Email, @ImageProperty);";

                using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _mySqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    // sqlCommand.CommandTimeout = 180;

                    sqlCommand.Parameters.AddWithValue("@PropertyOptions", request.PropertyOptions);
                    sqlCommand.Parameters.AddWithValue("@PropertyType", request.PropertyType);
                    sqlCommand.Parameters.AddWithValue("@PropertyArea", request.PropertyArea);
                    sqlCommand.Parameters.AddWithValue("@Address", request.Address);
                    sqlCommand.Parameters.AddWithValue("@State", request.State);
                    sqlCommand.Parameters.AddWithValue("@City", request.City);
                    sqlCommand.Parameters.AddWithValue("@Price", request.Price);                   
                    sqlCommand.Parameters.AddWithValue("@ContactNo", request.ContactNo);
                    sqlCommand.Parameters.AddWithValue("@Email", request.Email);
                    sqlCommand.Parameters.AddWithValue("@ImageProperty", request.ImageProperty);



                    int Status = await sqlCommand.ExecuteNonQueryAsync();

                    if (Status <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Register Query Not Executed";
                        return response;
                    }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            finally
            {
                await _mySqlConnection.CloseAsync();
                await _mySqlConnection.DisposeAsync();
            }

            return response;

        }

        [HttpGet]
        public async Task<PostPropertyResponse> GetProperty(int propertyId)
        {
            PostPropertyResponse res = new PostPropertyResponse();
            res.IsSuccess = true;
            res.Message = "SuccessFul";
            PostPropertyFetch response = new PostPropertyFetch();

            try
            {
                if (_mySqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                string selectQuery = @"SELECT PropertyId, PropertyOptions, PropertyType, PropertyArea, Address, State, City, Price, ContactNo, Email, ImageProperty
               FROM PostForm
               WHERE PropertyId = @PropertyId;";

                using (SqlCommand sqlCommand = new SqlCommand(selectQuery, _mySqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@PropertyID", propertyId);
                    SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();

                        response.PropertyId = reader.GetInt32(reader.GetOrdinal("PropertyId"));
                        response.PropertyOptions = reader["PropertyOptions"].ToString();
                        response.PropertyType = reader["PropertyType"].ToString();
                        response.PropertyArea = reader.GetDecimal(reader.GetOrdinal("PropertyArea"));
                        response.Address = reader["Address"].ToString();
                        response.State = reader["State"].ToString();
                        response.City = reader["City"].ToString();
                        response.Price = reader.GetDecimal(reader.GetOrdinal("Price"));
                        response.ContactNo = reader["ContactNo"].ToString();
                        response.Email = reader["Email"].ToString();
                        response.ImageProperty = reader["ImageProperty"].ToString();
                    }
                    else
                    {
                        // Handle the case where no data was found for the specified property ID.
                        res.IsSuccess = false;
                        res.Message = "Property not found.";
                        return res;
                    }
                }
            }
            catch (SqlException ex)
            {
                // Handle database-related exceptions.
                res.IsSuccess = false;
                res.Message = ex.Message;
            }
            finally
            {
                if (_mySqlConnection.State == System.Data.ConnectionState.Open)
                {
                    await _mySqlConnection.CloseAsync();
                }
            }

            // Populate the PostPropertyResponse object and return it.
            res.IsSuccess = true;
            res.PropertyData = response;
            return res;
        }


        public async Task<List<PostPropertyFetch>> GetAllProperties()
        {
            try
            {
                List<PostPropertyFetch> properties = new List<PostPropertyFetch>();



                _mySqlConnection.Open();

                string sqlQuery = "SELECT * FROM PostForm"; // Replace with your table name

                using (SqlCommand command = new SqlCommand(sqlQuery, _mySqlConnection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PostPropertyFetch property = new PostPropertyFetch
                            {
                                PropertyId = reader.GetInt32(reader.GetOrdinal("PropertyId")),
                                PropertyOptions = reader.GetString(reader.GetOrdinal("PropertyOptions")).Trim(),
                                PropertyType = reader.GetString(reader.GetOrdinal("PropertyType")).Trim(),
                                PropertyArea = reader.GetDecimal(reader.GetOrdinal("PropertyArea")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                State = reader.GetString(reader.GetOrdinal("State")),
                                City = reader.GetString(reader.GetOrdinal("City")).Trim(),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                ContactNo = reader.GetString(reader.GetOrdinal("ContactNo")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                /* ImageName = imageFolderPath + reader.GetString(reader.GetOrdinal("ImageName"))*/
                                ImageProperty = reader.GetString(reader.GetOrdinal("ImageProperty"))
                            };

                            properties.Add(property);
                        }
                    }
                }


                return properties;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<PostPropertyFetch>> FilterProperties(List<PostPropertyFetch> details, Filter filter)
        {
            var filterDetails = details.Where(p =>
                (filter.minPrice == 0 || p.Price >= filter.minPrice) &&
                (filter.maxPrice == 0 || p.Price <= filter.maxPrice) &&
                (filter.minArea == 0 || p.PropertyArea >= filter.minArea) &&
                (filter.maxArea == 0 || p.PropertyArea <= filter.maxArea) &&
                (string.IsNullOrEmpty(filter.City) || p.City == filter.City) &&
                (filter.PropertyType == null || filter.PropertyType.Length == 0 || filter.PropertyType.Contains(p.PropertyType))
            ).ToList();

            return filterDetails;
        }


    }
}
