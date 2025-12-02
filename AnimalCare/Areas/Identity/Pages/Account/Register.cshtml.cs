// Licensed to the .NET Foundation under one or more agreements.
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
using AnimalCare.Models;
using AnimalCare.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace AnimalCare.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly AnimalCareDbContext _context;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            AnimalCareDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public SelectList Roles { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "I am a")]
            public string Role { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Populate role dropdown
            Roles = new SelectList(new[]
            {
                new { Value = "Veterinarian", Text = "Veterinarian" },
                new { Value = "Receptionist", Text = "Receptionist" }
            }, "Value", "Text");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Repopulate roles for return
            Roles = new SelectList(new[]
            {
                new { Value = "Veterinarian", Text = "Veterinarian" },
                new { Value = "Receptionist", Text = "Receptionist" }
            }, "Value", "Text");

            if (ModelState.IsValid)
            {
                // Validate role selection
                if (Input.Role != "Veterinarian" && Input.Role != "Receptionist")
                {
                    ModelState.AddModelError(string.Empty, "Please select a valid role.");
                    return Page();
                }

                // Check if email already has a user account
                var existingUser = await _userManager.FindByEmailAsync(Input.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Input.Email", "This email already has an account. Please login instead.");
                    return Page();
                }

                // Handle Veterinarian registration
                if (Input.Role == "Veterinarian")
                {
                    var vet = await _context.Veterinarians
                        .FirstOrDefaultAsync(v => v.Email == Input.Email && v.IsActive);

                    if (vet == null)
                    {
                        ModelState.AddModelError("Input.Email",
                            "This email is not registered as a Veterinarian in our system. Please contact the administrator.");
                        return Page();
                    }

                    // Check if this Veterinarian already has a user account
                    var existingVetUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.VeterinarianId == vet.Id);

                    if (existingVetUser != null)
                    {
                        ModelState.AddModelError("Input.Email",
                            "This Veterinarian profile already has a user account. Please login instead.");
                        return Page();
                    }

                    // Create user linked to veterinarian
                    var user = CreateUser();
                    user.FirstName = vet.FirstName;
                    user.LastName = vet.LastName;
                    user.PhoneNumber = vet.PhoneNumber;
                    user.VeterinarianId = vet.Id;
                    user.CreatedAt = DateTime.UtcNow;
                    user.IsActive = true;

                    await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        // Assign Veterinarian role
                        await _userManager.AddToRoleAsync(user, "Veterinarian");

                        _logger.LogInformation("Veterinarian created a new account with password.");

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                // Handle Receptionist registration
                else if (Input.Role == "Receptionist")
                {
                    // Check if email exists in Receptionists table
                    var receptionist = await _context.Receptionists
                        .FirstOrDefaultAsync(r => r.Email == Input.Email && r.IsActive);

                    if (receptionist == null)
                    {
                        ModelState.AddModelError("Input.Email",
                            "This email is not registered as a Receptionist in our system. Please contact the administrator.");
                        return Page();
                    }

                    // Create user linked to receptionist
                    var user = CreateUser();
                    user.FirstName = receptionist.FirstName;
                    user.LastName = receptionist.LastName;
                    user.PhoneNumber = receptionist.PhoneNumber;
                    user.ReceptionistId = receptionist.Id;
                    user.CreatedAt = DateTime.UtcNow;
                    user.IsActive = true;

                    await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        // Assign Receptionist role
                        await _userManager.AddToRoleAsync(user, "Receptionist");

                        _logger.LogInformation("Receptionist created a new account with password.");

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}