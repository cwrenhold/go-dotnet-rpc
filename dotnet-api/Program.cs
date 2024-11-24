using DotNetApi;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMassTransit(x =>
{
    x.SetRabbitMqReplyToRequestClientFactory();

    x.AddRequestClient<TestRequest>(RequestTimeout.After(s: 5));

    x.UsingRabbitMq((context, cfg) =>
    {
        var hostname = builder.Configuration["Rabbit:Host"] ?? "localhost";
        var port = builder.Configuration["Rabbit:Port"] ?? "5672";
        var username = builder.Configuration["Rabbit:User"] ?? "guest";
        var password = builder.Configuration["Rabbit:Pass"] ?? "guest";

        cfg.Host($"{hostname}", "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/test", async (IRequestClient<TestRequest> client) =>
{
    try
    {
        var response = await client.GetResponse<TestResponse>(new TestRequest(DateTime.UtcNow));

        return Results.Ok(response.Message);
    }
    catch (RequestTimeoutException)
    {
        return Results.BadRequest("Request timed out");
    }
});

app.Run();

