using LogSight.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationContext")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

app.MapGet("/api/Log/Service", async (ApplicationDbContext context, CancellationToken cancellationToken)
    => Results.Ok(await context.Services.ToListAsync()))
    .WithName("Service")
.WithOpenApi();

app.MapGet("/api/Log", async (ApplicationDbContext context, CancellationToken cancellationToken, int? pageSize, int? pageNumber) =>
{
    if (pageNumber == null || pageNumber == 0) pageNumber = 1;
    if (pageSize == null || pageSize == 0) pageSize = 20;

    var result = await context.Logs
        .OrderByDescending(x => x.CreateOn)
        .Skip((pageNumber.Value - 1) * pageSize.Value)
        .Take(pageSize.Value)
        .ToListAsync(cancellationToken);

    return Results.Ok(result);
}).WithName("LogList")
.WithOpenApi();

app.MapGet("/", () => "Hello World!");
app.Run();


