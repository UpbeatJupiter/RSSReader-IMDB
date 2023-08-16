using System;
using Microsoft.EntityFrameworkCore;
using RSSWebApp.Data.Entity;

namespace RSSWebApp.Data.Context
{
    public class IMDBContext : DbContext
    {
        public IMDBContext()
        {

        }

		public IMDBContext(DbContextOptions options) : base(options) { }
		public DbSet<Movie> Movies { get; set; }
        public DbSet<Award> Awards { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MovieCategory> MovieCategories { get; set; }
        public DbSet<CastRole> CastRoles { get; set; }
        public DbSet<Cast> Casts { get; set; }
        public DbSet<MovieCastRole> MovieCastRoles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost;Database=IMDB;trusted_connection=true;TrustServerCertificate=true;");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cast>()
                .HasMany<Award>(c => c.Awards)
                .WithOne(a => a.Cast);

            modelBuilder.Entity<Award>()
                .HasOne(a => a.Cast)
                .WithMany(c => c.Awards);

            modelBuilder.Entity<Movie>()
                 .Property(x => x.MovieRate).HasMaxLength(10);

            modelBuilder.Entity<MovieCategory>().HasKey(mc => new { mc.MovieId, mc.CategoryId });

            modelBuilder.Entity<MovieCategory>()
                .HasOne<Movie>(mc => mc.Movie)
                .WithMany(mcat => mcat.MovieCategories)
                .HasForeignKey(mc => mc.MovieId);

            modelBuilder.Entity<MovieCategory>()
                .HasOne<Category>(mc => mc.Category)
                .WithMany(mcat => mcat.MovieCategories)
                .HasForeignKey(mc => mc.CategoryId);

            modelBuilder.Entity<MovieCastRole>().HasKey(mcr => new { mcr.MovieId, mcr.CastId, mcr.CastRoleId });


        }


    }

}

