using System.Collections.Concurrent;

namespace RazorPageBase.Services;

/// <summary>
/// 開發用信箱：尚未設定 Gmail 應用程式密碼（或寄送失敗）時，驗證信存在這裡，
/// 可到 /DevMailbox 頁面查看並點擊驗證連結。只存在記憶體，程式重啟後清空。
/// </summary>
public class DevMailbox
{
    private readonly ConcurrentQueue<DevMail> _mails = new();

    public void Add(string to, string subject, string htmlBody, string note)
    {
        _mails.Enqueue(new DevMail(to, subject, htmlBody, note, DateTime.Now));
        while (_mails.Count > 50 && _mails.TryDequeue(out _)) { }
    }

    public IReadOnlyList<DevMail> All() => _mails.Reverse().ToList();
}

public record DevMail(string To, string Subject, string HtmlBody, string Note, DateTime SentAt);
