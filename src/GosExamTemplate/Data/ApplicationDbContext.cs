using GosExamTemplate.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GosExamTemplate.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<ItemTag> ItemTags => Set<ItemTag>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Item>(item =>
        {
            item.HasOne(i => i.Owner)
                .WithMany(u => u.Items)
                .HasForeignKey(i => i.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            item.HasOne(i => i.Category)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            item.Property(i => i.Price).HasPrecision(18, 2);
            item.HasIndex(i => i.Title);
        });

        builder.Entity<ItemTag>(itemTag =>
        {
            itemTag.HasKey(it => new { it.ItemId, it.TagId });

            itemTag.HasOne(it => it.Item)
                .WithMany(i => i.ItemTags)
                .HasForeignKey(it => it.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            itemTag.HasOne(it => it.Tag)
                .WithMany(t => t.ItemTags)
                .HasForeignKey(it => it.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Order>(order =>
        {
            order.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            order.HasOne(o => o.Item)
                .WithMany(i => i.Orders)
                .HasForeignKey(o => o.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Category>().HasIndex(c => c.Name).IsUnique();
        builder.Entity<Tag>().HasIndex(t => t.Name).IsUnique();
    }
}
