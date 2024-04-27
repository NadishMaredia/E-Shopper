
using Microsoft.AspNetCore.Authentication;

namespace EShop.Services.ShoppingCartAPI.Utility
{
    public class ApiAuthHttpClientHandler : DelegatingHandler
    {
        private readonly HttpContextAccessor _contextAccessor;
        public ApiAuthHttpClientHandler(HttpContextAccessor _contextAccessor)
        {
            this._contextAccessor = _contextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _contextAccessor.HttpContext.GetTokenAsync("access-token");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }


    }
}
