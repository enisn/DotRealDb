using DotRealDb.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.Logging;
using Sample.DotRealDb.Web.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.DotRealDb.Web.Server.Data
{
    public class SampleDbContext : DotRealDbContext
    {
        protected SampleDbContext() : base()
        {
        }

        public SampleDbContext(DbContextOptions options, IDotRealChangeTracker tracker) : base(options, tracker)
        {
        }

        public DbSet<WeatherForecast> WeatherForecasts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherForecast>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).HasValueGenerator<GuidValueGenerator>().ValueGeneratedOnAdd();
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
