using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorEF.Models;

namespace RazorEF.Areas.Admin.Pages.User
{
    public class AddRoleModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MyBlogContext _context;

        public AddRoleModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            MyBlogContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public AppUser User { get; set; }

        [BindProperty]
        [Display(Name = "Roles")]
        public string[] RoleNames { get; set; }
        public SelectList AllRoles { get; set; }
        public List<IdentityRoleClaim<string>> ClaimsInRoleClaims { get; set; }
        public List<IdentityUserClaim<string>> ClaimsInUserClaims { get; set; }

        async Task GetClaims(string userId)
        {
            var listUserRoles = from role in _context.Roles
                                join user in _context.UserRoles on role.Id equals user.RoleId
                                where user.UserId == userId
                                select role;

            var claimsInRoleClaims = from claim in _context.RoleClaims
                                     join role in listUserRoles on claim.RoleId equals role.Id
                                     select claim;

            ClaimsInRoleClaims = await claimsInRoleClaims.ToListAsync();

            ClaimsInUserClaims = await (from claim in _context.UserClaims
                                        where claim.UserId == userId
                                        select claim).ToListAsync();
        }

        public async Task<IActionResult> OnGetAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return NotFound("User ID not found");

            User = await _userManager.FindByIdAsync(userId);
            if (User == null)
            {
                return NotFound("Unable to load user.");
            }

            // 18.1 Get RolesName of user
            RoleNames = (await _userManager.GetRolesAsync(User)).ToArray<string>();

            // 18.1 Get AllRoles from database
            List<string> roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            AllRoles = new SelectList(roleNames);

            await GetClaims(userId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return NotFound("User ID not found");

            User = await _userManager.FindByIdAsync(userId);
            if (User == null)
            {
                return NotFound("Unable to load user.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await GetClaims(userId);

            // 18.3 Delete and add roles
            var oldRoleNames = (await _userManager.GetRolesAsync(User)).ToArray<string>();
            // old: [Vip, Editor]
            // new: [Admin, Editor]
            var deleteRoles = oldRoleNames.Where(r => !RoleNames.Contains(r));
            var addRoles = RoleNames.Where(r => !oldRoleNames.Contains(r));

            var resultDelete = await _userManager.RemoveFromRolesAsync(User, deleteRoles);
            if (!resultDelete.Succeeded)
            {
                resultDelete.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }

            var resultAdd = await _userManager.AddToRolesAsync(User, addRoles);
            if (!resultAdd.Succeeded)
            {
                resultAdd.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }

            StatusMessage = $"Updated role for user: {User.UserName}";

            return RedirectToPage("./Index");
        }
    }
}
