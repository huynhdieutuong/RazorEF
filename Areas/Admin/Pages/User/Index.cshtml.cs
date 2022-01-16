using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorEF.Models;

namespace RazorEF.Areas.Admin.Pages.User
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        public IndexModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        [TempData]
        public string StatusMessage { get; set; }

        // 19.1 Create UserAndRole to add RoleNames
        public class UserAndRole : AppUser
        {
            public string RoleNames { get; set; }
        }

        public List<UserAndRole> Users { get; set; }

        [BindProperty(SupportsGet = true, Name = "p")]
        public int CurrentPage { get; set; }
        public int CountPages { get; set; }
        public const int ITEMS_PER_PAGE = 10;
        public int TotalUsers { get; set; }

        public async Task OnGetAsync(string searchText)
        {
            TotalUsers = await _userManager.Users.CountAsync();
            CountPages = (int)Math.Ceiling((double)TotalUsers / ITEMS_PER_PAGE);
            if (CurrentPage < 1) CurrentPage = 1;
            if (CurrentPage > CountPages) CurrentPage = CountPages;

            var qr = from user in _userManager.Users
                     orderby user.UserName
                     select user;

            if (!string.IsNullOrEmpty(searchText))
            {
                ViewData["searchText"] = searchText;
                qr = (IOrderedQueryable<AppUser>)qr.Where(user => user.UserName.Contains(searchText));
            }

            Users = await qr.Skip((CurrentPage - 1) * ITEMS_PER_PAGE)
                            .Take(ITEMS_PER_PAGE)
                            .Select(user => new UserAndRole() // 19.2 Convert AppUser to UserAndRole
                            {
                                Id = user.Id,
                                UserName = user.UserName
                            })
                            .ToListAsync();

            // 19.3 Get Roles for User
            foreach (var user in Users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleNames = string.Join(", ", roles);
            }
        }

        public void OnPost() => RedirectToPage();
    }
}
