using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SellingSolutions.Areas.Identity.Data;
using SellingSolutions.Models;

namespace SellingSolutions.Controllers
{
    public class ProductsController : Controller
    {
        // --- Interfaces ---
        private readonly SellingSolutionsContext _context;
        private readonly UserManager<SellingSolutionsUser> _userManager;
        private readonly IHttpContextAccessor _session;

        /*
        Self-note:
        - Make sure to use 'SellingSolutionsUser' and NOT 'IdentityUser' to avoid running into the 'InvalidOperationException' error
        */

        public ProductsController(SellingSolutionsContext context, UserManager<SellingSolutionsUser> userManager, IHttpContextAccessor session)
        {
            // --- Global variables ---
            _context = context;
            _userManager = userManager;
            _session = session;
        }

        [Authorize]
        public ActionResult MyPosts()
        {
            // Get the current session User
            var seller = _userManager.GetUserName(User);

            // Compare all products where it is the same as the current session User
            var products = _context.Product.Where(product => product.Seller == seller);

            // Return the 'MyPosts.cshtml' Razor view with 'products' being mapped
            return View("MyPosts", products);
        }

        // Requires a non-empty 'id' parameter
        [Authorize]
        public async Task<IActionResult> Purchase(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get the current product with the matching ID
            var products = await _context.Product.FirstOrDefaultAsync(product => product.ID == id);

            if (products == null)
            {
                return NotFound();
            }

            // Return Razor page with 'products' being mapped
            return View(products);
        }

        [Authorize]
        [HttpPost]
        [ActionName("Purchase")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PurchaseConfirmed([Bind("ItemID,Quantity,Price,PriceGST,PriceTotal")] Cart cart)
        {
            /*
            Note:
            - to avoid future headaches, make sure to include the variable below like so
            - otherwise, using 'Product product' as an additional parameter will incorrectly reference to the DatabaseModel.<Column> attributes
            */
            // Instantiate a variable that GETs the current ID of the product
            var products = await _context.Product.FirstOrDefaultAsync(product => product.ID == cart.ItemID);

            // Get current session 'Cart'
            string cartID = _session.HttpContext.Session.GetString("cartID");

            // Check if there is no existing 'CartID'
            if (string.IsNullOrEmpty(cartID) == true)
            {
                // ...create a new instance of a 'CartID'
                cartID = Guid.NewGuid().ToString();
            }

            // Convert the current session 'CartID' to string
            cart.CartID = cartID.ToString();

            // Update 'Cart.Price' respectively
            cart.Price = cart.Quantity * products.Price;
            cart.PriceGST = cart.Price / 11;

            // --- Stock Check ---
            if (cart.Quantity > products.Quantity)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Create the cart
                _context.Add(cart);

                await _context.SaveChangesAsync();

                // Amount of items in cart
                var checkCount = _session.HttpContext.Session.GetInt32("cartCount");

                // Set number of items in cart to 0, otherwise, to the int of 'checkCount'
                int cartCount = checkCount == null ? 0 : (int)checkCount;

                // Append the 'Cart' database with current Product
                _session.HttpContext.Session.SetString("cartID", cartID.ToString());

                // Update the cart by +1
                _session.HttpContext.Session.SetInt32("cartCount", ++cartCount);

                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Products
        public async Task<IActionResult> Index(string searchString)
        {
            // Use LINQ to query the database
            var products = from entity in _context.Product
                           select entity;

            // Check if we receive a non-empty string...
            if (!String.IsNullOrEmpty(searchString))
            {
                // ...match all records in the 'Product' database with the search string value
                products = products.Where(product => product.Category.Contains(searchString));
            }

            return View(await products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FirstOrDefaultAsync(m => m.ID == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Image,Name,Category,Quantity,Price,Seller")] Product product)
        {
            if (ModelState.IsValid)
            {
                // Get current session User
                var seller = _userManager.GetUserName(User);

                // Auto-assign the 'Product.Seller' attribute to the current session User
                product.Seller = seller;

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Image,Name,Category,Quantity,Price,Seller")] Product product)
        {
            if (id != product.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var seller = _userManager.GetUserName(User);

                    product.Seller = seller;

                    _context.Update(product);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [Authorize]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ID == id);
        }
    }
}
