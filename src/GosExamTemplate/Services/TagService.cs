using GosExamTemplate.Data;
using GosExamTemplate.Dtos.Tags;
using GosExamTemplate.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace GosExamTemplate.Services;

public class TagService(ApplicationDbContext db) : ITagService
{
    public async Task<IReadOnlyList<TagDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await db.Tags
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                ItemsCount = t.ItemTags.Count,
            })
            .ToListAsync(ct);
    }

    public async Task<TagDto?> GetAsync(int id, CancellationToken ct = default)
    {
        return await db.Tags
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                ItemsCount = t.ItemTags.Count,
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<TagDto> CreateAsync(TagDto dto, CancellationToken ct = default)
    {
        var entity = new Tag { Name = dto.Name.Trim() };
        db.Tags.Add(entity);
        await db.SaveChangesAsync(ct);
        dto.Id = entity.Id;
        return dto;
    }

    public async Task<bool> UpdateAsync(TagDto dto, CancellationToken ct = default)
    {
        var entity = await db.Tags.FirstOrDefaultAsync(t => t.Id == dto.Id, ct);
        if (entity is null) return false;

        entity.Name = dto.Name.Trim();
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await db.Tags.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (entity is null) return false;

        db.Tags.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
