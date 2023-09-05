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
        public async Task<string> PostPropertyDetails(PostPropertyRecord record)
        {
            try
            {
                string ImageName = await saveImage(record.ImageFile);
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MyDBConnection"));

                SqlCommand cmd = new SqlCommand("insert into PostForm values(' " + record.PropertyOptions + " ', ' " + record.PropertyType + " ',' " + record.PropertyArea + " ','" + record.Address + "','" + record.State + "','" + record.City + "','" + record.Price + "','" + record.ContactNo + "','" + record.Email + "','" + ImageName + "',' " + record.ImageFile + " ')", con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return "Post Property add successfully";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [NonAction]
        public async Task<string> saveImage(IFormFile Imagefile)
        {
            string imageName = new String(Path.GetFileNameWithoutExtension(Imagefile.FileName).Take(10).ToArray()).Replace(' ', '-');
            imageName = imageName + DateTime.Now.ToString(" yymmssfff") + Path.GetExtension(Imagefile.FileName);
            var imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", imageName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await Imagefile.CopyToAsync(fileStream);
            }
            return imageName;
        }
    }
}
