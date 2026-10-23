using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Models;

namespace RazorPageBase.Pages.InstrumentPages
{
    public class EditModel : PageModel
    {
        private readonly RazorPageBaseContext _context;

        public EditModel(RazorPageBaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Instrument Instrument { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrument = await _context.Instrument.FirstOrDefaultAsync(m => m.Id == id);
            if (instrument == null)
            {
                return NotFound();
            }
            Instrument = instrument;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Instrument.AssetCode = Instrument.AssetCode?.Trim().ToUpper() ?? "";

            if (await _context.Instrument.AnyAsync(i => i.AssetCode == Instrument.AssetCode && i.Id != Instrument.Id))
            {
                ModelState.AddModelError("Instrument.AssetCode", "此財產編號已被其他儀器使用");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Instrument).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InstrumentExists(Instrument.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            TempData["Success"] = $"已更新儀器「{Instrument.Name}」";
            return RedirectToPage("./Index");
        }

        private bool InstrumentExists(int id)
        {
            return _context.Instrument.Any(e => e.Id == id);
        }
    }
}
