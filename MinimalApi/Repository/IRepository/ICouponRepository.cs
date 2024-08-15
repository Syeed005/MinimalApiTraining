using MinimalApi.Data.Dto;

namespace MinimalApi.Repository.IRepository {
    public interface ICouponRepository {
        Task<ICollection<Coupon>> GetAllAsync();
        Task<Coupon> GetAsync(int id, bool tracked);
        Task<Coupon> GetAsync(string name, bool tracked);
        Task CreateAsync(Coupon coupon);
        Task UpdateAsync(Coupon coupon);
        Task RemoveAsync(Coupon coupon);
        Task SaveAsync();
    }
}
