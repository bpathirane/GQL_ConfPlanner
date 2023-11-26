using ConferencePlanner.GraphQL;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;
using ConferencePlanner.GraphQL.EventListeners;
using ConferencePlanner.GraphQL.Sessions;
using ConferencePlanner.GraphQL.Speakers;
using ConferencePlanner.GraphQL.Tracks;
using ConferencePlanner.GraphQL.Types;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
});

builder.Services.AddPooledDbContextFactory<ApplicationDbContext>(options =>
{
    options.UseSqlite("Data Source=conference.db");
    options.EnableSensitiveDataLogging(true);
});

builder.Services
    .AddGraphQLServer()
    .RegisterDbContext<ApplicationDbContext>(DbContextKind.Pooled)
    .AddQueryType(d => d.Name("Query"))
        .AddTypeExtension<SpeakerQueries>()
    .AddMutationType(d => d.Name("Mutation"))
        .AddTypeExtension<SpeakerMutations>()
        .AddTypeExtension<SessionMutations>()
        .AddTypeExtension<TrackMutations>()
    .AddType<SpeakerType>()
    .AddType<AttendeeType>()
    .AddType<SessionType>()
    .AddType<TrackType>()
    .AddGlobalObjectIdentification()
    .AddQueryFieldToMutationPayloads()
    .AddDataLoader<SpeakerByIdDataLoader>()
    .AddDataLoader<SessionByIdDataLoader>()
    .AddDiagnosticEventListener<CustomExecutionEventListener>()
    .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGraphQL();

app.Run();