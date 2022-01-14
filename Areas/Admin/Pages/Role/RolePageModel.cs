using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorEF.Models;

namespace RazorEF.Areas.Admin.Pages.Role
{
    public class RolePageModel : PageModel
    {
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly MyBlogContext _context;

        public RolePageModel(RoleManager<IdentityRole> roleManger, MyBlogContext context)
        {
            _roleManager = roleManger;
            _context = context;
        }

        [TempData]
        public string StatusMessage { get; set; }
    }
}