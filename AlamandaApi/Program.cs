using AlamandaApi.Services.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

var mongoSettings = new MongoDbSettings
{
    ConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING") ?? "",
    DatabaseName = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME") ?? "",
};
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "";

builder.Services.AddSingleton(mongoSettings);
builder.Services.AddSingleton<UserService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
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

var allowedHostsEnv = Environment.GetEnvironmentVariable("ALLOWED_HOSTS") ?? "";
var allowedHosts = allowedHostsEnv.Split(',', StringSplitOptions.RemoveEmptyEntries);

builder.Services.AddCors(options =>
{
  options.AddPolicy("DefaultCorsPolicy", policy =>
  {
    policy
    .SetIsOriginAllowed(origin =>
    {
      if (string.IsNullOrEmpty(origin))
      {
        return true;
      }
      return allowedHosts.Contains(origin);
    })
    .AllowAnyHeader()
    .AllowAnyMethod();
  });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware importante: precisa estar antes do UseAuthentication/UseAuthorization
app.UseRouting();

app.UseCors("DefaultCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
