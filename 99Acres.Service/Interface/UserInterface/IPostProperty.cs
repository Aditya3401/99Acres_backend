using _99Acres.Service.Entities.PostProperty;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _99Acres.Service.Interface.UserInterface
{
    public interface IPostProperty
    {
        public Task<PostPropertyResponse> PostPropertyDetails(PostPropertyRecord request);
        public Task<PostPropertyResponse> GetProperty(int propertyId);
        public Task<List<PostPropertyFetch>> GetAllProperties();

        public Task<List<PostPropertyFetch>> FilterProperties(List<PostPropertyFetch> details, Filter filter);
    }
}
