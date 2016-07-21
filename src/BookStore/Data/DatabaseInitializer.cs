using BookStore.Models;
using FastMember;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BookStore.Data
{
    public interface IDatabaseInitializer
    {
        Task Seed();
    }

    public class DatabaseInitializer : IDatabaseInitializer
    {
        private BookStoreContext _ctx;
        private readonly UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;
        private string _dbName;
        private readonly ILogger _logger;

        public DatabaseInitializer(BookStoreContext context, UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _ctx = context;
            _dbName = _ctx.Database.GetDbConnection().Database;
            _logger = loggerFactory.CreateLogger<DatabaseInitializer>();
        }

        public async Task Seed()
        {
            _ctx.Database.EnsureCreated();

            // Add Mvc.Client to the known applications.
            //if (_ctx.Applications.Any())
            //{
            //    _ctx.RemoveRange(_ctx.Applications);
            //    _ctx.SaveChanges();
            //}

            // initialize tables with demo records
            try
            {
                _ctx.ChangeTracker.AutoDetectChangesEnabled = false;

                if (!_ctx.Countries.Any())
                {
                    _ctx.Countries.Add(new Country { Name = "USA" });
                    _ctx.SaveChanges();
                }

                if (!_ctx.States.Any())
                {
                    BulkWriteToServer<State>(@"../BookStore/Resources/States.json", "States");
                }

                if (!_ctx.Cities.Any())
                {
                    BulkWriteToServer<City>(@"../BookStore/Resources/Cities.json", "Cities");
                }

                if (!_ctx.Zips.Any())
                {
                    BulkWriteToServer<Zip>(@"../BookStore/Resources/Zips.json", "Zips");
                }

                if (await _roleManager.FindByNameAsync("Admin") == null)
                {
                    await _roleManager.CreateAsync(new ApplicationRole() { Name = "Admin" });
                }

                if (await _roleManager.FindByNameAsync("Customer") == null)
                {
                    await _roleManager.CreateAsync(new ApplicationRole() { Name = "Customer" });
                }

                if (!_ctx.Users.Any())
                {
                    var path = @"../BookStore/Resources/Users.json";
                    using (StreamReader r = new StreamReader(new FileStream(path, FileMode.Open)))
                    {
                        var customers = JsonConvert.DeserializeObject<List<ApplicationUser>>(r.ReadToEnd());
                        foreach (var customer in customers)
                        {
                            var user = new ApplicationUser
                            {
                                FirstName = customer.FirstName,
                                LastName = customer.LastName,
                                UserName = customer.UserName,
                                Email = customer.Email,
                                EmailConfirmed = customer.EmailConfirmed,
                                PhoneNumber = customer.PhoneNumber
                            };
                            await _userManager.CreateAsync(user, customer.FirstName + "w0rd!");
                            await _userManager.AddToRoleAsync(user, "Customer");
                        }
                    }

                    string AdminMail = "admin@test.com";
                    if (await _userManager.FindByNameAsync(AdminMail) == null)
                    {
                        var user = new ApplicationUser
                        {
                            FirstName = "Admin",
                            LastName = "Admin",
                            UserName = AdminMail,
                            Email = AdminMail,
                            EmailConfirmed = true,
                            PhoneNumber = "1-(222)333-4444"
                        };
                        await _userManager.CreateAsync(user, "Adminw0rd!");
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }

                    string CustomerMail = "customer@test.com";
                    if (await _userManager.FindByNameAsync(CustomerMail) == null)
                    {
                        var user = new ApplicationUser
                        {
                            FirstName = "Cutomer",
                            LastName = "Customer",
                            UserName = CustomerMail,
                            Email = CustomerMail,
                            EmailConfirmed = true,
                            PhoneNumber = "1-(333)444-5555"
                        };
                        await _userManager.CreateAsync(user, "Customerw0rd!");
                        await _userManager.AddToRoleAsync(user, "Customer");
                    }
                }

                if (!_ctx.Addresses.Any())
                {
                    var path = @"../BookStore/Resources/Addresses.json";
                    using (StreamReader r = new StreamReader(new FileStream(path, FileMode.Open)))
                    {
                        var json = r.ReadToEnd();
                        var data = JsonConvert.DeserializeObject<List<Address>>(json);
                        var keys = ((JObject)JToken.Parse(json).First).Properties().Select(x => x.Name).ToList();
                        var userIds = _ctx.Users.Select(x => x.Id).ToArray();
                        //get the value and index of the current iteration
                        foreach (var address in data.Select((value, i) => new { i, value }))
                        {
                            address.value.UserId = userIds[address.i];
                        }
                        using (var bulkCopy = new SqlBulkCopy(_ctx.Database.GetDbConnection().ConnectionString,
                                SqlBulkCopyOptions.KeepIdentity))
                        using (var reader = ObjectReader.Create(data))
                        {
                            bulkCopy.DestinationTableName = $"[{_dbName}].[dbo].[Addresses]";
                            bulkCopy.BulkCopyTimeout = 300;
                            keys.Add("UserId");
                            foreach (var key in keys)
                            {
                                bulkCopy.ColumnMappings.Add(key, key);
                            }
                            bulkCopy.WriteToServer(reader);
                        }
                    }
                }

                if (!_ctx.Authors.Any())
                {
                    BulkWriteToServer<Author>(@"../BookStore/Resources/Authors.json", "Authors");
                }

                if (!_ctx.Publishers.Any())
                {
                    BulkWriteToServer<Publisher>(@"../BookStore/Resources/Publishers.json", "Publishers");
                }

                if (!_ctx.Books.Any())
                {
                    BulkWriteToServer<Book>(@"../BookStore/Resources/Books.json", "Books");
                }

                if (!_ctx.BookAuthors.Any())
                {
                    BulkWriteToServer<BookAuthor>(@"../BookStore/Resources/BookAuthors.json", "BookAuthors");
                }

                if (!_ctx.OrderStatuses.Any())
                {
                    var orderStatuses = new List<OrderStatus> {
                    new OrderStatus { Name = "Pending", Description = "Awaiting confirmation."},
                    new OrderStatus { Name = "Processing", Description = "In progress."},
                    new OrderStatus { Name = "Complete", Description = "Order has been shipped/picked up, and receipt is confirmed."},
                    new OrderStatus { Name = "Cancelled", Description = "Cancelled."},
                };
                    _ctx.OrderStatuses.AddRange(orderStatuses);
                    _ctx.SaveChanges();
                }

                if (!_ctx.BookTypes.Any())
                {
                    var bookTypes = new List<BookType> {
                    new BookType { Name = "Print" },
                    new BookType { Name = "eBook" },
                };
                    _ctx.BookTypes.AddRange(bookTypes);
                    _ctx.SaveChanges();
                };

                if (!_ctx.Orders.Any())
                {
                    CreateRandomOrders();
                }

                //Full-Text search
                using (var connection = _ctx.Database.GetDbConnection())
                {
                    try
                    {
                        connection.Open();
                        var cmd = connection.CreateCommand();
                        // check Is Full-Text Search Installed
                        cmd.CommandText = "SELECT FULLTEXTSERVICEPROPERTY('IsFullTextInstalled');";
                        var result = (int)cmd.ExecuteScalar();
                        if (result == 1)
                        {
                            // check is Full-Text Catalog exist
                            cmd.CommandText = "SELECT COUNT(1) FROM sys.fulltext_catalogs WHERE [name] = 'BookCatalog'";
                            result = (int)cmd.ExecuteScalar();
                            if (result == 0)
                            {
                                _ctx.Database.ExecuteSqlCommand("CREATE FULLTEXT CATALOG [BookCatalog] WITH ACCENT_SENSITIVITY = ON AS DEFAULT");
                                _ctx.Database.ExecuteSqlCommand("CREATE FULLTEXT INDEX ON [dbo].[Books] ("
                                    + "UpTitle Language 1033, "
                                    + "Title Language 1033, "
                                    + "SubTitle Language 1033, "
                                    + "Isbn Language 1033, "
                                    + "FullDesc Language 1033, "
                                    + "ShortDesc Language 1033"
                                    + ") KEY INDEX PK_Books ON [BookCatalog]; ");
                                _ctx.Database.ExecuteSqlCommand("CREATE FULLTEXT INDEX ON [dbo].[Authors] ("
                                    + "FirstName Language 1033, "
                                    + "LastName Language 1033"
                                    + ") KEY INDEX PK_Authors ON [BookCatalog]; ");
                                _ctx.Database.ExecuteSqlCommand("CREATE FULLTEXT INDEX ON [dbo].[Publishers] ("
                                    + " Name Language 1033 "
                                    + ") KEY INDEX PK_Publishers ON [BookCatalog]; ");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(4, "Full-Text search creation error: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1, ex.Message);
            }
            finally
            {
                _ctx.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }

        /// <summary>
        /// Loads data from file to SQL table using SqlBulkCopy
        /// </summary>
        /// <param name="filePath">JSON data file path</param>
        /// <param name="tableName">table name for loading data</param>
        private void BulkWriteToServer<T>(string filePath, string tableName)
        {
            try
            {
                using (StreamReader r = new StreamReader(new FileStream(filePath, FileMode.Open)))
                {
                    var json = r.ReadToEnd();
                    var data = JsonConvert.DeserializeObject<IEnumerable<T>>(json);
                    var keys = ((JObject)JToken.Parse(json).First).Properties().Select(x => x.Name).ToList();

                    using (var bulkCopy = new SqlBulkCopy(_ctx.Database.GetDbConnection().ConnectionString,
                        // option for insert Id's
                        SqlBulkCopyOptions.KeepIdentity))
                    using (var reader = ObjectReader.Create(data))
                    {
                        bulkCopy.DestinationTableName = $"[{_dbName}].[dbo].[{tableName}]";
                        bulkCopy.BulkCopyTimeout = 300;
                        foreach (var key in keys)
                        {
                            bulkCopy.ColumnMappings.Add(key, key);
                        }
                        bulkCopy.WriteToServer(reader);
                    }
                }
            }
            catch (Exception ex)
            {
               _logger.LogError(2, "BulkWriteToServer<T>() error: " + ex.Message);
            }
        }

        /// <summary>
        /// Creates orders with random values
        /// </summary>
        private void CreateRandomOrders()
        {
            try
            {
                //_ctx.Database.ExecuteSqlCommand($"delete from [{_dbName}].[dbo].[OrderLines]; dbcc checkident ('[{_dbName}].[dbo].[OrderLines]', reseed, 0);");
                //_ctx.Database.ExecuteSqlCommand($"delete from [{_dbName}].[dbo].[Orders]; dbcc checkident ('[{_dbName}].[dbo].[Orders]', reseed, 0);");

                Random rnd = new Random();
                var orderStatusCnt = _ctx.OrderStatuses.Count();
                var userIds = _ctx.Users.Select(x => x.Id).ToArray();
                var addresses = _ctx.Addresses.Select(x => x).ToList();
                var userCnt = userIds.Count();
                var startDay = new DateTime(2016, 1, 1);
                var range = (DateTime.Today - startDay);
                var orders = new List<Order>();

                for (int i = 0; i < 150; i++)
                {
                    var creationDate = startDay + new TimeSpan((long)(rnd.NextDouble() * range.Ticks));
                    var userId = userIds[rnd.Next(userCnt)];
                    orders.Add(new Order
                    {
                        CreationDate = creationDate,
                        Number = creationDate.ToString("yyMMddHHmmss"),
                        UserId = userId,
                        AddressId = addresses.FirstOrDefault(x => x.UserId == userId).Id,
                        StatusId = (rnd.Next(2)%2 == 0) ? 3 : rnd.Next(1, orderStatusCnt + 1)
                    }
                    );
                }
                _ctx.Orders.AddRange(orders);
                _ctx.SaveChanges();

                // create order lines with random values
                var bookCnt = _ctx.Books.Count();
                var bookTypeCnt = _ctx.BookTypes.Count();
                var orderLines = new List<OrderLine>();
                var books = _ctx.Books.Select(x => x).ToList();

                foreach (var order in orders)
                {
                    var orderLineCnt = rnd.Next(1, 6);
                    //random bookIds with no duplicates in each order
                    HashSet<int> bookIds = new HashSet<int>();
                    while (bookIds.Count < orderLineCnt)
                    {
                        bookIds.Add(rnd.Next(1, bookCnt + 1));
                    }

                    foreach (var bookId in bookIds)
                    {
                        var bookTypeId = rnd.Next(1, bookTypeCnt + 1);
                        var book = books.Find(x => x.Id == bookId);

                        orderLines.Add(new OrderLine
                        {
                            BookId = bookId,
                            BookTypeId = bookTypeId,
                            Price = (bookTypeId == 1) ? (book.PrintPrice ?? 0) : (book.EbookPrice ?? 0),
                            OrderId = order.Id,
                            Quantity = rnd.Next(1, 6)
                        });
                    }
                }

                _ctx.OrderLines.AddRange(orderLines);
                _ctx.SaveChanges();

                _ctx.Database.ExecuteSqlCommand(@"update o set o.TotalQty = ol.TotalQty, o.TotalSum = ol.TotalSum " +
                $"from [{_dbName}].[dbo].[Orders] o, (select ol.OrderId, sum(ol.Price * ol.Quantity) TotalSum, sum(ol.Quantity) TotalQty " +
                $"from [{_dbName}].[dbo].[OrderLines] ol group by ol.OrderId) ol where o.Id = ol.OrderId;");

            }
            catch (Exception ex)
            {
                _logger.LogError(3, "CreateRandomOrders() error: " + ex.Message);
            }
        }

        //Does't work in RTM
        //example for identity insert with addRange() method
        //using (var dbContextTransaction = _ctx.Database.BeginTransaction())
        //{
        //    try
        //    {   // insert publishers with Id
        //        _ctx.Database.ExecuteSqlCommand(@"delete from Publisher; dbcc checkident ('Publisher', reseed, 0);");
        //        _ctx.Database.ExecuteSqlCommand(@"set identity_insert Publisher on");
        //        _ctx.Publishers.AddRange(publishers);
        //        _ctx.SaveChanges();
        //        dbContextTransaction.Commit();
        //    }
        //    catch (Exception ex)
        //    {
        //        dbContextTransaction.Rollback();
        //    }
        //    finally
        //    {
        //        _ctx.Database.ExecuteSqlCommand(@"set identity_insert Publisher off");
        //    }
        //}
    }
}
