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
    public class EditModel : RolePageModel
    {
        public EditModel(RoleManager<IdentityRole> roleManger, MyBlogContext context) : base(roleManger, context)
        {
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public IdentityRole Role { get; set; }

        public class InputModel
        {
            [Display(Name = "Role name")]
            [Required(ErrorMessage = "{0} is required")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} must be between {2} - {1} chars")]
            public string Name { get; set; }
        }

        // 17.2 binding roleId to get Role
        public async Task<IActionResult> OnGet(string roleId)
        {
            if (roleId == null) return NotFound("Not found roleId");

            Role = await _roleManager.FindByIdAsync(roleId);
            if (Role != null)
            {
                Input = new InputModel()
                {
                    Name = Role.Name
                };
                return Page();
            };

            return NotFound("Not found role");
        }

        // 17.3 Edit role
        public async Task<IActionResult> OnPostAsync(string roleId)
        {
            if (roleId == null) return NotFound("Not found roleId");

            Role = await _roleManager.FindByIdAsync(roleId);
            if (Role == null) return NotFound("Not found role");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Role.Name = Input.Name;
            var result = await _roleManager.UpdateAsync(Role);
            if (result.Succeeded)
            {
                StatusMessage = $"Edit {Input.Name} role success.";
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
