using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Text.RegularExpressions;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using BookStore.ViewModels;

namespace BookStore.Data
{
    public class BookRepository : GenericRepository<Book>
    {
        public BookRepository(BookStoreContext ctx, IMapper mapper) : base(ctx, mapper) { }

        public IQueryable<Book> GetRequestedBooks(string query)
        {
            var books = _ctx.Books.FromSql("SELECT DISTINCT " +
                "b.[Id],b.[Contents],b.[EbookPrice],b.[FullDesc],b.[ImgCoverUrl],b.[Isbn]," +
                "b.[Pages], b.[PrintPrice], b.[PublicationDate], b.[PublisherId], b.[ShortDesc], b.[SubTitle]," +
                "b.[Title], b.[UpTitle], (bft.[Rank] + ISNULL(aft.[Rank], 0) + ISNULL(pft.[Rank], 0)) as [SearchRank]" +
                "FROM dbo.Books b " +
                "LEFT JOIN dbo.BookAuthors ba ON b.Id = ba.BookId " +
                "LEFT JOIN dbo.Authors a ON ba.AuthorId = a.Id " +
                "LEFT JOIN dbo.Publishers p ON b.PublisherId = p.Id " +
                $"INNER JOIN FREETEXTTABLE(Books, (UpTitle, Title, SubTitle, FullDesc, Isbn, ShortDesc), '{query}') bft ON b.Id = bft.[Key] " +
                $"LEFT JOIN FREETEXTTABLE(Authors,(FirstName, LastName), '{query}') aft ON ba.AuthorId = aft.[Key] " +
                $"LEFT JOIN FREETEXTTABLE(Publishers, Name, '{query}') pft ON b.PublisherId = pft.[Key]");

            return books;
        }

        public IQueryable<Book> GetBestsellers()
        {
            return from b in _ctx.Books
                   join gsub in (from ol in _ctx.OrderLines
                                 group ol by ol.BookId into g
                                 select new { Id = g.Key, Cnt = g.Count() })
                   on b.Id equals gsub.Id
                   orderby gsub.Cnt descending
                   select b;
        }

        public IQueryable<Book> GetAlsoBought(int bookId)
        {
            return from b in _ctx.Books
                   join ol in _ctx.OrderLines
                       on b.Id equals ol.BookId
                   where ol.BookId != bookId
                         && (from o in _ctx.OrderLines
                             where o.BookId == bookId
                             select o.OrderId).Contains(ol.OrderId)
                   select b;
        }

        public override void Update(Book book)
        {
            try
            {
                var bookInDb = _ctx.Books
                    .Include(a => a.BookAuthors)
                    .Include(p => p.Publisher)
                    .SingleOrDefault(b => b.Id == book.Id);

                // remove intersect author Id's (there is no need to change them)
                var authorIds = new HashSet<int>((book.BookAuthors.Select(a => a.AuthorId))
                        .Except(bookInDb.BookAuthors.Select(a => a.AuthorId)));
                var inDbAuthorIds = new HashSet<int>((bookInDb.BookAuthors.Select(a => a.AuthorId))
                    .Except(book.BookAuthors.Select(a => a.AuthorId)));

                // remove deleted authors
                foreach (var authorId in inDbAuthorIds)
                {
                    var author = bookInDb.BookAuthors.Select(a => a).SingleOrDefault(a => a.AuthorId == authorId);
                    bookInDb.BookAuthors.Remove(author);
                }

                // add new authors
                foreach (var authorId in authorIds)
                {
                    var author = _ctx.Authors.Select(a => new BookAuthor
                    {
                        AuthorId = a.Id,
                        Author = a
                    }).SingleOrDefault(a => a.AuthorId == authorId);

                    bookInDb.BookAuthors.Add(author);
                }

                var s = _ctx.Entry(bookInDb).State;
                // copy all other fields
                bookInDb = _mapper.Map(book, bookInDb);
                s = _ctx.Entry(bookInDb).State;
                //_ctx.SaveChangesAsync();
                //base.Update(bookInDb);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}