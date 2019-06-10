﻿
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Dfc.CourseDirectory.Common;
using Dfc.CourseDirectory.Models.Enums;
using Dfc.CourseDirectory.Models.Models.Courses;
using Dfc.CourseDirectory.Services.CourseService;
using Dfc.CourseDirectory.Services.Interfaces.CourseService;
using Dfc.CourseDirectory.Web.ViewModels;
using Dfc.CourseDirectory.Services.Interfaces.BlobStorageService;


namespace Dfc.CourseDirectory.Web.Controllers
{

    public class BlobStorageController : Controller
    {
        private readonly ILogger<BlobStorageController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICourseService _courseService;
        private readonly IBlobStorageService _blobService;

        //private IHostingEnvironment _env;
        private ISession _session => _contextAccessor.HttpContext.Session;

        public BlobStorageController(
                ILogger<BlobStorageController> logger,
                IHttpContextAccessor contextAccessor,
                ICourseService courseService,
                IBlobStorageService blobService)
        {
            Throw.IfNull(logger, nameof(logger));
            Throw.IfNull(contextAccessor, nameof(contextAccessor));
            Throw.IfNull(courseService, nameof(courseService));
            Throw.IfNull(blobService, nameof(blobService));

            _logger = logger;
            _contextAccessor = contextAccessor;
            _courseService = courseService;
            _blobService = blobService;
        }

        [Authorize]
        public IActionResult Index()
        {
            int? UKPRN = _session.GetInt32("UKPRN");
            if (!UKPRN.HasValue)
                return RedirectToAction("Index", "Home", new { errmsg = "Please select a Provider." });

            //var vm = GetBlobStorageViewModel(_courseService, UKPRN, "");
            return View(); //vm);
        }

        public FileStreamResult GetBulkUploadTemplateFile()
        {
            MemoryStream ms = new MemoryStream();
            Task task = _blobService.GetBulkUploadTemplateFileAsync(ms);
            task.Wait();
            ms.Position = 0;
            FileStreamResult result = new FileStreamResult(ms, MediaTypeNames.Text.Plain);
            result.FileDownloadName = "bulk upload template.csv";
            return result;
        }

        [Authorize]
        public FileStreamResult GetBulkUploadErrors(int? UKPRN)
        {
            if (!UKPRN.HasValue)
                return null;

            IEnumerable<Course> courses = _courseService.GetYourCoursesByUKPRNAsync(new CourseSearchCriteria(UKPRN))
                                                        .Result
                                                        .Value
                                                        .Value
                                                        .SelectMany(o => o.Value)
                                                        .SelectMany(i => i.Value);

            //int[] pendingStatuses = new int[] { (int)RecordStatus.Pending, (int)RecordStatus.BulkUploadPending, (int)RecordStatus.APIPending, (int)RecordStatus.MigrationPending, (int)RecordStatus.MigrationReadyToGoLive, (int)RecordStatus.BulkUploadReadyToGoLive };
            //int[] bulkStatuses = new int[] { (int)RecordStatus.BulkUploadPending };
            //IEnumerable<Course> validCourses = courses.Where(c => c.IsValid);

            IEnumerable<CourseValidationResult> results = _courseService.CourseValidationMessages(courses, ValidationMode.EditCourseBU).Value;

            int counter = 1;
            IEnumerable<string> headers = new string[] { "ID,Row Number,Column Name,Error Description" };
            IEnumerable<string> csvlines = results.SelectMany(r => r.Issues
                                                                    .Select(i => string.Join(",", new string[] { counter.ToString(), counter++.ToString(), i, i } ))
                                                             );
            string report = string.Join(Environment.NewLine, headers.Concat(csvlines));
            byte[] data = Encoding.ASCII.GetBytes(report);
            MemoryStream ms = new MemoryStream(data);
            //Task task = _blobService.UploadFileAsync("uploadtest.csv", ms);
            //task.Wait();

            //ms = new MemoryStream(data); 
            //task = _blobService.DownloadFileAsync($"{UKPRN.ToString()}/Bulk Upload/Files/test.csv", ms);
            //task.Wait();
            ms.Position = 0;
            FileStreamResult result = new FileStreamResult(ms, MediaTypeNames.Text.Plain);
            DateTime d = DateTime.UtcNow;
            result.FileDownloadName = $"Bulk_upload_error_{d.Day.TwoChars()}_{d.Month.TwoChars()}_{d.Year}_{d.Hour.TwoChars()}_{d.Minute.TwoChars()}.csv";
            return result;
        }
    }

    internal static class TwoCharsClass
    {
        internal static string TwoChars(this int extendee)
        {
            return extendee.ToString().Length < 2 ? $"0{extendee.ToString()}" : extendee.ToString();
        }
    }

}
