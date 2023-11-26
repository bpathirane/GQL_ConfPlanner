using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;

using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Tracks
{
    [ExtendObjectType("Mutation")]
    public class TrackMutations
    {
        public async Task<AddTrackPayload> AddTrackAsync(
            AddTrackInput input,
            [Service] IDbContextFactory<ApplicationDbContext> dbContextFactory,
            CancellationToken cancellationToken
        )
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();

            var track = new Track { Name = input.Name };
            context.Tracks.Add(track);

            await context.SaveChangesAsync(cancellationToken);

            return new AddTrackPayload(track);
        }

        public async Task<RenameTrackPayload> RenameTrackAsync(
            RenameTrackInput input,
            [Service] IDbContextFactory<ApplicationDbContext> dbContextFactory,
            CancellationToken cancellationToken
        )
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();

            var track = await context.Tracks.FindAsync(input!.Id, cancellationToken);
            if (track is null)
            {
                return new RenameTrackPayload(
                    new[] { new UserError("Track not found", "TRACK_NOT_FOUND") }
                );
            }
            track.Name = input.Name;

            await context.SaveChangesAsync(cancellationToken);

            return new RenameTrackPayload(track);
        }
    }
}