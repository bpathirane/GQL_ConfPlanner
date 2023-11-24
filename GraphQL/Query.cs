using System.Linq;

using ConferencePlanner.GraphQL.Data;

using HotChocolate;

namespace ConferencePlanner.GraphQL
{
    public class Query
    {
        public IQueryable<Speaker> GetSpeakers(ApplicationDbContext context) =>
            context.Speakers;
    }
}