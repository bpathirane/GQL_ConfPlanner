using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;

using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Types
{
    public class AttendeeType : ObjectType<Attendee>
    {
        protected override void Configure(IObjectTypeDescriptor<Attendee> descriptor)
        {
            descriptor
                .ImplementsNode()
                .IdField(t => t.Id)
                .ResolveNode((ctx, id) => ctx.DataLoader<AttendeeByIdDataLoader>().LoadAsync(id, ctx.RequestAborted));

            descriptor
                .Field(t => t.SessionsAttendees)
                .ResolveWith<AttendeeResolvers>(t => t.GetSessionsAsync(default!, default!, default))
                .UseDbContext<ApplicationDbContext>()
                .Name("sessions");
        }

        private class AttendeeResolvers
        {
            private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
            private readonly ILogger _logger;

            public AttendeeResolvers(
                IDbContextFactory<ApplicationDbContext> dbContextFactory,
                ILogger<AttendeeResolvers> logger
            )
            {
                _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
                _logger = logger;
            }

            public async Task<IEnumerable<Session>> GetSessionsAsync(
                Attendee attendee,
                SessionByIdDataLoader sessionById,
                CancellationToken cancellationToken
            )
            {
                await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

                int[] speakerIds = await dbContext.Attendees
                    .Where(a => a.Id == attendee.Id)
                    .Include(a => a.SessionsAttendees)
                    .SelectMany(a => a.SessionsAttendees.Select(s => s.SessionId))
                    .ToArrayAsync(cancellationToken);

                return await sessionById.LoadAsync(speakerIds, cancellationToken);
            }
        }
    }
}