using ConferencePlanner.GraphQL;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;
using ConferencePlanner.GraphQL.EventListeners;
using ConferencePlanner.GraphQL.Speakers;
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
    .AddQueryType<Query>()
    .AddMutationType(d => d.Name("Mutation"))
        .AddTypeExtension<SpeakerMutations>()
    .AddType<SpeakerType>()
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