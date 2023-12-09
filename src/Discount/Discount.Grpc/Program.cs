using Discount.Grpc.Services;
using Discount.Infra.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfra(builder.Configuration);
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<DiscounterService>();

app.Run();