angular.module("umbraco").controller("2FactorAuthentication.DashboardController",
    function ($scope, twoFactorService, userService) {

        $scope.error2FA = "";
        $scope.code = "";
        $scope.enabledText = "disabled";
        $scope.enabled = false;
        $scope.qrCodeUri = "";
        $scope.secret = "";
        $scope.email = "";
        $scope.applicationName = "";
        $scope.TOTPAuthEnabled = false;
        

        twoFactorService.getTOTPAuthenticatorSetupCode().then(function (response) {
            $scope.qrCodeUri = response.data.QrCodeUri;
            $scope.secret = response.data.Secret;
            $scope.email = response.data.Email;
            $scope.applicationName = response.data.ApplicationName;

            var typeNumber = 0;
            var errorCorrectionLevel = 'H';
            var qr = qrcode(typeNumber, errorCorrectionLevel);
            qr.addData(response.data.QrCodeUri);
            qr.make();
            document.getElementById('qrcode').innerHTML = qr.createSvgTag(5);
        });

        userService.getCurrentUser().then(function (userData) {
            twoFactorService.getEnabled(userData.id).then(function (response) {
                if (response.data) {
                    $scope.enabledText = "enabled";
                    $scope.enabled = true;
                    $scope.TOTPAuthEnabled = true;
                }
            });
        });

        $scope.validateAndSaveTOTPAuth = function (code, secret) {
            $scope.error2FA = "";
            twoFactorService.validateAndSaveTOTPAuth(code, secret)
                .then(function (response) {
                    if (response.data === true) {
                        $scope.enabledText = "enabled";
                        $scope.enabled = true;
                        $scope.TOTPAuthEnabled = true;
                    } else {
                        $scope.error2FA = "Invalid code entered.";
                    }
                }, function () {
                    $scope.error2FA = "Error validating code.";
                });
        };

        $scope.disable = function () {
            twoFactorService.disable()
                .then(function (response) {
                    if (response.data === true) {
                        $scope.enabledText = "disabled";
                        $scope.enabled = false;
                        $scope.TOTPAuthEnabled = false;
                      
                        twoFactorService.getTOTPAuthenticatorSetupCode().then(function (response) {
                           
                            $scope.qrCodeUri = response.data.QrCodeUri;
                            $scope.secret = response.data.Secret;
                            $scope.email = response.data.Email;
                            $scope.applicationName = response.data.ApplicationName;
                        });
                    } else {
                        $scope.error2FA = "You don't have permission to do this action, please contact your administrator.";
                    }
                });
        };
    });