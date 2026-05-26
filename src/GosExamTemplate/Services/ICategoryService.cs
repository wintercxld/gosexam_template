using GosExamTemplate.Dtos.Categories;

namespace GosExamTemplate.Services;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct = default);

    Task<CategoryDto?> GetAsync(int id, CancellationToken ct = default);

    Task<CategoryDto> CreateAsync(CategoryDto dto, CancellationToken ct = default);

    Task<bool> UpdateAsync(CategoryDto dto, CancellationToken ct = default);

    Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}
