using System.Linq;
using Microsoft.AspNet.Identity;
using U2.Models;
using System.Threading.Tasks;
using OtpNet;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.Identity;

namespace U2.Middleware
{
    public class TOTPAuthenticationProvider : IUserTokenProvider<BackOfficeIdentityUser, int>
    {
        public Task<string> GenerateAsync(string purpose, UserManager<BackOfficeIdentityUser, int> manager, BackOfficeIdentityUser user)
        {
            return Task.FromResult((string)null);
        }

        public Task<bool> ValidateAsync(string purpose, string token, UserManager<BackOfficeIdentityUser, int> manager, BackOfficeIdentityUser user)
        {
            using (var scope = Current.ScopeProvider.CreateScope(autoComplete: true))
            {
                var userSettings = scope.Database.Fetch<U2UserSettings>(
                    "WHERE [userId] = @userId AND [IsAuthenticatorEnabled] = 1", new { 
                        userId = user.Id,
                    })
                    .FirstOrDefault();

                if (userSettings == null)
                    return Task.FromResult(false);

                var otp = new Totp(Base32Encoding.ToBytes(userSettings.UserSecret));
                bool valid = otp.VerifyTotp(token, out long timeStepMatched, new VerificationWindow(1, 1));

                return Task.FromResult(valid);
            }
        }

        public Task NotifyAsync(string token, UserManager<BackOfficeIdentityUser, int> manager, BackOfficeIdentityUser user)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsValidProviderForUserAsync(UserManager<BackOfficeIdentityUser, int> manager, BackOfficeIdentityUser user)
        {
            using (var scope = Current.ScopeProvider.CreateScope(autoComplete: true))
            {
                var userSettings = scope.Database.Fetch<U2UserSettings>(
                   $"WHERE [userId] = {user.Id} AND [IsAuthenticatorEnabled] = 1")
                   .FirstOrDefault();

                if (userSettings == null)
                    return Task.FromResult(false);

                return Task.FromResult(userSettings.IsAuthenticatorEnabled);
            }
               
        }
    }
}