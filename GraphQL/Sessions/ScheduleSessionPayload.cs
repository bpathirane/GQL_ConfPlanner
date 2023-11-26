
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;

using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Sessions
{
    public class ScheduleSessionPayload : SessionPayloadBase
    {
        public ScheduleSessionPayload(Session session)
            : base(session)
        {
        }

        public ScheduleSessionPayload(UserError error)
            : base(new[] { error })
        {
        }

        public async Task<Track?> GetTrackAsync(
            TrackByIdDataLoader trackById,
            CancellationToken cancellationToken)
        {
            if (Session is null)
            {
                return null;
            }

            return await trackById.LoadAsync(Session.Id, cancellationToken);
        }

        public async Task<IEnumerable<Speaker>> GetSpeakersAsync
        (
            [Service] IDbContextFactory<ApplicationDbContext> dbContextFactory,
            SpeakerByIdDataLoader speakerByIdDataLoader,
            CancellationToken cancellationToken
        )
        {
            if (Session is null)
            {
                return null;
            }
            await using var context = await dbContextFactory.CreateDbContextAsync();

            int[] speakerIds = await context.Sessions
                .Where(s => s.Id == Session.Id)
                .Include(s => s.SessionSpeakers)
                .SelectMany(s => s.SessionSpeakers.Select(t => t.SpeakerId))
                .ToArrayAsync();

            return await speakerByIdDataLoader.LoadAsync(speakerIds, cancellationToken);
        }
    }
}