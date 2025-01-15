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
using System.Drawing;
using StudentTaskManagement.Controllers;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class AccountController : _BaseController
    {
        protected readonly StudentTaskManagementContext dbContext;
        protected readonly ILogger _logger;
        private readonly UserManager<L1Students> _userManager;
        private readonly SignInManager<L1Students> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(StudentTaskManagementContext dbContext, ILogger<NotificationPresetController> logger, UserManager<L1Students> userManager, SignInManager<L1Students> signInManager, IEmailService emailService, IWebHostEnvironment webHostEnvironment)
        : base(dbContext, logger, userManager, signInManager, emailService, webHostEnvironment)
        {
            this.dbContext = dbContext;
            this._logger = logger;
            this._userManager = userManager;
            this._signInManager = signInManager;
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

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel viewModel, string? returnUrl)
        {
            ViewData["ActiveMenu"] = "LoginRegistration";
            viewModel.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            var user = await _userManager.FindByEmailAsync(viewModel.Email);

            if (user != null && !user.EmailConfirmed && (await _userManager.CheckPasswordAsync(user, viewModel.Password)))
            {
                return Json(new
                {
                    success = false,
                    title = "Email not confirmed yet.",
                    message = "The email address must be confirmed before the account is activated."
                });
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, viewModel.Password, viewModel.RememberMe, true);//false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                    });
                }
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Incorrect Email or Password."
                });
            }
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
        public async Task<IActionResult> Registration(L1StudentsViewModel viewModel)
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
                                                    body {{ font-family: Arial, sans-serif; line-height: 1.6; margin: 0;padding: 0; background-color: #f4f4f4; }}

                                                    .container {{ max-width: 600px; margin: 20px auto; padding: 30px; background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1); }}

                                                    .button {{ background-color: #256D85; color: white !important; padding: 12px 25px; text-decoration: none; border-radius: 4px; display: inline-block; margin: 15px 0; font-weight: bold; }}
                                                    
                                                    .button:hover {{ background-color: #2B4865; color: white; }}

                                                    .link {{ word-break: break-all; color: #666; font-size: 14px; }}

                                                    .header {{ color: #333; margin-bottom: 20px; }}

                                                    .footer {{margin-top: 30px; color: #666; font-size: 14px; }}
                                                </style>
                                            </head>
                                            <body>
                                                <div class='container'>
                                                    <h2 class='header'>Hello, {HttpUtility.HtmlEncode(user.UserName)}!</h2>
                                                    <p>Thanks for joining Taskky. To start using your account, please verify your email address.</p>
                                                    <a href='{confirmationLink}' class='button'>Verify Email</a>
                                                    <p class='link'>If the button doesn't work, Try clicking the link <a href='{confirmationLink}' style='color: #1A4B6E; text-decoration: underline;'>right here</a></p>
                                                    <div class='footer'>
                                                        <p>Best regards,<br>The Taskky Team</p>
                                                    </div>
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

        [HttpGet]
        [AllowAnonymous]
        public IActionResult EmailConfrimationExpired()
        {
            ViewData["ActiveMenu"] = "LoginRegistration";
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResendEmailConfirmation()
        {
            ViewData["ActiveMenu"] = "LoginRegistration";
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendEmailConfirmation(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new
                {
                    success = false,
                    message = "Please provide an email address.",
                    title = "Error!"
                });
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(new
                {
                    success = false,
                    message = "User not found.",
                    title = "Error!"
                });
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                return Json(new
                {
                    success = false,
                    message = "Email is already confirmed.",
                    title = "Information"
                });
            }

            try
            {
                // Generate new confirmation token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action("ConfirmEmail", "Account",
                    new { userId = user.Id, token = token }, Request.Scheme);

                // Prepare email content (using the same template as registration)
                string subject = "Confirm your email";
                string body = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; line-height: 1.6; margin: 0;padding: 0; background-color: #f4f4f4; }}
                            .container {{ max-width: 600px; margin: 20px auto; padding: 30px; background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1); }}
                            .button {{ background-color: #256D85; color: white !important; padding: 12px 25px; text-decoration: none; border-radius: 4px; display: inline-block; margin: 15px 0; font-weight: bold; }}
                            .button:hover {{ background-color: #2B4865; color: white; }}
                            .link {{ word-break: break-all; color: #666; font-size: 14px; }}
                            .header {{ color: #333; margin-bottom: 20px; }}
                            .footer {{margin-top: 30px; color: #666; font-size: 14px; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <h2 class='header'>Hello, {HttpUtility.HtmlEncode(user.UserName)}!</h2>
                            <p>You requested a new confirmation email. To verify your email address, please click the button below.</p>
                            <a href='{confirmationLink}' class='button'>Verify Email</a>
                            <p class='link'>If the button doesn't work, Try clicking the link <a href='{confirmationLink}' style='color: #1A4B6E; text-decoration: underline;'>right here</a></p>
                            <div class='footer'>
                                <p>Best regards,<br>The Taskky Team</p>
                            </div>
                        </div>
                    </body>
                    </html>";

                await _emailService.SendEmailAsync(email, subject, body);

                return Json(new
                {
                    success = true,
                    message = "Confirmation email has been sent. Please check your inbox.",
                    title = "Success!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending confirmation email");
                return Json(new
                {
                    success = false,
                    message = "Failed to send confirmation email. Please try again.",
                    title = "Error!"
                });
            }
        }
        // Action to display images


        [HttpGet]
        // GET: Display Edit Profile page with user data
        public async Task<IActionResult> EditProfile()
        {
            ViewData["ActiveMenu"] = "EditProfile";

            try
            {
                // Get user from database
                var user = await _userManager.FindByIdAsync(LoginStudentId);
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
                    DOB = user.DOB.Date,
                    DOBDisplay = user.DOB.ToString("dd-MM-yyyy"),
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
        public async Task<IActionResult> UpdateProfile(L1StudentsViewModel viewModel, IFormFile? ProfileImage, IFormFile? StudentIdentityCard)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(LoginStudentId);

                    if (user != null)
                    {
                        // Handle Profile Image
                        if (ProfileImage != null && ProfileImage.Length > 0)
                        {
                            string profileUploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "profiles");
                            if (!Directory.Exists(profileUploadsFolder))
                                Directory.CreateDirectory(profileUploadsFolder);

                            string profileFileName = LoginStudentId + "_profile_" + Guid.NewGuid().ToString() + Path.GetExtension(ProfileImage.FileName);
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

                            string idCardFileName = LoginStudentId + "_idcard_" + Guid.NewGuid().ToString() + Path.GetExtension(StudentIdentityCard.FileName);
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

                        user.GuardianName = viewModel.GuardianName;
                        user.GuardianContactNumber = viewModel.GuardianContactNumber;
                        user.GuardianEmailAddress = viewModel.GuardianEmailAddress;
                        user.GuardianRelationship = viewModel.GuardianRelationship;

                        user.EducationStage = viewModel.EducationStage;
                        user.EducationalYear = viewModel.EducationalYear;
                        user.InstitutionName = viewModel.InstitutionName;


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


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            try
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
                return RedirectToAction("EmailConfrimationExpired", "Account");
            }
            catch (Exception ex)
            {
                return View("Error");
            }
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
        public IActionResult ForgotPassword()
        {
            return View();
        }

/*        [HttpPost]
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
        }*/

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() 
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
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
        public async Task<IActionResult> AjaxCheckDuplicateUsername(string username, bool isEditProfile = false)
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
                var isDuplicate = false;

                if (isEditProfile && User.Identity.IsAuthenticated)
                {
                    // Get current user
                    var currentUser = await _userManager.GetUserAsync(User);
                    
                    // Username is duplicate only if it belongs to a different user
                    isDuplicate = user != null && user.Id != currentUser.Id;
                }
                else
                {
                    // For new registration, any existing username is a duplicate
                    isDuplicate = user != null;
                }

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
        public async Task<IActionResult> AjaxCheckDuplicateEmail(string email, bool isEditProfile = false)
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
                var isDuplicate = false;

                if (isEditProfile && User.Identity.IsAuthenticated)
                {
                    // Get current user
                    var currentUser = await _userManager.GetUserAsync(User);
                    
                    // Email is duplicate only if it belongs to a different user
                    isDuplicate = user != null && user.Id != currentUser.Id;
                }
                else
                {
                    // For new registration, any existing email is a duplicate
                    isDuplicate = user != null;
                }

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
    }
}



