using GosExamTemplate.Data;
using GosExamTemplate.Dtos.Common;
using GosExamTemplate.Dtos.Items;
using GosExamTemplate.Dtos.Orders;
using GosExamTemplate.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GosExamTemplate.Services;

public class ItemService(ApplicationDbContext db) : IItemService
{
    private const int DescriptionPreviewLength = 140;

    public async Task<MyItemsFilterDto> GetMyItemsAsync(string ownerId, MyItemsFilterDto filter, CancellationToken ct = default)
    {
        var query = db.Items.AsNoTracking().Where(i => i.OwnerId == ownerId);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim().ToLowerInvariant();
            query = query.Where(i =>
                i.Title.ToLower().Contains(search) ||
                i.Description.ToLower().Contains(search));
        }

        if (filter.CategoryId is int categoryId && categoryId > 0)
            query = query.Where(i => i.CategoryId == categoryId);

        if (filter.DateFrom is DateTime dateFrom)
            query = query.Where(i => i.CreatedAt >= DateTime.SpecifyKind(dateFrom, DateTimeKind.Utc));

        if (filter.DateTo is DateTime dateTo)
            query = query.Where(i => i.CreatedAt <= DateTime.SpecifyKind(dateTo, DateTimeKind.Utc));

        if (filter.PriceFrom is decimal priceFrom)
            query = query.Where(i => i.Price >= priceFrom);

        if (filter.PriceTo is decimal priceTo)
            query = query.Where(i => i.Price <= priceTo);

        (filter.Page, filter.PageSize) = Pagination.Normalize(filter.Page, filter.PageSize);
        filter.TotalCount = await query.CountAsync(ct);
        filter.Page = Pagination.ClampPage(filter.Page, filter.TotalPages);

