using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorEF.Models;

namespace RazorEF.Pages_Blog
{
    // 22.1 Use Policy InGenZ
    [Authorize(Policy = "InGenZ")] // Date of Birth 1997 - 2012
    public class DetailsModel : PageModel
    {
        private readonly RazorEF.Models.MyBlogContext _context;

        public DetailsModel(RazorEF.Models.MyBlogContext context)
        {
            _context = context;
        }

        public Article Article { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return Content("Article not found");
            }

            Article = await _context.articles.FirstOrDefaultAsync(m => m.Id == id);

            if (Article == null)
            {
                return Content("Article not found");
            }
            return Page();
        }
    }
}
