using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorEF.Models;

namespace RazorEF.Areas.Admin.Pages.Role
{
    public class AddRoleClaimModel : RolePageModel
    {
        public AddRoleClaimModel(RoleManager<IdentityRole> roleManger, MyBlogContext context) : base(roleManger, context)
        {
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public IdentityRole Role { get; set; }

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

        public async Task<IActionResult> OnGetAsync(string roleId)
        {
            Role = await _roleManager.FindByIdAsync(roleId);
            if (Role == null) return NotFound("Role not found");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string roleId)
        {
            Role = await _roleManager.FindByIdAsync(roleId);
            if (Role == null) return NotFound("Role not found");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 22.1 Check duplicate claim
            var claims = await _roleManager.GetClaimsAsync(Role);
            if (claims.Any(c => c.Type == Input.ClaimType && c.Value == Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, $"Claim {Input.ClaimType} is already taken");
                return Page();
            }

            // 22.2 Create claim
            var newClaim = new Claim(Input.ClaimType, Input.ClaimValue);
            var result = await _roleManager.AddClaimAsync(Role, newClaim);

            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(e =>
                {
                    ModelState.AddModelError(string.Empty, e.Description);
                });
                return Page();
            }

            StatusMessage = "Add claim successfully.";

            return RedirectToPage("./Edit", new { roleId = Role.Id });
        }
    }
}
