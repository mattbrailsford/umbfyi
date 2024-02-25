angular.module("umbraco")
    .controller("Umb.Fyi.Controllers.ImportController", function ($scope, $http, editorState, listViewHelper, udiService, contentEditingHelper, notificationsService) {

        var vm = this;

        vm.mediaType = "news";
        vm.dateRange = undefined;
        vm.fetchBtnState = "init";
        vm.mediaItems = [];
        vm.selectedMediaItems = [];

        vm.options = {
            datePickerConfig: {
                mode: "range",
                dateFormat: "Y-m-d",
                showMonths: 1,
                enableTime: false
            },
            tableProperties: [
                { alias: "friendlyTags", header: "Tags" },
                { alias: "date", header: "Date" }
            ]
        }

        function stripHtml (html) {
            let tmp = document.createElement("DIV");
            tmp.innerHTML = html;
            return tmp.textContent || tmp.innerText || "";
        }

        vm.datePickerChange = function (selectedDates, dateStr, instance) {
            if (selectedDates.length == 2) {
                vm.dateRange = selectedDates;
            }
        }

        vm.selectAll = function() {
            listViewHelper.selectAllItemsToggle(vm.mediaItems, vm.selectedMediaItems);
        }

        vm.isSelectedAll = function () {
            return listViewHelper.isSelectedAll(vm.mediaItems, vm.selectedMediaItems);
        }

        vm.selectItem = function (selectedItem, $index, $event) {
            $event.preventDefault();
            listViewHelper.selectHandler(selectedItem, $index, vm.mediaItems, vm.selectedMediaItems, $event);
        }

        vm.clickItem = function (item) {
            window.open(item.link);
        }

        vm.fetch = function () {
            vm.fetchBtnState = "busy";
            $http.get(`/umbraco/backoffice/api/backofficeumbfyiapi/getmediaitems?type=${vm.mediaType}&from=${vm.dateRange[0].toIsoDateString()}&to=${vm.dateRange[1].toIsoDateString() }`)
                .then(function (resp) {
                    vm.mediaItems = resp.data.map(i => {
                        return {
                            icon: "icon-document",
                            name: i.title,
                            friendlyTags: i.tags.join(", "),
                            ...i
                        }
                    });
                    vm.fetchBtnState = "init";
                })
                .catch(function (error) {
                    console.log(error);
                    vm.fetchBtnState = "error";
                });
        }

        vm.import = function () {
             
            vm.importBtnState = "busy";

            var content = angular.copy(editorState.current);
            var contentProperty = content.variants[0].tabs[0].properties.find(prop => prop.alias == 'content');
            var noSelectedItems = vm.selectedMediaItems.length;

            vm.mediaItems.filter(x => x.selected).reverse().forEach((itm, idx) => {

                // Convert selected item to content item
                var contentItem = {
                    udi: udiService.create("element"),
                    contentTypeKey: "808cbd76-e16f-44cb-93ca-f2b207771514",
                    itemEmoji: vm.mediaType == "news" ? "📰" : "📆",
                    itemTitle: itm.title,
                    itemDescription: stripHtml(itm.description),
                    itemLink: itm.link
                }

                if (itm.tags.includes("package")) contentItem.itemEmoji = "📦";
                else if (itm.tags.includes("youtube")) contentItem.itemEmoji = "📺";
                else if (itm.tags.includes("podcast")) contentItem.itemEmoji = "🎧";
                else if (itm.tags.includes("social")) contentItem.itemEmoji = "💬";
                else if (itm.tags.includes("rfc")) contentItem.itemEmoji = "👀";
                else if (itm.tags.includes("announcement")) contentItem.itemEmoji = "🚨";
                else if (itm.tags.includes("training")) contentItem.itemEmoji = "📚";

                contentProperty.value.contentData.push(contentItem);

                // Insert a layout item at the right index
                var layoutItem = {
                    contentUdi: contentItem.udi
                };

                var contentGroupAlias = vm.mediaType == "news" ? "miscellaneous" : "events";
                if (itm.tags.includes("hq") && itm.tags.includes("blog")) contentGroupAlias = "hq";
                if (itm.tags.includes("community") && itm.tags.includes("blog")) contentGroupAlias = "community";
                else if (itm.tags.includes("youtube") || itm.tags.includes("podcast")) contentGroupAlias = "watch-and-listen";
                else if (itm.tags.includes("package") || itm.tags.includes("marketplace")) contentGroupAlias = "packages";
                else if (itm.tags.includes("social")) contentGroupAlias = "social";
                else if (itm.tags.includes("training")) contentGroupAlias = "training";

                var groupContent = contentProperty.value.contentData.find(x => x.groupAlias == contentGroupAlias);
                var groupLayoutIndex = groupContent ? contentProperty.value.layout['Umbraco.BlockList'].findIndex(x => x.contentUdi == groupContent.udi) : -1;

                if (groupLayoutIndex >= 0) {
                    contentProperty.value.layout['Umbraco.BlockList'].splice(groupLayoutIndex + 1, 0, layoutItem);
                } else {
                    contentProperty.value.layout['Umbraco.BlockList'].push(layoutItem);
                }
            });

            // Trigger content update
            contentEditingHelper.reBindChangedProperties(editorState.current, content);

            // Clear selection
            listViewHelper.clearSelection(vm.mediaItems, null, vm.selectedMediaItems);

            // Update button
            vm.importBtnState = "success";

            // Show notification
            notificationsService.success("Import Successful", `Successfully imported ${noSelectedItems} items.`);
        }

    });