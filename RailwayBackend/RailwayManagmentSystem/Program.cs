using System.Reflection;
using System.Text;
using BusinessLayer.Interface;
using BusinessLayer.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using RepositoryLayer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Railway Managment system",
        Version = "v1",
        Description = "API for Address Book Application",
        Contact = new OpenApiContact
        {
            Name = "Akash Singh",
            Email = "akash.si8273@gmail.com"
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (!File.Exists(xmlPath))
    {
        File.WriteAllText(xmlPath, "<?xml version =\"1.0\"?><doc></doc>");
    }
    options.IncludeXmlComments(xmlPath);


    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter 'Bearer {your_token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});



var connectionSql = builder.Configuration.GetConnectionString("SqlConnection") 
    ?? throw new InvalidOperationException("SQL connection string 'SqlConnection' is not configured");
builder.Services.AddDbContext<RailwayContext>(options => options.UseSqlServer(connectionSql));

builder.Services.AddScoped<IRailwayBusinessLayer, RailwayBusinessLayer>();
builder.Services.AddScoped<IRailwayRL, RailwayRL>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<IJwtToken, JWTToken>();
builder.Services.AddScoped<ITrainBL, TrainBL>();
builder.Services.AddScoped<ITrainRL, TrainRL>();


// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
    policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular app URL
                      .AllowAnyHeader()
     .AllowAnyMethod();
    });
});


builder.Services.AddAuthorization();
builder.Services.AddAutoMapper(typeof(MapProfile));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
