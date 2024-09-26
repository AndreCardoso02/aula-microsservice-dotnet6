using Microsoft.AspNetCore.Authentication;

namespace GeekShopping.IdentityServer.Extensions
{
    public static class HttpContextExtension
    {
        public static async Task<bool> GetSchemeSupportsSignOutAsync(this HttpContext context, string scheme)
        {
            var provider = context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
            var handler = await provider.GetHandlerAsync(context, scheme);
            return (handler != null && handler is IAuthenticationSignOutHandler);
        }
    }
}
