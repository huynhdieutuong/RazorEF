using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorEF.Models;

namespace RazorEF.Areas.Admin.Pages.User
{
    public class EditUserRoleClaimModel : PageModel
    {
        private readonly MyBlogContext _context;
        private readonly UserManager<AppUser> _userManager;

        public EditUserRoleClaimModel(MyBlogContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Claim type")]
            [Required(ErrorMessage = "{0} is required")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} must be between {2} - {1} chars")]
            public string ClaimType { get; set; }

            [Display(Name = "Claim value")]
            [Required(ErrorMessage = "{0} is required")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} must be between {2} - {1} chars")]
            public string ClaimValue { get; set; }
        }
        public AppUser User { get; set; }

        public NotFoundObjectResult OnGet() => NotFound("Can not access");

        public async Task<IActionResult> OnGetAddClaimAsync(string userId)
        {
            User = await _userManager.FindByIdAsync(userId);
            if (User == null) return NotFound("User not found");
            return Page();
        }

        public async Task<IActionResult> OnPostAddClaimAsync(string userId)
        {
            User = await _userManager.FindByIdAsync(userId);
            if (User == null) return NotFound("User not found");

            if (!ModelState.IsValid) return Page();

            // Get user claims
            var claims = _context.UserClaims.Where(c => c.UserId == userId);

            // Check duplicate user claims
            if (claims.Any(c => c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, $"Claim {Input.ClaimType} : {Input.ClaimValue} is already taken.");
                return Page();
            }

            // Add claim for user
            await _userManager.AddClaimAsync(User, new Claim(Input.ClaimType, Input.ClaimValue));
            StatusMessage = "Add claim for user successfully.";

            return RedirectToPage("./AddRole", new { userId = userId });
        }
    }
}
