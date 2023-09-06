using _99Acres.Service.Entities.User;
using _99Acres.Service.Entities.UserEntities;
using _99Acres.Service.Interface.UserInterface;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

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
            request.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            UserRegisterResponse response = new UserRegisterResponse();
            response.IsSuccess = true;
            response.Message = "SuccessFul";

            try
            {

                if (_mySqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                string checkEmailQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email";

                using (SqlCommand checkEmailCommand = new SqlCommand(checkEmailQuery, _mySqlConnection))
                {
                    checkEmailCommand.Parameters.AddWithValue("@Email", request.Email);
                    int emailCount = (int)await checkEmailCommand.ExecuteScalarAsync();

                    if (emailCount > 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Email already exists";
                        return response;
                    }
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
            response.Message = "Successful";

            try
            {
                if (_mySqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                string SqlQuery = @"SELECT UserId, Email, Password, UserName, ContactNo
                            FROM Users
                            WHERE Email = @Email";

                using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _mySqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.CommandTimeout = 180;

                    sqlCommand.Parameters.AddWithValue("@Email", request.Email);
                   

                    using (DbDataReader dataReader = await sqlCommand.ExecuteReaderAsync())
                    {
                        if (dataReader.HasRows)
                        {
                            await dataReader.ReadAsync();
                            string hashedPasswordFromDb = dataReader["Password"].ToString();

                            if (BCrypt.Net.BCrypt.Verify(request.Password, hashedPasswordFromDb))
                            {
                                response.Message = "Login Successful";
                                response.data = new UserLoginInformation();
                                response.UserToken = new UserToken();
                                UserTokenGenerate tokenGenerate = new UserTokenGenerate(_configuration);

                                response.data.UserId = Convert.ToInt32(dataReader["UserId"]);
                                response.data.Email = dataReader["Email"].ToString();
                                response.data.UserName = dataReader["UserName"].ToString();
                                response.data.ContactNo = dataReader["ContactNo"].ToString();

                                response.UserToken.JWTToken = tokenGenerate.GenerateJWTToken(response.data.UserId);
                                response.UserToken.UserRefreshToken = tokenGenerate.GenerateRefresh();

                                tokenGenerate.UserTokenDB(response);
                            }
                            else
                            {
                                response.IsSuccess = false;
                                response.Message = "Invalid Password";
                                return response;
                            }
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = "Login Unsuccessful";
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

        public async Task<ForgotPasswordResponse> ForgotPassword(ForgotPasswordRequest request)
        {
            ForgotPasswordResponse response = new ForgotPasswordResponse();
            response.IsSuccess = true;
            response.Message = "SuccessFul";
            try
            {
                if (_mySqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                string SqlQuery = @"SELECT Email
                                    FROM Users
                                    WHERE Email=@Email";

                using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _mySqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.CommandTimeout = 180;

                    sqlCommand.Parameters.AddWithValue("@Email", request.Email);


                    using (DbDataReader dataReader = await sqlCommand.ExecuteReaderAsync())
                    {
                        if (dataReader.HasRows)
                        {
                            await dataReader.ReadAsync();
                            response.Message = "Email send Sucessful";
                            response.data = new Information();


                            response.data.Email = dataReader[0].ToString();
                            dataReader.Close();

                            /*string resetToken = GenerateResetToken();*/
                            (string resetToken, DateTime tokenCreationTime) = GenerateResetTokenWithTime();
                            await StoreResetTokenInDatabase(response.data.Email, resetToken, tokenCreationTime);

                            SendResetEmail(response.data.Email, resetToken);
                            dataReader.Close();

                            return response;
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



        
        public (string Token, DateTime CreationTime) GenerateResetTokenWithTime()
        {
            var randomNumber = new byte[32];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomNumber);
            }

            var token = Convert.ToBase64String(randomNumber);
            var creationTime = DateTime.Now;

            return (token, creationTime);
        }


        // send email 
        private async Task SendResetEmail(string email, string resetToken)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Pawan Sharma", "glpawansharma@gmail.com"));
            message.To.Add(new MailboxAddress("", email)); // Recipient's email
            message.Subject = "Password Reset";

            var builder = new BodyBuilder();
            builder.HtmlBody = $"<a href='https://localhost:7240/EmployeeForgotPassword/ResetPassword/reset?token={resetToken}'>Click here to reset your password</a>";
            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false); // SMTP server details
                await client.AuthenticateAsync("glpawansharma@gmail.com", "wxscgqxhmqhkhzlr"); // Your email credentials
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }



        //store generate token in database
        private async Task StoreResetTokenInDatabase(string email, string resetToken, DateTime tokenCreationTime)
        {
            try
            {
                string updateQuery = @"UPDATE Users
                               SET ResetToken = @ResetToken, TokenCreationTime = @TokenCreationTime
                               WHERE Email = @Email";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery, _mySqlConnection))
                {
                    updateCommand.CommandType = System.Data.CommandType.Text;
                    updateCommand.CommandTimeout = 180;

                    updateCommand.Parameters.AddWithValue("@Email", email);
                    updateCommand.Parameters.AddWithValue("@ResetToken", resetToken);
                    updateCommand.Parameters.AddWithValue("@TokenCreationTime", tokenCreationTime);

                    await updateCommand.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while storing reset token: {ex.Message}");
            }
        }



        public async Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest request)
        {
            request.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            ResetPasswordResponse response = new ResetPasswordResponse();
            response.IsSuccess = true;
            response.Message = "SuccessFul";
            try
            {
                if (_mySqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                if (await IsValidResetToken(request.ResetToken))
                {
                    string SqlQuery = "UPDATE Users SET Password = @Password WHERE ResetToken = @ResetToken";

                    using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _mySqlConnection))
                    {
                        sqlCommand.CommandType = System.Data.CommandType.Text;
                        sqlCommand.CommandTimeout = 180;

                        sqlCommand.Parameters.AddWithValue("@Password", request.Password);
                        sqlCommand.Parameters.AddWithValue("@ResetToken", request.ResetToken);

                        int rowsAffected = await sqlCommand.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            response.IsSuccess = true;
                            response.Message = "Password reset successful";
                        }
                        else
                        {
                            response.Message = "Password reset failed";
                        }
                    }
                }
                else
                {
                    response.Message = "Invalid reset token";
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

        private async Task<bool> IsValidResetToken(string resetToken)
        {
            try
            {
                if (_mySqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                string sqlQuery = "SELECT COUNT(*) FROM Users WHERE ResetToken = @ResetToken";

                using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, _mySqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.Parameters.AddWithValue("@ResetToken", resetToken);

                    int count = Convert.ToInt32(await sqlCommand.ExecuteScalarAsync());

                    return count > 0;
                }
            }
            catch (Exception)
            {
                // Handle exceptions if needed
                return false;
            }

        }

    }
}
