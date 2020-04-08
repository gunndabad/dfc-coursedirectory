﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfc.CourseDirectory.WebV2.Behaviors;
using Dfc.CourseDirectory.WebV2.Behaviors.Errors;
using Dfc.CourseDirectory.WebV2.Models;
using Dfc.CourseDirectory.WebV2.MultiPageTransaction;
using Dfc.CourseDirectory.WebV2.Validation;
using FluentValidation;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Dfc.CourseDirectory.WebV2.Features.NewApprenticeshipProvider.ApprenticeshipEmployerLocationsRegions
{
    using CommandResponse = OneOf<ModelWithErrors<Command>, Success>;

    public class Query : IRequest<Command>, IProviderScopedRequest
    {
        public Guid ProviderId { get; set; }
    }

    public class Command : IRequest<CommandResponse>, IProviderScopedRequest
    {
        public Guid ProviderId { get; set; }
        public IReadOnlyCollection<string> RegionIds { get; set; }
    }

    public class Handler :
        IRequestHandler<Query, Command>,
        IRequireUserCanSubmitQASubmission<Query>,
        IRequestHandler<Command, CommandResponse>,
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
                ProviderId = request.ProviderId,
                RegionIds = _flow.State.ApprenticeshipLocationRegionIds
            };
            return Task.FromResult(command);
        }

        public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
        {
            ValidateFlowState();

            var validator = new CommandValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                request.RegionIds ??= new List<string>();
                return new ModelWithErrors<Command>(request, validationResult);
            }

            _flow.Update(s => s.SetApprenticeshipLocationRegionIds(request.RegionIds));

            return new Success();
        }

        private void ValidateFlowState()
        {
            if (_flow.State.ApprenticeshipLocationType != ApprenticeshipLocationType.EmployerBased ||
                _flow.State.ApprenticeshipIsNational != false)
            {
                throw new ErrorException<InvalidFlowState>(new InvalidFlowState());
            }
        }

        private class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.RegionIds)
                    .Transform(v =>
                    {
                        if (v == null)
                        {
                            return v;
                        }

                        // Remove any IDs that are not regions or sub-regions
                        var subRegionIds = Region.All.SelectMany(r => r.SubRegions).Select(sr => sr.Id);
                        return v.Intersect(subRegionIds).ToList();
                    })
                    .NotEmpty()
                    .WithMessage("Select at least one sub-region");
            }
        }
    }
}