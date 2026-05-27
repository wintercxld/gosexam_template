using GosExamTemplate.Data;
using GosExamTemplate.Dtos.Common;
using GosExamTemplate.Dtos.Orders;
using GosExamTemplate.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GosExamTemplate.Services;

public class OrderService(ApplicationDbContext db) : IOrderService
{
    public async Task<PagedIndexDto<OrderListDto>> GetMyOrdersAsync(
        string userId,
        int? page = null,
        int? pageSize = null,
        CancellationToken ct = default)
    {
        var (normalizedPage, normalizedPageSize) = Pagination.Normalize(page, pageSize);
        var query = db.Orders.AsNoTracking().Where(o => o.UserId == userId);

        var totalCount = await query.CountAsync(ct);
        var totalPages = Pagination.GetTotalPages(totalCount, normalizedPageSize);
        normalizedPage = Pagination.ClampPage(normalizedPage, totalPages);

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(o => new OrderListDto
            {
                Id = o.Id,
                Quantity = o.Quantity,
                Comment = o.Comment,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                ItemId = o.ItemId,
                ItemTitle = o.Item != null ? o.Item.Title : string.Empty,
                UserId = o.UserId,
                UserName = o.User != null ? o.User.DisplayName : string.Empty,
            })
            .ToListAsync(ct);

        return new PagedIndexDto<OrderListDto>
        {
            Items = items,
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            TotalCount = totalCount,
        };
    }

    public async Task<OrderListDto?> GetAsync(int id, string userId, CancellationToken ct = default)
    {
        return await db.Orders
            .AsNoTracking()
            .Where(o => o.Id == id && o.UserId == userId)
            .Select(o => new OrderListDto
            {
                Id = o.Id,
                Quantity = o.Quantity,
                Comment = o.Comment,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                ItemId = o.ItemId,
                ItemTitle = o.Item != null ? o.Item.Title : string.Empty,
                UserId = o.UserId,
                UserName = o.User != null ? o.User.DisplayName : string.Empty,
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<OrderFormDto> BuildCreateFormAsync(int? itemId, CancellationToken ct = default)
    {
        return new OrderFormDto
        {
            ItemId = itemId ?? 0,
            AvailableItems = await GetItemOptionsAsync(itemId, ct),
        };
    }

    public async Task<OrderFormDto?> GetForEditAsync(int id, string userId, CancellationToken ct = default)
    {
        var dto = await db.Orders
            .AsNoTracking()
            .Where(o => o.Id == id && o.UserId == userId)
            .Select(o => new OrderFormDto
            {
                Id = o.Id,
                ItemId = o.ItemId,
                Quantity = o.Quantity,
                Comment = o.Comment,
                Status = o.Status,
            })
            .FirstOrDefaultAsync(ct);

        if (dto is null) return null;

        dto.AvailableItems = await GetItemOptionsAsync(dto.ItemId, ct);
        return dto;
    }

    public async Task<int> CreateAsync(OrderFormDto dto, string userId, CancellationToken ct = default)
    {
        var entity = new Order
        {
            Quantity = dto.Quantity,
            Comment = dto.Comment?.Trim(),
            Status = dto.Status,
            ItemId = dto.ItemId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
        };
        db.Orders.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(OrderFormDto dto, string userId, CancellationToken ct = default)
    {
        var entity = await db.Orders.FirstOrDefaultAsync(o => o.Id == dto.Id, ct);
        if (entity is null || entity.UserId != userId) return false;

        entity.Quantity = dto.Quantity;
        entity.Comment = dto.Comment?.Trim();
        entity.Status = dto.Status;
        entity.ItemId = dto.ItemId;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, string userId, CancellationToken ct = default)
    {
        var entity = await db.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);
        if (entity is null || entity.UserId != userId) return false;

        db.Orders.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private async Task<IEnumerable<SelectListItem>> GetItemOptionsAsync(int? selectedId, CancellationToken ct)
    {
        return await db.Items
            .AsNoTracking()
            .OrderBy(i => i.Title)
            .Select(i => new SelectListItem
            {
                Value = i.Id.ToString(),
                Text = i.Title,
                Selected = selectedId.HasValue && i.Id == selectedId.Value,
            })
            .ToListAsync(ct);
    }
}
