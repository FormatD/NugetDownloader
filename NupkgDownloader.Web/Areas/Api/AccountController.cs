using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using NupkgDownloader.Web.Models.AccountViewModels;
using System.Net.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NupkgDownloader.Web
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
         UserManager<ApplicationUser> userManager,
         SignInManager<ApplicationUser> signInManager,
         ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        [Route("Login")]
        public async Task<IActionResult> Login([FromForm]LoginViewModel model)
        {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return Json(new { status = 0, data = new[] { "" } ,action="/"});
            }
            if (result.RequiresTwoFactor)
            {
                //return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                //ModelState.AddModelError(string.Empty, "RequiresTwoFactor.");
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return Json(new { status = 1, msg = "User account locked out." });
                //return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Json(new { status = 1, msg = "Invalid login attempt." });
            }

            return Json(new object());
            // If we got this far, something failed, redisplay form
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
