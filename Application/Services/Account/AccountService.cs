using Application.Common;
using Application.Common.Exceptions;
using Application.ServiceContract;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace Application.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly TestUserStore _testUsers;
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly IEventService _eventService;


        public AccountService(TestUserStore testUsers, IIdentityServerInteractionService interactionService, IEventService eventService)
        {
            _testUsers = testUsers;
            _interactionService = interactionService;
            _eventService = eventService;
        }

        public async Task<bool> SignInAsync(string userName,string password,bool rememberLogin, string returnUrl,HttpContext context)
        {
            var isValidUser = _testUsers.ValidateCredentials(userName, password);
            if (isValidUser == false )
            {
                await _eventService.RaiseAsync(new UserLoginFailureEvent(userName, "invalid credentials", true));
                return false;
            }

            var request = await _interactionService.GetAuthorizationContextAsync(returnUrl);
            string clientId = context != null ? request.Client.ClientId : string.Empty;

            var user = _testUsers.FindByUsername(userName);
            
            await _eventService.RaiseAsync(new UserLoginSuccessEvent(userName, user.SubjectId, user.Username, true, clientId));

            var authProperties = new AuthenticationProperties();
            if (AccountOptions.AllowRememberLogin && rememberLogin)
            {
                authProperties.IsPersistent = true;
                authProperties.ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration);
            }

            var issuer = new IdentityServerUser(user.SubjectId) { DisplayName = user.Username };

            await context.SignInAsync(issuer, authProperties);
            return true;
        }

        public async Task<SignOutResponse> SignOutAsync(string logOutId,ClaimsPrincipal user,HttpContext context)
        {
            if (user?.Identity.IsAuthenticated is false)
                return null;

            var logOut = await _interactionService.GetLogoutContextAsync(logOutId);

            string idp = GetIDP(user,context).Result;

            if(logOutId is null)
            {
                // if there's no current logout context, we need to create one
                // this captures necessary info from the current logged in user
                // before we signout and redirect away to the external IdP for signout
                logOutId = await _interactionService.CreateLogoutContextAsync();
            }

            if (user?.Identity.IsAuthenticated is true)
            {
                // delete cookies
                await context.SignOutAsync();
                await _eventService.RaiseAsync(new UserLogoutSuccessEvent(user.GetSubjectId(), user.GetDisplayName()));
            }

            return SignOutResponse.Create(logOut.PostLogoutRedirectUri, logOutId, idp);
        }

        private async Task<string> GetIDP(ClaimsPrincipal user, HttpContext context)
        {
            var idp = user.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

            bool isRealExternalProvider = idp != null && idp != Common.IdentityServerConstants.LocalIdentityProvider;
            if (isRealExternalProvider is false)
                throw new InvalidProviderException($"Invalid Provider");

            var providerSupportsSignout = await context.GetSchemeSupportsSignOutAsync(idp);
            if (providerSupportsSignout is false)
                throw new InvalidOperationException($"your provider doesnt support for Signout Operation");

            return idp;
        }
    }
}