using System.Text.Json;
using DotNetApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/test", async (IConfiguration configuration) =>
{
    try
    {
        var client = new RpcClient(configuration);
        await client.StartAsync();

        var request = new TestRequest(DateTime.UtcNow);

        var response = await client.CallAsync(JsonSerializer.Serialize(request));

        var result = JsonSerializer.Deserialize<TestResponse>(response);

        return Results.Ok(result);
    }
    catch (Exception)
    {
        return Results.BadRequest("Request timed out");
    }
});

app.Run();

