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
    public class EditRoleClaimModel : RolePageModel
    {
        public EditRoleClaimModel(RoleManager<IdentityRole> roleManger, MyBlogContext context) : base(roleManger, context)
        {
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public IdentityRole Role { get; set; }
        public IdentityRoleClaim<string> Claim { get; set; }

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

        public async Task<IActionResult> OnGetAsync(int claimId)
        {
            Claim = _context.RoleClaims.Where(c => c.Id == claimId).FirstOrDefault();
            if (Claim == null) return NotFound("Claim not found");

            Role = await _roleManager.FindByIdAsync(Claim.RoleId);
            if (Role == null) return NotFound("Role not found");

            Input = new InputModel()
            {
                ClaimType = Claim.ClaimType,
                ClaimValue = Claim.ClaimValue
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int claimId)
        {
            Claim = _context.RoleClaims.Where(c => c.Id == claimId).FirstOrDefault();
            if (Claim == null) return NotFound("Claim not found");

            Role = await _roleManager.FindByIdAsync(Claim.RoleId);
            if (Role == null) return NotFound("Role not found");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check duplicate claim
            var claims = _context.RoleClaims.Where(c => c.RoleId == Role.Id);
            if (claims.Any(c => c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue && c.Id != Claim.Id))
            {
                ModelState.AddModelError(string.Empty, $"Claim {Input.ClaimType} : {Input.ClaimValue} is already taken");
                return Page();
            }

            // Update claim
            Claim.ClaimType = Input.ClaimType;
            Claim.ClaimValue = Input.ClaimValue;

            await _context.SaveChangesAsync();

            StatusMessage = "Update claim successfully.";

            return RedirectToPage("./Edit", new { roleId = Role.Id });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int claimId)
        {
            Claim = _context.RoleClaims.Where(c => c.Id == claimId).FirstOrDefault();
            if (Claim == null) return NotFound("Claim not found");

            Role = await _roleManager.FindByIdAsync(Claim.RoleId);
            if (Role == null) return NotFound("Role not found");

            await _roleManager.RemoveClaimAsync(Role, new Claim(Claim.ClaimType, Claim.ClaimValue));

            StatusMessage = "Delete claim successfully.";

            return RedirectToPage("./Edit", new { roleId = Role.Id });
        }
    }
}
