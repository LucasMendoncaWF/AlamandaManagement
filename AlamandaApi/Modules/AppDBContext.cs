using Microsoft.EntityFrameworkCore;
using AlamandaApi.Services.User;
using AlamandaApi.Services.Team;
using AlamandaApi.Services.Comics;
using AlamandaApi.Services.Art;
using AlamandaApi.Services.Role;
using AlamandaApi.Services.Chapters;
using AlamandaApi.Services.Category;
using AlamandaApi.Services.Cart;
using AlamandaApi.Services.Language;
using AlamandaApi.Services.FieldsSchema;

namespace AlamandaApi.Data {
  public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserModel> Users { get; set; }
    public DbSet<TeamMemberModel> TeamMembers { get; set; }
    public DbSet<RefreshTokenModel> RefreshTokens { get; set; }
    public DbSet<ComicModel> Comics { get; set; }
    public DbSet<ColorModel> ColorType { get; set; }
    public DbSet<CoverModel> CoverType { get; set; }
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
      public List<FieldInfo> Fields { get; set; } = [];
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

      // ___________________________ CART ___________________________
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


      // translations
      // ___________________________ CATEGORIES ___________________________
      modelBuilder.Entity<LanguageModel>().ToTable("Languages");
      modelBuilder.Entity<CategoryTranslationModel>().ToTable("CategoriesTranslations");
      modelBuilder.Entity<CategoryModel>()
        .HasMany(c => c.Translations)
        .WithOne(t => t.Category)
        .HasForeignKey(t => t.CategoryId)
        .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<CategoryTranslationModel>()
        .HasOne(t => t.Language)
        .WithMany()
        .HasForeignKey(t => t.LanguageId)
        .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<CategoryTranslationModel>()
        .HasIndex(t => new { t.CategoryId, t.LanguageId })
        .IsUnique();

      // ___________________________ ROLES ___________________________
      modelBuilder.Entity<RoleTranslationModel>().ToTable("RolesTranslations");
      modelBuilder.Entity<RoleModel>()
        .HasMany(c => c.Translations)
        .WithOne(t => t.Role)
        .HasForeignKey(t => t.RoleId)
        .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<RoleTranslationModel>()
        .HasOne(t => t.Language)
        .WithMany()
        .HasForeignKey(t => t.LanguageId)
        .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<RoleTranslationModel>()
        .HasIndex(t => new { t.RoleId, t.LanguageId })
        .IsUnique();

      // ___________________________ COLORS ___________________________
      modelBuilder.Entity<ColorModelTranslation>().ToTable("ColorTypeTranslations");
      modelBuilder.Entity<ComicModel>()
        .HasOne(tm => tm.ColorModel)
        .WithMany()
        .HasForeignKey(tm => tm.Color)
        .OnDelete(DeleteBehavior.SetNull);

      // ___________________________ Cover ___________________________
      modelBuilder.Entity<CoverModelTranslation>().ToTable("CoverTypeTranslations");
      modelBuilder.Entity<ComicModel>()
        .HasOne(tm => tm.CoverModel)
        .WithMany()
        .HasForeignKey(tm => tm.Cover)
        .OnDelete(DeleteBehavior.SetNull);

      // ___________________________ STATUS ___________________________
      modelBuilder.Entity<StatusModelTranslation>().ToTable("StatusTranslations");
      modelBuilder.Entity<ComicModel>()
        .HasOne(tm => tm.StatusModel)
        .WithMany()
        .HasForeignKey(tm => tm.Status)
        .OnDelete(DeleteBehavior.SetNull);

      // ___________________________ COMIC ___________________________
      modelBuilder.Entity<ComicTranslationsModel>().ToTable("ComicsTranslations");
      modelBuilder.Entity<ComicModel>()
        .HasMany(c => c.Translations)
        .WithOne(t => t.Comic)
        .HasForeignKey(t => t.ComicId)
        .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<ComicTranslationsModel>()
        .HasOne(t => t.Language)
        .WithMany()
        .HasForeignKey(t => t.LanguageId)
        .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<ComicTranslationsModel>()
        .HasIndex(t => new { t.ComicId, t.LanguageId })
        .IsUnique();
    }
  }
}