using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Edit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Activity Activity { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.FindAsync(request.Activity.Id);

                //Return a null result when the activity is not found
                if (activity == null) return null;

                _mapper.Map(request.Activity, activity);

                //If nothing is written to DB then result will be false
                //If number of changes are greater than 0 then true
                var result = await _context.SaveChangesAsync() > 0;

                await _context.SaveChangesAsync();

                if (!result) return Result<Unit>.Failure("Failed to update activity");

                //If successful notify ActivitiesApiController the request was successful
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}