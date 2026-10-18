$root = "C:\2\repos\RazorPageBase\RazorPageBase"

$files = @{
"Areas\Identity\Pages\Account\Login.cshtml" = @'
@page
@model LoginModel

@{
    ViewData["Title"] = "登入";
}

<h1>@ViewData["Title"]</h1>
<div class="row">
    <div class="col-md-4">
        <section>
            <form id="account" method="post">
                <h2>使用本機帳號登入。</h2>
                <hr />
                <div asp-validation-summary="ModelOnly" class="text-danger" role="alert"></div>
                <div class="form-floating mb-3">
                    <input asp-for="Input.Email" class="form-control" autocomplete="username" aria-required="true" placeholder="name@example.com" />
                    <label asp-for="Input.Email" class="form-label">電子郵件</label>
                    <span asp-validation-for="Input.Email" class="text-danger"></span>
                </div>
                <div class="form-floating mb-3">
                    <input asp-for="Input.Password" class="form-control" autocomplete="current-password" aria-required="true" placeholder="password" />
                    <label asp-for="Input.Password" class="form-label">密碼</label>
                    <span asp-validation-for="Input.Password" class="text-danger"></span>
                </div>
                <div class="checkbox mb-3">
                    <label asp-for="Input.RememberMe" class="form-label">
                        <input class="form-check-input" asp-for="Input.RememberMe" />
                        @Html.DisplayNameFor(m => m.Input.RememberMe)
                    </label>
                </div>
                <div>
                    <button id="login-submit" type="submit" class="w-100 btn btn-lg btn-primary">登入</button>
                </div>
                <div>
                    <p>
                        <a id="forgot-password" asp-page="./ForgotPassword">忘記密碼？</a>
                    </p>
                    <p>
                        <a asp-page="./Register" asp-route-returnUrl="@Model.ReturnUrl">註冊新帳號</a>
                    </p>
                    <p>
                        <a id="resend-confirmation" asp-page="./ResendEmailConfirmation">重新發送電子郵件驗證</a>
                    </p>
                </div>
            </form>
        </section>
    </div>
    <div class="col-md-6 col-md-offset-2">
        <section>
            <h3>使用其他服務登入。</h3>
            <hr />
            @{
                if ((Model.ExternalLogins?.Count ?? 0) == 0)
                {
                    <div>
                        <p>
                            尚未設定外部驗證服務。請參考此篇 <a href="https://go.microsoft.com/fwlink/?LinkID=532715">說明文章</a>，了解如何設定外部登入支援。
                        </p>
                    </div>
                }
                else
                {
                    <form id="external-account" asp-page="./ExternalLogin" asp-route-returnUrl="@Model.ReturnUrl" method="post" class="form-horizontal">
                        <div>
                            <p>
                                @foreach (var provider in Model.ExternalLogins!)
                                {
                                    <button type="submit" class="btn btn-primary" name="provider" value="@provider.Name" title="使用 @provider.DisplayName 帳號登入">@provider.DisplayName</button>
                                }
                            </p>
                        </div>
                    </form>
                }
            }
        </section>
    </div>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
'@

"Areas\Identity\Pages\Account\Register.cshtml" = @'
@page
@model RegisterModel
@{
    ViewData["Title"] = "註冊";
}

<h1>@ViewData["Title"]</h1>

<div class="row">
    <div class="col-md-4">
        <form id="registerForm" asp-route-returnUrl="@Model.ReturnUrl" method="post">
            <h2>建立新帳號。</h2>
            <hr />
            <div asp-validation-summary="ModelOnly" class="text-danger" role="alert"></div>
            <div class="form-floating mb-3">
                <input asp-for="Input.Email" class="form-control" autocomplete="username" aria-required="true" placeholder="name@example.com" />
                <label asp-for="Input.Email">電子郵件</label>
                <span asp-validation-for="Input.Email" class="text-danger"></span>
            </div>
            <div class="form-floating mb-3">
                <input asp-for="Input.Password" class="form-control" autocomplete="new-password" aria-required="true" placeholder="password" />
                <label asp-for="Input.Password">密碼</label>
                <span asp-validation-for="Input.Password" class="text-danger"></span>
            </div>
            <div class="form-floating mb-3">
                <input asp-for="Input.ConfirmPassword" class="form-control" autocomplete="new-password" aria-required="true" placeholder="password" />
                <label asp-for="Input.ConfirmPassword">確認密碼</label>
                <span asp-validation-for="Input.ConfirmPassword" class="text-danger"></span>
            </div>
            <button id="registerSubmit" type="submit" class="w-100 btn btn-lg btn-primary">註冊</button>
        </form>
    </div>
    <div class="col-md-6 col-md-offset-2">
        <section>
            <h3>使用其他服務註冊。</h3>
            <hr />
            @{
                if ((Model.ExternalLogins?.Count ?? 0) == 0)
                {
                    <div>
                        <p>
                            尚未設定外部驗證服務。請參考此 <a href="https://go.microsoft.com/fwlink/?LinkID=532715">說明文章</a>，了解如何設定外部登入支援。
                        </p>
                    </div>
                }
                else
                {
                    <form id="external-account" asp-page="./ExternalLogin" asp-route-returnUrl="@Model.ReturnUrl" method="post" class="form-horizontal">
                        <div>
                            <p>
                                @foreach (var provider in Model.ExternalLogins!)
                                {
                                    <button type="submit" class="btn btn-primary" name="provider" value="@provider.Name" title="使用 @provider.DisplayName 帳號登入">@provider.DisplayName</button>
                                }
                            </p>
                        </div>
                    </form>
                }
            }
        </section>
    </div>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
'@

"Areas\Identity\Pages\Account\AccessDenied.cshtml" = @'
@page
@model AccessDeniedModel
@{
    ViewData["Title"] = "拒絕存取";
}

<header>
    <h1 class="text-danger">@ViewData["Title"]</h1>
    <p class="text-danger">您無權存取此資源。</p>
</header>
'@

"Areas\Identity\Pages\Account\ForgotPassword.cshtml" = @'
@page
@model ForgotPasswordModel
@{
    ViewData["Title"] = "忘記密碼？";
}

<h1>@ViewData["Title"]</h1>
<h2>請輸入您的電子郵件。</h2>
<hr />
<div class="row">
    <div class="col-md-4">
        <form method="post">
            <div asp-validation-summary="ModelOnly" class="text-danger" role="alert"></div>
            <div class="form-floating mb-3">
                <input asp-for="Input.Email" class="form-control" autocomplete="username" aria-required="true" placeholder="name@example.com" />
                <label asp-for="Input.Email" class="form-label">電子郵件</label>
                <span asp-validation-for="Input.Email" class="text-danger"></span>
            </div>
            <button type="submit" class="w-100 btn btn-lg btn-primary">重設密碼</button>
        </form>
    </div>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
'@

"Areas\Identity\Pages\Account\ResetPassword.cshtml" = @'
@page
@model ResetPasswordModel
@{
    ViewData["Title"] = "重設密碼";
}

<h1>@ViewData["Title"]</h1>
<h2>重設您的密碼。</h2>
<hr />
<div class="row">
    <div class="col-md-4">
        <form method="post">
            <div asp-validation-summary="ModelOnly" class="text-danger" role="alert"></div>
            <input asp-for="Input.Code" type="hidden" />
            <div class="form-floating mb-3">
                <input asp-for="Input.Email" class="form-control" autocomplete="username" aria-required="true" placeholder="name@example.com" />
                <label asp-for="Input.Email" class="form-label">電子郵件</label>
                <span asp-validation-for="Input.Email" class="text-danger"></span>
            </div>
            <div class="form-floating mb-3">
                <input asp-for="Input.Password" class="form-control" autocomplete="new-password" aria-required="true" placeholder="請輸入您的密碼。" />
                <label asp-for="Input.Password" class="form-label">密碼</label>
                <span asp-validation-for="Input.Password" class="text-danger"></span>
            </div>
            <div class="form-floating mb-3">
                <input asp-for="Input.ConfirmPassword" class="form-control" autocomplete="new-password" aria-required="true" placeholder="請再次確認您的密碼。" />
                <label asp-for="Input.ConfirmPassword" class="form-label">確認密碼</label>
                <span asp-validation-for="Input.ConfirmPassword" class="text-danger"></span>
            </div>
            <button type="submit" class="w-100 btn btn-lg btn-primary">重設</button>
        </form>
    </div>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
'@

"Areas\Identity\Pages\Account\ConfirmEmail.cshtml" = @'
@page
@model ConfirmEmailModel
@{
    ViewData["Title"] = "確認電子郵件";
}

<h1>@ViewData["Title"]</h1>
<partial name="_StatusMessage" model="Model.StatusMessage" />
'@

"Areas\Identity\Pages\Account\ExternalLogin.cshtml" = @'
@page
@model ExternalLoginModel
@{
    ViewData["Title"] = "註冊";
}

<h1>@ViewData["Title"]</h1>
<h2 id="external-login-title">關聯您的 @Model.ProviderDisplayName 帳號。</h2>
<hr />

<p id="external-login-description" class="text-info">
    您已使用 <strong>@Model.ProviderDisplayName</strong> 驗證成功。
    請在下方輸入此站使用的電子郵件地址，然後按下「註冊」完成登入。
</p>

<div class="row">
    <div class="col-md-4">
        <form asp-page-handler="Confirmation" asp-route-returnUrl="@Model.ReturnUrl" method="post">
            <div asp-validation-summary="ModelOnly" class="text-danger" role="alert"></div>
            <div class="form-floating mb-3">
                <input asp-for="Input.Email" class="form-control" autocomplete="email" placeholder="Please enter your email."/>
                <label asp-for="Input.Email" class="form-label">電子郵件</label>
                <span asp-validation-for="Input.Email" class="text-danger"></span>
            </div>
            <button type="submit" class="w-100 btn btn-lg btn-primary">註冊</button>
        </form>
    </div>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
'@

"Areas\Identity\Pages\Account\ResendEmailConfirmation.cshtml" = @'
@page
@model ResendEmailConfirmationModel
@{
    ViewData["Title"] = "重新發送電子郵件驗證";
}

<h1>@ViewData["Title"]</h1>
<h2>請輸入您的電子郵件。</h2>
<hr />
<div class="row">
    <div class="col-md-4">
        <form method="post">
            <div asp-validation-summary="All" class="text-danger" role="alert"></div>
            <div class="form-floating mb-3">
                <input asp-for="Input.Email" class="form-control" aria-required="true" placeholder="name@example.com" />
                <label asp-for="Input.Email" class="form-label">電子郵件</label>
                <span asp-validation-for="Input.Email" class="text-danger"></span>
            </div>
            <button type="submit" class="w-100 btn btn-lg btn-primary">重新發送</button>
        </form>
    </div>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
'@

"Areas\Identity\Pages\Account\Lockout.cshtml" = @'
@page
@model LockoutModel
@{
    ViewData["Title"] = "已被鎖定";
}

<header>
    <h1 class="text-danger">@ViewData["Title"]</h1>
    <p class="text-danger">此帳號已被鎖定，請稍後再試。</p>
</header>
'@

"Areas\Identity\Pages\Account\Manage\Index.cshtml" = @'
@page
@model IndexModel
@{
    ViewData["Title"] = "個人資料";
    ViewData["ActivePage"] = ManageNavPages.Index;
}

<h3>@ViewData["Title"]</h3>
<partial name="_StatusMessage" for="StatusMessage" />
<div class="row">
    <div class="col-md-6">
        <form id="profile-form" method="post">
            <div asp-validation-summary="ModelOnly" class="text-danger" role="alert"></div>
            <div class="form-floating mb-3">
                <input asp-for="Username" class="form-control" placeholder="Please choose your username." disabled />
                <label asp-for="Username" class="form-label">使用者名稱</label>
            </div>
            <div class="form-floating mb-3">
                <input asp-for="Input.PhoneNumber" class="form-control" placeholder="Please enter your phone number."/>
                <label asp-for="Input.PhoneNumber" class="form-label">電話號碼</label>
                <span asp-validation-for="Input.PhoneNumber" class="text-danger"></span>
            </div>
            <button id="update-profile-button" type="submit" class="w-100 btn btn-lg btn-primary">儲存</button>
        </form>
    </div>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
'@

"Areas\Identity\Pages\Account\Manage\ChangePassword.cshtml" = @'
@page
@model ChangePasswordModel
@{
    ViewData["Title"] = "變更密碼";
    ViewData["ActivePage"] = ManageNavPages.ChangePassword;
}

<h3>@ViewData["Title"]</h3>
<partial name="_StatusMessage" for="StatusMessage" />
<div class="row">
    <div class="col-md-6">
        <form id="change-password-form" method="post">
            <div asp-validation-summary="ModelOnly" class="text-danger" role="alert"></div>
            <div class="form-floating mb-3">
                <input asp-for="Input.OldPassword" class="form-control" autocomplete="current-password" aria-required="true" placeholder="請輸入舊密碼。" />
                <label asp-for="Input.OldPassword" class="form-label">舊密碼</label>
                <span asp-validation-for="Input.OldPassword" class="text-danger"></span>
            </div>
            <div class="form-floating mb-3">
                <input asp-for="Input.NewPassword" class="form-control" autocomplete="new-password" aria-required="true" placeholder="請輸入新密碼。" />
                <label asp-for="Input.NewPassword" class="form-label">新密碼</label>
                <span asp-validation-for="Input.NewPassword" class="text-danger"></span>
            </div>
            <div class="form-floating mb-3">
                <input asp-for="Input.ConfirmPassword" class="form-control" autocomplete="new-password" aria-required="true" placeholder="請確認新密碼。" />
                <label asp-for="Input.ConfirmPassword" class="form-label">確認新密碼</label>
                <span asp-validation-for="Input.ConfirmPassword" class="text-danger"></span>
            </div>
            <button type="submit" class="w-100 btn btn-lg btn-primary">更新密碼</button>
        </form>
    </div>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
'@

"Areas\Identity\Pages\Account\RegisterConfirmation.cshtml" = @'
@page
@model RegisterConfirmationModel
@{
    ViewData["Title"] = "註冊確認";
}

<h1>@ViewData["Title"]</h1>
@{
    if (@Model.DisplayConfirmAccountLink)
    {
<p>
    本應用目前尚未設定實際的電子郵件發送服務，請參閱 <a href="https://aka.ms/aspaccountconf">說明文件</a> 以設定實際的電子郵件發送。
    通常系統會發送以下連結： <a id="confirm-link" href="@Model.EmailConfirmationUrl">點此確認您的帳號</a>
</p>
    }
    else
    {
<p>
        請檢查您的電子郵件並完成帳號確認。
</p>
    }
}
'@

"Areas\Identity\Pages\Account\ConfirmEmailChange.cshtml" = @'
@page
@model ConfirmEmailChangeModel
@{
    ViewData["Title"] = "確認電子郵件變更";
}

<h1>@ViewData["Title"]</h1>
<partial name="_StatusMessage" model="Model.StatusMessage" />
'@

"Areas\Identity\Pages\Account\Manage\DeletePersonalData.cshtml" = @'
@page
@model DeletePersonalDataModel
@{
    ViewData["Title"] = "刪除個人資料";
    ViewData["ActivePage"] = ManageNavPages.PersonalData;
}

<h3>@ViewData["Title"]</h3>

<div class="alert alert-warning" role="alert">
    <p>
        <strong>刪除此資料將永久移除您的帳號，且無法復原。</strong>
    </p>
</div>

<div>
    <form id="delete-user" method="post">
        <div asp-validation-summary="ModelOnly" class="text-danger" role="alert"></div>
        @if (Model.RequirePassword)
        {
            <div class="form-floating mb-3">
                <input asp-for="Input.Password" class="form-control" autocomplete="current-password" aria-required="true" placeholder="請輸入您的密碼。" />
                <label asp-for="Input.Password" class="form-label">密碼</label>
                <span asp-validation-for="Input.Password" class="text-danger"></span>
            </div>
        }
        <button class="w-100 btn btn-lg btn-danger" type="submit">刪除資料並關閉我的帳號</button>
    </form>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
'@

"Areas\Identity\Pages\Account\Manage\ExternalLogins.cshtml" = @'
@page
@model ExternalLoginsModel
@{
    ViewData["Title"] = "管理外部登入";
    ViewData["ActivePage"] = ManageNavPages.ExternalLogins;
}

<partial name="_StatusMessage" for="StatusMessage" />
@if (Model.CurrentLogins?.Count > 0)
{
    <h3>已註冊的登入方式</h3>
    <table class="table">
        <tbody>
            @foreach (var login in Model.CurrentLogins)
            {
                <tr>
                    <td id="@($"login-provider-{login.LoginProvider}")">@login.ProviderDisplayName</td>
                    <td>
                        @if (Model.ShowRemoveButton)
                        {
                            <form id="@($"remove-login-{login.LoginProvider}")" asp-page-handler="RemoveLogin" method="post">
                                <div>
                                    <input asp-for="@login.LoginProvider" name="LoginProvider" type="hidden" />
                                    <input asp-for="@login.ProviderKey" name="ProviderKey" type="hidden" />
                                    <button type="submit" class="btn btn-primary" title="從您的帳號移除此 @login.ProviderDisplayName 登入方式">移除</button>
                                </div>
                            </form>
                        }
                        else
                        {
                            @: &nbsp;
                        }
                    </td>
                </tr>
            }
        </tbody>
    </table>
}
@if (Model.OtherLogins?.Count > 0)
{
    <h4>新增其他登入服務</h4>
    <hr />
    <form id="link-login-form" asp-page-handler="LinkLogin" method="post" class="form-horizontal">
        <div id="socialLoginList">
            <p>
                @foreach (var provider in Model.OtherLogins)
                {
                    <button id="@($"link-login-button-{provider.Name}")" type="submit" class="btn btn-primary" name="provider" value="@provider.Name" title="使用 @provider.DisplayName 帳號登入或連結">@provider.DisplayName</button>
                }
            </p>
        </div>
    </form>
}
'@

"Areas\Identity\Pages\Account\Manage\EnableAuthenticator.cshtml" = @'
@page
@model EnableAuthenticatorModel
@{
    ViewData["Title"] = "設定驗證器應用程式";
    ViewData["ActivePage"] = ManageNavPages.TwoFactorAuthentication;
}

<partial name="_StatusMessage" for="StatusMessage" />
<h3>@ViewData["Title"]</h3>
<div>
    <p>使用驗證器應用程式請依照下列步驟：</p>
    <ol class="list">
        <li>
            <p>
                下載二階段驗證的應用程式，例如 Microsoft Authenticator（
                <a href="https://go.microsoft.com/fwlink/?Linkid=825072">Android</a>、<a href="https://go.microsoft.com/fwlink/?Linkid=825073">iOS</a>）或 Google Authenticator（Android / iOS）。
            </p>
        </li>
        <li>
            <p>掃描 QR Code 或手動輸入此金鑰 <kbd>@Model.SharedKey</kbd> 至驗證器應用程式。空格與大小寫不影響。</p>
            <div class="alert alert-info">如需產生 QR Code 的說明，請參閱 <a href="https://go.microsoft.com/fwlink/?Linkid=852423">說明</a>。</div>
            <div id="qrCode"></div>
            <div id="qrCodeData" data-url="@Model.AuthenticatorUri"></div>
        </li>
        <li>
            <p>
                完成掃描或輸入金鑰後，驗證器應用程式會產生一組一次性代碼。請在下方輸入該代碼以驗證。
            </p>
            <div class="row">
                <div class="col-md-6">
                    <form id="send-code" method="post">
                        <div class="form-floating mb-3">
                            <input asp-for="Input.Code" class="form-control" autocomplete="off" placeholder="請輸入驗證碼。" />
                            <label asp-for="Input.Code" class="control-label form-label">驗證碼</label>
                            <span asp-validation-for="Input.Code" class="text-danger"></span>
                        </div>
                        <button type="submit" class="w-100 btn btn-lg btn-primary">驗證</button>
                        <div asp-validation-summary="ModelOnly" class="text-danger" role="alert"></div>
                    </form>
                </div>
            </div>
        </li>
    </ol>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
'@

"Areas\Identity\Pages\Account\Manage\ShowRecoveryCodes.cshtml" = @'
@page
@model ShowRecoveryCodesModel
@{
    ViewData["Title"] = "備援碼";
    ViewData["ActivePage"] = "TwoFactorAuthentication";
}

<partial name="_StatusMessage" for="StatusMessage" />
<h3>@ViewData["Title"]</h3>
<div class="alert alert-warning" role="alert">
    <p>
        <strong>請妥善保存這些備援碼。</strong>
    </p>
    <p>
        如果您遺失裝置且沒有備援碼，將可能無法再次登入您的帳號。
    </p>
</div>
<div class="row">
    <div class="col-md-12">
        @for (var row = 0; row < (Model.RecoveryCodes?.Length ?? 0); row += 2)
        {
            <code class="recovery-code">@Model.RecoveryCodes![row]</code><text>&nbsp;</text><code class="recovery-code">@Model.RecoveryCodes[row + 1]</code><br />
        }
    </div>
</div>
'@

"Areas\Identity\Pages\Account\Manage\TwoFactorAuthentication.cshtml" = @'
@page
@using Microsoft.AspNetCore.Http.Features
@model TwoFactorAuthenticationModel
@{
    ViewData["Title"] = "雙重驗證 (2FA)";
    ViewData["ActivePage"] = ManageNavPages.TwoFactorAuthentication;
}

<partial name="_StatusMessage" for="StatusMessage" />
<h3>@ViewData["Title"]</h3>
@{
    var consentFeature = HttpContext.Features.Get<ITrackingConsentFeature>();
    @if (consentFeature?.CanTrack ?? true)
    {
        if (Model.Is2faEnabled)
        {
            if (Model.RecoveryCodesLeft == 0)
            {
                <div class="alert alert-danger">
                    <strong>您已沒有備援碼。</strong>
                    <p>您必須先 <a asp-page="./GenerateRecoveryCodes">產生新的備援碼</a>，才能使用備援碼登入。</p>
                </div>
            }
            else if (Model.RecoveryCodesLeft == 1)
            {
                <div class="alert alert-danger">
                    <strong>您還剩 1 組備援碼。</strong>
                    <p>您可以 <a asp-page="./GenerateRecoveryCodes">產生新的備援碼</a>。</p>
                </div>
            }
            else if (Model.RecoveryCodesLeft <= 3)
            {
                <div class="alert alert-warning">
                    <strong>您還剩 @Model.RecoveryCodesLeft 組備援碼。</strong>
                    <p>建議您 <a asp-page="./GenerateRecoveryCodes">產生新的備援碼</a>。</p>
                </div>
            }

            if (Model.IsMachineRemembered)
            {
                <form method="post" style="display: inline-block">
                    <button type="submit" class="btn btn-primary">忘記此瀏覽器</button>
                </form>
            }
            <a asp-page="./Disable2fa" class="btn btn-primary">停用 2FA</a>
            <a asp-page="./GenerateRecoveryCodes" class="btn btn-primary">重設備援碼</a>
        }

        <h4>驗證器應用程式</h4>
        @if (!Model.HasAuthenticator)
        {
            <a id="enable-authenticator" asp-page="./EnableAuthenticator" class="btn btn-primary">新增驗證器應用程式</a>
        }
        else
        {
            <a id="enable-authenticator" asp-page="./EnableAuthenticator" class="btn btn-primary">設定驗證器應用程式</a>
            <a id="reset-authenticator" asp-page="./ResetAuthenticator" class="btn btn-primary">重設驗證器應用程式</a>
        }
    }
    else
    {
        <div class="alert alert-danger">
            <strong>尚未接受隱私與 Cookie 政策。</strong>
            <p>您必須先接受政策後，才能啟用雙重驗證。</p>
        </div>
    }
}

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
'@

"Areas\Identity\Pages\Account\Manage\GenerateRecoveryCodes.cshtml" = @'
@page
@model GenerateRecoveryCodesModel
@{
    ViewData["Title"] = "產生雙重驗證 (2FA) 備援碼";
    ViewData["ActivePage"] = ManageNavPages.TwoFactorAuthentication;
}

<partial name="_StatusMessage" for="StatusMessage" />
<h3>@ViewData["Title"]</h3>
<div class="alert alert-warning" role="alert">
    <p>
        <span class="glyphicon glyphicon-warning-sign"></span>
        <strong>請將這些備援碼妥善保存。</strong>
    </p>
    <p>
        如果您遺失裝置且沒有備援碼，將可能無法再次登入您的帳號。
    </p>
    <p>
        產生新的備援碼不會變更驗證器應用程式所使用的金鑰。若要變更金鑰，請使用 <a asp-page="./ResetAuthenticator">重設驗證器金鑰</a>。
    </p>
</div>
<div>
    <form method="post">
        <button class="btn btn-danger" type="submit">產生備援碼</button>
    </form>
</div>
'@

"Areas\Identity\Pages\Account\ForgotPasswordConfirmation.cshtml" = @'
@page
@model ForgotPasswordConfirmation
@{
    ViewData["Title"] = "重設密碼已送出";
}

<h1>@ViewData["Title"]</h1>
<p>
    請檢查您的電子郵件以重設密碼。
</p>
'@

"Areas\Identity\Pages\Account\ResetPasswordConfirmation.cshtml" = @'
@page
@model ResetPasswordConfirmationModel
@{
    ViewData["Title"] = "重設密碼完成";
}

<h1>@ViewData["Title"]</h1>
<p>
    您的密碼已重設。請 <a asp-page="./Login">點此登入</a>。
</p>
'@

"Areas\Identity\Pages\Account\Logout.cshtml" = @'
@page
@model LogoutModel
@{
    ViewData["Title"] = "登出";
}

<header>
    <h1>@ViewData["Title"]</h1>
    @{
        if (User.Identity?.IsAuthenticated ?? false)
        {
            <form class="form-inline" asp-area="Identity" asp-page="/Account/Logout" asp-route-returnUrl="@Url.Page("/", new { area = "" })" method="post">
                <button type="submit" class="nav-link btn btn-link text-dark">按此登出</button>
            </form>
        }
        else
        {
            <p>您已成功登出應用程式。</p>
        }
    }
</header>
'@

"Areas\Identity\Pages\Account\LoginWith2fa.cshtml" = @'
@page
@model LoginWith2faModel
@{
    ViewData["Title"] = "雙重驗證";
}

<h1>@ViewData["Title"]</h1>
<hr />
<p>您的登入受驗證器應用程式保護。請輸入驗證器提供的代碼。</p>
<div class="row">
    <div class="col-md-4">
        <form method="post" asp-route-returnUrl="@Model.ReturnUrl">
            <input asp-for="RememberMe" type="hidden" />
            <div asp-validation-summary="ModelOnly" class="text-danger" role="alert"></div>
            <div class="form-floating mb-3">
                <input asp-for="Input.TwoFactorCode" class="form-control" autocomplete="off" />
                <label asp-for="Input.TwoFactorCode" class="form-label">驗證碼</label>
                <span asp-validation-for="Input.TwoFactorCode" class="text-danger"></span>
            </div>
            <div class="checkbox mb-3">
                <label asp-for="Input.RememberMachine" class="form-label">
                    <input asp-for="Input.RememberMachine" />
                    @Html.DisplayNameFor(m => m.Input.RememberMachine)
                </label>
            </div>
            <div>
                <button type="submit" class="w-100 btn btn-lg btn-primary">登入</button>
            </div>
        </form>
    </div>
</div>
<p>
    如果無法使用驗證器裝置，您可以
    <a id="recovery-code-login" asp-page="./LoginWithRecoveryCode" asp-route-returnUrl="@Model.ReturnUrl">使用備援碼登入</a>。
</p>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
'@

"Areas\Identity\Pages\Account\LoginWithRecoveryCode.cshtml" = @'
@page
@model LoginWithRecoveryCodeModel
@{
    ViewData["Title"] = "備援碼驗證";
}

<h1>@ViewData["Title"]</h1>
<hr />
<p>
    您已選擇使用備援碼登入。此登入在您提供驗證器代碼或停用 2FA 並重新登入前，不會被記住。
</p>
<div class="row">
    <div class="col-md-4">
        <form method="post">
            <div asp-validation-summary="ModelOnly" class="text-danger" role="alert"></div>
            <div class="form-floating mb-3">
                <input asp-for="Input.RecoveryCode" class="form-control" autocomplete="off" placeholder="備援碼" />
                <label asp-for="Input.RecoveryCode" class="form-label">備援碼</label>
                <span asp-validation-for="Input.RecoveryCode" class="text-danger"></span>
            </div>
            <button type="submit" class="w-100 btn btn-lg btn-primary">登入</button>
        </form>
    </div>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
'@

"Areas\Identity\Pages\Account\Manage\ResetAuthenticator.cshtml" = @'
@page
@model ResetAuthenticatorModel
@{
    ViewData["Title"] = "重設驗證器金鑰";
    ViewData["ActivePage"] = ManageNavPages.TwoFactorAuthentication;
}

<partial name="_StatusMessage" for="StatusMessage" />
<h3>@ViewData["Title"]</h3>
<div class="alert alert-warning" role="alert">
    <p>
        <span class="glyphicon glyphicon-warning-sign"></span>
        <strong>重設驗證器金鑰後，您的驗證器應用程式將無法使用，直到您重新設定它。</strong>
    </p>
    <p>
        此程序會停用 2FA，直到您驗證驗證器應用程式。
        若未完成驗證器設定，可能會導致無法存取您的帳號。
    </p>
</div>
<div>
    <form id="reset-authenticator-form" method="post">
        <button id="reset-authenticator-button" class="btn btn-danger" type="submit">重設驗證器金鑰</button>
    </form>
</div>
'@

"Areas\Identity\Pages\Account\Manage\Disable2fa.cshtml" = @'
@page
@model Disable2faModel
@{
    ViewData["Title"] = "停用雙重驗證 (2FA)";
    ViewData["ActivePage"] = ManageNavPages.TwoFactorAuthentication;
}

<partial name="_StatusMessage" for="StatusMessage" />
<h3>@ViewData["Title"]</h3>

<div class="alert alert-warning" role="alert">
    <p>
        <strong>此操作僅會停用 2FA。</strong>
    </p>
    <p>
        停用 2FA 並不會變更驗證器應用程式所使用的金鑰。如果您要更改金鑰，請使用 <a asp-page="./ResetAuthenticator">重設驗證器金鑰</a>。
    </p>
</div>

<div>
    <form method="post">
        <button class="btn btn-danger" type="submit">停用 2FA</button>
    </form>
</div>
'@
}

# 寫入檔案
foreach ($rel in $files.Keys) {
    $full = Join-Path $root $rel
    $dir = Split-Path $full -Parent
    if (-not (Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
    }
    if (Test-Path $full) {
        Copy-Item -Path $full -Destination "$full.bak" -Force
    }
    $files[$rel] | Out-File -FilePath $full -Encoding utf8 -Force
    Write-Host "已寫入: $full"
}

Write-Host "完成。已備份原始檔案（存在時為 .bak）。"