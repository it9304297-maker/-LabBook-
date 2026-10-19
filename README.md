# RazorPageBaseTW v0.2tw
RazorPage 基礎 startup project, including TW localization Identity with two role (Admin, User)

1. 下載後解壓縮,把目錄的唯讀屬性取消. 右鍵>內容>唯讀 不要打勾, 用VS打開專案重建.
2. 打開"工具>Nuget套件管理員>套件管理主控台",
3. 輸入: Add-Migration init1
4. 再輸入: Update-Database
5. IdentitySeeder會自動建立角色 Admin, User; 預設管理者:admin@pccu.com/123456 使用者:user1@pccu.com/123456
 

6. 啓用註冊寄信功能:

步驟 1.
打開應用程式進入點 Program.cs，將你的 EmailSender 註冊進依賴注入 (DI) 容器中： 確保你的 AddDefaultIdentity 設定中，如果有開啟

    options.SignIn.RequireConfirmedAccount = true

    options.SignIn.RequireConfirmedEmail = true

Identity 會在使用者註冊時自動調用這個 EmailSender 發送驗證信。

Program.cs，找到這行為註記//拿掉

    // 4. Email sender
    //builder.Services.AddTransient<IEmailSender, EmailSender>();


步驟 2. 由於 Gmail 的安全機制，你不能直接在程式碼中填入你的 Google 登入密碼。
登入你的 Google 帳戶安全設定。
確保帳戶已啟用 「兩步驟驗證」。
在搜尋欄輸入「應用程式密碼」或在兩步驟驗證選單底部找到 「應用程式密碼 (App Passwords)」。
輸入一個自訂名稱（例如：MyAspNetCoreApp），點選產生。
複製這串 16 位元的密碼（程式設定時要使用，中間的空格請忽略）。 

步驟 3：安裝 MailKit 套件與實作 IEmailSender
透過: 工具>NuGet套件管理員>套件管理主控台, 輸入MailKit搜尋, 打勾版本 >安裝 MailKit：

步驟 4.  打開輸appsettings.json,填入smtp的資料

步驟 5：測試註冊是否有寄信驗證(有可能被判為垃圾信.因來源是localhost)
因為伺服器沒有設定ip.故驗證信連結為localhost.要在同一台機器,執行專案.用gmail信箱來收信測.
 




 
