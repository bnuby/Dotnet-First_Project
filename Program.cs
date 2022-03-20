using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Filters;

using First_Project.Data;
using First_Project.Services.CharacterService;
using First_Project.Services.WeaponService;
using First_Project.Services.FightService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var Configuration = builder.Configuration;

// Add Server connection

var useSqlite = Boolean.Parse(Configuration.GetSection("AppSettings:useSqlite").Value);
String connection;
if (useSqlite)
{
  connection = Configuration.GetConnectionString("SqliteConnection");
  builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(connection));
}
else
{
  connection = Configuration.GetConnectionString("DefaultConnection");
  builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connection));
}
Console.WriteLine($"~ connection: {connection}");

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
  c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
  {
    Description = "Standard Authorization header using the Bearer schema. Example: \"bearer {token}\"",
    In = ParameterLocation.Header,
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey
  });
  c.OperationFilter<SecurityRequirementsOperationFilter>();
});

var Service = builder.Services;

Service.AddScoped<ICharacterService, CharacterService>();
Service.AddScoped<IAuthRepository, AuthRepository>();
Service.AddScoped<IWeaponService, WeaponService>();
Service.AddScoped<IFightService, FightService>();

Service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
      ValidateIssuer = false,
      ValidateAudience = false
    };
  });
Service.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
Service.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI(c => 
  {
    c.IndexStream = () => File.OpenRead("wwwroot/swashbuckle.html");
  });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
