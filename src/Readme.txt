  _    _ ___         _    _           _                           ___  ______      
 | |  | |__ \       | |  | |         | |                         |__ \|  ____/\    
 | |  | |  ) |      | |  | |_ __ ___ | |__  _ __ __ _  ___ ___      ) | |__ /  \   
 | |  | | / /       | |  | | '_ ` _ \| '_ \| '__/ _` |/ __/ _ \    / /|  __/ /\ \  
 | |__| |/ /_       | |__| | | | | | | |_) | | | (_| | (_| (_) |  / /_| | / ____ \ 
  \____/|____|       \____/|_| |_| |_|_.__/|_|  \__,_|\___\___/  |____|_|/_/    \_\
                                                                                   
                                                                                   

To enable 2FA 

1. Set your issuer. This helps users destinguish in their authenticator app what site this is for. Helpful
   if your users use the same username across multiple sites.
    Add to your AppSettings in your web.config  <add key="TOTPIssuer" value="Your Site's Name" />

2. Enable 2FA by adding the following to ConfigureUmbracoAuthentication in your owin startup class

using Umbraco.Core.Mapping;
using System;
using Umbraco.Web.Security;
using U2.Middleware;
using Umbraco.Core.Models.Identity;
using Umbraco.Core.Security;
...

// To enable 2FA use the two factor sign in cookie and configure the usermanager to use U2BackOfficeUserManager
app.UseTwoFactorSignInCookie(Umbraco.Core.Constants.Security.BackOfficeTwoFactorAuthenticationType,
    TimeSpan.FromMinutes(20));

UmbracoMapper umbracoMapper = Umbraco.Core.Composing.Current.Mapper;
app.ConfigureUserManagerForUmbracoBackOffice<U2BackOfficeUserManager, BackOfficeIdentityUser>(
    Umbraco.Web.Composing.Current.RuntimeState,
    GlobalSettings,
    (options, context) =>
    {
        var membershipProvider = MembershipProviderExtensions.GetUsersMembershipProvider().AsUmbracoMembershipProvider();
        var userManager = U2BackOfficeUserManager.Create(options,
            Services.UserService,
            Services.MemberTypeService,
            Services.EntityService,
            Services.ExternalLoginService,
            membershipProvider,
            GlobalSettings,
            umbracoMapper);
        return userManager;
    });

  For example:
    protected override void ConfigureUmbracoAuthentication(IAppBuilder app)
    {
        // Enable token authentication, must be called before ConfigureUmbracoAuthentication
        app.UseUmbracoBackOfficeTokenAuth(new BackOfficeAuthServerProviderOptions());

        // To enable 2FA use the two factor sign in cookie and configure the usermanager to use U2BackOfficeUserManager
        app.UseTwoFactorSignInCookie(Umbraco.Core.Constants.Security.BackOfficeTwoFactorAuthenticationType,
            TimeSpan.FromMinutes(20));

        UmbracoMapper umbracoMapper = Umbraco.Core.Composing.Current.Mapper;
        app.ConfigureUserManagerForUmbracoBackOffice<U2BackOfficeUserManager, BackOfficeIdentityUser>(
            Umbraco.Web.Composing.Current.RuntimeState,
            GlobalSettings,
            (options, context) =>
            {
                var membershipProvider = MembershipProviderExtensions.GetUsersMembershipProvider().AsUmbracoMembershipProvider();
                var userManager = U2BackOfficeUserManager.Create(options,
                    Services.UserService,
                    Services.MemberTypeService,
                    Services.EntityService,
                    Services.ExternalLoginService,
                    membershipProvider,
                    GlobalSettings,
                    umbracoMapper);
                return userManager;
            });

        // Must call the base implementation to configure the default back office authentication config.
        base.ConfigureUmbracoAuthentication(app);
    }