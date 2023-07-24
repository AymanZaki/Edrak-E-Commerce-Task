using Edrak.Order.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Edrak.Order.Data
{
    public class OrdersDBContext : DbContext
    {
        public OrdersDBContext(DbContextOptions optionsBuilder) : base(optionsBuilder)
        {
        }

        DbSet<Entities.Order> Orders { get; set; }
        DbSet<OrderLineItem> OrderLineItems { get; set; }
        DbSet<Customer> Customers { get; set; }
        DbSet<OrderStatus> OrderStatuses { get; set; }
        DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();

            var entities = from e in ChangeTracker.Entries()
                           where e.State == EntityState.Added
                               || e.State == EntityState.Modified
                           select e.Entity;
            foreach (var entity in entities)
            {
                var validationContext = new ValidationContext(entity);
                Validator.ValidateObject(
                    entity,
                    validationContext,
                    validateAllProperties: true);
            }

            return await base.SaveChangesAsync();
        }
        public override int SaveChanges()
        {
            OnBeforeSaving();
            var entities = from e in ChangeTracker.Entries()
                           where e.State == EntityState.Added
                               || e.State == EntityState.Modified
                           select e.Entity;
            foreach (var entity in entities)
            {
                var validationContext = new ValidationContext(entity);
                Validator.ValidateObject(
                    entity,
                    validationContext,
                    validateAllProperties: true);
            }

            return base.SaveChanges();
        }
        private void OnBeforeSaving()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is BaseEntity entity)
                {
                    entity.LastModifiedDate = DateTime.UtcNow;
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entity.CreatedDate = DateTime.UtcNow;
                            break;
                    }
                }
            }
        }
    }
}
