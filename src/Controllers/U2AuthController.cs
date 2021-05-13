using OtpNet;
using System.Linq;
using System.Web.Http;
using U2.Models;
using U2.Services;
using Umbraco.Web.WebApi;

namespace U2.Controllers
{
    public class U2AuthController : UmbracoAuthorizedApiController
    {
        private readonly U2Service _u2Service;

        public U2AuthController(U2Service u2Service)
        {
            _u2Service = u2Service;
        }

        [HttpGet]
        public bool TwoFactorEnabled()
        {

            var user = Security.CurrentUser;

            var result = _u2Service.GetTwoFactorEnabled(user.Id);

            return result;
        }

        [HttpGet]
        public U2MFASetup TOTPAuthenticatorSetupCode()
        {
            var user = Security.CurrentUser;
            byte[] secretKey = KeyGeneration.GenerateRandomKey(20);
            string secret = Base32Encoding.ToString(secretKey);
            string userName = user.Username;

            string issuer = System.Configuration.ConfigurationManager.AppSettings["TOTPIssuer"] ?? "U2";
            string qrCodeUri = $"otpauth://totp/{issuer}:{userName}?secret={secret}&issuer={issuer}" ;

            var twoFactorAuthInfo = new U2MFASetup();
            twoFactorAuthInfo.Email = user.Email;
            twoFactorAuthInfo.Secret = secret;
            twoFactorAuthInfo.ApplicationName = issuer;
            twoFactorAuthInfo.QrCodeUri = qrCodeUri;

            return twoFactorAuthInfo;
        }

        [HttpPost]
        public bool ValidateAndSaveTOTPAuth(string code, string secret)
        {
            var user = Security.CurrentUser;
            return _u2Service.ValidateAndSaveTOTPAuth(code, user.Id, secret);
        }

        [HttpPost]
        public bool Disable()
        {
            var result = 0;
            var isAdmin = Security.CurrentUser.Groups.Select(x => x.Name == "Administrators").FirstOrDefault();
            if (isAdmin)
            {
                var user = Security.CurrentUser;
                result = _u2Service.Disable(user.Id);
                //if more than 0 rows have been deleted, the query ran successfully
            }
            return result != 0;
        }
    }
}