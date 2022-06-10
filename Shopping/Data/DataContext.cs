using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shopping.Data.Entities;

namespace Shopping.Data;

public class DataContext : IdentityDbContext<User>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Country>? Countries { get; set; }
    public DbSet<City>? Cities { get; set; }
    public DbSet<Category>? Categories { get; set; }
    public DbSet<State>? States { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Country>(country =>
        {
            country.HasIndex(c => c.Name).IsUnique();
            country.HasMany(c => c.States).WithOne(s => s.Country).OnDelete(DeleteBehavior.Cascade);
        }); 

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();

        modelBuilder.Entity<State>(state =>
        {
            state.HasIndex("Name", "CountryId").IsUnique();
            state.HasOne(s => s.Country).WithMany(c => c.States).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<City>(city =>
        {
            city.HasIndex("Name", "StateId").IsUnique();
            city.HasOne(c => c.State).WithMany(s => s.Cities).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
