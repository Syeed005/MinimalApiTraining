using Microsoft.EntityFrameworkCore;

namespace MinimalApi.Database {
    public class ApplicationDbContext : DbContext {
        public ApplicationDbContext()
        {
            
        }
        public DbSet<Coupon> Coupons { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            
        }
    }
}
