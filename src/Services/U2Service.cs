using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using U2.Models;
using Umbraco.Core.Composing;
using Umbraco.Core.Persistence;
using Umbraco.Core.Scoping;

namespace U2.Services
{
    public class U2Service
    {
        private readonly IScopeProvider scopeProvider;

        public U2Service(IScopeProvider scopeProvider)
        {
            this.scopeProvider = scopeProvider;
        }

        public bool GetTwoFactorEnabled(int id)
        {
            using (var scope = scopeProvider.CreateScope(autoComplete: true))
            {
                var result = scope.Database.Fetch<U2UserSettings>("WHERE [userId] = @userId AND [IsAuthenticatorEnabled] = 1", new { userId = id });
                return result.Any();
            }
        }

        public bool ValidateAndSaveTOTPAuth(string code, int userId, string secret = null)
        {
            if(code == null)
            {
                return false;
            }
            using (var scope = scopeProvider.CreateScope(autoComplete: true))
            {
                try
                {
                    var all = scope.Database.Fetch<U2UserSettings>("WHERE userId = @userId",
                        new { userId = userId });

                    var user = all.FirstOrDefault();
                    if (user != null)
                    {
                        var otp = new Totp(Base32Encoding.ToBytes(user.UserSecret));
                        bool isValid = otp.VerifyTotp(code, out long timeStepMatched, new VerificationWindow(2, 2));

                        if (isValid == false)
                            return false;

                        if (!user.IsAuthenticatorEnabled)
                        {
                            user.IsAuthenticatorEnabled = true;
                            var update = scope.Database.Update(user);
                            isValid = update > 0;
                            return isValid;
                        }
                        return true;
                    }
                    else
                    {
                        // new setup
                        user = new U2UserSettings { 
                            UserId = userId,
                            UserSecret = secret,
                            IsAuthenticatorEnabled = true
                        };

                        var otp = new Totp(Base32Encoding.ToBytes(user.UserSecret));
                        bool isValid = otp.VerifyTotp(code, out long timeStepMatched, new VerificationWindow(2, 2));

                        if (isValid)
                        {
                            var result = scope.Database.Insert(user);
                            return true;
                        }
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Current.Logger.Error(typeof(U2Service), ex);
                }

                return false;
            }
        }

        public int Disable(int userId)
        {
            using (var scope = scopeProvider.CreateScope(autoComplete: true))
            {
                var result = scope.Database.Delete<U2UserSettings>("WHERE [userId] = @userId", new { userId = userId });
                return result;
            }
        }
    }
}