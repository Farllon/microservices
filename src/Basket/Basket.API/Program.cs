using Basket.API.Endpoints;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IDatabase>(provider =>
{
    var configurations = provider.GetRequiredService<IConfiguration>();
    
    return ConnectionMultiplexer.Connect(
        configurations.GetConnectionString("Redis")!)
        .GetDatabase(0);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapBasket();

app.Run();