using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;

namespace ConferencePlanner.GraphQL
{
    [ExtendObjectType("Query")]
    public class SpeakerQueries
    {
        public IQueryable<Speaker> GetSpeakers(ApplicationDbContext context) =>
            context.Speakers;

        public Task<Speaker> GetSpeakerAsync([ID(nameof(Speaker))] int id,
        SpeakerByIdDataLoader dataLoader,
        CancellationToken cancellationToken) =>
        dataLoader.LoadAsync(id, cancellationToken);
    }
}