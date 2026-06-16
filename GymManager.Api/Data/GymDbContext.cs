using GymManager.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymManager.Api.Data;

public class GymDbContext : DbContext
{
    public GymDbContext(DbContextOptions<GymDbContext> options)
        : base(options)
    {
    }

    public DbSet<Gym> Gyms => Set<Gym>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Member> Members => Set<Member>();

    public DbSet<MembershipPayment> MembershipPayments => Set<MembershipPayment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Member>()
            .HasIndex(x => x.LastName);

        modelBuilder.Entity<Member>()
            .HasIndex(x => x.Phone);

        modelBuilder.Entity<MembershipPayment>()
            .HasIndex(x => x.EndDate);
    }
}