using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

/* 
 * 
 
獲取 Google「應用程式密碼」（關鍵步驟）

如果直接填寫個人的 Gmail 登入密碼，系統會回傳 Authentication failed 錯誤。請依照以下步驟產生 16 位數的專用密碼：
1. 開啟雙重驗證：
	• 前往 Google 帳戶安全性設定。
	• 確認「登入 Google 的方式」區塊中的 「兩步驟驗證」(2-Step Verification) 已經開啟。如果是關閉狀態，請點進去依指示完成綁定。
2. 產生應用程式密碼：
	• 在同一個安全性頁面，點擊 「兩步驟驗證」 進去。
	• 捲動到頁面最下方，會看到 「應用程式密碼」(App passwords) 選項，點擊它。
	• 在「應用程式名稱」欄位隨意輸入一個名字（例如：MailKit App 或 C# Test），然後點擊 「建立」。
3. 複製密碼：
	• 畫面上彈出的黃色背景方框中，會顯示一組 16 位元的英文字母密碼（例如：xxxx yyyy zzzz aaaa）。
	• 複製這組密碼，並貼到 C# 程式碼中的 AuthenticateAsync 密碼參數中（中間的空格會自動忽略，留著或刪除皆可）。


補充：如果使用 Port 465 怎麼改？

Gmail 也支援 SSL 專用連線（Port 465），如果你所在的網路環境封鎖了 Port 587，可以將連線程式碼改為：
await client.ConnectAsync("://gmail.com", 465, SecureSocketOptions.SslOnConnect);

 
 * 
 */

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(
        string email,
        string subject,
        string htmlMessage)
    {
        var host = _configuration["Email:Host"]
            ?? throw new InvalidOperationException(
                "Email:Host is not configured.");

        var port = _configuration.GetValue<int>("Email:Port");

        var from = _configuration["Email:From"]
            ?? throw new InvalidOperationException(
                "Email:From is not configured.");

        var username = _configuration["Email:UserName"];
        var password = _configuration["Email:Password"];

        var message = new MimeMessage();

        message.From.Add(MailboxAddress.Parse(from));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = subject;

        message.Body = new BodyBuilder
        {
            HtmlBody = htmlMessage
        }.ToMessageBody();

        using var client = new SmtpClient();


        //// 送信/////////
        try
        {
            // 1. 連線到 Gmail 的 SMTP 伺服器
            // 參數：主機名, Port 587, 使用 StartTls 加密
            await client.ConnectAsync(
                    host,
                    port,
                    SecureSocketOptions.StartTls);

            if (!string.IsNullOrWhiteSpace(username))
            {
                // 2.帳號密碼驗證
                // ⚠️ 密碼欄位請填入第二步產生的「16 位數應用程式密碼」，而不是你平常登入 Gmail 的密碼
                await client.AuthenticateAsync(
                    username,
                    password);
            }
            // 3. 發送郵件
            await client.SendAsync(message);
            Console.WriteLine("Gmail 郵件發送成功！ for 註冊會員:" + MailboxAddress.Parse(email));

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Gmail 郵件發送失敗！{ex.Message} for 註冊會員:" + MailboxAddress.Parse(email));
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
 

    }
}