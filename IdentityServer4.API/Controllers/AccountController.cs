using Application.ServiceContract;
using IdentityServer4.API.DTO;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController:ControllerBase
    {
        private readonly TestUserStore _testUsers;
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _eventService;
        private readonly IAccountService _accountService;
        public AccountController(TestUserStore testUsers, IIdentityServerInteractionService interactionService,
                                    IClientStore clientStore, IAuthenticationSchemeProvider schemeProvider,
                                    IEventService eventService, IAccountService accountService)
        {
            _testUsers = testUsers;
            _interactionService = interactionService;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _eventService = eventService;
            _accountService = accountService;
        }

        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(dto);

            var isSuccessSignIn = await _accountService.SignInAsync(dto.UserName, dto.Password, dto.RememberLogin,dto.ReturnUrl, HttpContext);
            if(isSuccessSignIn is false)
                return BadRequest("invalid User Credentials");

            // need to add some logic here.
            return Ok("user login was successfull");

            // in this part user should redirect to the return url. i will compelete this part when I need to interact with UI

            //    if (context.IsNativeClient())
            //    {
            //        return RedirectToAction(dto.ReturnUrl); // its a test one we cant use it for now;
            //    }
            //    return RedirectToAction(dto.ReturnUrl);
            //}
            //if (string.IsNullOrEmpty(dto.ReturnUrl))
            //    return Redirect("Home page"); // its should decleare before use

            //if (Url.IsLocalUrl(dto.ReturnUrl))
            //    return Redirect(dto.ReturnUrl);

            //else
            //    throw new InvalidDataException("invalid Return Url");
            
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Logout([FromBody]string logoutId)
        {
            var response = await _accountService.SignOutAsync(logoutId, User, HttpContext);

            string url = Url.Action("Logout", new { logoutId = response.LogoutId });
            return SignOut(new AuthenticationProperties { RedirectUri = url }, response.ExternalAuthenticationScheme);
        }


    }
}
