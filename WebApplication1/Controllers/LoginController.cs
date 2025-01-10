using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentTaskManagement.Models;
using StudentTaskManagement.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace WebApplication1.Controllers
{
    public class LoginController : Controller
    {
        private readonly StudentTaskManagementContext dbContext;
        public IActionResult Index()
        {
            return View();
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
    }
}



