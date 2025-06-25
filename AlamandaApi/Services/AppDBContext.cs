using Microsoft.EntityFrameworkCore;
using AlamandaApi.Services.User;
using AlamandaApi.Services.Team;

namespace AlamandaApi.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserModel> Users { get; set; }
    public DbSet<TeamMemberModel> TeamMembers { get; set; }
    public DbSet<RefreshTokenModel> RefreshTokens { get; set; }
  }
}