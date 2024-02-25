angular.module("umbraco")
    .controller("Umb.Fyi.Controllers.SummarizeLinkDialogController", function ($scope, $http) {

        var cfg = $scope.model.config;

        var vm = this;
        vm.canApply = cfg.canApply;
        vm.loading = true;
        vm.summary = "";

        vm.init = function () {

            vm.loading = true;

            $http.get(`/umbraco/backoffice/api/backofficeumbfyiapi/summarizelink?dataTypeKey=${cfg.dataTypeKey}&link=${encodeURIComponent(cfg.link)}`)
                .then(function (resp) {
                    vm.summary = resp.data.summary;
                    vm.loading = false;
                })
                .catch(function (error) {
                    vm.summary = "There was an error summarizing the given link: " + error.data
                    vm.loading = false;
                });
        };

        vm.close = function () {
            $scope.model.close();
        };

        vm.apply = function () {
            $scope.model.apply(vm.summary);
        };

        vm.regenerate = function () {
            vm.init();
        };

        vm.init();

    });