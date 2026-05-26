using GosExamTemplate.Dtos.Tags;

namespace GosExamTemplate.Services;

public interface ITagService
{
    Task<IReadOnlyList<TagDto>> GetAllAsync(CancellationToken ct = default);

    Task<TagDto?> GetAsync(int id, CancellationToken ct = default);

    Task<TagDto> CreateAsync(TagDto dto, CancellationToken ct = default);

    Task<bool> UpdateAsync(TagDto dto, CancellationToken ct = default);

    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
