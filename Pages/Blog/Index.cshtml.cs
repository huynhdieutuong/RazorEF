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

        public IList<Article> Article { get;set; }

        public async Task OnGetAsync()
        {
            Article = await _context.articles.ToListAsync();
        }
    }
}
