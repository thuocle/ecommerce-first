namespace API_Test1.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public virtual DbSet<ProductTypes> ProductTypes { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<ProductImages> ProductImages { get; set; }
        public virtual DbSet<ProductReviews> ProductReviews { get; set; }
        public virtual DbSet<Payments> Payments { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<OrderDetails> OrderDetails { get; set; }
        public virtual DbSet<OrderStatuses> OrderStatuses { get; set; }

    }
}
