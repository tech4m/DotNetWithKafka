using MassTransit;
using MassTransitKafkaDotNet.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using System;
using System.Net;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMassTransit(x =>
{
    x.UsingInMemory();
    x.AddRider(rider =>
    {
        rider.AddConsumer<VideoDeletedEventConsumer>();
        rider.AddProducer<VideoCreatedEvent>(nameof(VideoCreatedEvent));
        rider.UsingKafka((context, k) =>
        {
            k.Host("localhost:9092");
            k.TopicEndpoint<VideoDeletedEvent>(nameof(VideoDeletedEvent), GetUniqueName(nameof(VideoDeletedEvent)), e =>
            {
                e.CheckpointInterval = TimeSpan.FromSeconds(10);
                e.ConfigureConsumer<VideoDeletedEventConsumer>(context);
                e.CreateIfMissing(t =>
                {
                });
            });
        });
    });
});

string GetUniqueName(string eventName)
{
    string hostName = Dns.GetHostName();
    string callingAssembly = Assembly.GetCallingAssembly().GetName().Name;
    return $"{hostName}.{callingAssembly}.{eventName}";
}

builder.Services.AddMassTransitHostedService(true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


