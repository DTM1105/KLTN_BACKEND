﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneSolution_API.Models
{
    public class ImageResult
    {
        public bool Success { get; set; }
        public string ImageName { get; set; }
        public string ErrorMessage { get; set; }
    }

}