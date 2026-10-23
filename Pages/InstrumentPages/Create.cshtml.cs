using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Models;

namespace RazorPageBase.Pages.InstrumentPages
{
    public class CreateModel : PageModel
    {
        private readonly RazorPageBaseContext _context;

        public CreateModel(RazorPageBaseContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Instrument Instrument { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            Instrument.AssetCode = Instrument.AssetCode?.Trim().ToUpper() ?? "";

            if (await _context.Instrument.AnyAsync(i => i.AssetCode == Instrument.AssetCode))
            {
                ModelState.AddModelError("Instrument.AssetCode", "此財產編號已存在");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Instrument.Add(Instrument);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"已新增儀器「{Instrument.Name}」";

            return RedirectToPage("./Index");
        }
    }
}
