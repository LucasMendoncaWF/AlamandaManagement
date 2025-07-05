using Microsoft.EntityFrameworkCore;
using AlamandaApi.Services.User;
using AlamandaApi.Services.Team;
using AlamandaApi.Services.Comics;
using AlamandaApi.Services.Art;
using AlamandaApi.Services.Role;
using AlamandaApi.Services.Chapters;
using AlamandaApi.Services.Category;
using AlamandaApi.Services.Cart;

namespace AlamandaApi.Data {
  public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserModel> Users { get; set; }
    public DbSet<TeamMemberModel> TeamMembers { get; set; }
    public DbSet<RefreshTokenModel> RefreshTokens { get; set; }
    public DbSet<ComicModel> Comics { get; set; }
    public DbSet<CategoryModel> Categories { get; set; }
    public DbSet<ChapterModel> Chapters { get; set; }
    public DbSet<CartModel> Carts { get; set; }
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
           "ComicsMembers",
           j => j.HasOne<ComicModel>().WithMany().HasForeignKey("ComicId"),
           j => j.HasOne<TeamMemberModel>().WithMany().HasForeignKey("TeamMemberId")
       );

      modelBuilder.Entity<TeamMemberModel>()
        .HasMany(tm => tm.Roles)
        .WithMany(r => r.TeamMembers)
        .UsingEntity<Dictionary<string, object>>(
          "TeamMemberRole",
          j => j.HasOne<RoleModel>().WithMany().HasForeignKey("RolesId"),
          j => j.HasOne<TeamMemberModel>().WithMany().HasForeignKey("TeamMemberId")
        );

      modelBuilder.Entity<ComicModel>()
        .HasMany(tm => tm.Categories)
        .WithMany(r => r.Comics)
        .UsingEntity<Dictionary<string, object>>(
          "ComicCategory",
          j => j.HasOne<CategoryModel>().WithMany().HasForeignKey("CategoryId"),
          j => j.HasOne<ComicModel>().WithMany().HasForeignKey("ComicId")
        );

      modelBuilder.Entity<UserModel>()
        .HasOne(tm => tm.Permission)
        .WithMany()
        .HasForeignKey(tm => tm.PermissionId)
        .OnDelete(DeleteBehavior.SetNull);

      modelBuilder.Entity<UserModel>()
          .HasOne(u => u.Cart)
          .WithOne(c => c.User)
          .HasForeignKey<CartModel>(c => c.UserId);

      modelBuilder.Entity<CartItemModel>()
          .HasOne(ci => ci.Cart)
          .WithMany(c => c.Items)
          .HasForeignKey(ci => ci.CartId);

      modelBuilder.Entity<CartItemModel>()
          .HasOne(ci => ci.Comic)
          .WithMany()
          .HasForeignKey(ci => ci.ComicId);
    }
  }
}