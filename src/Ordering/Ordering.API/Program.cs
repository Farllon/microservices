using MongoDB.Driver;
using Ordering.API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IMongoClient>(provider =>
    new MongoClient(provider
        .GetRequiredService<IConfiguration>()
        .GetConnectionString("MongoDB")));

builder.Services.AddScoped<IMongoDatabase>(provider => provider
    .GetRequiredService<IMongoClient>()
    .GetDatabase("ordering-service"));

builder.Services.AddScoped<IMongoCollection<Order>>(provider => provider
    .GetRequiredService<IMongoDatabase>()
    .GetCollection<Order>("orders"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();