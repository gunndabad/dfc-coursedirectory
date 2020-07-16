﻿CREATE TABLE [Pttcd].[CourseRuns]
(
	[CourseRunId] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_CourseRuns] PRIMARY KEY,
	[CourseId] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [FK_CourseRuns_Course] FOREIGN KEY REFERENCES [Pttcd].[Courses] ([CourseId]),
	[CourseRunStatus] TINYINT,
	[CreatedOn] DATETIME,
	[CreatedBy] NVARCHAR(MAX),
	[UpdatedOn] DATETIME,
	[UpdatedBy] NVARCHAR(MAX),
	[CourseName] NVARCHAR(MAX),
	[VenueId] UNIQUEIDENTIFIER CONSTRAINT [FK_CourseRuns_Venue] FOREIGN KEY REFERENCES [Pttcd].[Venues] ([VenueId]),
	[ProviderCourseId] NVARCHAR(MAX),
	[DeliveryMode] TINYINT,
	[FlexibleStartDate] BIT,
	[StartDate] DATE,
	[CourseWebsite] NVARCHAR(MAX),
	[Cost] DECIMAL,
	[CostDescription] NVARCHAR(MAX),
	[DurationUnit] TINYINT,
	[DurationValue] INT,
	[StudyMode] TINYINT,
	[AttendancePattern] TINYINT,
	[National] BIT
)
