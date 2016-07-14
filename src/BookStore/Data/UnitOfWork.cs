using AutoMapper;
using BookStore.Models;
using System.Threading.Tasks;

namespace BookStore.Data
{
    public class UnitOfWork //: IDisposable
    {
        private BookStoreContext ctx;
        private IMapper mapper;
        private BookRepository bookRepository;
        private GenericRepository<BookType> bookTypeRepository;
        private GenericRepository<Author> authorRepository;
        private GenericRepository<Publisher> publisherRepository;
        private GenericRepository<Tag> tagRepository;
        private OrderRepository orderRepository;
        private GenericRepository<OrderLine> orderLineRepository;
        private GenericRepository<OrderStatus> orderStatusRepository;
        private UserRepository userRepository;
        private GenericRepository<Country> countryRepository;
        private GenericRepository<State> stateRepository;
        private GenericRepository<City> cityRepository;
        private GenericRepository<Zip> zipRepository;
        private GenericRepository<Address> addressRepository;

        public UnitOfWork(BookStoreContext _ctx, IMapper _mapper)
        {
            ctx = _ctx;
            mapper = _mapper;
        }

        public BookRepository BookRepository
        {
            get
            {
                if (bookRepository == null)
                {
                    bookRepository = new BookRepository(ctx, mapper);
                }
                return bookRepository;
            }
        }

        public GenericRepository<BookType> BookTypeRepository
        {
            get
            {
                if (bookTypeRepository == null)
                {
                    bookTypeRepository = new GenericRepository<BookType>(ctx, mapper);
                }
                return bookTypeRepository;
            }
        }

        public GenericRepository<Author> AuthorRepository
        {
            get
            {
                if (authorRepository == null)
                {
                    authorRepository = new GenericRepository<Author>(ctx, mapper);
                }
                return authorRepository;
            }
        }

        public OrderRepository OrderRepository
        {
            get
            {
                if (orderRepository == null)
                {
                    orderRepository = new OrderRepository(ctx, mapper);
                }
                return orderRepository;
            }
        }

        public GenericRepository<OrderLine> OrderLineRepository
        {
            get
            {
                if (orderLineRepository == null)
                {
                    orderLineRepository = new GenericRepository<OrderLine>(ctx, mapper);
                }
                return orderLineRepository;
            }
        }


        public GenericRepository<OrderStatus> OrderStatusRepository
        {
            get
            {
                if (orderStatusRepository == null)
                {
                    orderStatusRepository = new GenericRepository<OrderStatus>(ctx, mapper);
                }
                return orderStatusRepository;
            }
        }

        public UserRepository UserRepository
        {
            get
            {
                if (userRepository == null)
                {
                    userRepository = new UserRepository(ctx, mapper);
                }
                return userRepository;
            }
        }

        public GenericRepository<Publisher> PublisherRepository
        {
            get
            {
                if (publisherRepository == null)
                {
                    publisherRepository = new GenericRepository<Publisher>(ctx, mapper);
                }
                return publisherRepository;
            }
        }

        public GenericRepository<Tag> TagRepository
        {
            get
            {
                if (tagRepository == null)
                {
                    tagRepository = new GenericRepository<Tag>(ctx, mapper);
                }
                return tagRepository;
            }
        }

        public GenericRepository<Country> CountryRepository
        {
            get
            {
                if (countryRepository == null)
                {
                    countryRepository = new GenericRepository<Country>(ctx, mapper);
                }
                return countryRepository;
            }
        }

        public GenericRepository<State> StateRepository
        {
            get
            {
                if (stateRepository == null)
                {
                    stateRepository = new GenericRepository<State>(ctx, mapper);
                }
                return stateRepository;
            }
        }

        public GenericRepository<City> CityRepository
        {
            get
            {
                if (cityRepository == null)
                {
                    cityRepository = new GenericRepository<City>(ctx, mapper);
                }
                return cityRepository;
            }
        }

        public GenericRepository<Zip> ZipRepository
        {
            get
            {
                if (zipRepository == null)
                {
                    zipRepository = new GenericRepository<Zip>(ctx, mapper);
                }
                return zipRepository;
            }
        }

        public GenericRepository<Address> AddressRepository
        {
            get
            {
                if (addressRepository == null)
                {
                    addressRepository = new GenericRepository<Address>(ctx, mapper);
                }
                return addressRepository;
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await ctx.SaveChangesAsync() > 0;
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ctx.Dispose();
                }
                disposedValue = true;
            }
        }

        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}
        #endregion
    }
}
