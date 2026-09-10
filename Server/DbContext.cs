using Microsoft.EntityFrameworkCore;

namespace EncryptedDbAtRest.Server;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Tenant> Tenants { get; set; }

    public DbContext(DbContextOptions<DbContext> options)
        : base(options)
    {
    }

    public override int SaveChanges()
    {
        foreach (var entry in ChangeTracker.Entries().Where(p =>
                     p.State == EntityState.Added
                     || p.State == EntityState.Modified).ToArray())
        {
            if (entry.Entity is Instance instance)
            {
                instance.Tenant.PrepareInstance(instance);
            }
        }

        return base.SaveChanges();
    }
}