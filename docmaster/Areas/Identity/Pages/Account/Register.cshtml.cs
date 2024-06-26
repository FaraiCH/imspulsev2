﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using docmaster.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace docmaster.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<docmasterUser> _signInManager;
        private readonly UserManager<docmasterUser> _userManager;
        private readonly IUserStore<docmasterUser> _userStore;
        private readonly IUserEmailStore<docmasterUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<docmasterUser> userManager,
            IUserStore<docmasterUser> userStore,
            SignInManager<docmasterUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Company")]
            [RegularExpression(@"^[a-zA-Z ]*-([0-9]{4})", ErrorMessage = "Company name must have a unique 4-digit key. Example: IMSPulse-1234")]
            public string Company { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                Response.Redirect("/");
            }
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            try
            {
                string passwordCom = null;
                int counter = 0;
                returnUrl ??= Url.Content("~/");
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                

                using (var conn = new MySqlConnection("Server=92.205.25.31; Database=imspulse; Uid=manny; Pwd=@Paradice1;"))
                {
                    conn.Open();
               
                    //// Retrieve all rows
                    using (var cmd = new MySqlCommand("SELECT * FROM imspulse.farai_document_statuses", conn))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                passwordCom = reader.GetString(3);
                              
                                if(Input.Company == passwordCom)
                                {
                                    counter++;
                                }
                            }
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        if (counter > 0)
                        {
                            var user = new docmasterUser { UserName = Input.Email, Email = Input.Email, FirstName = Input.FirstName, LastName = Input.LastName, Company = Input.Company };

                            var result = await _userManager.CreateAsync(user, Input.Password);

                            if (result.Succeeded)
                            {
                                string folderName = "/var/www/html/imspulse/bunch-box/" + Input.Company;
                                string QMSName = "/var/www/html/imspulse/bunch-box/" + Input.Company + "/QMS";
                                string OperationsName = "/var/www/html/imspulse/bunch-box/" + Input.Company + "/Operations";
                                string ResourcesName = "/var/www/html/imspulse/bunch-box/" + Input.Company + "/Resources";
                                // If directory does not exist, create it
                                if (!Directory.Exists(folderName))
                                {
                                    Directory.CreateDirectory(folderName);
                                    if (!Directory.Exists(QMSName))
                                    {
                                        Directory.CreateDirectory(QMSName);
                                    }
                                    if (!Directory.Exists(OperationsName))
                                    {
                                        Directory.CreateDirectory(OperationsName);
                                    }
                                    if (!Directory.Exists(ResourcesName))
                                    {
                                        Directory.CreateDirectory(ResourcesName);
                                    }
                                }
                                _logger.LogInformation("User created a new account with password.");

                                var userId = await _userManager.GetUserIdAsync(user);
                                await _userManager.AddToRoleAsync(user, "Basic");
                                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                                var callbackUrl = Url.Page(
                                    "/Account/ConfirmEmail",
                                    pageHandler: null,
                                    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                                    protocol: Request.Scheme);

                                await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                                {
                                    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                                }
                                else
                                {
                                    await _signInManager.SignInAsync(user, isPersistent: false);
                                    return LocalRedirect(returnUrl);
                                }
                            }
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                        else
                        {
                            TempData["ErrorMes"] = "Company did not match. Please Make sure you use the same company name and pin you registered with";
                            ModelState.AddModelError(Input.Company, "The Company Does Not Exist On This Platform");
                        }

                    }



                }
          
            }
            catch (Exception ex)
            {
                ViewData["Data"] = ex.Message;
               
            }
         
         
            // If we got this far, something failed, redisplay form
            return Page();
        }

        private docmasterUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<docmasterUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(docmasterUser)}'. " +
                    $"Ensure that '{nameof(docmasterUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<docmasterUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<docmasterUser>)_userStore;
        }
    }
}
