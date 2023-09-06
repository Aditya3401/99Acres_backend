using _99Acres.Service.Entities.UserEntities;
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
    public class UserUpdateDetails : IUserUpdateDetails
    {
        public readonly IConfiguration _configuration;
        public readonly SqlConnection _mySqlConnection;
        public UserUpdateDetails(IConfiguration configuration)
        {
            _configuration = configuration;
            _mySqlConnection = new SqlConnection(_configuration.GetConnectionString("MyDBConnection"));
        }
        public async Task<UserUpdateResponse> UpdateUser(UserUpdateRequest request)
        {
            UserUpdateResponse response = new UserUpdateResponse();

            try
            {
                if (_mySqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                
                string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE UserID = @UserID";

                using (SqlCommand checkUserCommand = new SqlCommand(checkUserQuery, _mySqlConnection))
                {
                    checkUserCommand.Parameters.AddWithValue("@UserID", request.UserId);
                    int userCount = (int)await checkUserCommand.ExecuteScalarAsync();

                    if (userCount == 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "User not found";
                        return response;
                    }
                }

                // Update user information
                string updateQuery = @"UPDATE Users
                               SET UserName = @UserName, Email = @Email, ContactNo = @ContactNo
                               WHERE UserId = @UserId;";

                using (SqlCommand sqlCommand = new SqlCommand(updateQuery, _mySqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@UserID", request.UserId);
                    sqlCommand.Parameters.AddWithValue("@UserName", request.UserName);
                    sqlCommand.Parameters.AddWithValue("@Email", request.Email);
                    sqlCommand.Parameters.AddWithValue("@ContactNo", request.ContactNo);

                    int status = await sqlCommand.ExecuteNonQueryAsync();

                    if (status <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "User update query not executed";
                    }
                    else
                    {
                        response.IsSuccess = true;
                        response.Message = "User information updated successfully";
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
