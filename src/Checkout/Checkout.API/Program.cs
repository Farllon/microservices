using Checkout.API;
using Checkout.API.Endpoints;
using Checkout.API.Services.Basket;
using Checkout.API.Services.Discount;
using Checkout.API.Services.Payment;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IBasketService, BasketService>(client => 
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("Services:Basket:Url")!));

builder.Services.AddHttpClient<IOrderingService, OrderingService>(client => 
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("Services:Ordering:Url")!));

builder.Services.AddScoped<IDiscountService, DiscountService>();

builder.Services.AddGrpcClient<Discounter.DiscounterClient>(client =>
    client.Address = new Uri(builder.Configuration.GetValue<string>("Services:Discount:Url")!));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCheckout();

app.Run();