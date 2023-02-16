using IdentityServer4.Models;

namespace IdentityServer4.API.Common
{
    public static class Extensions
    {
        public static bool IsNativeClient(this AuthorizationRequest request)
        {
            // the redirect url from the user should not have http or https inside it because it means this url comes from external resource

            bool isExternalUrl = !request.RedirectUri.StartsWith("https", StringComparison.Ordinal)
                                    && !request.RedirectUri.StartsWith("http", StringComparison.Ordinal);

            return isExternalUrl;
        }
    }
}
