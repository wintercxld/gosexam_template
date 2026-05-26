using GosExamTemplate.Dtos.Items;

namespace GosExamTemplate.Services;

public interface IItemService
{
    Task<MyItemsFilterDto> GetMyItemsAsync(string ownerId, MyItemsFilterDto filter, CancellationToken ct = default);

    Task<PublicItemsFilterDto> GetPublicItemsAsync(PublicItemsFilterDto filter, CancellationToken ct = default);

    Task<ItemDetailsDto?> GetDetailsAsync(int id, CancellationToken ct = default);

    Task<ItemFormDto?> GetForEditAsync(int id, string ownerId, CancellationToken ct = default);

    Task<ItemFormDto> BuildCreateFormAsync(CancellationToken ct = default);

    Task<int> CreateAsync(ItemFormDto dto, string ownerId, CancellationToken ct = default);

    Task<bool> UpdateAsync(ItemFormDto dto, string ownerId, CancellationToken ct = default);

    Task<bool> DeleteAsync(int id, string ownerId, CancellationToken ct = default);

    Task<bool> IsOwnedByAsync(int itemId, string ownerId, CancellationToken ct = default);
}
