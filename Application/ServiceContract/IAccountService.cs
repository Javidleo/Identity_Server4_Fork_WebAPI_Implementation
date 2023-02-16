using Application.Services.Account;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.ServiceContract
{
    public interface IAccountService
    {
        Task<bool> SignInAsync(string userName, string password, bool rememberLogin, string returnUrl, HttpContext context);
        Task<SignOutResponse> SignOutAsync(string logOutId, ClaimsPrincipal user, HttpContext context);

    }
}
