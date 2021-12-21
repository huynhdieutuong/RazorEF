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

        public async Task OnGetAsync(string searchText)
        {
            var qr = from a in _context.articles select a;

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
