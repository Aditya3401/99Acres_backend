using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _99Acres.Service.Entities.PostProperty
{
    public class PostPropertyRecord
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyOptions { get; set; }
        public string PropertyType { get; set; }
        public float PropertyArea { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public double Price { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string ImageProperty { get; set; }
    }
    public class PostPropertyResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public PostPropertyFetch PropertyData { get; set; }
    }
    public class PostPropertyFetch
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyOptions { get; set; }
        public string PropertyType { get; set; }
        public decimal PropertyArea { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public decimal Price { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string ImageProperty { get; set; }
    }

    public class Filter
    {
        public decimal minPrice { get; set; }
        public decimal maxPrice { get; set; }
        public decimal minArea { get; set; }
        public decimal maxArea { get; set; }
        public string? City { get; set; }
        public string[]? PropertyType { get; set; }

    }
}
