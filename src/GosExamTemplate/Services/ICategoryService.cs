using GosExamTemplate.Dtos.Categories;
using GosExamTemplate.Dtos.Common;

namespace GosExamTemplate.Services;

public interface ICategoryService
{
    Task<PagedIndexDto<CategoryDto>> GetPagedAsync(
        int? page = null,
        int? pageSize = null,
        CancellationToken ct = default);

    Task<CategoryDto?> GetAsync(int id, CancellationToken ct = default);

    Task<CategoryDto> CreateAsync(CategoryDto dto, CancellationToken ct = default);

    Task<bool> UpdateAsync(CategoryDto dto, CancellationToken ct = default);

    Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}
