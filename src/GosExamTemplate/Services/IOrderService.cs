using GosExamTemplate.Dtos.Common;
using GosExamTemplate.Dtos.Orders;

namespace GosExamTemplate.Services;

public interface IOrderService
{
    Task<PagedIndexDto<OrderListDto>> GetMyOrdersAsync(
        string userId,
        int? page = null,
        int? pageSize = null,
        CancellationToken ct = default);

    Task<OrderListDto?> GetAsync(int id, string userId, CancellationToken ct = default);

    Task<OrderFormDto> BuildCreateFormAsync(int? itemId, CancellationToken ct = default);

    Task<OrderFormDto?> GetForEditAsync(int id, string userId, CancellationToken ct = default);

    Task<int> CreateAsync(OrderFormDto dto, string userId, CancellationToken ct = default);

    Task<bool> UpdateAsync(OrderFormDto dto, string userId, CancellationToken ct = default);

    Task<bool> DeleteAsync(int id, string userId, CancellationToken ct = default);
}
