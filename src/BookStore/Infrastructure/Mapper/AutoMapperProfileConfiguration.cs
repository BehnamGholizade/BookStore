using AutoMapper;
using BookStore.Models;
using BookStore.ViewModels;
using System;

namespace BookStore.Infrastructure.Mapper
{
    //http://stackoverflow.com/a/35451804/4346513
    public class AutoMapperProfileConfiguration : Profile
    {
        [Obsolete]
        protected override void Configure()
        {
            CreateMap<Author, AuthorViewModel>();
            CreateMap<AuthorViewModel, Author>();

            CreateMap<Book, Book>()
                .ForMember(x => x.BookAuthors, opt => opt.Ignore());

            CreateMap<Book, BookViewModel>();
            CreateMap<BookViewModel, Book>();

            CreateMap<Publisher, PublisherViewModel>();
            CreateMap<PublisherViewModel, Publisher>();

            CreateMap<Author, AuthorViewModel>();
            CreateMap<AuthorViewModel, Author>();

            CreateMap<Address, AddressViewModel>();
            CreateMap<AddressViewModel, Address>();

            CreateMap<ApplicationUser, UserViewModel>();
            CreateMap<UserViewModel, ApplicationUser>();
        }
    }
}
