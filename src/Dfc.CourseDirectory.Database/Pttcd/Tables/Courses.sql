﻿CREATE TABLE [Pttcd].[Courses]
(
	[CourseId] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_Courses] PRIMARY KEY,
	[CourseStatus] INT,
	[CreatedOn] DATETIME,
	[CreatedBy] NVARCHAR(MAX),
	[UpdatedOn] DATETIME,
	[UpdatedBy] NVARCHAR(MAX),
	[TribalCourseId] INT,
	[LearnAimRef] VARCHAR(50),
	[ProviderUkprn] INT,
	[CourseDescription] NVARCHAR(MAX),
	[EntryRequirements] NVARCHAR(MAX),
	[WhatYoullLearn] NVARCHAR(MAX),
	[HowYoullLearn] NVARCHAR(MAX),
	[WhatYoullNeed] NVARCHAR(MAX),
	[HowYoullBeAssessed] NVARCHAR(MAX),
	[WhereNext] NVARCHAR(MAX)
)
