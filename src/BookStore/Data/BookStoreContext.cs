using BookStore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BookStore.Data
{
    public class BookStoreContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public BookStoreContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().HasIndex(x => x.PublicationDate);
            modelBuilder.Entity<Order>().HasIndex(x => x.CreationDate);

            modelBuilder.Entity<BookAuthor>()
                .HasKey(x => new { x.BookId, x.AuthorId });

            modelBuilder.Entity<BookAuthor>()
                .HasOne(b => b.Book)
                .WithMany(ba => ba.BookAuthors)
                .HasForeignKey(b => b.BookId);

            modelBuilder.Entity<BookAuthor>()
                .HasOne(b => b.Author)
                .WithMany(ba => ba.BookAuthors)
                .HasForeignKey(b => b.AuthorId);

            modelBuilder.Entity<Address>()
                .HasOne(a => a.Country)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Address>()
                .HasOne(a => a.State)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Address>()
                .HasOne(a => a.City)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Address>()
                .HasOne(a => a.Zip)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Country>()
                .HasIndex(x => x.Name);

            modelBuilder.Entity<State>()
                .HasIndex(x => x.CountryId);
            modelBuilder.Entity<State>()
                .HasIndex(x => x.Name);

            modelBuilder.Entity<City>()
                .HasIndex(x => x.StateId);
            modelBuilder.Entity<City>()
                .HasIndex(x => x.Name);

            modelBuilder.Entity<Zip>()
                .HasIndex(x => x.CityId);
            modelBuilder.Entity<Zip>()
                .HasIndex(x => x.Code);

            base.OnModelCreating(modelBuilder);
        }

        // Products
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }

        // Orders
        public DbSet<BookType> BookTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Zip> Zips { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
    }
}