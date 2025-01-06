using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentTaskManagement.Models;
using StudentTaskManagement.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Web;
using static StudentTaskManagement.Utilities.GeneralEnum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly StudentTaskManagementContext dbContext;

        private readonly UserManager<L1Students> _userManager;
        private readonly SignInManager<L1Students> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        public AccountController(UserManager<L1Students> userManager, SignInManager<L1Students> signInManager, ILogger<AccountController> logger, IEmailService emailService, IWebHostEnvironment webHostEnvironment)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._logger = logger;
            this._emailService = emailService;
            this._webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            ViewData["ActiveMenu"] = "LoginRegistration";
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
            };
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Registration()
        {
            ViewData["ActiveMenu"] = "LoginRegistration";
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(L1StudentsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new L1Students { 
                    UserName = viewModel.Username,
                    Email = viewModel.EmailAddress,
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    DOB = viewModel.DOB,
                    Gender = viewModel.Gender,
                    GuardianContactNumber = viewModel.GuardianContactNumber,
                    GuardianEmailAddress = viewModel.GuardianEmailAddress,
                    GuardianName = viewModel.GuardianName,
                    GuardianRelationship = viewModel.GuardianRelationship,
                    EducationalYear = viewModel.EducationalYear,
                    EducationStage = viewModel.EducationStage,
                    InstitutionName = viewModel.InstitutionName,
                    AccountStatus = (int)StudentAccountStatus.Active,
                };
                var result = await _userManager.CreateAsync(user, viewModel.Password);

                if (result.Succeeded)
                {
                    // Email Confirmation token
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", 
                        new { userId = user.Id, token = token }, Request.Scheme);

                    // Prepare email content
                    string subject = "Confirm your email";
                    string body = $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <style>
                                body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                                .container {{ padding: 20px; }}
                                .button {{ 
                                    background-color: #007bff;
                                    color: white;
                                    padding: 10px 20px;
                                    text-decoration: none;
                                    border-radius: 5px;
                                    display: inline-block;
                                    margin: 20px 0;
                                }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <h2>Welcome {HttpUtility.HtmlEncode(user.UserName)}!</h2>
                                <p>Thank you for registering with our Student Task Management System.</p>
                                <p>Please confirm your email address by clicking the button below:</p>
                                <a href='{confirmationLink}' class='button'>Confirm Email</a>
                                <p>If the button doesn't work, you can copy and paste this link into your browser:</p>
                                <p>{confirmationLink}</p>
                                <p>Best regards,<br>Taskky Team</p>
                            </div>
                        </body>
                        </html>";

                    try
                    {
                        await _emailService.SendEmailAsync(
                            "long631998@gmail.com",//user.Email,
                            subject, body);

                        return Json(new
                        {
                            success = true,
                            message = "Registration successful! Please check your email for confirmation.",
                            title = "Success!"
                        });
                    }
                    catch (Exception ex)
                    {
                        // Log the error
                        _logger.LogError(ex, "Error during registration process");
                
                        return Json(new { 
                            success = false,
                            message = "Registration failed. Please try again.",
                            title = "Error!"
                        });
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(viewModel);
        }

        // Action to display images
        public async Task<IActionResult> GetImage(string userId, string imageType)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            string imagePath = imageType.ToLower() == "profile"
                ? user.ProfileImage
                : user.StudentIdentityCard;

            if (string.IsNullOrEmpty(imagePath))
                return NotFound();

            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));
            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            return PhysicalFile(fullPath, "image/jpeg"); // or determine content type based on file extension
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel viewModel, string? returnUrl)
        {
            ViewData["ActiveMenu"] = "LoginRegistration";
            viewModel.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

                var user = await _userManager.FindByEmailAsync(viewModel.Email);

                if (user != null && !user.EmailConfirmed && (await _userManager.CheckPasswordAsync(user, viewModel.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View(viewModel);
                }

                var result = await _signInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password, viewModel.RememberMe, true);//false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            
            return View(viewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        // GET: Display Edit Profile page with user data
        public async Task<IActionResult> EditProfile()
        {
            try
            {
                // Get current user ID
                /*                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }*/

                // Get user from database
                var user = await _userManager.FindByIdAsync("d57f6928-0f01-4b96-a1bf-a07fc3347e28");
                if (user == null)
                {
                    return NotFound();
                }

                // Map user data to view model
                var viewModel = new L1StudentsViewModel
                {
                    Username = user.UserName,
                    EmailAddress = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DOB = user.DOB,
                    Gender = user.Gender,
                    ExistingProfileImagePath = user.ProfileImage,

                    GuardianName = user.GuardianName,
                    GuardianContactNumber = user.GuardianContactNumber,
                    GuardianEmailAddress = user.GuardianEmailAddress,
                    GuardianRelationship = user.GuardianRelationship,

                    EducationStage = user.EducationStage,
                    EducationalYear = user.EducationalYear,
                    InstitutionName = user.InstitutionName,
                    ExistingStudentIdentityCardPath = user.StudentIdentityCard,
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the error
                return RedirectToAction("Error", new { message = "Error loading profile" });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateProfile(L1StudentsViewModel viewModel, IFormFile? ProfileImage, IFormFile? StudentIdentityCard)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var user = await _userManager.FindByIdAsync("d57f6928-0f01-4b96-a1bf-a07fc3347e28");

                    if (user != null)
                    {
                        // Handle Profile Image
                        if (ProfileImage != null && ProfileImage.Length > 0)
                        {
                            string profileUploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "profiles");
                            if (!Directory.Exists(profileUploadsFolder))
                                Directory.CreateDirectory(profileUploadsFolder);

                            string profileFileName = userId + "_profile_" + Guid.NewGuid().ToString() + Path.GetExtension(ProfileImage.FileName);
                            string profileFilePath = Path.Combine(profileUploadsFolder, profileFileName);

                            // Delete old profile image if exists
                            if (!string.IsNullOrEmpty(user.ProfileImage))
                            {
                                string oldProfilePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ProfileImage.TrimStart('/'));
                                if (System.IO.File.Exists(oldProfilePath))
                                    System.IO.File.Delete(oldProfilePath);
                            }

                            // Save new profile image
                            using (var fileStream = new FileStream(profileFilePath, FileMode.Create))
                            {
                                await ProfileImage.CopyToAsync(fileStream);
                            }
                            user.ProfileImage = "/uploads/profiles/" + profileFileName;
                        }

                        // Handle Student Identity Card
                        if (StudentIdentityCard != null && StudentIdentityCard.Length > 0)
                        {
                            string idCardUploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "idcards");
                            if (!Directory.Exists(idCardUploadsFolder))
                                Directory.CreateDirectory(idCardUploadsFolder);

                            string idCardFileName = userId + "_idcard_" + Guid.NewGuid().ToString() + Path.GetExtension(StudentIdentityCard.FileName);
                            string idCardFilePath = Path.Combine(idCardUploadsFolder, idCardFileName);

                            // Delete old ID card if exists
                            if (!string.IsNullOrEmpty(user.StudentIdentityCard))
                            {
                                string oldIdCardPath = Path.Combine(_webHostEnvironment.WebRootPath, user.StudentIdentityCard.TrimStart('/'));
                                if (System.IO.File.Exists(oldIdCardPath))
                                    System.IO.File.Delete(oldIdCardPath);
                            }

                            // Save new ID card
                            using (var fileStream = new FileStream(idCardFilePath, FileMode.Create))
                            {
                                await StudentIdentityCard.CopyToAsync(fileStream);
                            }
                            user.StudentIdentityCard = "/uploads/idcards/" + idCardFileName;
                        }

                        // Update other user properties
                        user.UserName = viewModel.Username;
                        user.Email = viewModel.EmailAddress;
                        user.FirstName = viewModel.FirstName;
                        user.LastName = viewModel.LastName;
                        user.DOB = viewModel.DOB;
                        user.Gender = viewModel.Gender;
                        ProfileImage = viewModel.ProfileImage;

                        user.GuardianName = viewModel.GuardianName;
                        user.GuardianContactNumber = viewModel.GuardianContactNumber;
                        user.GuardianEmailAddress = viewModel.GuardianEmailAddress;
                        user.GuardianRelationship = viewModel.GuardianRelationship;

                        user.EducationStage = viewModel.EducationStage;
                        user.EducationalYear = viewModel.EducationalYear;
                        user.InstitutionName = viewModel.InstitutionName;
                        StudentIdentityCard = viewModel.StudentIdentityCard;


                        var result = await _userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            return Json(new
                            {
                                success = true,
                                message = "Profile updated successfully",
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating profile");
                }
            }
            return Json(new { success = false, message = "Failed to update profile" });
        }



        #region
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, returnUrl);
            return new ChallengeResult(provider, properties);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Action("~/");

            LoginViewModel loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");

                return View("Index", loginViewModel);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information.");

                return View("Index", loginViewModel);
            }

            // Email confirmation
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            L1Students user = null;

            if (email != null)
            {
                user = await _userManager.FindByEmailAsync(email);
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View("Index", loginViewModel);
                }
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                if (email != null)
                {
                    user = await _userManager.FindByEmailAsync(email);

                    if (user == null)//if (user != null) 
                    {
                        user = new L1Students
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                        };

                        await _userManager.CreateAsync(user);

                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
                        _logger.Log(LogLevel.Warning, confirmationLink);

                        ViewBag.ErrorTitle = "Registration Successful";
                        ViewBag.ErrorMessage = "Before you can Login, please confirm your" + "email, by clicking on the confirmation link we have emailed you";
                        return View("Error");
                    }
                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }
            }

            ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
            ViewBag.ErrorMessage = "Please contact support on";
            return View("Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            ViewData["ActiveMenu"] = "LoginRegistration";
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Account");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"This User ID {userId} is invalid";
                return View("NotFound");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View();
            }

            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(viewModel.Email);
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var passwordResetLink = Url.Action("RestPassword", "Account", new { email = viewModel.Email, token = token }, Request.Scheme);

                    _logger.Log(LogLevel.Warning, passwordResetLink);

                    return View("ForgotPasswordConfirmation");
                }
                return View("ForgotPasswordConfirmation");
            }
            return View(viewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(viewModel.Email);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, viewModel.Token, viewModel.Password);
                    if (result.Succeeded)
                    {
                        // lockout
                        if (await _userManager.IsLockedOutAsync(user))
                        {
                            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                        }

                        return View("ResetPasswordConfirmation");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(viewModel);
                }
                return View("ResetPasswordConfirmation");
            }
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);

            var userHasPassword = await _userManager.HasPasswordAsync(user);

            if (!userHasPassword) 
            {
                return RedirectToAction("ChangePassword");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Index");
                }

                var result = await _userManager.ChangePasswordAsync(user, viewModel.CurrentPassword, viewModel.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                await _signInManager.RefreshSignInAsync(user);
                return View("ChangePasswordConfirmation");
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AddPassword()
        {
            var user = await _userManager.GetUserAsync(User);

            var userHasPassword = await _userManager.HasPasswordAsync(user);

            if (userHasPassword)
            {
                return RedirectToAction("ChangePassword");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPassword(AddPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                var result = await _userManager.AddPasswordAsync(user, viewModel.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                await _signInManager.RefreshSignInAsync(user);
                return View("AddPasswordConfirmation");
            }

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Logout() 
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("", "");
        }


        public _GeneralViewModel PasswordVerification(string password, byte[]? salt = null)
        {
            _GeneralViewModel viewModel = new _GeneralViewModel();

            string hashed = string.Empty;
            if (salt == null)
            {
                salt = RandomNumberGenerator.GetBytes(128 / 8);
                hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!, salt: salt, prf: KeyDerivationPrf.HMACSHA256, iterationCount: 100000, numBytesRequested: 256 / 8));

                viewModel.Password = password;
                viewModel.PasswordSalt = salt;
                viewModel.HashedPassword = hashed;

                return (viewModel);
            }
            else
            {
                hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!, salt: salt, prf: KeyDerivationPrf.HMACSHA256, iterationCount: 100000, numBytesRequested: 256 / 8));

                viewModel.IsPasswordSameHased = password == hashed ? true : false;

                return (viewModel);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDeined()
        { 
            return View(); 
        }

        #endregion

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AjaxCheckDuplicateUsername(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return Json(new
                    {
                        success = false,
                        isDuplicate = false,
                        message = "Username cannot be empty"
                    });
                }

                var user = await _userManager.FindByNameAsync(username);
                var isDuplicate = user != null;

                return Json(new
                {
                    success = true,
                    isDuplicate = isDuplicate,
                    message = isDuplicate ? "This username is already taken" : "Username is available"
                });
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, "Error checking duplicate username");
                return Json(new
                {
                    success = false,
                    isDuplicate = false,
                    message = "Error checking username availability"
                });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AjaxCheckDuplicateEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return Json(new
                    {
                        success = false,
                        isDuplicate = false,
                        message = "Email cannot be empty"
                    });
                }

                var user = await _userManager.FindByEmailAsync(email);
                var isDuplicate = user != null;

                return Json(new
                {
                    success = true,
                    isDuplicate = isDuplicate,
                    message = isDuplicate ? "This email is already registered" : "Email is available"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking duplicate email");
                return Json(new
                {
                    success = false,
                    isDuplicate = false,
                    message = "Error checking email availability"
                });
            }
        }
    }
}



