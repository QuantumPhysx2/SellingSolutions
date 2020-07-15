using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellingSolutions.Areas.Identity.Data;
using SellingSolutions.Models;

namespace SellingSolutions.Controllers
{
    public class HomeController : Controller
    {
        private readonly SellingSolutionsContext _context;
        private readonly UserManager<SellingSolutionsUser> _userManager;

        public HomeController(SellingSolutionsContext context, UserManager<SellingSolutionsUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            // User the 'UserManager' and grab the current session User
            var user = _userManager.GetUserName(User);

            // Use SQL to match all products in the 'Product' database where it is the same as the current session User
            var products = _context.Product.Where(product => product.Seller == user);

            // Return the Razor view with 'products' being mapped
            return View("Index", products);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
