using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Mutations.AddSpeaker
{
    public class AddSpeakerPayload
    {
        public AddSpeakerPayload(Speaker speaker)
        {
            Speaker = speaker;
        }

        public Speaker Speaker { get; private set; }
    }
}