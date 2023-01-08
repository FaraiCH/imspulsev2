// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using docmaster.Areas.Identity.Data;
using docmaster.Models;
using docmaster.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace docmaster.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<docmasterUser> _userManager;
        private readonly IEmailSender _emailSender;
        IEmailService _emailService = null;
        public ForgotPasswordModel(UserManager<docmasterUser> userManager, IEmailSender emailSender, IEmailService emailService)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _emailService = emailService;
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
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }



                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                var content = "<p>Hi Farai,</p>" + $"Please reset your password by < a href = '{HtmlEncoder.Default.Encode(callbackUrl)}' > clicking here </ a >." +
                "<br><p>Regards</p><p>The IMS Pulse Team</p>";

                var emailData = new EmailDataModel
                {
                    EmailToId = "faraichaka@gmail.com",
                    EmailToName = "Farai",
                    EmailSubject = "Test Email",
                    EmailBody =

                    "<table class='wrapper layout-primary' width='100 %' cellpadding='0' cellspacing='0'>" +
                        "<tr>" +
                            "<td align='center'>" +
                                "<table class='content' width='100%' cellpadding='0' cellspacing='0'>" +
                                    "<tr>" +
                                        "<td width='100%' cellpadding='0' cellspacing='0'>" +
                                            "<table class='inner-body' align='center' width='570' cellpadding='0' cellspacing='0'>" +
                                                "<tr>" +
                                                    "<td class='content-cell text-center' >" +
                                                        "<img style='width: 100%' src='http://imspulse.com/storage/app/media/imscc.png' alt='Image'>" +
                                                    "</td>" +
                                                "</tr>" +

                                                "<tr>" +
                                                    "<td class='content-cell'>" +
                                                        content +
                                                    "</td>" +
                                                "</ tr >" +
                                            "</ table >" +
                                        "</ td >" +
                                    "</ tr >" +
                                "</ table >" +
                            "</ td >" +
                        "</ tr >" +
                    "</ table > "
                };
                _emailService.SendEmail(emailData);
                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Reset Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
