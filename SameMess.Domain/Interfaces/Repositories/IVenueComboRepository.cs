using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IVenueComboRepository : IBaseRepository<VenueCombo>
{
    /// <summary>Tất cả combo đang hoạt động (kèm Venue).</summary>
    Task<List<VenueCombo>> GetActiveWithVenueAsync();

    /// <summary>Combo theo id (kèm Venue).</summary>
    Task<VenueCombo?> GetWithVenueAsync(Guid id);
}
