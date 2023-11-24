using ConferencePlanner.GraphQL;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.Mutations.AddSpeaker;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite("Data Source=conference.db"));

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGraphQL();

app.Run();
