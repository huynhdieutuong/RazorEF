using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorEF.Models;

namespace RazorEF.Pages_Blog
{
    public class EditModel : PageModel
    {
        private readonly RazorEF.Models.MyBlogContext _context;
        private readonly IAuthorizationService _authorizationService;

        public EditModel(RazorEF.Models.MyBlogContext context, IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Article).State = EntityState.Modified;

            try
            {
                // 24.2 Check user meets CanUpdateArticle policy
                var canUpdate = await _authorizationService.AuthorizeAsync(this.User, Article, "CanUpdateArticle");
                if (canUpdate.Succeeded)
                {
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return Content("Don't have permission to access");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(Article.Id))
                {
                    return Content("Article not found");
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ArticleExists(int id)
        {
            return _context.articles.Any(e => e.Id == id);
        }
    }
}
