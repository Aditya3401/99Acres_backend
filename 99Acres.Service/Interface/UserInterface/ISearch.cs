using _99Acres.Service.Entities.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _99Acres.Service.Interface.UserInterface
{
    public interface ISearch
    {
        public List<SearchClass> searchProperty(string searchTerm);
    }
}
