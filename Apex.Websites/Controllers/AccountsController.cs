using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Apex.Data.Entities.Accounts;
using Apex.Services.Emails;
using Apex.Websites.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Apex.Websites.Controllers
{
    public class AccountsController : Controller
    {
        private readonly ILogger<AccountsController> _logger;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly IQueuedEmailService _queuedEmailService;

        public AccountsController(
            ILogger<AccountsController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IQueuedEmailService queuedEmailService)
        {
            _logger = logger;

            _userManager = userManager;
            _signInManager = signInManager;

            _queuedEmailService = queuedEmailService;
        }

        #region SignIn & SignOut

        public IActionResult SignIn(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName.Trim(), model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    ApplicationUser user = await GetCurrentUserAsync();

                    return RedirectToLocal(returnUrl);
                }

                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, $"[SignIn] User account: {model.UserName} locked out.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            return View(model);
        }

        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                return View("Error");
            }

            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);

            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();

            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                return View("Error");
            }

            // Generate the token and send it.
            string code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);

            if (string.IsNullOrEmpty(code))
            {
                return View("Error");
            }

            string message = "Your security code is: " + code;

            if (model.SelectedProvider == "Email")
            {
                await SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);
            }
            else if (model.SelectedProvider == "Phone")
            {
                //await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
            }

            return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login.
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                return View("Error");
            }

            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel verifyCodeModel)
        {
            if (!ModelState.IsValid)
            {
                return View(verifyCodeModel);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(verifyCodeModel.Provider, verifyCodeModel.Code, verifyCodeModel.RememberMe, verifyCodeModel.RememberBrowser);

            if (result.Succeeded)
            {
                return RedirectToLocal(verifyCodeModel.ReturnUrl);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning(2, $"[VerifyCode] User account locked out.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid code.");
            }

            return View(verifyCodeModel);
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToLocal();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            string redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { ReturnUrl = returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");

                return View("SignIn");
            }

            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();

            if (externalLoginInfo == null)
            {
                return RedirectToAction("SignIn");
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, isPersistent: false);

            if (result.Succeeded)
            {
                // Update any authentication tokens if login succeeded.
                await _signInManager.UpdateExternalAuthenticationTokensAsync(externalLoginInfo);

                return RedirectToLocal(returnUrl);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning(2, $"[ExternalLoginCallback] External Login Provider: {externalLoginInfo.LoginProvider} locked out.");

                return RedirectToAction(nameof(SignIn));
            }

            // If the user does not have an account, then ask the user to create an account.
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["LoginProvider"] = externalLoginInfo.LoginProvider;

            var email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);

            return View(nameof(ExternalLoginConfirmation), new ExternalLoginConfirmationViewModel { Email = email });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel externalLoginModel, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            // Get the information about the user from the external login provider.
            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();

            if (externalLoginInfo == null)
            {
                return View("ExternalLoginFailure");
            }

            var user = new ApplicationUser { UserName = externalLoginModel.Email, Email = externalLoginModel.Email };
            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                result = await _userManager.AddLoginAsync(user, externalLoginInfo);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Update any authentication tokens as well.
                    await _signInManager.UpdateExternalAuthenticationTokensAsync(externalLoginInfo);

                    return RedirectToLocal(returnUrl);
                }
            }

            AddErrorsToModelState(result);

            return View(externalLoginModel);
        }

        #endregion

        #region Register

        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = model.UserName.Trim(),
                    Email = model.UserName.Trim()
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link.
                    string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    string callbackUrl = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

                    await SendEmailAsync(
                        model.Email,
                        "Confirm your account",
                        $"Please confirm your account by clicking this link: <a href=\"{callbackUrl}\">link</a>");

                    return RedirectToAction(nameof(RegisterEmail));
                }

                AddErrorsToModelState(result);
            }

            return View(model);
        }

        public IActionResult RegisterEmail()
        {
            return View();
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(code))
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user != null)
                {
                    var result = await _userManager.ConfirmEmailAsync(user, code);

                    if (result.Succeeded)
                    {
                        return View();
                    }
                }
            }

            return View("ConfirmEmailError");
        }

        #endregion

        #region Forgot Password

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);

                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed.
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link.
                string code = await _userManager.GeneratePasswordResetTokenAsync(user);
                string callbackUrl = Url.Action(nameof(ResetPassword), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

                await SendEmailAsync(
                    model.Email,
                    "Reset Password",
                    "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");

                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            return View(model);
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        public IActionResult ResetPassword(string code = null)
        {
            return string.IsNullOrEmpty(code) ? View("ResetPasswordError") : View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);

                if (user == null)
                {
                    // Don't reveal that the user does not exist.
                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                }

                AddErrorsToModelState(result);
            }

            return View(model);
        }

        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        #endregion

        #region Helpers

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        private void AddErrorsToModelState(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl = null)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private async Task SendEmailAsync(string email, string subject, string message)
        {
            await _queuedEmailService.CreateAsync(email, subject, message);
        }

        #endregion
    }
}