        filter.Items = await ProjectToList(
            query.OrderByDescending(i => i.CreatedAt)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize),
            ct);
        filter.AvailableCategories = await GetCategoryOptionsAsync(filter.CategoryId, ct);
        return filter;
    }

    public async Task<PublicItemsFilterDto> GetPublicItemsAsync(PublicItemsFilterDto filter, CancellationToken ct = default)
    {
        var query = db.Items.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim().ToLowerInvariant();
            query = query.Where(i => i.Title.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(filter.Owner))
        {
            var ownerSearch = filter.Owner.Trim().ToLowerInvariant();
            query = query.Where(i =>
                (i.Owner != null && i.Owner.DisplayName.ToLower().Contains(ownerSearch)) ||
                (i.Owner != null && i.Owner.UserName != null && i.Owner.UserName.ToLower().Contains(ownerSearch)));
        }

        if (filter.TagId is int tagId && tagId > 0)
            query = query.Where(i => i.ItemTags.Any(it => it.TagId == tagId));

        if (filter.PriceFrom is decimal priceFrom)
            query = query.Where(i => i.Price >= priceFrom);

        if (filter.PriceTo is decimal priceTo)
            query = query.Where(i => i.Price <= priceTo);

        (filter.Page, filter.PageSize) = Pagination.Normalize(filter.Page, filter.PageSize);
        filter.TotalCount = await query.CountAsync(ct);
        filter.Page = Pagination.ClampPage(filter.Page, filter.TotalPages);

        filter.Items = await ProjectToList(
            query.OrderByDescending(i => i.CreatedAt)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize),
            ct);
        filter.AvailableTags = await GetTagOptionsAsync(filter.TagId, ct);
        filter.AvailableOwners = await db.Users
            .AsNoTracking()
            .OrderBy(u => u.DisplayName)
            .Select(u => new SelectListItem
            {
                Value = u.DisplayName,
                Text = u.DisplayName,
                Selected = filter.Owner != null && filter.Owner == u.DisplayName,
            })
            .ToListAsync(ct);

        return filter;
    }

    public async Task<ItemDetailsDto?> GetDetailsAsync(int id, CancellationToken ct = default)
    {
        var details = await db.Items
            .AsNoTracking()
            .Where(i => i.Id == id)
            .Select(i => new ItemDetailsDto
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                Price = i.Price,
                ImageUrl = i.ImageUrl,
                CreatedAt = i.CreatedAt,
                CategoryId = i.CategoryId,
                CategoryName = i.Category != null ? i.Category.Name : string.Empty,
                OwnerId = i.OwnerId,
                OwnerName = i.Owner != null ? i.Owner.DisplayName : string.Empty,
                Tags = i.ItemTags
                    .Where(it => it.Tag != null)
                    .Select(it => it.Tag!.Name)
                    .ToList(),
                Orders = i.Orders
                    .OrderByDescending(o => o.CreatedAt)
                    .Select(o => new OrderListDto
                    {
                        Id = o.Id,
                        Quantity = o.Quantity,
                        Comment = o.Comment,
                        Status = o.Status,
                        CreatedAt = o.CreatedAt,
                        ItemId = o.ItemId,
                        ItemTitle = i.Title,
                        UserId = o.UserId,
                        UserName = o.User != null ? o.User.DisplayName : string.Empty,
                    })
                    .ToList(),
            })
            .FirstOrDefaultAsync(ct);

        return details;
    }

    public async Task<ItemFormDto?> GetForEditAsync(int id, string ownerId, CancellationToken ct = default)
    {
        var dto = await db.Items
            .AsNoTracking()
            .Where(i => i.Id == id && i.OwnerId == ownerId)
            .Select(i => new ItemFormDto
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                Price = i.Price,
                ImageUrl = i.ImageUrl,
                CategoryId = i.CategoryId,
                TagIds = i.ItemTags.Select(it => it.TagId).ToList(),
            })
            .FirstOrDefaultAsync(ct);

        if (dto is null) return null;

        dto.AvailableCategories = await GetCategoryOptionsAsync(dto.CategoryId, ct);
        dto.AvailableTags = await GetTagOptionsAsync(dto.TagIds, ct);
        return dto;
    }

    public async Task<ItemFormDto> BuildCreateFormAsync(CancellationToken ct = default)
    {
        return new ItemFormDto
        {
            AvailableCategories = await GetCategoryOptionsAsync(null, ct),
            AvailableTags = await GetTagOptionsAsync(Array.Empty<int>(), ct),
        };
    }

    public async Task<int> CreateAsync(ItemFormDto dto, string ownerId, CancellationToken ct = default)
    {
        var entity = new Item
        {
            Title = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            Price = dto.Price,
            ImageUrl = string.IsNullOrWhiteSpace(dto.ImageUrl) ? null : dto.ImageUrl.Trim(),
            CategoryId = dto.CategoryId,
            OwnerId = ownerId,
            CreatedAt = DateTime.UtcNow,
            ItemTags = dto.TagIds.Distinct().Select(id => new ItemTag { TagId = id }).ToList(),
        };
        db.Items.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(ItemFormDto dto, string ownerId, CancellationToken ct = default)
    {
        var entity = await db.Items
            .Include(i => i.ItemTags)
            .FirstOrDefaultAsync(i => i.Id == dto.Id, ct);
        if (entity is null || entity.OwnerId != ownerId) return false;

        entity.Title = dto.Title.Trim();
        entity.Description = dto.Description.Trim();
        entity.Price = dto.Price;
        entity.ImageUrl = string.IsNullOrWhiteSpace(dto.ImageUrl) ? null : dto.ImageUrl.Trim();
        entity.CategoryId = dto.CategoryId;

        var desiredTagIds = dto.TagIds.Distinct().ToHashSet();
        var currentTagIds = entity.ItemTags.Select(it => it.TagId).ToHashSet();

        foreach (var removed in entity.ItemTags.Where(it => !desiredTagIds.Contains(it.TagId)).ToList())
            entity.ItemTags.Remove(removed);

        foreach (var addedId in desiredTagIds.Where(id => !currentTagIds.Contains(id)))
            entity.ItemTags.Add(new ItemTag { ItemId = entity.Id, TagId = addedId });

        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, string ownerId, CancellationToken ct = default)
    {
        var entity = await db.Items.FirstOrDefaultAsync(i => i.Id == id, ct);
        if (entity is null || entity.OwnerId != ownerId) return false;

        db.Items.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public Task<bool> IsOwnedByAsync(int itemId, string ownerId, CancellationToken ct = default)
        => db.Items.AnyAsync(i => i.Id == itemId && i.OwnerId == ownerId, ct);

    private static Task<List<ItemListDto>> ProjectToList(IQueryable<Item> query, CancellationToken ct)
    {
        return query
            .Select(i => new ItemListDto
            {
                Id = i.Id,
                Title = i.Title,
                ShortDescription = i.Description.Length > DescriptionPreviewLength
                    ? i.Description.Substring(0, DescriptionPreviewLength) + "..."
                    : i.Description,
                Price = i.Price,
                ImageUrl = i.ImageUrl,
                CreatedAt = i.CreatedAt,
                CategoryName = i.Category != null ? i.Category.Name : string.Empty,
                OwnerId = i.OwnerId,
                OwnerName = i.Owner != null ? i.Owner.DisplayName : string.Empty,
                Tags = i.ItemTags
                    .Where(it => it.Tag != null)
                    .Select(it => it.Tag!.Name)
                    .ToList(),
            })
            .ToListAsync(ct);
    }

    private async Task<IEnumerable<SelectListItem>> GetCategoryOptionsAsync(int? selectedId, CancellationToken ct)
    {
        return await db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = selectedId.HasValue && c.Id == selectedId.Value,
            })
            .ToListAsync(ct);
    }

    private Task<List<SelectListItem>> GetTagOptionsAsync(int? selectedId, CancellationToken ct)
    {
        return db.Tags
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name,
                Selected = selectedId.HasValue && t.Id == selectedId.Value,
            })
            .ToListAsync(ct);
    }

    private async Task<IEnumerable<SelectListItem>> GetTagOptionsAsync(IEnumerable<int> selectedIds, CancellationToken ct)
    {
        var selected = selectedIds.ToHashSet();
        var tags = await db.Tags
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new { t.Id, t.Name })
            .ToListAsync(ct);

        return tags.Select(t => new SelectListItem
        {
            Value = t.Id.ToString(),
            Text = t.Name,
            Selected = selected.Contains(t.Id),
        });
    }
}
