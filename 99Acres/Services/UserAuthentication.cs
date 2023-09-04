using _99Acres.Service.Entities.User;
using _99Acres.Service.Entities.UserEntities;
using _99Acres.Service.Interface.UserInterface;
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
    public class UserAuthentication : IUserAuthentication
    {
        public readonly IConfiguration _configuration;
        public readonly SqlConnection _mySqlConnection;
        public UserAuthentication(IConfiguration configuration)
        {
            _configuration = configuration;
            _mySqlConnection = new SqlConnection(_configuration.GetConnectionString("MyDBConnection").ToString());
        }
        public async Task<UserRegisterResponse> RegisterUser(UserRegisterRequest request)
        {
            UserRegisterResponse response = new UserRegisterResponse();
            response.IsSuccess = true;
            response.Message = "SuccessFul";

            try
            {

                if (_mySqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                string SqlQuery = @"INSERT INTO Users 
                                    (UserName,Password,Email,ContactNo) Values 
                                    (@UserName, @Password,@Email,@ContactNo);";

                using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _mySqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    // sqlCommand.CommandTimeout = 180;
                   
                    sqlCommand.Parameters.AddWithValue("@UserName", request.UserName);
                    sqlCommand.Parameters.AddWithValue("@Password", request.Password);
                    sqlCommand.Parameters.AddWithValue("@Email", request.Email);
                    sqlCommand.Parameters.AddWithValue("@ContactNo", request.ContactNo);
                    


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
        public async Task<UserLoginResponse> LoginUser(UserLoginRequest request)
        {
            UserLoginResponse response = new UserLoginResponse();

            response.IsSuccess = true;
            response.Message = "SuccessFul";

            try
            {
                if (_mySqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                string SqlQuery = @"SELECT UserId,Email,Password
                                    FROM Users
                                    WHERE Email=@Email AND Password=@Password";

                using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _mySqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.CommandTimeout = 180;

                    sqlCommand.Parameters.AddWithValue("@Email", request.Email);
                   
                    sqlCommand.Parameters.AddWithValue("@Password", request.Password);

                    using (DbDataReader dataReader = await sqlCommand.ExecuteReaderAsync())
                    {
                        if (dataReader.HasRows)
                        {
                            await dataReader.ReadAsync();
                            response.Message = "Login Successful";
                            response.data = new UserLoginInformation();
                            response.UserToken = new UserToken();
                            UserTokenGenerate tokenGenerate = new UserTokenGenerate(_configuration);

                            response.data.UserId = Convert.ToInt32(dataReader["UserId"]); // Corrected line
                            response.data.Email = dataReader["Email"].ToString();

                            response.UserToken.JWTToken = tokenGenerate.GenerateJWTToken(response.data.UserId);
                            response.UserToken.UserRefreshToken = tokenGenerate.GenerateRefresh();

                            tokenGenerate.UserTokenDB(response);
                        }

                        else
                        {
                            response.IsSuccess = false;
                            response.Message = "Login Unsuccesful";
                            return response;
                        }

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
