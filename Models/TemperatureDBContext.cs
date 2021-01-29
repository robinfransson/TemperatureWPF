using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace TemperatureWPF.Models
{
    public partial class TemperatureDBContext : DbContext
    {
        public TemperatureDBContext()
        {
        }

        public TemperatureDBContext(DbContextOptions<TemperatureDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Indoor> Indoors { get; set; }
        public virtual DbSet<Outdoor> Outdoors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json")
                       .Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("TemperatureDB"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Indoor>(entity =>
            {
                entity.ToTable("Indoor");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<Outdoor>(entity =>
            {
                entity.ToTable("Outdoor");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
