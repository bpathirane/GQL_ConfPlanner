using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;

using HotChocolate.Resolvers;

using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Types
{
    public class SessionType : ObjectType<Session>
    {
        protected override void Configure(IObjectTypeDescriptor<Session> descriptor)
        {
            descriptor
                .ImplementsNode()
                .IdField(n => n.Id)
                .ResolveNode((ctx, id) => ctx.DataLoader<SessionByIdDataLoader>().LoadAsync(id, ctx.RequestAborted));

            descriptor
                .Field(t => t.SessionSpeakers)
                .ResolveWith<SessionResolvers>(t => t.GetSpeakersAsync(default!, default!))
                .UseDbContext<ApplicationDbContext>()
                .Name("speakers");

            descriptor
                .Field(t => t.SessionAttendees)
                .ResolveWith<SessionResolvers>(t => t.GetAttendeesAsync(default!, default!))
                .UseDbContext<ApplicationDbContext>()
                .Name("attendees");

            descriptor
                .Field(t => t.Track)
                .ResolveWith<SessionResolvers>(t => t.GetTrackAsync(default!, default!));

            descriptor
                .Field(t => t.TrackId)
                .ID(nameof(Track));
        }

        private class SessionResolvers
        {
            private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
            private readonly ILogger _logger;

            public SessionResolvers(IDbContextFactory<ApplicationDbContext> dbContextFactory,
                ILogger<SessionResolvers> logger)
            {
                _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
                _logger = logger;
            }

            internal async Task<IEnumerable<Attendee>> GetAttendeesAsync(IResolverContext context, AttendeeByIdDataLoader dataLoader)
            {
                var session = context.Parent<Session>();
                await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

                int[] attendeeIds = await dbContext.Sessions
                    .Where(s => s.Id == session.Id)
                    .Include(s => s.SessionAttendees)
                    .SelectMany(s => s.SessionAttendees.Select(a => a.AttendeeId))
                    .ToArrayAsync(context.RequestAborted);

                return await dataLoader.LoadAsync(attendeeIds, context.RequestAborted);
            }

            internal async Task<IEnumerable<Speaker>> GetSpeakersAsync(IResolverContext context, SpeakerByIdDataLoader dataLoader)
            {
                var session = context.Parent<Session>();
                await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

                int[] speakerIds = await dbContext.Sessions
                    .Where(s => s.Id == session.Id)
                    .Include(s => s.SessionSpeakers)
                    .SelectMany(s => s.SessionSpeakers.Select(s => s.SpeakerId))
                    .ToArrayAsync(context.RequestAborted);

                return await dataLoader.LoadAsync(speakerIds, context.RequestAborted);
            }

            internal async Task<Track?> GetTrackAsync(IResolverContext context, TrackByIdDataLoader dataLoader)
            {
                var session = context.Parent<Session>();

                return session.TrackId is null ? null : await dataLoader.LoadAsync(session.TrackId.Value, context.RequestAborted);
            }
        }
    }
}