using System.Security.Claims;
using AspNetCore.Authentication.Basic;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHeaderPropagation(o => {
    o.Headers.Add("Authorization");
});
builder.Services.AddHttpClient("patientApi",
        (sp, client) => {
            client.BaseAddress = new Uri(sp.GetRequiredService<IConfiguration>()["API_URL"]);
        })
    .AddHeaderPropagation();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(sp.GetRequiredService<IConfiguration>()["REDIS_URL"]));

builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .InitializeOnStartup()
    .AddRemoteSchemasFromRedis("patients", sp =>
        sp.GetRequiredService<IConnectionMultiplexer>());

builder.Services.AddAuthentication()
    .AddBasic(options => {
        options.Events = new BasicEvents {
            OnValidateCredentials = context => {
                context.Principal = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, context.Username)
                }, context.Scheme.Name));
                context.Success();
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHeaderPropagation();

app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL()
    .RequireAuthorization();

app.Run();
