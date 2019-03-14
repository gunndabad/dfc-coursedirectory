﻿
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Dfc.CourseDirectory.Common;
using Dfc.CourseDirectory.Models.Enums;
using Dfc.CourseDirectory.Models.Models.Courses;
using Dfc.CourseDirectory.Services.CourseService;
using Dfc.CourseDirectory.Services.Interfaces.CourseService;
using Dfc.CourseDirectory.Web.ViewModels;


namespace Dfc.CourseDirectory.Web.Controllers
{

    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICourseService _courseService;

        private IHostingEnvironment _env;
        private ISession _session => _contextAccessor.HttpContext.Session;

        public DashboardController(
                ILogger<DashboardController> logger,
                IHttpContextAccessor contextAccessor,
                ICourseService courseService,
                IHostingEnvironment env)
        {
            Throw.IfNull(logger, nameof(logger));
            Throw.IfNull(contextAccessor, nameof(contextAccessor));
            Throw.IfNull(courseService, nameof(courseService));
            Throw.IfNull(env, nameof(env));

            _logger = logger;
            _contextAccessor = contextAccessor;
            _courseService = courseService;
            _env = env;
        }


        public static DashboardViewModel GetDashboardViewModel(ICourseService service, int? UKPRN)
        {
            if (!UKPRN.HasValue)
                return new DashboardViewModel();

            IEnumerable<Course> courses = service.GetYourCoursesByUKPRNAsync(new CourseSearchCriteria(UKPRN))
                                                 .Result
                                                 .Value
                                                 .Value
                                                 .SelectMany(o => o.Value)
                                                 .SelectMany(i => i.Value);

            IEnumerable<CourseValidationResult> result = service.PendingCourseValidationMessages(courses)
                                                                .Value;
            IEnumerable<string> courseMessages = result.SelectMany(c => c.Issues);
            IEnumerable<string> runMessages    = result.SelectMany(c => c.RunValidationResults)
                                                       .SelectMany(r => r.Issues);
            IEnumerable<string> messages       = courseMessages.Concat(runMessages)
                                                               .GroupBy(i => i)
                                                               .Select(g => $"{ g.LongCount() } { g.Key }");

            IEnumerable<ICourseStatusCountResult> counts = service.GetCourseCountsByStatusForUKPRN(new CourseSearchCriteria(UKPRN))
                                                                         .Result
                                                                         .Value;

            int[] pendingStatuses = new int[] { (int)RecordStatus.Pending, (int)RecordStatus.BulkUloadPending, (int)RecordStatus.APIPending, (int)RecordStatus.MigrationPending };
            DashboardViewModel vm = new DashboardViewModel()
            {
                 ValidationHeader = $"{ courseMessages.LongCount() + runMessages.LongCount() } data items require attention",
                 ValidationMessages = messages,
                 LiveCourseCount = counts.FirstOrDefault(c => c.Status == (int)RecordStatus.Live).Count,
                 ArchivedCourseCount = counts.FirstOrDefault(c => c.Status == (int)RecordStatus.Archived).Count,
                 PendingCourseCount = (from ICourseStatusCountResult c in counts
                                       join int p in pendingStatuses
                                       on c.Status equals p
                                       select c.Count).Sum() //,
                 //RecentlyModifiedCourses = new List<Course>() { new Course(), new Course() }
            };
            return vm;
        }

        [Authorize]
        public IActionResult Index()
        {
            //_session.SetString("Option", "Dashboard");
            //return RedirectToAction("Index", "PublishCourses", new { publishMode = PublishMode.Migration });

            int? UKPRN = _session.GetInt32("UKPRN");
            if (!UKPRN.HasValue)
                return RedirectToAction("Index", "Home", new { errmsg = "Please select a Provider." });

            return View(GetDashboardViewModel(_courseService, UKPRN));
        }
    }
}