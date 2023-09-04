using _99Acres.Service.Entities.PostProperty;
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
    public  class PostPropertyService : IPostProperty
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
        public async Task<int> PostPropertyDetails(PostPropertyRecord record)
        {
           
               
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("MyDBConnection")))
                {
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string insertQuery = "Insert into PostProperty(PropertyOptions, PropertyType, PropertyArea, State, City, Price, ContactNo, Email, ImageUrl, ImageFile)Values(@PropertyOptions, @PropertyType, @PropertyArea, @State, @City, @Price, @ContactNo, @Email, @ImageUrl, @ImageFile)";

                            using (SqlCommand command = new SqlCommand(insertQuery, connection, transaction))
                            {

                                command.Parameters.AddWithValue("@PropertyOptions", record.PropertyOptions);
                                command.Parameters.AddWithValue("@PropertyType", record.PropertyType);
                                command.Parameters.AddWithValue("@PropertyArea", record.PropertyArea);
                                command.Parameters.AddWithValue("@State", record.State);
                                command.Parameters.AddWithValue("@City", record.City);
                                command.Parameters.AddWithValue("@Price", record.Price);
                                command.Parameters.AddWithValue("@ContactNo", record.ContactNo);
                                command.Parameters.AddWithValue("@Email", record.Email);
                                record.ImageUrl = await saveImage(record.ImageFile);
                                command.Parameters.AddWithValue("@ImageUrl", record.ImageUrl);
                                command.Parameters.AddWithValue("@ImageFie", record.ImageFile);


                                command.ExecuteNonQuery();

                                transaction.Commit();

                                return record.PropertyId;  // Assuming OperatorId is the generated ID from NewOperatorEntry table
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }
        [NonAction]
        public async Task<string> saveImage(IFormFile Imagefile)
        {
            string imageName = new String(Path.GetFileNameWithoutExtension(Imagefile.FileName).Take(10).ToArray()).Replace(' ', '-');
            imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(Imagefile.FileName);
            var imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", imageName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await Imagefile.CopyToAsync(fileStream);
            }
            return imageName;
        }
    }
}
