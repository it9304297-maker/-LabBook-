using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Extensions;
using RazorPageBase.Models;

namespace RazorPageBase.Pages.InstrumentPages
{
    public class IndexModel : PageModel
    {
        private readonly RazorPageBaseContext _context;

        public IndexModel(RazorPageBaseContext context)
        {
            _context = context;
        }

        public IList<Instrument> Instrument { get; set; } = default!;

        // ch01 教學「新增搜尋」：用查詢字串綁定搜尋條件
        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public InstrumentCategory? Category { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool OnlyAvailable { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SortOrder { get; set; }

        public SelectList Categories { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Categories = new SelectList(CategoryItems(), "Value", "Text", Category);

            var query = _context.Instrument.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                query = query.Where(i => i.Name.Contains(SearchString)
                                      || i.AssetCode.Contains(SearchString)
                                      || (i.Brand != null && i.Brand.Contains(SearchString))
                                      || i.Location.Contains(SearchString));
            }
            if (Category.HasValue)
            {
                query = query.Where(i => i.Category == Category.Value);
            }
            if (OnlyAvailable)
            {
                query = query.Where(i => i.Status == InstrumentStatus.Available);
            }

            query = SortOrder switch
            {
                "rate" => query.OrderBy(i => i.HourlyRate),
                "rate_desc" => query.OrderByDescending(i => i.HourlyRate),
                "name" => query.OrderBy(i => i.Name),
                _ => query.OrderBy(i => i.AssetCode)
            };

            Instrument = await query.AsNoTracking().ToListAsync();
        }

        private static IEnumerable<SelectListItem> CategoryItems() =>
            Enum.GetValues<InstrumentCategory>().Select(c => new SelectListItem
            {
                Value = c.ToString(),
                Text = c.GetDisplayName()
            });
    }
}
