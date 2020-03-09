﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.CourseDirectory.WebV2.DataStore.Sql.Queries;
using Dfc.CourseDirectory.WebV2.Models;
using Xunit;

namespace Dfc.CourseDirectory.WebV2.Tests.Features.ApprenticeshipQA
{
    public class ApprenticeshipAssessmentTests : TestBase
    {
        public ApprenticeshipAssessmentTests(CourseDirectoryApplicationFactory factory)
            : base(factory)
        {
        }

        [Theory]
        [InlineData(TestUserType.ProviderSuperUser)]
        [InlineData(TestUserType.ProviderUser)]
        public async Task Get_ProviderUserCannotAccess(TestUserType userType)
        {
            // Arrange
            var ukprn = 12345;

            var providerId = await TestData.CreateProvider(
                ukprn: ukprn,
                providerName: "Provider 1",
                apprenticeshipQAStatus: ApprenticeshipQAStatus.Submitted);

            var providerUserId = $"{ukprn}-user";
            await TestData.CreateUser(providerUserId, "somebody@provider1.com", "Provider 1", "Person");

            var apprenticeshipId = await TestData.CreateApprenticeship(ukprn);

            await TestData.CreateApprenticeshipQASubmission(
                providerId,
                submittedOn: Clock.UtcNow,
                submittedByUserId: providerUserId,
                providerMarketingInformation: "The overview",
                apprenticeshipIds: new[] { apprenticeshipId });

            await User.AsTestUser(userType, providerId);

            // Act
            var response = await HttpClient.GetAsync($"apprenticeship-qa/apprenticeship-assessments/{apprenticeshipId}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Get_ApprenticeshipDoesNotExistReturnsBadRequest()
        {
            // Arrange
            await User.AsHelpdesk();

            var apprenticeshipId = Guid.NewGuid();

            // Act
            var response = await HttpClient.GetAsync($"apprenticeship-qa/apprenticeship-assessments/{apprenticeshipId}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Get_NoSubmissionReturnsBadRequest()
        {
            // Arrange
            var ukprn = 12345;

            var providerId = await TestData.CreateProvider(
                ukprn: ukprn,
                providerName: "Provider 1",
                apprenticeshipQAStatus: ApprenticeshipQAStatus.Submitted);

            var apprenticeshipId = await TestData.CreateApprenticeship(ukprn);

            await User.AsHelpdesk();

            // Act
            var response = await HttpClient.GetAsync($"apprenticeship-qa/apprenticeship-assessments/{apprenticeshipId}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData(ApprenticeshipQAStatus.Failed)]
        [InlineData(ApprenticeshipQAStatus.NotStarted)]
        [InlineData(ApprenticeshipQAStatus.Passed)]
        [InlineData(ApprenticeshipQAStatus.UnableToComplete)]
        public async Task Get_SubmissionAtInvalidStatusReturnsBadRequest(ApprenticeshipQAStatus qaStatus)
        {
            // Arrange
            var ukprn = 12345;

            var providerId = await TestData.CreateProvider(
                ukprn: ukprn,
                providerName: "Provider 1",
                apprenticeshipQAStatus: qaStatus);

            var providerUserId = $"{ukprn}-user";
            await TestData.CreateUser(providerUserId, "somebody@provider1.com", "Provider 1", "Person");

            var apprenticeshipId = await TestData.CreateApprenticeship(ukprn);

            await TestData.CreateApprenticeshipQASubmission(
                providerId,
                submittedOn: Clock.UtcNow,
                submittedByUserId: providerUserId,
                providerMarketingInformation: "The overview",
                apprenticeshipIds: new[] { apprenticeshipId });

            await User.AsHelpdesk();

            // Act
            var response = await HttpClient.GetAsync($"apprenticeship-qa/apprenticeship-assessments/{apprenticeshipId}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Get_NewSubmissionSucceeds()
        {
            // Arrange
            var ukprn = 12345;

            var providerId = await TestData.CreateProvider(
                ukprn: ukprn,
                providerName: "Provider 1",
                apprenticeshipQAStatus: ApprenticeshipQAStatus.Submitted);

            var providerUserId = $"{ukprn}-user";
            await TestData.CreateUser(providerUserId, "somebody@provider1.com", "Provider 1", "Person");

            var apprenticeshipId = await TestData.CreateApprenticeship(
                ukprn,
                apprenticeshipTitle: "Test title",
                marketingInformation: "Test marketing info");

            await TestData.CreateApprenticeshipQASubmission(
                providerId,
                submittedOn: Clock.UtcNow,
                submittedByUserId: providerUserId,
                providerMarketingInformation: "The overview",
                apprenticeshipIds: new[] { apprenticeshipId });

            await User.AsHelpdesk();

            // Act
            var response = await HttpClient.GetAsync($"apprenticeship-qa/apprenticeship-assessments/{apprenticeshipId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var doc = await response.GetDocument();
            Assert.Equal("QA Apprenticeship training course information - Course Directory", doc.Title);
            Assert.Equal("Test title", doc.QuerySelector("h1").TextContent);
            Assert.Equal(
                "Test marketing info",
                doc.GetElementById("pttcd-apprenticeship-qa-apprenticeship-assessment-marketing-information").TextContent);
        }

        [Fact]
        public async Task Get_AlreadyAssessedSubmissionRendersExpectedOutput()
        {
            // Arrange
            var ukprn = 12345;

            var providerId = await TestData.CreateProvider(
                ukprn: ukprn,
                providerName: "Provider 1",
                apprenticeshipQAStatus: ApprenticeshipQAStatus.Submitted);

            var providerUserId = $"{ukprn}-user";
            await TestData.CreateUser(providerUserId, "somebody@provider1.com", "Provider 1", "Person");

            var apprenticeshipId = await TestData.CreateApprenticeship(
                ukprn,
                apprenticeshipTitle: "Test title",
                marketingInformation: "Test marketing info");

            var submissionId = await TestData.CreateApprenticeshipQASubmission(
                providerId,
                submittedOn: Clock.UtcNow,
                submittedByUserId: providerUserId,
                providerMarketingInformation: "The overview",
                apprenticeshipIds: new[] { apprenticeshipId });

            await TestData.CreateApprenticeshipQAApprenticeshipAssessment(
                submissionId,
                apprenticeshipId,
                assessedByUserId: User.UserId.ToString(),
                assessedOn: Clock.UtcNow,
                compliancePassed: true,
                complianceFailedReasons: ApprenticeshipQAApprenticeshipComplianceFailedReasons.None,
                complianceComments: null,
                stylePassed: false,
                styleFailedReasons: ApprenticeshipQAApprenticeshipStyleFailedReasons.JobRolesIncluded | ApprenticeshipQAApprenticeshipStyleFailedReasons.TermCourseUsed,
                styleComments: "Bad style, yo");

            await User.AsHelpdesk();

            // Act
            var response = await HttpClient.GetAsync($"apprenticeship-qa/apprenticeship-assessments/{apprenticeshipId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var doc = await response.GetDocument();
            //
            Assert.Equal("checked", doc.GetElementById("CompliancePassed").GetAttribute("checked"));
            Assert.Null(doc.GetElementById("StylePassed").GetAttribute("checked"));
            Assert.Equal("checked", doc.GetElementWithLabel("Job roles included").GetAttribute("checked"));
            Assert.Equal("checked", doc.GetElementWithLabel("Term 'course' used").GetAttribute("checked"));
            Assert.Equal("Bad style, yo", doc.GetElementById("StyleComments").TextContent);
        }

        [Fact]
        public async Task Post_MissingCompliancePassedRendersErrorMessage()
        {
            // Arrange
            var ukprn = 12345;

            var providerId = await TestData.CreateProvider(
                ukprn: ukprn,
                providerName: "Provider 1",
                apprenticeshipQAStatus: ApprenticeshipQAStatus.Submitted);

            var providerUserId = $"{ukprn}-user";
            await TestData.CreateUser(providerUserId, "somebody@provider1.com", "Provider 1", "Person");

            var apprenticeshipId = await TestData.CreateApprenticeship(ukprn);

            await TestData.CreateApprenticeshipQASubmission(
                providerId,
                submittedOn: Clock.UtcNow,
                submittedByUserId: providerUserId,
                providerMarketingInformation: "The overview",
                apprenticeshipIds: new[] { apprenticeshipId });

            await User.AsHelpdesk();

            var requestContent = new FormUrlEncodedContentBuilder()
                .With("StylePassed", false)
                .ToContent();

            // Act
            var response = await HttpClient.PostAsync(
                $"apprenticeship-qa/apprenticeship-assessments/{apprenticeshipId}",
                requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var doc = await response.GetDocument();
            doc.AssertHasError("CompliancePassed", "PLACEHOLDER");
        }
        
        [Fact]
        public async Task Post_MissingComplianceFailedReasonsWhenFailedRendersErrorMessage()
        {
            // Arrange
            var ukprn = 12345;

            var providerId = await TestData.CreateProvider(
                ukprn: ukprn,
                providerName: "Provider 1",
                apprenticeshipQAStatus: ApprenticeshipQAStatus.Submitted);

            var providerUserId = $"{ukprn}-user";
            await TestData.CreateUser(providerUserId, "somebody@provider1.com", "Provider 1", "Person");

            var apprenticeshipId = await TestData.CreateApprenticeship(ukprn);

            await TestData.CreateApprenticeshipQASubmission(
                providerId,
                submittedOn: Clock.UtcNow,
                submittedByUserId: providerUserId,
                providerMarketingInformation: "The overview",
                apprenticeshipIds: new[] { apprenticeshipId });

            await User.AsHelpdesk();

            var requestContent = new FormUrlEncodedContentBuilder()
                .With("CompliancePassed", false)
                .With("StylePassed", true)
                .ToContent();

            // Act
            var response = await HttpClient.PostAsync(
                $"apprenticeship-qa/apprenticeship-assessments/{apprenticeshipId}",
                requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var doc = await response.GetDocument();
            doc.AssertHasError("ComplianceFailedReasons", "PLACEHOLDER");
        }

        [Fact]
        public async Task Post_MissingStylePassedRendersErrorMessage()
        {
            // Arrange
            var ukprn = 12345;

            var providerId = await TestData.CreateProvider(
                ukprn: ukprn,
                providerName: "Provider 1",
                apprenticeshipQAStatus: ApprenticeshipQAStatus.Submitted);

            var providerUserId = $"{ukprn}-user";
            await TestData.CreateUser(providerUserId, "somebody@provider1.com", "Provider 1", "Person");

            var apprenticeshipId = await TestData.CreateApprenticeship(ukprn);

            await TestData.CreateApprenticeshipQASubmission(
                providerId,
                submittedOn: Clock.UtcNow,
                submittedByUserId: providerUserId,
                providerMarketingInformation: "The overview",
                apprenticeshipIds: new[] { apprenticeshipId });

            await User.AsHelpdesk();

            var requestContent = new FormUrlEncodedContentBuilder()
                .With("CompliancePassed", false)
                .ToContent();

            // Act
            var response = await HttpClient.PostAsync(
                $"apprenticeship-qa/apprenticeship-assessments/{apprenticeshipId}",
                requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var doc = await response.GetDocument();
            doc.AssertHasError("StylePassed", "PLACEHOLDER");
        }

        [Fact]
        public async Task Post_MissingStyleFailedReasonsWhenFailedRendersErrorMessage()
        {
            // Arrange
            var ukprn = 12345;

            var providerId = await TestData.CreateProvider(
                ukprn: ukprn,
                providerName: "Provider 1",
                apprenticeshipQAStatus: ApprenticeshipQAStatus.Submitted);

            var providerUserId = $"{ukprn}-user";
            await TestData.CreateUser(providerUserId, "somebody@provider1.com", "Provider 1", "Person");

            var apprenticeshipId = await TestData.CreateApprenticeship(ukprn);

            await TestData.CreateApprenticeshipQASubmission(
                providerId,
                submittedOn: Clock.UtcNow,
                submittedByUserId: providerUserId,
                providerMarketingInformation: "The overview",
                apprenticeshipIds: new[] { apprenticeshipId });

            await User.AsHelpdesk();

            var requestContent = new FormUrlEncodedContentBuilder()
                .With("CompliancePassed", true)
                .With("StylePassed", false)
                .ToContent();

            // Act
            var response = await HttpClient.PostAsync(
                $"apprenticeship-qa/apprenticeship-assessments/{apprenticeshipId}",
                requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var doc = await response.GetDocument();
            doc.AssertHasError("StyleFailedReasons", "PLACEHOLDER");
        }

        [Fact]
        public async Task Post_ComplianceAndStylePassedUpdatesSubmissionStatusAndRendersConfirmationPage()
        {
            // Arrange
            var ukprn = 12345;

            var providerId = await TestData.CreateProvider(
                ukprn: ukprn,
                providerName: "Provider 1",
                apprenticeshipQAStatus: ApprenticeshipQAStatus.Submitted);

            var providerUserId = $"{ukprn}-user";
            await TestData.CreateUser(providerUserId, "somebody@provider1.com", "Provider 1", "Person");

            var apprenticeshipId = await TestData.CreateApprenticeship(ukprn);

            var submissionId = await TestData.CreateApprenticeshipQASubmission(
                providerId,
                submittedOn: Clock.UtcNow,
                submittedByUserId: providerUserId,
                providerMarketingInformation: "The overview",
                apprenticeshipIds: new[] { apprenticeshipId });

            await User.AsHelpdesk();

            var requestContent = new FormUrlEncodedContentBuilder()
                .With("CompliancePassed", true)
                .With("StylePassed", true)
                .ToContent();

            // Act
            var response = await HttpClient.PostAsync(
                $"apprenticeship-qa/apprenticeship-assessments/{apprenticeshipId}",
                requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var doc = await response.GetDocument();
            Assert.Equal("QA Apprenticeship training course information - Course Directory", doc.Title);
            Assert.Equal("Pass", doc.GetElementById("pttcd-apprenticeship-qa-apprenticeship-submission-confirmation-compliance-status").TextContent.Trim());
            Assert.Equal("Pass", doc.GetElementById("pttcd-apprenticeship-qa-apprenticeship-submission-confirmation-style-status").TextContent.Trim());
            Assert.Equal(
                "This apprenticeship training course has PASSED quality assurance.",
                doc.GetElementById("pttcd-apprenticeship-qa-apprenticeship-submission-confirmation-overall-status-message").TextContent.Trim());

            var submissionStatus = await WithSqlQueryDispatcher(dispatcher => dispatcher.ExecuteQuery(
                new GetLatestApprenticeshipQASubmissionForProvider()
                {
                    ProviderId = providerId
                }));
            Assert.True(submissionStatus.AsT1.ApprenticeshipAssessmentsPassed.Value);
        }

        [Fact]
        public async Task Post_CompliancePassedAndStyleFailedUpdatesSubmissionStatusAndRendersConfirmationPage()
        {
            // Arrange
            var ukprn = 12345;

            var providerId = await TestData.CreateProvider(
                ukprn: ukprn,
                providerName: "Provider 1",
                apprenticeshipQAStatus: ApprenticeshipQAStatus.Submitted);

            var providerUserId = $"{ukprn}-user";
            await TestData.CreateUser(providerUserId, "somebody@provider1.com", "Provider 1", "Person");

            var apprenticeshipId = await TestData.CreateApprenticeship(ukprn);

            var submissionId = await TestData.CreateApprenticeshipQASubmission(
                providerId,
                submittedOn: Clock.UtcNow,
                submittedByUserId: providerUserId,
                providerMarketingInformation: "The overview",
                apprenticeshipIds: new[] { apprenticeshipId });

            await User.AsHelpdesk();

            var requestContent = new FormUrlEncodedContentBuilder()
                .With("CompliancePassed", true)
                .With("StylePassed", false)
                .With("StyleFailedReasons", ApprenticeshipQAProviderStyleFailedReasons.JobRolesIncluded)
                .With("StyleFailedReasons", ApprenticeshipQAProviderStyleFailedReasons.TermFrameworkUsed)
                .With("StyleComments", "Some feedback")
                .ToContent();

            // Act
            var response = await HttpClient.PostAsync(
                $"apprenticeship-qa/apprenticeship-assessments/{apprenticeshipId}",
                requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var doc = await response.GetDocument();
            Assert.Equal("QA Apprenticeship training course information - Course Directory", doc.Title);
            Assert.Equal("Pass", doc.GetElementById("pttcd-apprenticeship-qa-apprenticeship-submission-confirmation-compliance-status").TextContent.Trim());
            Assert.Equal("Fail", doc.GetElementById("pttcd-apprenticeship-qa-apprenticeship-submission-confirmation-style-status").TextContent.Trim());
            Assert.Equal(
                "This apprenticeship training course has FAILED quality assurance.",
                doc.GetElementById("pttcd-apprenticeship-qa-apprenticeship-submission-confirmation-overall-status-message").TextContent.Trim());

            var submissionStatus = await WithSqlQueryDispatcher(dispatcher => dispatcher.ExecuteQuery(
                new GetLatestApprenticeshipQASubmissionForProvider()
                {
                    ProviderId = providerId
                }));
            Assert.False(submissionStatus.AsT1.ApprenticeshipAssessmentsPassed.Value);
        }

        [Fact]
        public async Task Post_ComplianceFailedAndStylePassedUpdatesSubmissionStatusAndRendersConfirmationPage()
        {
            // Arrange
            var ukprn = 12345;

            var providerId = await TestData.CreateProvider(
                ukprn: ukprn,
                providerName: "Provider 1",
                apprenticeshipQAStatus: ApprenticeshipQAStatus.Submitted);

            var providerUserId = $"{ukprn}-user";
            await TestData.CreateUser(providerUserId, "somebody@provider1.com", "Provider 1", "Person");

            var apprenticeshipId = await TestData.CreateApprenticeship(ukprn);

            var submissionId = await TestData.CreateApprenticeshipQASubmission(
                providerId,
                submittedOn: Clock.UtcNow,
                submittedByUserId: providerUserId,
                providerMarketingInformation: "The overview",
                apprenticeshipIds: new[] { apprenticeshipId });

            await User.AsHelpdesk();

            var requestContent = new FormUrlEncodedContentBuilder()
                .With("CompliancePassed", false)
                .With("ComplianceFailedReasons", ApprenticeshipQAProviderComplianceFailedReasons.IncorrectOfsetGradeUsed)
                .With("ComplianceFailedReasons", ApprenticeshipQAProviderComplianceFailedReasons.UnverifiableClaim)
                .With("ComplianceComments", "Some feedback")
                .With("StylePassed", true)
                .ToContent();

            // Act
            var response = await HttpClient.PostAsync(
                $"apprenticeship-qa/apprenticeship-assessments/{apprenticeshipId}",
                requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var doc = await response.GetDocument();
            Assert.Equal("QA Apprenticeship training course information - Course Directory", doc.Title);
            Assert.Equal("Fail", doc.GetElementById("pttcd-apprenticeship-qa-apprenticeship-submission-confirmation-compliance-status").TextContent.Trim());
            Assert.Equal("Pass", doc.GetElementById("pttcd-apprenticeship-qa-apprenticeship-submission-confirmation-style-status").TextContent.Trim());
            Assert.Equal(
                "This apprenticeship training course has FAILED quality assurance.",
                doc.GetElementById("pttcd-apprenticeship-qa-apprenticeship-submission-confirmation-overall-status-message").TextContent.Trim());

            var submissionStatus = await WithSqlQueryDispatcher(dispatcher => dispatcher.ExecuteQuery(
                new GetLatestApprenticeshipQASubmissionForProvider()
                {
                    ProviderId = providerId
                }));
            Assert.False(submissionStatus.AsT1.ApprenticeshipAssessmentsPassed.Value);
        }

        [Fact]
        public async Task Post_ComplianceAndStyleFailedUpdatesSubmissionStatusAndRendersConfirmationPage()
        {
            // Arrange
            var ukprn = 12345;

            var providerId = await TestData.CreateProvider(
                ukprn: ukprn,
                providerName: "Provider 1",
                apprenticeshipQAStatus: ApprenticeshipQAStatus.Submitted);

            var providerUserId = $"{ukprn}-user";
            await TestData.CreateUser(providerUserId, "somebody@provider1.com", "Provider 1", "Person");

            var apprenticeshipId = await TestData.CreateApprenticeship(ukprn);

            var submissionId = await TestData.CreateApprenticeshipQASubmission(
                providerId,
                submittedOn: Clock.UtcNow,
                submittedByUserId: providerUserId,
                providerMarketingInformation: "The overview",
                apprenticeshipIds: new[] { apprenticeshipId });

            await User.AsHelpdesk();

            var requestContent = new FormUrlEncodedContentBuilder()
                .With("CompliancePassed", false)
                .With("ComplianceFailedReasons", ApprenticeshipQAProviderComplianceFailedReasons.IncorrectOfsetGradeUsed)
                .With("ComplianceFailedReasons", ApprenticeshipQAProviderComplianceFailedReasons.UnverifiableClaim)
                .With("ComplianceComments", "Some compliance feedback")
                .With("StylePassed", false)
                .With("StyleFailedReasons", ApprenticeshipQAProviderStyleFailedReasons.JobRolesIncluded)
                .With("StyleFailedReasons", ApprenticeshipQAProviderStyleFailedReasons.TermFrameworkUsed)
                .With("StyleComments", "Some style feedback")
                .ToContent();

            // Act
            var response = await HttpClient.PostAsync(
                $"apprenticeship-qa/apprenticeship-assessments/{apprenticeshipId}",
                requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var doc = await response.GetDocument();
            Assert.Equal("QA Apprenticeship training course information - Course Directory", doc.Title);
            Assert.Equal("Fail", doc.GetElementById("pttcd-apprenticeship-qa-apprenticeship-submission-confirmation-compliance-status").TextContent.Trim());
            Assert.Equal("Fail", doc.GetElementById("pttcd-apprenticeship-qa-apprenticeship-submission-confirmation-style-status").TextContent.Trim());
            Assert.Equal(
                "This apprenticeship training course has FAILED quality assurance.",
                doc.GetElementById("pttcd-apprenticeship-qa-apprenticeship-submission-confirmation-overall-status-message").TextContent.Trim());

            var submissionStatus = await WithSqlQueryDispatcher(dispatcher => dispatcher.ExecuteQuery(
                new GetLatestApprenticeshipQASubmissionForProvider()
                {
                    ProviderId = providerId
                }));
            Assert.False(submissionStatus.AsT1.ApprenticeshipAssessmentsPassed.Value);
        }
    }
}
