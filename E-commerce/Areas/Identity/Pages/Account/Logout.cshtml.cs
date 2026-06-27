using E_commerceEntity.Entity; // Ensure you include your entity namespace
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace E_commerce.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        // 1. Update the field to use APP_User
        private readonly SignInManager<APP_User> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        // 2. Update the constructor injection to use APP_User
        public LogoutModel(SignInManager<APP_User> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            _logger.LogInformation("User logged out.");

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                return RedirectToPage();
            }
        }
    }
}