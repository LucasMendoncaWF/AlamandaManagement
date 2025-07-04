using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using AlamandaApi.Data;
using AlamandaApi.Services.User;
using AlamandaApi.Services.Team;
using AlamandaApi.Services.FieldsSchema;
using AlamandaApi.Services.Art;
using AlamandaApi.Services.CRUD;
using Microsoft.EntityFrameworkCore;
using AlamandaApi.Services.Role;
using AlamandaApi.Services.Comics;

namespace AlamandaApi {
  public class Startup {
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration) {
      Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services) {
      var dbHost = Environment.GetEnvironmentVariable("PG_HOST");
      var dbPort = Environment.GetEnvironmentVariable("PG_PORT");
      var dbUser = Environment.GetEnvironmentVariable("PG_USER");
      var dbPassword = Environment.GetEnvironmentVariable("PG_PASSWORD");
      var dbName = Environment.GetEnvironmentVariable("PG_DATABASE");

      var connectionString = $"server={dbHost};Port={dbPort};Username={dbUser};password={dbPassword};database={dbName}";
      services.AddMemoryCache();
      services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
      services.AddScoped(typeof(CRUDService<>));
      services.AddScoped<AuthService>();
      services.AddScoped<UserService>();
      services.AddScoped<ComicsService>();
      services.AddScoped<TeamService>();
      services.AddScoped<RoleService>();
      services.AddScoped<ArtService>();
      services.AddScoped<FieldsSchemaService>();

      var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "";
      services.AddAuthentication(options => {
        options.DefaultAuthenticateScheme = "Bearer";
        options.DefaultChallengeScheme = "Bearer";
      }).AddJwtBearer("Bearer", options => {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
          ValidateIssuer = false,
          ValidateAudience = false
        };
      });

      services.AddAuthorization(options => {
        options.AddPolicy("AdminOnly", policy =>
          policy.RequireRole("admin"));
      });

      var allowedHostsEnv = Environment.GetEnvironmentVariable("ALLOWED_HOSTS") ?? "";
      var allowedHosts = allowedHostsEnv.Split(',', StringSplitOptions.RemoveEmptyEntries);

      services.AddCors(options => {
        options.AddPolicy("DefaultCorsPolicy", policy => {
          policy
            .SetIsOriginAllowed(origin => {
              if (string.IsNullOrEmpty(origin)) return true;
              return allowedHosts.Contains(origin);
            })
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
      });

      services.AddControllers()
        .AddJsonOptions(options => {
          options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
          options.JsonSerializerOptions.WriteIndented = true;
        });

      services.AddEndpointsApiExplorer();
      services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app) {
      app.UseRouting();
      
      app.UseCors("DefaultCorsPolicy");

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseSwagger();
      app.UseSwaggerUI();

      app.UseStaticFiles();

      app.UseEndpoints(endpoints => {
        endpoints.MapControllers();
      });
    }
  }
}
