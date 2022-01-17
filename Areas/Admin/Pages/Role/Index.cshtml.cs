using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazorEF.Models;

namespace RazorEF.Areas.Admin.Pages.Role
{
    [Authorize(Roles = "Admin")] // "Admin,Vip,Editor" => Admin || Vip || Editor
    // [Authorize(Roles = "Vip")]
    // [Authorize(Roles = "Editor")]
    // => Admin && Vip && Editor

    /*
        When update user role, user need logout and login to affect
        If want auto affect, use this code:
        services.Configure<SecurityStampValidatorOptions>(options =>
        {
            // Trên 30 giây truy cập lại sẽ nạp lại thông tin User (Role)
            // SecurityStamp trong bảng User đổi -> nạp lại thông tinn Security
            options.ValidationInterval = TimeSpan.FromSeconds(30);
        });
    */
    public class IndexModel : RolePageModel
    {
        public IndexModel(RoleManager<IdentityRole> roleManger, MyBlogContext context) : base(roleManger, context)
        {
        }

        public class RoleModel : IdentityRole
        {
            public string[] Claims { get; set; }
        }

        public List<RoleModel> Roles { get; set; }

        public async Task OnGet()
        {
            var roles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
            Roles = new List<RoleModel>();
            foreach (var role in roles)
            {
                // 21.1 Get Claims's Role by role (Roles table)
                var claims = await _roleManager.GetClaimsAsync(role);
                var claimsString = claims.Select(c => c.Type + "=" + c.Value);

                var roleModel = new RoleModel()
                {
                    Name = role.Name,
                    Id = role.Id,
                    Claims = claimsString.ToArray()
                };
                Roles.Add(roleModel);
            }
        }

        public void OnPost() => RedirectToPage();
    }
}
