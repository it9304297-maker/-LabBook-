using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPageBase.Services;

namespace RazorPageBase.Pages
{
    /// <summary>
    /// 開發用信箱：顯示尚未透過 Gmail 寄出的驗證信。只在開發環境 (VS 執行) 開放。
    /// </summary>
    public class DevMailboxModel : PageModel
    {
        private readonly DevMailbox _mailbox;
        private readonly IWebHostEnvironment _env;

        public DevMailboxModel(DevMailbox mailbox, IWebHostEnvironment env)
        {
            _mailbox = mailbox;
            _env = env;
        }

        public IReadOnlyList<DevMail> Mails { get; set; } = [];

        public IActionResult OnGet()
        {
            // 正式上線環境不開放，避免別人看到驗證信
            if (!_env.IsDevelopment())
            {
                return NotFound();
            }

            Mails = _mailbox.All();
            return Page();
        }
    }
}
