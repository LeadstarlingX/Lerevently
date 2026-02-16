using Lerevently.Api.Extenstions;
using Lerevently.Common.Application;
using Lerevently.Common.Infrastructure;
using Lerevently.Modules.Events.Application;
using Lerevently.Modules.Events.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine($"Connection String: {builder.Configuration.GetConnectionString("Database")}");


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();


builder.Services.AddApplication([AssemblyReference.Assembly]);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEventsModule(builder.Configuration);


builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(options => { options.CustomSchemaIds(t => t.FullName?.Replace("+", ".")); });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();
}

EventsModule.MapEndpoints(app);

app.Run();