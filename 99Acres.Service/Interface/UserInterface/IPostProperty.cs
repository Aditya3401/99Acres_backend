using _99Acres.Service.Entities.PostProperty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _99Acres.Service.Interface.UserInterface
{
    public interface IPostProperty
    {
        public Task<int> PostPropertyDetails(PostPropertyRecord record);
    }
}
