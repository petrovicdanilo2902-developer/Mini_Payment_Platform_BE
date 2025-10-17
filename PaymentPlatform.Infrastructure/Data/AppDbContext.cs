using Microsoft.EntityFrameworkCore;
using PaymentPlatform.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace PaymentPlatform.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Card> Cards { get; set; }
        public DbSet<Payment> Payments { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(u => u.Email).IsUnique();
                b.Property(u => u.Email).IsRequired();
                b.Property(u => u.PasswordHash).IsRequired();
                b.Property(u => u.FullName).IsRequired();
            });

            // Cards
            builder.Entity<Card>(b =>
            {
                b.ToTable("cards");
                b.HasKey(x => x.Id);
                b.Property(x => x.Brand).HasMaxLength(32).IsRequired();
                b.Property(x => x.Last4).HasMaxLength(4).IsRequired();
                b.Property(x => x.Token).HasMaxLength(128).IsRequired();
                b.HasIndex(x => new { x.UserId, x.CreatedAt });
            });

            // Payments
            builder.Entity<Payment>(b =>
            {
                b.ToTable("payments");
                b.HasKey(x => x.Id);
                b.Property(x => x.Amount).HasColumnType("numeric(18,2)");
                b.Property(x => x.Currency).HasMaxLength(8).IsRequired();
                b.Property(x => x.Status)
                    .HasConversion<string>()          // enum kao tekst
                    .HasMaxLength(16)
                    .IsRequired();

                b.HasIndex(x => new { x.UserId, x.CreatedAt });

                b.HasOne(x => x.Card)
                 .WithMany(c => c.Payments)
                 .HasForeignKey(x => x.CardId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
