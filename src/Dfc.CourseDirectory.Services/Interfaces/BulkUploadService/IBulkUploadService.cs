﻿using System.Collections.Generic;
using System.IO;
using Dfc.CourseDirectory.Services.Models.Courses;

namespace Dfc.CourseDirectory.Services.Interfaces.BulkUploadService
{
    public interface IBulkUploadService
    {
        List<string> ProcessBulkUpload(Stream stream, int providerUKPRN, string userId, bool uploadCourses);        
        List<BulkUploadCourse> PolulateLARSData(List<BulkUploadCourse> bulkUploadcourses, out List<string> errors);
        List<Course> MappingBulkUploadCourseToCourse(List<BulkUploadCourse> bulkUploadcourses, string userId, out List<string> errors);

        int CountCsvLines(Stream stream);

        int BulkUploadSecondsPerRecord { get; }
    }
}
