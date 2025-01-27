using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using Shared.UserIdentity;

namespace Server.Features.UserIdentity
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserIdentityController : ControllerBase
    {
        private SignInManager<ApplicationUser> signInManager;
        private UserManager<ApplicationUser> userManager;

        public UserIdentityController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpGet("BFFUser")]
        [AllowAnonymous]
        public ActionResult<BFFUserInfoDTO> GetCurrentUser()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BFFUserInfoDTO.Anonymous;
            }

            return new BFFUserInfoDTO()
            {
                Claims = User.Claims.Select(claim => new ClaimValueDTO { Type = claim.Type, Value = claim.Value }).ToList()
            };
        }

        [HttpGet("login/{provider}")]
        public async Task<ActionResult> LoginRedirectToExternalProvider([FromRoute] string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "UserIdentity", new { returnUrl = "/" });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [Authorize]
        [HttpGet("Logout")]
        public async Task<ActionResult> LogoutCurrentUser()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return Redirect("/");
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string ReturnUrl = null)
        {
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info.Principal == null)
            {
                return Redirect("/User/Login");
            }

            var user = await userManager.FindByNameAsync(info.Principal.Identity.Name);
            if (info is not null && user is null)
            {
                ApplicationUser _user = new ApplicationUser
                {
                    UserName = info.Principal.Identity.Name,
                };

                var result = await userManager.CreateAsync(_user);

                if (result.Succeeded)
                {
                    result = await userManager.AddLoginAsync(_user, info);
                    await signInManager.SignInAsync(_user, isPersistent: false, info.LoginProvider);
                    return LocalRedirect("/");
                }
            }

            string pictureURI = info.Principal.Claims.Where(claim => claim.Type == "picture").First().Value;

            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: false);
            return signInResult switch
            {
                Microsoft.AspNetCore.Identity.SignInResult { Succeeded: true } => LocalRedirect("/"),
                _ => Redirect("/Error")
            };
        }
    }
}
