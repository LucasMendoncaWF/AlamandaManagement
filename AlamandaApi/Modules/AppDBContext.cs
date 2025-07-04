using Microsoft.EntityFrameworkCore;
using AlamandaApi.Services.User;
using AlamandaApi.Services.Team;
using AlamandaApi.Services.Comics;
using AlamandaApi.Services.Art;

namespace AlamandaApi.Data {
  public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserModel> Users { get; set; }
    public DbSet<TeamMemberModel> TeamMembers { get; set; }
    public DbSet<RefreshTokenModel> RefreshTokens { get; set; }
    public DbSet<ComicModel> Comics { get; set; }
    public DbSet<RoleModel> Roles { get; set; }
    public DbSet<ArtModel> FanArts { get; set; }
    public DbSet<PermissionModel> Permissions { get; set; }

    public class PagedResult<T> {
      public List<T> Items { get; set; } = [];
      public int TotalPages { get; set; }
      public int CurrentPage { get; set; }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<TeamMemberModel>()
       .HasMany(tm => tm.Comics)
       .WithMany(c => c.TeamMembers)
       .UsingEntity<Dictionary<string, object>>(
           "comicsmembers",
           j => j.HasOne<ComicModel>().WithMany().HasForeignKey("ComicId"),
           j => j.HasOne<TeamMemberModel>().WithMany().HasForeignKey("TeamMemberId")
       );

      modelBuilder.Entity<TeamMemberModel>()
        .HasOne(tm => tm.Role)
        .WithMany()
        .HasForeignKey(tm => tm.RoleId)
        .OnDelete(DeleteBehavior.SetNull);

      modelBuilder.Entity<UserModel>()
        .HasOne(tm => tm.Permission)
        .WithMany()
        .HasForeignKey(tm => tm.PermissionId)
        .OnDelete(DeleteBehavior.SetNull);
    }
  }
}