# RazorPageBaseTW
RazorPage 基礎 startup project, including TW localization Identity with two role (Admin, User)

1. 下載後解壓縮,用VS打開專案重建.
2. 打開"工具>Nuget套件管理員>套件管理主控台", 輸入: Add-Migration init1
3. 再輸入: Update-Database
4. 執行專案.註刪一位管理者帳號.
5. 到資料庫把AspNetUserRoles表格,新增一筆Admin的對應.(先至AspNetRoles查好Admin角色的Id)
