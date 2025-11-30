using System.Threading.Tasks;
using AnimalCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AnimalCare.Controllers
{
    [Authorize]
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

        // GET: Account/Profile
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        // POST: Account/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Check if anything changed
            bool profileChanged =
                model.FirstName != user.FirstName ||
                model.LastName != user.LastName ||
                model.Email != user.Email ||
                model.PhoneNumber != user.PhoneNumber;

            bool passwordChangeRequested =
                !string.IsNullOrWhiteSpace(model.CurrentPassword) ||
                !string.IsNullOrWhiteSpace(model.NewPassword) ||
                !string.IsNullOrWhiteSpace(model.ConfirmPassword);

            // If nothing changed, show error
            if (!profileChanged && !passwordChangeRequested)
            {
                ModelState.AddModelError(string.Empty, "No changes were made. Please update at least one field.");
                return View(model);
            }

            // If user wants to change password, validate all password fields
            if (passwordChangeRequested)
            {
                if (string.IsNullOrWhiteSpace(model.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is required to make any changes.");
                    return View(model);
                }

                // Verify current password FIRST before doing anything
                var passwordCheck = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
                if (!passwordCheck)
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                    return View(model);
                }

                // If changing password, validate new password fields
                if (!string.IsNullOrWhiteSpace(model.NewPassword) || !string.IsNullOrWhiteSpace(model.ConfirmPassword))
                {
                    if (string.IsNullOrWhiteSpace(model.NewPassword))
                    {
                        ModelState.AddModelError("NewPassword", "New password is required.");
                        return View(model);
                    }

                    if (string.IsNullOrWhiteSpace(model.ConfirmPassword))
                    {
                        ModelState.AddModelError("ConfirmPassword", "Please confirm your new password.");
                        return View(model);
                    }

                    if (model.NewPassword != model.ConfirmPassword)
                    {
                        ModelState.AddModelError("ConfirmPassword", "The new password and confirmation password do not match.");
                        return View(model);
                    }
                }
            }
            else if (profileChanged)
            {
                // If changing profile info WITHOUT password, require current password for security
                if (string.IsNullOrWhiteSpace(model.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is required to make any changes.");
                    return View(model);
                }

                // Verify current password
                var passwordCheck = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
                if (!passwordCheck)
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                    return View(model);
                }
            }

            // Remove password fields from model validation
            ModelState.Remove("CurrentPassword");
            ModelState.Remove("NewPassword");
            ModelState.Remove("ConfirmPassword");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Update profile information
            if (profileChanged)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;

                // Check if email changed
                if (model.Email != user.Email)
                {
                    var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                    if (!setEmailResult.Succeeded)
                    {
                        foreach (var error in setEmailResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }

                    // Update username to match email
                    var setUsernameResult = await _userManager.SetUserNameAsync(user, model.Email);
                    if (!setUsernameResult.Succeeded)
                    {
                        foreach (var error in setUsernameResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }
                }

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            // Handle password change if both fields provided
            bool passwordChanged = false;
            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                var passwordResult = await _userManager.ChangePasswordAsync(
                    user,
                    model.CurrentPassword!,
                    model.NewPassword);

                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                passwordChanged = true;
                _logger.LogInformation("User changed their password successfully.");
            }

            // Refresh sign-in
            await _signInManager.RefreshSignInAsync(user);

            // Set appropriate success message
            if (profileChanged && passwordChanged)
            {
                TempData["SuccessMessage"] = "Your profile and password have been updated successfully.";
            }
            else if (passwordChanged)
            {
                TempData["SuccessMessage"] = "Your password has been changed successfully.";
            }
            else
            {
                TempData["SuccessMessage"] = "Your profile has been updated successfully.";
            }

            return RedirectToAction(nameof(Profile));
        }


        //verify password

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyPassword([FromBody] string password)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false });
            }

            var isValid = await _userManager.CheckPasswordAsync(user, password);
            return Json(new { success = isValid });
        }

    }
}