using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;

using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Sessions
{

    [ExtendObjectType("Query")]
    public class SessionQueries
    {
        public async Task<IEnumerable<Session>> GetSessionsAsync(
            ApplicationDbContext context,
            CancellationToken cancellationToken)
                => await context.Sessions.ToListAsync(cancellationToken);

        public Task<Session> GetSessionByIdAsync(
            [ID(nameof(Session))] int id,
            SessionByIdDataLoader dataLoader,
            CancellationToken cancellationToken
        ) => dataLoader.LoadAsync(id, cancellationToken);

        public async Task<IEnumerable<Session>> GetSessionsByIdAsync(
            [ID(nameof(Session))] int[] ids,
            SessionByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
                => await dataLoader.LoadAsync(ids, cancellationToken);
    }
}