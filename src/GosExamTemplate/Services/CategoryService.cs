using GosExamTemplate.Data;
using GosExamTemplate.Dtos.Categories;
using GosExamTemplate.Dtos.Common;
using GosExamTemplate.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace GosExamTemplate.Services;

public class CategoryService(ApplicationDbContext db) : ICategoryService
{
    public async Task<PagedIndexDto<CategoryDto>> GetPagedAsync(
        int? page = null,
        int? pageSize = null,
        CancellationToken ct = default)
    {
        var (normalizedPage, normalizedPageSize) = Pagination.Normalize(page, pageSize);
        var query = db.Categories.AsNoTracking();

        var totalCount = await query.CountAsync(ct);
        var totalPages = Pagination.GetTotalPages(totalCount, normalizedPageSize);
        normalizedPage = Pagination.ClampPage(normalizedPage, totalPages);

        var items = await query
            .OrderBy(c => c.Name)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ItemsCount = c.Items.Count,
            })
            .ToListAsync(ct);

        return new PagedIndexDto<CategoryDto>
        {
            Items = items,
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            TotalCount = totalCount,
        };
    }

    public async Task<CategoryDto?> GetAsync(int id, CancellationToken ct = default)
    {
        return await db.Categories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ItemsCount = c.Items.Count,
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<CategoryDto> CreateAsync(CategoryDto dto, CancellationToken ct = default)
    {
        var entity = new Category
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
        };
        db.Categories.Add(entity);
        await db.SaveChangesAsync(ct);
        dto.Id = entity.Id;
        return dto;
    }

    public async Task<bool> UpdateAsync(CategoryDto dto, CancellationToken ct = default)
    {
        var entity = await db.Categories.FirstOrDefaultAsync(c => c.Id == dto.Id, ct);
        if (entity is null) return false;

        entity.Name = dto.Name.Trim();
        entity.Description = dto.Description?.Trim();
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (entity is null) return false;

        db.Categories.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => db.Categories.AnyAsync(c => c.Id == id, ct);
}
