using Microsoft.EntityFrameworkCore;
using AlamandaApi.Data;
using AlamandaApi.Services.User;
using AlamandaApi.Services.Team;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AlamandaApi.Services.FieldsSchema;
using System.Text.Json.Serialization;
using AlamandaApi.Services.Art;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

var dbHost = Environment.GetEnvironmentVariable("MYSQL_HOST");
var dbUser = Environment.GetEnvironmentVariable("MYSQL_USER");
var dbPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
var dbName = Environment.GetEnvironmentVariable("MYSQL_DATABASE");

var connectionString = $"server={dbHost};user={dbUser};password={dbPassword};database={dbName}";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TeamService>();
builder.Services.AddScoped<ArtService>();
builder.Services.AddScoped<FieldsSchemaService>();

var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "";
builder.Services.AddAuthentication(options => {
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
  options.RequireHttpsMetadata = false;
  options.SaveToken = true;
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
    ValidateIssuer = false,
    ValidateAudience = false
  };
});

builder.Services.AddAuthorization(options => {
  options.AddPolicy("AdminOnly", policy =>
      policy.RequireRole("admin"));
});

var allowedHostsEnv = Environment.GetEnvironmentVariable("ALLOWED_HOSTS") ?? "";
var allowedHosts = allowedHostsEnv.Split(',', StringSplitOptions.RemoveEmptyEntries);

builder.Services.AddControllers()
  .AddJsonOptions(options =>
  {
      options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      options.JsonSerializerOptions.WriteIndented = true;
  });

builder.Services.AddCors(options => {
  options.AddPolicy("DefaultCorsPolicy", policy => {
    policy
      .SetIsOriginAllowed(origin => {
        if (string.IsNullOrEmpty(origin)) {
          return true;
        }

        return allowedHosts.Contains(origin);
      })
      .AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials();
  });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseRouting();

app.UseCors("DefaultCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.UseStaticFiles();

app.Run();
