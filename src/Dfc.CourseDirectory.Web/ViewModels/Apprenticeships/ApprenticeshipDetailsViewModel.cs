﻿
using Dfc.CourseDirectory.Models.Models.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dfc.CourseDirectory.Services.Interfaces.CourseService;
using Dfc.CourseDirectory.Web.ViewModels.YourCourses;
using System.ComponentModel.DataAnnotations;

namespace Dfc.CourseDirectory.Web.ViewModels.Apprenticeships
{
    public class ApprenticeshipDetailViewModel
    {
        public string ApprenticeshipTitle { get; set; }

        public string Information { get; set; }


        public string Website { get; set; }

        public string Email { get; set; }

        public string Telephone { get; set; }

        public string ContactUsIUrl { get; set; }



    }
}