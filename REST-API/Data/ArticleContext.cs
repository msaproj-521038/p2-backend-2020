using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using REST_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST_API.Data
{
    public class ArticleContext: DbContext
    {
        // an empty constructor
        public ArticleContext() { }

        // base(options) calls the base class's constructor,
        // in this case, our base class is DbContext
        public ArticleContext(DbContextOptions<ArticleContext> options) : base(options) { }

        // Use DbSet<Article> to query or read and 
        // write information about an Article
        public DbSet<Article> Article { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<ArticleField> ArticleField { get; set; }
        public static System.Collections.Specialized.NameValueCollection AppSettings { get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //User has a 1 to many relationship with Article for now.
            modelBuilder.Entity<Article>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(p => p.UserID);

            // Article has a 1 to many relationship with ArticleField.
            modelBuilder.Entity<ArticleField>()
                .HasOne<Article>()
                .WithMany()
                .HasForeignKey(p => p.ArticleID);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
           .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
           .AddJsonFile("appsettings.json")
           .Build();

            // Use sqlite database as it is cheaper, consider switching to MySQL or PostgreSQL for commercial production.
            optionsBuilder.UseSqlite(configuration.GetConnectionString("ArticleContext"));
        }
    }
}
