using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Umbraco.Core.Cache;
using Umbraco.Core.Composing;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Mapping;
using Umbraco.Core.Models.Identity;
using Umbraco.Core.Security;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Security;


namespace U2.Middleware
{
    /// <summary>
    /// Subclass the default BackOfficeUserManager and extend it to support 2FA
    /// </summary>
    public class U2BackOfficeUserManager : BackOfficeUserManager, IUmbracoBackOfficeTwoFactorOptions
    {
        public U2BackOfficeUserManager(IUserStore<BackOfficeIdentityUser, int> store) : base(store)
        { }

        /// <summary>
        /// Creates a BackOfficeUserManager instance with all default options and the default BackOfficeUserManager 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="userService"></param>
        /// <param name="entityService"></param>
        /// <param name="externalLoginService"></param>
        /// <param name="membershipProvider"></param>
        /// <returns></returns>
        public static U2BackOfficeUserManager Create(
            IdentityFactoryOptions<U2BackOfficeUserManager> options,
            IUserService userService,
            IMemberTypeService memberTypeService,
            IEntityService entityService,
            IExternalLoginService externalLoginService,
            MembershipProviderBase membershipProvider,
            IGlobalSettings globalSettings, 
            UmbracoMapper umbracoMapper)
        {
            if (options == null) throw new ArgumentNullException("options");
            if (userService == null) throw new ArgumentNullException("userService");
            if (entityService == null) throw new ArgumentNullException("entityService");
            if (externalLoginService == null) throw new ArgumentNullException("externalLoginService");

            var manager = new U2BackOfficeUserManager(new U2BackOfficeUserStore(userService, memberTypeService, externalLoginService, entityService, membershipProvider, globalSettings, umbracoMapper, Current.AppCaches));
            manager.InitUserManager(manager, membershipProvider, options.DataProtectionProvider, Current.Configs.GetConfig<IUmbracoSettingsSection>().Content);

            //Here you can specify the 2FA providers that you want to implement
            var dataProtectionProvider = options.DataProtectionProvider;
            manager.RegisterTwoFactorProvider("TOTPAuthenticator", new TOTPAuthenticationProvider());

            return manager;
        }

        /// <inheritdoc />
        /// <summary>
        /// Override to return true
        /// </summary>
        public override bool SupportsUserTwoFactor
        {
            get { return true; }
        }

        /// <summary>
        /// Return the view for the 2FA screen
        /// </summary>
        /// <param name="owinContext"></param>
        /// <param name="umbracoContext"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public string GetTwoFactorView(IOwinContext owinContext, UmbracoContext umbracoContext, string username)
        {
            return "../App_Plugins/U2/2fa-login.html";
        }
    }
}