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
        public const int ITEMS_PER_PAGE = 5;

        public async Task OnGetAsync(string searchText)
        {
            int totalArticles = await _context.articles.CountAsync();
            CountPages = (int)Math.Ceiling((double)totalArticles / ITEMS_PER_PAGE);
            if (CurrentPage < 1) CurrentPage = 1;
            if (CurrentPage > CountPages) CurrentPage = CountPages;

            var qr = (from a in _context.articles select a)
                     .Skip((CurrentPage - 1) * ITEMS_PER_PAGE)
                     .Take(ITEMS_PER_PAGE);

            if (!string.IsNullOrEmpty(searchText))
            {
                ViewData["searchText"] = searchText;
                qr = qr.Where(a => a.Title.Contains(searchText));
            }

            qr = qr.OrderByDescending(a => a.Created);

            Article = await qr.ToListAsync();
        }
    }
}
