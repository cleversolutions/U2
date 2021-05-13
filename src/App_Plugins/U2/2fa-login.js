angular.module("umbraco").controller("2FactorAuthentication.LoginController",
    function ($scope, $cookies, localizationService, userService, externalLoginInfo, resetPasswordCodeInfo, $timeout, authResource, editorService, twoFactorService) {

        $scope.code = "";
        $scope.provider = "";
        $scope.providers = [];
        $scope.step = "send";

        $scope.providers = "TOTPAuthenticator";
        $scope.step = "code";

        $scope.validate = function (provider, code) {
            provider = "TOTPAuthenticator";
            $scope.error2FA = "";
            $scope.code = code;
            authResource.verify2FACode(provider, code)
                .then(function (data) {
                    userService.setAuthenticationSuccessful(data);
                    $scope.$parent.vm.onLogin();
                }, function () { $scope.error2FA = "Invalid code entered." });
        };
    });