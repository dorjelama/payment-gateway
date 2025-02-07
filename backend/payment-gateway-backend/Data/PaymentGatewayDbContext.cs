using Microsoft.EntityFrameworkCore;
using payment_gateway_backend.Entities;
using payment_gateway_backend.Models;

namespace payment_gateway_backend.Data;

public class PaymentGatewayDbContext : DbContext
{
    public PaymentGatewayDbContext(DbContextOptions<PaymentGatewayDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<PaymentTransaction> Transactions { get; set; }
    public DbSet<EventLog> EventLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaymentTransaction>()
            .HasOne(t => t.User)
            .WithMany(u => u.Transactions)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}