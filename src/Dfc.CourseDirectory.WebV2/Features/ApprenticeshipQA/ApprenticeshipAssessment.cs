﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfc.CourseDirectory.WebV2.DataStore.CosmosDb;
using Dfc.CourseDirectory.WebV2.DataStore.CosmosDb.Queries;
using Dfc.CourseDirectory.WebV2.DataStore.Sql;
using Dfc.CourseDirectory.WebV2.DataStore.Sql.Queries;
using Dfc.CourseDirectory.WebV2.Models;
using Dfc.CourseDirectory.WebV2.Security;
using Dfc.CourseDirectory.WebV2.Validation;
using FluentValidation;
using Mapster;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Dfc.CourseDirectory.WebV2.Features.ApprenticeshipQA.ApprenticeshipAssessment
{
    public enum ErrorReason
    {
        ApprenticeshipDoesNotExist,
        NoValidSubmission,
    }

    public class Query : IRequest<OneOf<Error<ErrorReason>, ViewModel>>
    {
        public Guid ApprenticeshipId { get; set; }
    }

    public class ViewModel : Command
    {
        public Guid ProviderId { get; set; }
        public string ApprenticeshipTitle { get; set; }
        public string ApprenticeshipMarketingInformation { get; set; }
        public bool IsReadOnly { get; set; }
    }

    public class Command : IRequest<OneOf<Error<ErrorReason>, ModelWithErrors<ViewModel>, ConfirmationViewModel>>
    {
        public Guid ApprenticeshipId { get; set; }
        public bool? CompliancePassed { get; set; }
        public ApprenticeshipQAApprenticeshipComplianceFailedReasons? ComplianceFailedReasons { get; set; }
        public string ComplianceComments { get; set; }
        public bool? StylePassed { get; set; }
        public ApprenticeshipQAApprenticeshipStyleFailedReasons? StyleFailedReasons { get; set; }
        public string StyleComments { get; set; }
    }

    public class ConfirmationViewModel
    {
        public Guid ApprenticeshipId { get; set; }
        public Guid ProviderId { get; set; }
        public string ApprenticeshipTitle { get; set; }
        public bool CompliancePassed { get; set; }
        public ApprenticeshipQAApprenticeshipComplianceFailedReasons ComplianceFailedReasons { get; set; }
        public string ComplianceComments { get; set; }
        public bool StylePassed { get; set; }
        public ApprenticeshipQAApprenticeshipStyleFailedReasons StyleFailedReasons { get; set; }
        public string StyleComments { get; set; }
        public bool Passed { get; set; }
    }

    public class Handler :
        IRequestHandler<Query, OneOf<Error<ErrorReason>, ViewModel>>,
        IRequestHandler<Command, OneOf<Error<ErrorReason>, ModelWithErrors<ViewModel>, ConfirmationViewModel>>
    {
        private readonly ISqlQueryDispatcher _sqlQueryDispatcher;
        private readonly ICosmosDbQueryDispatcher _cosmosDbQueryDispatcher;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IClock _clock;

        public Handler(
            ISqlQueryDispatcher sqlQueryDispatcher,
            ICosmosDbQueryDispatcher cosmosDbQueryDispatcher,
            ICurrentUserProvider currentUserProvider,
            IClock clock)
        {
            _sqlQueryDispatcher = sqlQueryDispatcher;
            _cosmosDbQueryDispatcher = cosmosDbQueryDispatcher;
            _currentUserProvider = currentUserProvider;
            _clock = clock;
        }

        public async Task<OneOf<Error<ErrorReason>, ViewModel>> Handle(Query request, CancellationToken cancellationToken)
        {
            var errorOrData = await CheckStatus(request.ApprenticeshipId);

            if (errorOrData.Value is Error<ErrorReason>)
            {
                return errorOrData.AsT0;
            }
            else
            {
                return CreateViewModel(errorOrData.AsT1);
            }
        }

        public async Task<OneOf<Error<ErrorReason>, ModelWithErrors<ViewModel>, ConfirmationViewModel>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var errorOrData = await CheckStatus(request.ApprenticeshipId);

            if (errorOrData.Value is Error<ErrorReason>)
            {
                return errorOrData.AsT0;
            }

            var data = errorOrData.AsT1;

            if (data.QAStatus != ApprenticeshipQAStatus.Submitted && data.QAStatus != ApprenticeshipQAStatus.InProgress)
            {
                return new Error<ErrorReason>(ErrorReason.NoValidSubmission);
            }

            var validator = new CommandValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var vm = CreateViewModel(data);
                request.Adapt(vm);

                return new ModelWithErrors<ViewModel>(vm, validationResult);
            }

            // Sanitize request
            if (request.CompliancePassed == true)
            {
                request.ComplianceFailedReasons = ApprenticeshipQAApprenticeshipComplianceFailedReasons.None;
                request.ComplianceComments = null;
            }
            if (request.StylePassed == true)
            {
                request.StyleFailedReasons = ApprenticeshipQAApprenticeshipStyleFailedReasons.None;
                request.StyleComments = null;
            }

            var maybeProviderId = await GetProviderIdForApprenticeship(request.ApprenticeshipId);
            var providerId = maybeProviderId.AsT1;

            var currentUserId = _currentUserProvider.GetCurrentUser().UserId;

            var passed = IsQAPassed(request.CompliancePassed.Value, request.StylePassed.Value);

            var overallPassed = data.LatestSubmission.ProviderAssessmentPassed.HasValue ?
                data.LatestSubmission.ProviderAssessmentPassed.Value && passed :
                (bool?)null;

            await _sqlQueryDispatcher.ExecuteQuery(
                new SetProviderApprenticeshipQAStatus()
                {
                    ProviderId = providerId,
                    ApprenticeshipQAStatus = ApprenticeshipQAStatus.InProgress
                });

            await _sqlQueryDispatcher.ExecuteQuery(
                new CreateApprenticeshipQAApprenticeshipAssessment()
                {
                    ApprenticeshipQASubmissionId = data.LatestSubmission.ApprenticeshipQASubmissionId,
                    ApprenticeshipId = request.ApprenticeshipId,
                    AssessedByUserId = currentUserId,
                    AssessedOn = _clock.UtcNow,
                    ComplianceComments = request.ComplianceComments,
                    ComplianceFailedReasons = request.ComplianceFailedReasons ?? ApprenticeshipQAApprenticeshipComplianceFailedReasons.None,
                    CompliancePassed = request.CompliancePassed.Value,
                    Passed = passed,
                    StyleComments = request.StyleComments,
                    StyleFailedReasons = request.StyleFailedReasons ?? ApprenticeshipQAApprenticeshipStyleFailedReasons.None,
                    StylePassed = request.StylePassed
                });

            await _sqlQueryDispatcher.ExecuteQuery(
                new UpdateApprenticeshipQASubmission()
                {
                    ApprenticeshipQASubmissionId = data.LatestSubmission.ApprenticeshipQASubmissionId,
                    Passed = overallPassed,
                    LastAssessedByUserId = currentUserId,
                    LastAssessedOn = _clock.UtcNow,
                    ProviderAssessmentPassed = data.LatestSubmission.ProviderAssessmentPassed,
                    ApprenticeshipAssessmentsPassed = passed
                });

            var confirmVm = CreateViewModel(data).Adapt<ConfirmationViewModel>();
            request.Adapt(confirmVm);
            confirmVm.Passed = IsQAPassed(request.CompliancePassed.Value, request.StylePassed.Value);

            return confirmVm;
        }

        private async Task<OneOf<Error<ErrorReason>, Data>> CheckStatus(Guid apprenticeshipId)
        {
            var maybeProviderId = await GetProviderIdForApprenticeship(apprenticeshipId);

            if (maybeProviderId.Value is NotFound)
            {
                return new Error<ErrorReason>(ErrorReason.ApprenticeshipDoesNotExist);
            }

            var providerId = maybeProviderId.AsT1;

            var qaStatus = await _sqlQueryDispatcher.ExecuteQuery(
                new GetProviderApprenticeshipQAStatus()
                {
                    ProviderId = providerId
                });

            if (qaStatus == ApprenticeshipQAStatus.NotStarted)
            {
                return new Error<ErrorReason>(ErrorReason.NoValidSubmission);
            }

            var maybeLatestSubmission = await _sqlQueryDispatcher.ExecuteQuery(
                new GetLatestApprenticeshipQASubmissionForProvider()
                {
                    ProviderId = providerId
                });

            if (maybeLatestSubmission.Value is None)
            {
                return new Error<ErrorReason>(ErrorReason.NoValidSubmission);
            }

            var latestSubmission = maybeLatestSubmission.AsT1;

            var submissionApprenticeship = latestSubmission.Apprenticeships
                .SingleOrDefault(a => a.ApprenticeshipId == apprenticeshipId);

            if (submissionApprenticeship == null)
            {
                return new Error<ErrorReason>(ErrorReason.NoValidSubmission);
            }

            var assessment = await _sqlQueryDispatcher.ExecuteQuery(
                new GetLatestApprenticeshipQAApprenticeshipAssessmentForSubmission()
                {
                    ApprenticeshipQASubmissionId = latestSubmission.ApprenticeshipQASubmissionId,
                    ApprenticeshipId = apprenticeshipId
                });

            return new Data()
            {
                QAStatus = qaStatus,
                LatestSubmission = latestSubmission,
                SubmissionApprenticeship = submissionApprenticeship,
                LatestAssessment = assessment,
            };
        }

        private ViewModel CreateViewModel(Data data) => new ViewModel()
        {
            ApprenticeshipId = data.ApprenticeshipId,
            ApprenticeshipMarketingInformation = data.SubmissionApprenticeship.ApprenticeshipMarketingInformation,
            ApprenticeshipTitle = data.SubmissionApprenticeship.ApprenticeshipTitle,
            ProviderId = data.ProviderId,
            ComplianceComments = data.LatestAssessment.Match(_ => string.Empty, v => v.ComplianceComments),
            ComplianceFailedReasons = data.LatestAssessment.Match(_ => ApprenticeshipQAApprenticeshipComplianceFailedReasons.None, v => v.ComplianceFailedReasons),
            CompliancePassed = data.LatestAssessment.Match(_ => null, v => v.CompliancePassed),
            StyleComments = data.LatestAssessment.Match(_ => string.Empty, v => v.StyleComments),
            StyleFailedReasons = data.LatestAssessment.Match(_ => ApprenticeshipQAApprenticeshipStyleFailedReasons.None, v => v.StyleFailedReasons),
            StylePassed = data.LatestAssessment.Match(_ => null, v => v.StylePassed),
            IsReadOnly = !(data.QAStatus == ApprenticeshipQAStatus.Submitted || data.QAStatus == ApprenticeshipQAStatus.InProgress)
        };

        private async Task<OneOf<NotFound, Guid>> GetProviderIdForApprenticeship(Guid apprenticeshipId)
        {
            var providerUkprn = await _cosmosDbQueryDispatcher.ExecuteQuery(
                new GetProviderUkprnForApprenticeship()
                {
                    ApprenticeshipId = apprenticeshipId
                });

            if (providerUkprn.Value is NotFound)
            {
                return new NotFound();
            }

            var provider = await _cosmosDbQueryDispatcher.ExecuteQuery(
                new GetProviderByUkprn()
                {
                    Ukprn = providerUkprn.AsT1
                });
            var providerId = provider.Id;

            return providerId;
        }

        private static bool IsQAPassed(bool compliancePassed, bool stylePassed) =>
            compliancePassed && stylePassed;

        private class Data
        {
            public Guid ApprenticeshipId { get; set; }
            public Guid ProviderId { get; set; }
            public ApprenticeshipQAStatus QAStatus { get; set; }
            public ApprenticeshipQASubmission LatestSubmission { get; set; }
            public ApprenticeshipQASubmissionApprenticeship SubmissionApprenticeship { get; set; }
            public OneOf<None, ApprenticeshipQAApprenticeshipAssessment> LatestAssessment { get; set; }
        }

        private class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.CompliancePassed)
                    .NotNull()
                    .WithMessageForAllRules("An outcome must be selected");

                RuleFor(c => c.ComplianceFailedReasons)
                    .NotNull()
                    .NotEqual(ApprenticeshipQAApprenticeshipComplianceFailedReasons.None)
                    .When(c => c.CompliancePassed == false)
                    .WithMessageForAllRules("A reason must be selected");

                RuleFor(c => c.ComplianceComments)
                    .NotEmpty()
                    .When(c => c.CompliancePassed == false)
                    .WithMessageForAllRules("Enter comments for the reason selected");

                RuleFor(c => c.StylePassed)
                    .NotNull()
                    .WithMessageForAllRules("An outcome must be selected");

                RuleFor(c => c.StyleFailedReasons)
                    .NotNull()
                    .NotEqual(ApprenticeshipQAApprenticeshipStyleFailedReasons.None)
                    .When(c => c.StylePassed == false)
                    .WithMessageForAllRules("A reason must be selected");

                RuleFor(c => c.StyleComments)
                    .NotEmpty()
                    .When(c => c.StylePassed == false)
                    .WithMessageForAllRules("Enter comments for the reason selected");
            }
        }
    }
}
