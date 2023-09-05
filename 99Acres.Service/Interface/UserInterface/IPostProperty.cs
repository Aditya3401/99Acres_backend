﻿using _99Acres.Service.Entities.PostProperty;
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
        public Task<string> PostPropertyDetails(PostPropertyRecord record);
        public Task<string> saveImage(IFormFile Imagefile);
    }
}
