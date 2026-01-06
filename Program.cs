using System.Text;
using Microsoft.Extensions.Options;
using PokeAPIService.Clients;
using PokeAPIService.Models.Options;
using PokeAPIService.Services;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddMemoryCache();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

builder.Services.AddHttpClient<IPokeApiClient, PokeApiClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<PokeApiOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
}).AddPolicyHandler(GetRetryPolicy()).AddPolicyHandler(GetCircuitBreakerPolicy());

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions.HandleTransientHttpError()
        .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        onRetry: (outcome, timespan, retryAttempt, context) => { Console.WriteLine("retry"); }
        );
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 5, durationOfBreak: TimeSpan.FromSeconds(30),
        onBreak: (outcome, timespan) => { Console.WriteLine("break"); },
        onReset: () => { Console.WriteLine("reset"); }
        );
}

builder.Services.AddScoped<IPokemonService, PokemonService>();

builder.Services.Configure<PokeApiOptions>(
    builder.Configuration.GetSection("PokeApi"));

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();