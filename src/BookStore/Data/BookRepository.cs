using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
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
            // replace non alphanumeric chars by space and remove extra spaces
            var regexString = Regex.Replace(Regex.Replace(query, @"[^a-zA-Z0-9 ]", " ").Trim().ToUpperInvariant(), @"\s+", " ");
            // remove duplicates
            var tagsSet = new HashSet<string>(regexString.Split(' ').ToList());

            int tagCount = tagsSet.Count();
            var tagsQueryString = "'" + string.Join("','", tagsSet) + "'";

            return from b in _ctx.Books
                    where (
                    from bt in _ctx.BookTags
                    from t in _ctx.Tags
                    where bt.TagId == t.Id && tagsQueryString.Contains(t.Name)
                    group bt by bt.BookId into grp
                    where grp.Count() == tagCount
                    select grp.Key).Contains(b.Id)
                    select b;
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
                    .Include(t => t.BookTags)
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

                // remove intersect tag Id's (there is no need to change them)
                var tagIds = new HashSet<int>((book.BookTags.Select(a => a.TagId)).Except(bookInDb.BookTags.Select(a => a.TagId)));
                var inDbTagIds = new HashSet<int>((bookInDb.BookTags.Select(a => a.TagId)).Except(book.BookTags.Select(a => a.TagId)));

                // remove deleted tags
                foreach (var tagId in inDbTagIds)
                {
                    var tag = bookInDb.BookTags.Select(a => a).SingleOrDefault(a => a.TagId == tagId);
                    bookInDb.BookTags.Remove(tag);
                }

                // add new tags
                foreach (var tagId in tagIds)
                {
                    var tag = _ctx.Tags.Select(a => new BookTag
                    {
                        TagId = a.Id,
                        Tag = a
                    }).SingleOrDefault(a => a.TagId == tagId);

                    bookInDb.BookTags.Add(tag);
                }

                // copy all other fields
                bookInDb = _mapper.Map(book, bookInDb);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}