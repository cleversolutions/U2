angular.module("umbraco").controller("2FactorAuthentication.DashboardController",
    function ($scope, twoFactorService, userService, usersResource, notificationsService) {

        $scope.error2FA = "";
        $scope.code = "";
        $scope.enabledText = "disabled";
        $scope.enabled = false;
        $scope.qrCodeUri = "";
        $scope.secret = "";
        $scope.email = "";
        $scope.applicationName = "";
        $scope.TOTPAuthEnabled = false;
        $scope.isAdmin = false;
        $scope.users = [];
        $scope.isLoading = false;


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

        var getEnabledUserSettings = function () {
            $scope.isLoading = true;
            twoFactorService.getEnabledUserSettings().then(function (response) {
                usersResource.getUsers(response.data).then(function (userData) {
                    $scope.users = userData;
                    $scope.isLoading = false;
                });
            });
        };

        userService.getCurrentUser().then(function (userData) {
            $scope.isAdmin = _.indexOf(userData.userGroups, 'admin') != -1;
            if ($scope.isAdmin) {
                getEnabledUserSettings();
            }
        });

        twoFactorService.getEnabled().then(function (response) {
            if (response.data) {
                $scope.enabledText = "enabled";
                $scope.enabled = true;
                $scope.TOTPAuthEnabled = true;
            }
        });

        $scope.validateAndSaveTOTPAuth = function (code, secret) {
            $scope.error2FA = "";
            $scope.isLoading = true;
            twoFactorService.validateAndSaveTOTPAuth(code, secret)
                .then(function (response) {
                    if (response.data === true) {
                        $scope.enabledText = "enabled";
                        $scope.enabled = true;
                        $scope.TOTPAuthEnabled = true;
                    } else {
                        $scope.error2FA = "Invalid code entered.";
                    }
                    $scope.isLoading = false;
                }, function () {
                    $scope.error2FA = "Error validating code.";
                    $scope.isLoading = false;
                });
        };

        $scope.disable = function () {
            $scope.isLoading = true;
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
                    $scope.isLoading = false;
                }, function () {
                    $scope.isLoading = false;
                });
        };

        $scope.disableByAdmin = function (id) {
            if (confirm("Are you sure?")) {
                $scope.isLoading = true;
                twoFactorService.disableByAdmin(id)
                    .then(function (response) {
                        if (response.data === true) {
                            notificationsService.success("Success", "successfully");
                            getEnabledUserSettings();
                        } else {
                            notificationsService.erorr("ERROR", "Unexpected error occurred, please try again in some minutes");
                        }
                        $scope.isLoading = false;
                    }, function () {
                        $scope.isLoading = false;
                    });
            }
        };
    });