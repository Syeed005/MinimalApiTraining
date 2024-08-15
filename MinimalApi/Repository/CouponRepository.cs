using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Database;
using MinimalApi.Repository.IRepository;

namespace MinimalApi.Repository {
    public class CouponRepository : ICouponRepository {
        private readonly ApplicationDbContext _db;

        public CouponRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task CreateAsync(Coupon coupon) {
            //await _db.AddAsync(coupon);
            await _db.Coupons.AddAsync(coupon);
        }

        public async Task<ICollection<Coupon>> GetAllAsync() {
            return await _db.Coupons.ToListAsync();
        }

        public async Task<Coupon> GetAsync(int id, bool tracked = true) {
            if (tracked == false) {
                return await _db.Coupons.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            }
            return await _db.Coupons.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Coupon> GetAsync(string name, bool tracked = false) {
            if (tracked) {
                return await _db.Coupons.AsNoTracking().FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
            }
            return await _db.Coupons.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
        }

        public async Task RemoveAsync(Coupon coupon) {
           _db.Coupons.Remove(coupon);
        }

        public async Task SaveAsync() {
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Coupon coupon) {
            _db.Coupons.Update(coupon);
        }
    }
}
