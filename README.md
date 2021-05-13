# U2
Umbraco 8 Two Factor Authentication

Enable two factor authentication using a TOTP authenticator app (ie Google Authenticator or Microsoft Authenticator)

## Credits
Based on https://github.com/Dallas-msc/umbraco-2fa-with-google-authenticator/tree/master/YubiKey2Factor

## Setup
1. Install the U2 nuget package
2. Add your issuer (a name people can use to remember your site by in their Authenticator App) to AppSettings
3. Update you Owin startup file as per the Readme.txt

## Usage
Once setup, you will see a new dashboard in the backoffice content section. Using your authenticator app of choice, scan the barcode, enter the verification code, hit enter. From now on your account will require a code from your authenticator app to login.

## Locked Out
If you ever loose your authenticator app but you have access to the database, simply delete your user account from the U2UserSettings table in your Umbraco database.
