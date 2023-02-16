namespace Application.Services.Account
{
    public class SignOutResponse
    {
        public string PostLogoutRedirectUri { get; set; }


        public string LogoutId { get; set; }
        public string ExternalAuthenticationScheme { get; set; }

        public SignOutResponse(string postLogoutRedirectUri, string logoutId, string externalAuthenticationScheme)
        {
            PostLogoutRedirectUri = postLogoutRedirectUri;
           
            LogoutId = logoutId;
            ExternalAuthenticationScheme = externalAuthenticationScheme;
        }
        public static SignOutResponse Create(string postLogoutRedirectUri,string logoutId, string externalAuthenticationScheme)
        => new(postLogoutRedirectUri,logoutId, externalAuthenticationScheme);
    }
}
