using ConferencePlanner.GraphQL.Data;

using HotChocolate;

using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Speakers
{
    [ExtendObjectType("Mutation")]
    public class SpeakerMutations
    {
        public async Task<AddSpeakerPayload> AddSpeakerAsync(AddSpeakerInput input, [Service] IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();

            var speaker = new Speaker
            {
                Name = input.Name,
                Bio = input.Bio,
                WebSite = input.WebSite
            };

            context.Speakers.Add(speaker);
            await context.SaveChangesAsync();

            return new AddSpeakerPayload(speaker);
        }
    }
}