using _99Acres.Service.Entities.PostProperty;
using _99Acres.Service.Entities.User;
using _99Acres.Service.Interface.UserInterface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
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

    }
}
