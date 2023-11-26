using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;

using HotChocolate.Resolvers;

using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Types
{
    public class TrackType : ObjectType<Track>
    {
        protected override void Configure(IObjectTypeDescriptor<Track> descriptor)
        {
            descriptor
                .ImplementsNode()
                .IdField(t => t.Id)
                .ResolveNode((ctx, id) =>
                    ctx.DataLoader<TrackByIdDataLoader>().LoadAsync(id, ctx.RequestAborted));

            descriptor
                .Field(t => t.Sessions)
                .ResolveWith<TrackResolvers>(t => t.GetSessionsAsync(default!, default!))
                .UseDbContext<ApplicationDbContext>()
                .Name("sessions");
        }

        private class TrackResolvers
        {
            private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
            private readonly ILogger _logger;
            public TrackResolvers(
                IDbContextFactory<ApplicationDbContext> dbContextFactory,
                ILogger<TrackResolvers> logger)
            {
                _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
                _logger = logger;
            }

            public async Task<IEnumerable<Session>> GetSessionsAsync(IResolverContext context, SessionByIdDataLoader dataLoader)
            {
                await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
                var track = context.Parent<Track>();

                int[] sessionIds = await dbContext.Sessions
                    .Where(t => t.TrackId == track.Id)
                    .Select(t => t.TrackId!.Value)
                    .ToArrayAsync(context.RequestAborted);

                return await dataLoader.LoadAsync(sessionIds, context.RequestAborted);
            }
        }
    }
}