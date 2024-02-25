angular.module("umbraco")
    .controller("Umb.Fyi.PropertyEditors.linkinputController", function ($scope, editorService, angularHelper) {

        $scope.openLink = function () {
            if ($scope.model.value.startsWith("http")) {
                window.open($scope.model.value);
            }
        }

        $scope.summarizeLink = function () {
            if ($scope.model.value.startsWith("http")) {
                editorService.open({
                    view: '/App_Plugins/UmbFyi/backoffice/views/dialogs/summarizelink.html',
                    size: 'small',
                    config: {
                        link: $scope.model.value,
                        dataTypeKey: $scope.model.dataTypeKey,
                        canApply: !!$scope.model.config.targetProperty
                    },
                    apply: function (summary) {
                        if ($scope.model.config.targetProperty) {
                            var found = angularHelper.traverseScopeChain($scope, s => s && s.vm && s.vm.constructor.name === "ElementEditorContentComponentController");
                            if (found && found.vm && found.vm.model) {
                                var prop = found.vm.model.variants[0].tabs[0].properties.find(p => p.alias == $scope.model.config.targetProperty);
                                if (prop) {
                                    prop.value = summary
                                }
                            }
                        }
                        editorService.close();
                    },
                    close: function () {
                        editorService.close();
                    }
                });
            }
        }

    });