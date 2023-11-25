using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;

using HotChocolate;
using HotChocolate.Execution.Configuration;
using HotChocolate.Resolvers;
using HotChocolate.Types;

using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Types
{
    public class SpeakerType : ObjectType<Speaker>
    {
        protected override void Configure(IObjectTypeDescriptor<Speaker> descriptor)
        {
            descriptor
                .Field(t => t.SessionSpeakers)
                .ResolveWith<SpeakerResolvers>(t => t.GetSessionsAsync(default!, default!, default))
                .UseDbContext<ApplicationDbContext>()
                .Name("sessions");
        }

        internal class SpeakerResolvers
        {
            private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
            private readonly ILogger _logger;
            public SpeakerResolvers(
                IDbContextFactory<ApplicationDbContext> dbContextFactory,
                ILogger<SpeakerResolvers> logger)
            {
                _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
                _logger = logger;
            }
            public async Task<IEnumerable<Session>> GetSessionsAsync(IResolverContext context,
            SessionByIdDataLoader sessionById,
            CancellationToken cancellationToken)
            {
                await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
                var speaker = context.Parent<Speaker>();

                _logger.LogInformation("Resolving sessions for speaker [{0}]", speaker);
                int[] sessionIds = await dbContext.Speakers
                    .Where(s => s.Id == speaker.Id)
                    .Include(s => s.SessionSpeakers)
                    .SelectMany(s => s.SessionSpeakers.Select(t => t.SessionId))
                    .ToArrayAsync(cancellationToken);

                return await sessionById.LoadAsync(sessionIds, cancellationToken);
            }
        }
    }
}