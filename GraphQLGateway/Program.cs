using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHeaderPropagation(o => {
    o.Headers.Add("Authorization");
});
builder.Services.AddHttpClient("patientApi",
        (sp, client) => {
            client.BaseAddress = new Uri(sp.GetRequiredService<IConfiguration>()["PATIENT_API_URL"]);
        })
    .AddHeaderPropagation();
builder.Services.AddHttpClient("providerApi",
        (sp, client) =>
        {
            client.BaseAddress = new Uri(sp.GetRequiredService<IConfiguration>()["PROVIDER_API_URL"]);
        })
    .AddHeaderPropagation();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(sp.GetRequiredService<IConfiguration>()["REDIS_URL"]));

builder.Services
    .AddGraphQLServer()
    .AddRemoteSchemasFromRedis("patients", sp =>
        sp.GetRequiredService<IConnectionMultiplexer>())
    .AddRemoteSchemasFromRedis("providers", sp =>
        sp.GetRequiredService<IConnectionMultiplexer>())
    .InitializeOnStartup();

var app = builder.Build();

app.UseHeaderPropagation();

app.MapGraphQL();

app.Run();
