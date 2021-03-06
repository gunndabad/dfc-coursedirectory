﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Dfc.CourseDirectory.Core.DataStore.Sql;
using Dfc.CourseDirectory.Core.DataStore.Sql.Queries;
using Dfc.CourseDirectory.Core.Models;
using Dfc.CourseDirectory.Core.Validation;
using Dfc.CourseDirectory.WebV2.Behaviors;
using Dfc.CourseDirectory.WebV2.Security;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Dfc.CourseDirectory.WebV2.Features.HidePassedNotification
{
    using CommandResponse = OneOf<ModelWithErrors<CommandViewModel>, Success>;

    public class Command : IRequest<CommandResponse>
    {
        public Guid ProviderId { get; set; }
    }

    public class CommandViewModel : Command
    {
    }

    public class CommandHandler :
        IRequestHandler<Command, CommandResponse>,
        IRestrictProviderType<Command>
    {
        private readonly ISqlQueryDispatcher _sqlQueryDispatcher;
        private readonly ICurrentUserProvider _currentUserProvider;

        public CommandHandler(
            ISqlQueryDispatcher sqlQueryDispatcher,
            ICurrentUserProvider currentUserProvider)
        {
            _sqlQueryDispatcher = sqlQueryDispatcher;
            _currentUserProvider = currentUserProvider;
        }

        ProviderType IRestrictProviderType<Command>.ProviderType => ProviderType.Apprenticeships;

        public async Task<CommandResponse> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var currentUser = _currentUserProvider.GetCurrentUser();
            if (currentUser.IsHelpdesk)
            {
                throw new NotAuthorizedException();
            }

            var maybeSubmission = await _sqlQueryDispatcher.ExecuteQuery(
                new GetLatestApprenticeshipQASubmissionForProvider()
                {
                    ProviderId = request.ProviderId
                });

            var submission = maybeSubmission.Match(
                notfound => throw new InvalidStateException(InvalidStateReason.InvalidApprenticeshipQASubmission),
                found => found);

            // Cannot hide passed notification if qa status is not passed
            if (submission.Passed != true)
            {
                throw new InvalidStateException(InvalidStateReason.InvalidApprenticeshipQAStatus);
            }

            // Cannot hide notification if it has already been hidden
            if (submission.HidePassedNotification)
            {
                throw new InvalidStateException();
            }

            // Hide notification
            var hideDialog = await _sqlQueryDispatcher.ExecuteQuery(
                new UpdateHidePassedNotification()
                {
                    ApprenticeshipQASubmissionId = submission.ApprenticeshipQASubmissionId,
                    HidePassedNotification = true
                });

            return hideDialog.Match(
                notfound => throw new InvalidStateException(InvalidStateReason.InvalidApprenticeshipQASubmission),
                success => success);
        }

        Guid IRestrictProviderType<Command>.GetProviderId(Command request) => request.ProviderId;
    }
}
