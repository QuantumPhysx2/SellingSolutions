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
    [Authorize]
    public class CartsController : Controller
    {
        private readonly SellingSolutionsContext _context;
        private readonly UserManager<SellingSolutionsUser> _userManager;
        private readonly IHttpContextAccessor _session;

        public CartsController(SellingSolutionsContext context, UserManager<SellingSolutionsUser> userManager, IHttpContextAccessor session)
        {
            _context = context;
            _userManager = userManager;
            _session = session;
        }

        public async Task<IActionResult> Purchase()
        {
            // Get the current session Cart 
            var cartID = _session.HttpContext.Session.GetString("cartID");

            // Get the Cart items
            var carts = _context.Cart.Where(cart => cart.CartID == cartID);

            // Get current session User
            var buyer = _userManager.GetUserName(User);

            // Generate a loop to append every item in 'Cart'
            foreach (Cart cart in carts.ToList())
            {
                // Variable checking the 'Product.ID' matches the product in 'Cart.ItemID'
                var products = await _context.Product.FirstOrDefaultAsync(product => product.ID == cart.ItemID);

                // --- Stock Control Check ---
                if (cart.Quantity > products.Quantity)
                {
                    // ...throw a redirect if the 'Cart.Quantity' exceeds the stock count in 'Product.Quantity'
                    return RedirectToAction("Index", "Products");
                }
                else
                {
                    // Update 'Product.Quantity'
                    products.Quantity -= cart.Quantity;

                    // Update 'Cart.Price' for 'Sales.Cost'
                    cart.Price = cart.Quantity * products.Price;

                    // Update the 'Product' database
                    _context.Update(products);

                    // Auto-assign the following 'Sales' columns using the respective values
                    Sales sales = new Sales
                    {
                        ItemID = cart.ItemID,
                        Quantity = cart.Quantity,
                        Cost = cart.Price,
                        Buyer = buyer,
                        PurchaseDate = DateTime.Now
                    };

                    // Update the 'Sales' database
                    _context.Update(sales);
                }
            }
            // Save all changes
            await _context.SaveChangesAsync();

            // Reset the 'Cart' products and count
            _session.HttpContext.Session.SetString("cartID", "");
            _session.HttpContext.Session.SetInt32("cartCount", 0);

            // Return back to the Razor view 'Products > Index'
            return RedirectToAction(nameof(Index), "Products");
        }

        [Authorize]
        [HttpPost]
        [ActionName("Purchase")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PurchaseConfirmed([Bind("ItemID,Quantity,Cost")] Sales sales)
        {
            var products = await _context.Product.FirstOrDefaultAsync(product => product.ID == sales.ItemID);

            if (products == null)
            {
                return NotFound();
            }

            // Check quantity count...
            if (products.Quantity <= 0)
            {
                // ...throw an error
                return RedirectToAction(nameof(Index));
            }

            // Update 'Product.Quantity'
            products.Quantity -= sales.Quantity;

            // Update 'Sales.Cost'
            /*sales.Cost = sales.Quantity * products.Price;*/

            // Run the 'Update-Database' NPM command
            _context.Update(products);

            // Wait until page fully loads before saving the changes
            await _context.SaveChangesAsync();

            // Return the User to 'Products > Index' on success
            return RedirectToAction(nameof(Index));
            
        }

        // GET: Carts
        public async Task<IActionResult> Index()
        {
            // Retrieve the current session Cart
            var cartID = _session.HttpContext.Session.GetString("cartID");

            // Check if the current session Cart matches any Cart ID in the 'Cart' database
            var carts = _context.Cart.Where(cart => cart.CartID == cartID);

            // Return the mapped varaible 'carts' to the Razor page
            return View(await carts.ToListAsync());
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .FirstOrDefaultAsync(m => m.ID == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,CartID,ItemID,Quantity,Price,PriceGST,PriceTotal")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,CartID,ItemID,Quantity,Price,PriceGST,PriceTotal")] Cart cart)
        {
            var products = await _context.Product.FirstOrDefaultAsync(product => product.ID == cart.ItemID);

            if (id != cart.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    cart.Price = cart.Quantity * products.Price;
                    cart.PriceGST = cart.Price / 11;

                    _context.Update(cart);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.ID))
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
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .FirstOrDefaultAsync(m => m.ID == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Cart.FindAsync(id);

            // Get Cart count
            var checkCount = _session.HttpContext.Session.GetInt32("cartCount");

            // Ternary statement of assigning cart count to null || value of 0
            int cartCount = checkCount == null ? 0 : (int)checkCount;

            _context.Cart.Remove(cart);

            await _context.SaveChangesAsync();

            // Update the Cart Count by -1
            _session.HttpContext.Session.SetInt32("cartCount", --cartCount);

            // Redirect to the 'Index' page on success
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Cart.Any(e => e.ID == id);
        }
    }
}
