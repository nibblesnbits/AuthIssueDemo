using System.Security.Claims;
using AspNetCore.Authentication.Basic;
using ProviderApi;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IConnectionMultiplexer>(c =>
    ConnectionMultiplexer.Connect(c.GetRequiredService<IConfiguration>()["REDIS_URL"]));

builder.Services.AddAuthentication()
    .AddBasic(options =>
    {
        options.Realm = "MyApp";
        options.Events = new BasicEvents
        {
            OnValidateCredentials = context =>
            {
                context.Principal = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, context.Username)
                }, context.Scheme.Name));
                context.Success();
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("IsPatient", p => p.RequireClaim(ClaimTypes.NameIdentifier, "patient"));
});

builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType<Query>()
    .InitializeOnStartup()
    .PublishSchemaDefinition(c => c
        .SetName("providerApi")
        .PublishToRedis("providers",
            sp => sp.GetRequiredService<IConnectionMultiplexer>()));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL()
    .RequireAuthorization();

app.Run();
