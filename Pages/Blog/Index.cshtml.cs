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
    // 10.1 If not login, prevent access Blog
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly RazorEF.Models.MyBlogContext _context;

        public IndexModel(RazorEF.Models.MyBlogContext context)
        {
            _context = context;
        }

        public IList<Article> Article { get; set; }
        [BindProperty(SupportsGet = true, Name = "p")]
        public int CurrentPage { get; set; }
        public int CountPages { get; set; }
        public const int ITEMS_PER_PAGE = 10;

        public async Task OnGetAsync(string searchText)
        {
            int totalPages = await _context.articles.CountAsync();
            CountPages = (int)Math.Ceiling((double)totalPages / ITEMS_PER_PAGE);
            if (CurrentPage < 1) CurrentPage = 1;
            if (CurrentPage > CountPages) CurrentPage = CountPages;

            var qr = from a in _context.articles
                     orderby a.Created descending
                     select a;

            if (!string.IsNullOrEmpty(searchText))
            {
                ViewData["searchText"] = searchText;
                qr = (IOrderedQueryable<Article>)qr.Where(a => a.Title.Contains(searchText));
            }

            Article = await qr.Skip((CurrentPage - 1) * ITEMS_PER_PAGE)
                              .Take(ITEMS_PER_PAGE)
                              .ToListAsync();
        }
    }
}
