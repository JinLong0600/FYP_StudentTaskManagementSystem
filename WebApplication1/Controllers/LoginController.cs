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

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> List(L1TasksViewModel viewModel)
        {

            var task = await dbContext.L0Admins.ToListAsync();

            return View(task);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {

            var task = await dbContext.L0Admins.FindAsync(id);

            return View(task);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(L1Tasks L1Task)
        {

            var task = await dbContext.L0Admins.FindAsync(L1Task.Id);

            if (task != null)
            {
                dbContext.L1Tasks.Remove(L1Task);
                await dbContext.SaveChangesAsync();
            }


            return View(task);
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



