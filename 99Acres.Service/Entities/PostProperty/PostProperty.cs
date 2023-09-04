using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _99Acres.Service.Entities.PostProperty
{
    public class PostPropertyRecord
    {
        public int PropertyId { get; set; }
        public string PropertyOptions { get; set; }
        public string PropertyType { get; set; }
        public float PropertyArea { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public double Price { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
    public class PostPropertyResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
