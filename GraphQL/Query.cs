using System.Linq;

using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;

using HotChocolate;

namespace ConferencePlanner.GraphQL
{
    public class Query
    {
        public IQueryable<Speaker> GetSpeakers(ApplicationDbContext context) =>
            context.Speakers;

        public Task<Speaker> GetSpeakerAsync(int id, SpeakerByIdDataLoader dataLoader, CancellationToken cancellationToken) =>
        dataLoader.LoadAsync(id, cancellationToken);
    }
}