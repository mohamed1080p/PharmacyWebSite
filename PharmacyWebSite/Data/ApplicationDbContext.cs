using Microsoft.EntityFrameworkCore;
using PharmacyWebSite.Models;

namespace PharmacyWebSite.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Add this new configuration for User
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.IsAdmin)
                    .HasDefaultValue(false); // Default to non-admin

                entity.HasIndex(u => u.Email)
                    .IsUnique(); // Ensure email uniqueness
            });
            modelBuilder.Entity<Medicine>(entity =>
            {
                entity.Property(m => m.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(m => m.Price)
                    .HasColumnType("decimal(18,2)");

                entity.Property(m => m.Stock)
                    .HasDefaultValue(0);
            });

            // Keep your existing configurations below
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Medicine)
                .WithMany(m => m.OrderItems)
                .HasForeignKey(oi => oi.MedicineId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Medicine)
                .WithMany(m => m.CartItems)
                .HasForeignKey(ci => ci.MedicineId);

            modelBuilder.Entity<MedicinePrescription>()
                .HasKey(mp => new { mp.MedicineId, mp.PrescriptionId });

            modelBuilder.Entity<MedicinePrescription>()
                .HasOne(mp => mp.Medicine)
                .WithMany(m => m.MedicinePrescriptions)
                .HasForeignKey(mp => mp.MedicineId);

            modelBuilder.Entity<MedicinePrescription>()
                .HasOne(mp => mp.Prescription)
                .WithMany(p => p.MedicinePrescriptions)
                .HasForeignKey(mp => mp.PrescriptionId);
        }

        // Keep existing DbSet properties
        public DbSet<User> Users { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<MedicinePrescription> MedicinePrescriptions { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
    }
}