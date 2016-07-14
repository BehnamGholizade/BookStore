using BookStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using System;

namespace BookStore.Infrastructure.Binders
{
    public class CartModelBinder : IModelBinder
    {
        private const string sessionKey = "Cart";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public CartModelBinder(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<ModelBindingResult> BindModelAsync(ModelBindingContext bindingContext)
        {
            // get the Cart from the session
            Cart cart = null;
            if (_session != null)
            {
                cart = _session.GetObjectFromJson<Cart>(sessionKey);
            }
            // create the Cart if there wasn't one in the session data
            if (cart == null)
            {
                cart = new Cart();
                if (_session != null)
                {
                    _session.SetObjectAsJson(sessionKey, cart);
                }
            }
            return Task.FromResult(ModelBindingResult.Success(bindingContext.ModelName)); // Task.FromResult(ModelBindingResult.Success(bindingContext.ModelName, cart));
        }

        Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
        {
            throw new NotImplementedException();
        }
    }
}
