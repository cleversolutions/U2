angular.module("umbraco.services").factory("twoFactorService", function ($http) {
    return {
        getEnabled: function () {
            return $http.get("/umbraco/backoffice/api/U2Auth/TwoFactorEnabled/");
        },
        getTOTPAuthenticatorSetupCode: function () {
            return $http.get("/umbraco/backoffice/api/U2Auth/TOTPAuthenticatorSetupCode/");
        },
        validateAndSaveTOTPAuth: function (code, secret) {
            return $http.post("/umbraco/backoffice/api/U2Auth/ValidateAndSaveTOTPAuth/?code=" + code + "&secret=" + secret);
        },
        disable: function () {
            return $http.post("/umbraco/backoffice/api/U2Auth/Disable/");
        },
        disableByAdmin: function (id) {
            return $http.post("/umbraco/backoffice/api/U2Auth/DisableByAdmin/?id=" + id);
        },
        get2FAProviders: function () {
            return $http.post("/umbraco/backoffice/api/U2Auth/Get2FAProviders/");
        },
        getEnabledUserSettings: function () {
            return $http.get("/umbraco/backoffice/api/U2Auth/GetEnabledUserSettings/");
        }
    };
});