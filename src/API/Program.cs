using API.Middleware;
using API.Models;
using API.Repository;
using API.Services;
using API.ViewModel;
using Azure.Core;
using LogSight.API.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationContext")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IServerRepository, ServerRepository>();
builder.Services.AddScoped<IServerService, ServerService>();

builder.Services.AddSwaggerGen(options =>
{
    //options.SwaggerDoc("LogSight", new OpenApiInfo
    //{
    //    Version = "v1",
    //    Title = "REST Api List",
    //    Description = "A simple example ASP.NET Core Web API",
    //    Contact = new OpenApiContact
    //    {
    //        Name = "Fatemeh Qanbari",
    //        Email = "fqanbari919@gmail.com",
    //    },
    //    License = new OpenApiLicense
    //    {
    //        Name = "Use under FQanbari",
    //        Url = new Uri("https://fqanbari.ir"),
    //    }
    //});

    ////var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    ////var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    ////options.IncludeXmlComments(xmlPath);
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
        Type = SecuritySchemeType.ApiKey
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler(a => a.UseCustomExceptionHandler());
app.UseAuthorization();
app.UseAuthentication();

app.MapPost("/login", async (UserLogin user, IUserService userService, CancellationToken cancellationToken)
    => await Login(user, userService, cancellationToken))
    .Accepts<UserLogin>("application/json")
    .Produces<string>();


app.MapGet("/api/Log/Server",
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async (IServerService serverService, CancellationToken cancellationToken)
    => Results.Ok(await serverService.GetAll(cancellationToken)))
    .WithName("Server")
    .Produces<List<Server>>(statusCode:200, contentType: "application/json")
.WithOpenApi();

app.MapGet("/api/Log",
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async (ILogService logService, CancellationToken cancellationToken, int? pageSize, int? pageNumber)
    => await GetLogs(logService, cancellationToken, pageSize, pageNumber))
    .WithName("LogList")
    .Produces<List<Log>>(statusCode: 200, contentType: "application/json")
.WithOpenApi();

app.MapGet("/", () => "Hello World!")
    .ExcludeFromDescription();

app.Run();


async Task<IResult> Login(UserLogin user, IUserService userService, CancellationToken cancellationToken)
{
    if (IsValidUserInput(user))
    {
        user.Password = API.Utilities.Utility.ComputeSha256Hash(user.Password);

        if (user.UserName == "guest" && user.Password == API.Utilities.Utility.ComputeSha256Hash("guest"))
        {
            return await GenerateJwtTokenForGuest();
        }

        var loggedInUser = await GetLoggedInUser(user, userService, cancellationToken);

        if (loggedInUser != null)
        {
            return await GenerateJwtToken(loggedInUser);
        }
    }

    return Results.Forbid();
}
async Task<IResult> GenerateJwtTokenForGuest()
{
    var guestUser = new UserViewModel
    {
        Id = 111,
        UserName = "guest",
        Password = "guest",
        FullName = "guest",
        Role = "guest"
    };

    return await GenerateJwtToken(guestUser);
}
bool IsValidUserInput(UserLogin user)
{
    return !string.IsNullOrWhiteSpace(user.UserName) && !string.IsNullOrWhiteSpace(user.Password);
}

async Task<UserViewModel> GetLoggedInUser(UserLogin user, IUserService userService, CancellationToken cancellationToken)
{
    var loggedInUser = await userService.Get(user, cancellationToken);

    if (loggedInUser == null && user.UserName != "guest")
    {
        throw new Exception("User not found!");
    }

    return loggedInUser;
}

async Task<IResult> GenerateJwtToken(UserViewModel loggedInUser)
{
    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, loggedInUser.Id.ToString()),
        new Claim(ClaimTypes.GivenName, loggedInUser.UserName),
        new Claim(ClaimTypes.Surname, loggedInUser.FullName),
        new Claim(ClaimTypes.Role, loggedInUser.Role)
    };

    var token = new JwtSecurityToken
    (
        issuer: builder.Configuration["Jwt:Issuer"],
        audience: builder.Configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddDays(7),
        notBefore: DateTime.UtcNow,
        signingCredentials: new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            SecurityAlgorithms.HmacSha256)
    );

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    return Results.Ok(tokenString);
}

async Task<IResult> GetLogs(ILogService logService, CancellationToken cancellationToken, int? pageSize, int? pageNumber)
{
    if (pageNumber == null || pageNumber == 0) pageNumber = 1;
    if (pageSize == null || pageSize == 0) pageSize = 20;

    var result = await logService.GetLogs(pageNumber.Value, pageSize.Value, cancellationToken);

    return Results.Ok(result);
}
