﻿using System;

namespace Dfc.CourseDirectory.WebV2.Models
{
    [Flags]
    public enum ApprenticeshipQAProviderStyleFailedReasons
    {
        None = 0,
        SpecificApprenticeshipDetailsGiven = 1,
        JobRolesIncluded = 2,
        TermStandardUsed = 4,
        TermFrameworkUsed = 8,
        TermLearnerOrStudentUsed = 16,
        TermCourseUsed = 32,
        Other = 64
    }
}
