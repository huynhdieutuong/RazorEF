using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorEF.Models;

namespace RazorEF.Areas.Admin.Pages.Role
{
    public class DeleteModel : RolePageModel
    {
        public DeleteModel(RoleManager<IdentityRole> roleManger, MyBlogContext context) : base(roleManger, context)
        {
        }

        public IdentityRole Role { get; set; }

        public async Task<IActionResult> OnGet(string roleId)
        {
            if (roleId == null) return NotFound("Not found roleId");

            Role = await _roleManager.FindByIdAsync(roleId);
            if (Role != null)
            {
                return Page();
            };

            return NotFound("Not found role");
        }

        public async Task<IActionResult> OnPostAsync(string roleId)
        {
            if (roleId == null) return NotFound("Not found roleId");

            Role = await _roleManager.FindByIdAsync(roleId);
            if (Role == null) return NotFound("Not found role");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _roleManager.DeleteAsync(Role);
            if (result.Succeeded)
            {
                StatusMessage = $"Delete {Role.Name} role success.";
                return RedirectToPage("./Index");
            }
            else
            {
                result.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
            }

            return Page();
        }
    }
}
