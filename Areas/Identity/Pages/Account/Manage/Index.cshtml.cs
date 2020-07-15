using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SellingSolutions.Areas.Identity.Data;

namespace SellingSolutions.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<SellingSolutionsUser> _userManager;
        private readonly SignInManager<SellingSolutionsUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public IndexModel(
            UserManager<SellingSolutionsUser> userManager,
            SignInManager<SellingSolutionsUser> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string Firstname { get; set; }

            [Required]
            public string Lastname { get; set; }

            [Required]
            public int Age { get; set; }

            public string Street { get; set; }

            public string Suburb { get; set; }

            public string State { get; set; }

            public string Country { get; set; }

            [DataType(DataType.PostalCode)]
            public string Postcode { get; set; }

            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userName = await _userManager.GetUserNameAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            
            Username = userName;

            // --- Mapping out the Identity User to 'Index' ---
            Input = new InputModel
            {
                UserName = email,
                Email = email,
                PhoneNumber = phoneNumber,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Age = user.Age,
                Street = user.Street,
                Suburb = user.Suburb,
                State = user.State,
                Country = user.Country,
                Postcode = user.Postcode
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // --- Validation Check for Each Input ---
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
                }
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }

            /*if (Input.UserName != Username)
            {
                user.UserName = Input.UserName;
            }*/

            if (Input.Firstname != user.Firstname)
            {
                user.Firstname = Input.Firstname;
            }

            if (Input.Lastname != user.Lastname)
            {
                user.Lastname = Input.Lastname;
            }

            if (Input.Age != user.Age)
            {
                user.Age = Input.Age;
            }

            if (Input.Street != user.Street)
            {
                user.Street = Input.Street;
            }

            if (Input.Suburb != user.Suburb)
            {
                user.Suburb = Input.Suburb;
            }

            if (Input.State != user.State)
            {
                user.State = Input.State;
            }

            if (Input.Country != user.Country)
            {
                user.Country = Input.Country;
            }

            if (Input.Postcode != user.Postcode)
            {
                user.Postcode = Input.Postcode;
            }

            // Update the Identity User on Post 
            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);

            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }
    }
}
