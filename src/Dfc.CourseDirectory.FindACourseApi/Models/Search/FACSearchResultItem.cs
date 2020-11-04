﻿namespace Dfc.CourseDirectory.FindACourseApi.Models
{
    public class FACSearchResultItem
    {
        public AzureSearchCourse Course { get; set; }
        public double? Distance { get; set; }
        public double Score { get; set; }
    }
}
