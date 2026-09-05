using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;
using System.ComponentModel.DataAnnotations;

namespace UKCrimeWeb.Areas.Identity.Pages.Account.Manage
{
    public class EnableAuthenticatorModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public EnableAuthenticatorModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public string? SharedKey { get; set; }

        public string? QrCodeImage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required]
            [Display(Name = "Verification Code")]
            public string? Code { get; set; }
        }

        public async Task OnGetAsync()
        {
            await LoadSharedKeyAndQrCodeAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeAsync();
                return Page();
            }

            var verificationCode = Input.Code!
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty);

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                verificationCode);

            if (!isValid)
            {
                ModelState.AddModelError(
                    "Input.Code",
                    "The verification code is invalid.");

                await LoadSharedKeyAndQrCodeAsync();
                return Page();
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);

            return RedirectToPage("/Account/Manage/TwoFactorAuthentication");
        }

        private async Task LoadSharedKeyAndQrCodeAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return;
            }

            var key = await _userManager.GetAuthenticatorKeyAsync(user);

            if (string.IsNullOrWhiteSpace(key))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                key = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            SharedKey = key;

            var email = await _userManager.GetEmailAsync(user);

            var authenticatorUri =
                $"otpauth://totp/BritishCrimeFiles:{Uri.EscapeDataString(email ?? "")}" +
                $"?secret={key}&issuer=BritishCrimeFiles";

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(
                authenticatorUri,
                QRCodeGenerator.ECCLevel.Q);

            var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(20);

            QrCodeImage =
                $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";
        }
    }
}