using GosExamTemplate.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GosExamTemplate.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        if (await db.Items.AnyAsync())
            return;

        var users = await SeedUsersAsync(userManager);
        var categories = await SeedCategoriesAsync(db);
        var tags = await SeedTagsAsync(db);
        await SeedItemsAndOrdersAsync(db, users, categories, tags);
    }

    private static async Task<List<ApplicationUser>> SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        var seed = new[]
        {
            new { Email = "alice@example.com", Display = "Алиса", Password = "Passw0rd!" },
            new { Email = "bob@example.com",   Display = "Боб",    Password = "Passw0rd!" },
            new { Email = "carol@example.com", Display = "Кэрол",  Password = "Passw0rd!" },
        };

        var users = new List<ApplicationUser>();
        foreach (var s in seed)
        {
            var existing = await userManager.FindByEmailAsync(s.Email);
            if (existing is not null)
            {
                users.Add(existing);
                continue;
            }

            var user = new ApplicationUser
            {
                UserName = s.Email,
                Email = s.Email,
                DisplayName = s.Display,
                EmailConfirmed = true,
            };
            var result = await userManager.CreateAsync(user, s.Password);
            if (!result.Succeeded)
                throw new InvalidOperationException("Не удалось создать пользователя seed: " +
                    string.Join("; ", result.Errors.Select(e => e.Description)));
            users.Add(user);
        }
        return users;
    }

    private static async Task<List<Category>> SeedCategoriesAsync(ApplicationDbContext db)
    {
        var names = new[] { "Электроника", "Книги", "Одежда", "Дом и сад", "Спорт" };
        var categories = new List<Category>();
        foreach (var name in names)
        {
            var existing = await db.Categories.FirstOrDefaultAsync(c => c.Name == name);
            if (existing is not null)
            {
                categories.Add(existing);
                continue;
            }
            var cat = new Category { Name = name, Description = $"Универсальная категория «{name}»" };
            db.Categories.Add(cat);
            categories.Add(cat);
        }
        await db.SaveChangesAsync();
        return categories;
    }

    private static async Task<List<Tag>> SeedTagsAsync(ApplicationDbContext db)
    {
        var names = new[] { "новое", "хит", "распродажа", "ручная работа", "эко", "ограниченная серия" };
        var tags = new List<Tag>();
        foreach (var name in names)
        {
            var existing = await db.Tags.FirstOrDefaultAsync(t => t.Name == name);
            if (existing is not null)
            {
                tags.Add(existing);
                continue;
            }
            var tag = new Tag { Name = name };
            db.Tags.Add(tag);
            tags.Add(tag);
        }
        await db.SaveChangesAsync();
        return tags;
    }

    private static async Task SeedItemsAndOrdersAsync(
        ApplicationDbContext db,
        List<ApplicationUser> users,
        List<Category> categories,
        List<Tag> tags)
    {
        var now = DateTime.UtcNow;
        var items = new List<Item>
        {
            BuildItem("Беспроводные наушники", "Удобные наушники с шумоподавлением, до 30 часов работы.", 5990m, users[0], categories[0], tags, [0, 1]),
            BuildItem("Смарт-часы Pro", "Спортивные часы с пульсометром и GPS.", 12990m, users[0], categories[0], tags, [1]),
            BuildItem("Книга «Чистый код»", "Классика для разработчиков. Подержанная, но в отличном состоянии.", 950m, users[1], categories[1], tags, [2, 4]),
            BuildItem("Зимняя куртка", "Тёплая куртка размер L. Носилась один сезон.", 4500m, users[1], categories[2], tags, [2]),
            BuildItem("Набор садовых инструментов", "Лопата, грабли, секатор — всё в одном комплекте.", 2100m, users[2], categories[3], tags, [3]),
            BuildItem("Йога-мат премиум", "Нескользящий коврик, толщина 6 мм, удобный ремень для переноски.", 1800m, users[2], categories[4], tags, [3, 4]),
            BuildItem("Кофемашина капсульная", "Бесшумная, готовит за 30 секунд. В комплекте 20 капсул.", 7990m, users[0], categories[0], tags, [1, 5]),
            BuildItem("Велосипед городской", "Скоростной городской велосипед, 7 передач, размер M.", 18900m, users[1], categories[4], tags, [5]),
        };

        for (int idx = 0; idx < items.Count; idx++)
            items[idx].CreatedAt = now.AddDays(-idx);

        db.Items.AddRange(items);
        await db.SaveChangesAsync();

        var orders = new List<Order>
        {
            new() { ItemId = items[0].Id, UserId = users[1].Id, Quantity = 1, Comment = "Хочу попробовать", Status = OrderStatus.Pending, CreatedAt = now.AddDays(-1) },
            new() { ItemId = items[2].Id, UserId = users[0].Id, Quantity = 1, Comment = "Заберу завтра", Status = OrderStatus.Confirmed, CreatedAt = now.AddDays(-3) },
            new() { ItemId = items[5].Id, UserId = users[1].Id, Quantity = 2, Comment = null, Status = OrderStatus.Completed, CreatedAt = now.AddDays(-5) },
        };
        db.Orders.AddRange(orders);
        await db.SaveChangesAsync();
    }

    private static Item BuildItem(
        string title,
        string description,
        decimal price,
        ApplicationUser owner,
        Category category,
        List<Tag> tags,
        int[] tagIndexes)
    {
        return new Item
        {
            Title = title,
            Description = description,
            Price = price,
            OwnerId = owner.Id,
            CategoryId = category.Id,
            ItemTags = tagIndexes.Select(i => new ItemTag { TagId = tags[i].Id }).ToList(),
        };
    }
}
