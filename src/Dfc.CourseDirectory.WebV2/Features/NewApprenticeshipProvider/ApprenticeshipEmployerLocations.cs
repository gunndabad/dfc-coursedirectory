﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Dfc.CourseDirectory.WebV2.Behaviors;
using Dfc.CourseDirectory.WebV2.Behaviors.Errors;
using Dfc.CourseDirectory.Core.Models;
using Dfc.CourseDirectory.WebV2.MultiPageTransaction;
using Dfc.CourseDirectory.Core.Validation;
using FluentValidation;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Dfc.CourseDirectory.WebV2.Features.NewApprenticeshipProvider.ApprenticeshipEmployerLocations
{
    public class Query : IRequest<Command>, IProviderScopedRequest
    {
        public Guid ProviderId { get; set; }
    }

    public class Command : IRequest<OneOf<ModelWithErrors<Command>, Success>>, IProviderScopedRequest
    {
        public Guid ProviderId { get; set; }
        public bool? National { get; set; }
    }

    public class Handler :
        IRequestHandler<Query, Command>,
        IRequireUserCanSubmitQASubmission<Query>,
        IRequestHandler<Command, OneOf<ModelWithErrors<Command>, Success>>,
        IRequireUserCanSubmitQASubmission<Command>
    {
        private readonly MptxInstanceContext<FlowModel> _flow;

        public Handler(MptxInstanceContext<FlowModel> flow)
        {
            _flow = flow;
        }

        public Task<Command> Handle(Query request, CancellationToken cancellationToken)
        {
            ValidateFlowState();

            var command = new Command()
            {
                National = _flow.State.ApprenticeshipIsNational,
                ProviderId = request.ProviderId
            };
            return Task.FromResult(command);
        }

        public async Task<OneOf<ModelWithErrors<Command>, Success>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            ValidateFlowState();

            var validator = new CommandValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return new ModelWithErrors<Command>(request, validationResult);
            }

            _flow.Update(s => s.SetApprenticeshipIsNational(request.National.Value));

            return new Success();
        }

        private void ValidateFlowState()
        {
            var locationType = _flow.State.ApprenticeshipLocationType;

            if (locationType != ApprenticeshipLocationType.EmployerBased &&
                locationType != ApprenticeshipLocationType.ClassroomBasedAndEmployerBased)
            {
                throw new ErrorException<InvalidFlowState>(new InvalidFlowState());
            }
        }

        private class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.National)
                    .NotNull()
                    .WithMessage("Enter whether you can deliver this training at employers’ locations anywhere in England");
            }
        }
    }
}
