using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorEF.Models;

namespace RazorEF.Pages_Blog
{
    public class DeleteModel : PageModel
    {
        private readonly RazorEF.Models.MyBlogContext _context;

        public DeleteModel(RazorEF.Models.MyBlogContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return Content("Article not found");
            }

            Article = await _context.articles.FindAsync(id);

            if (Article != null)
            {
                _context.articles.Remove(Article);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
