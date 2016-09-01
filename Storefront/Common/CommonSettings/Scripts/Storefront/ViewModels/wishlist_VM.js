//-----------------------------------------------------------------------
// Copyright 2016 Sitecore Corporation A/S
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file 
// except in compliance with the License. You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the 
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
// either express or implied. See the License for the specific language governing permissions 
// and limitations under the License.
// -------------------------------------------------------------------------------------------

function WishListHeaderViewModel(wishListHeader) {
    var self = this;
    var populate = wishListHeader != null ? true : false;

    self.externalId = populate ? ko.observable(wishListHeader.ExternalId) : ko.observable();
    self.name = populate ? ko.observable(wishListHeader.Name) : ko.observable();
    self.isFavorite = populate ? ko.observable(wishListHeader.IsFavorite) : ko.observable();
    self.detailsUrl = populate ? ko.observable(wishListHeader.DetailsUrl) : ko.observable();
    self.showEditName = ko.observable(false);
}

function WishListLineViewModel(wishListLine) {
    var self = this;
    var populate = wishListLine != null ? true : false;

    self.image = populate ? ko.observable(wishListLine.Image) : ko.observable();
    self.displayName = populate ? ko.observable(wishListLine.DisplayName) : ko.observable();
    self.color = populate ? ko.observable(wishListLine.Color) : ko.observable();
    self.lineDiscount = populate ? ko.observable(wishListLine.LineDiscount) : ko.observable();
    self.quantity = populate ? ko.observable(wishListLine.Quantity) : ko.observable();
    self.linePrice = populate ? ko.observable(wishListLine.LinePrice) : ko.observable();
    self.lineTotal = populate ? ko.observable(wishListLine.LineTotal) : ko.observable();
    self.externalLineId = populate ? ko.observable(wishListLine.ExternalLineId) : ko.observable();
    self.productUrl = populate ? ko.observable(wishListLine.ProductUrl) : ko.observable();
    self.productId = populate ? ko.observable(wishListLine.ProductId) : ko.observable();
    self.variantId = populate ? ko.observable(wishListLine.VariantId) : ko.observable();
    self.productCatalog = populate ? ko.observable(wishListLine.ProductCatalog) : ko.observable();
    self.wishListId = populate ? ko.observable(wishListLine.WishListId) : ko.observable();
    self.giftCardAmount = populate ? ko.observable(wishListLine.GiftCardAmount) : ko.observable();
}

function WishListViewModel(wishList) {
    var self = this;
    var populate = wishList != null ? true : false;

    self.lines = ko.observableArray();
    if (populate) {
        $(wishList.Lines).each(function () {
            self.lines.push(new WishListLineViewModel(this));
        });
    }

    self.showLoader = ko.observable(false);
    self.name = populate ? ko.observable(wishList.Name) : ko.observable();
    self.externalId = populate ? ko.observable(wishList.ExternalId) : ko.observable();
    self.isFavorite = populate ? ko.observable(wishList.IsFavorite) : ko.observable();
    self.isNotEmpty = ko.computed({
        read: function () { return self.lines().length !== 0 && self.showLoader() === false },
        write: function () { }
    });
    self.isEmpty = ko.computed({
        read: function () { return self.lines().length === 0 && self.showLoader() === false },
        write: function () { }
    });

    self.reload = function (data) {
        self.lines.removeAll();
        $(data.Lines).each(function () {
            self.lines.push(new WishListLineViewModel(this));
        });

        self.name(data.Name);
        self.isFavorite(data.IsFavorite);
        self.externalId(data.ExternalId);
        self.isNotEmpty(self.lines().length !== 0);
        self.isEmpty(self.lines().length === 0);
    }

    self.load = function (wishListId) {
        if (wishListId === undefined || wishListId.length === 0) {
            return;
        }

        ClearGlobalMessages();
        self.showLoader(true);
        var data = {};
        data.ExternalId = wishListId;
        AJAXPost(StorefrontUri("api/storefront/wishlist/getWishList"), JSON.stringify(data), function (data, success, sender) {
            if (success && data.Success) {
                self.reload(data);
            }

            self.showLoader(false);
            ShowGlobalMessages(data);
        });
    }

    self.deleteItem = function (item, event) {
        ClearGlobalMessages();
        var deleteButton = $($(event.currentTarget)[0].firstChild);
        deleteButton.removeClass("glyphicon-remove-circle");
        deleteButton.addClass("glyphicon glyphicon-refresh glyphicon-refresh-animate");
        AJAXPost(StorefrontUri('api/storefront/wishlist/deleteLineItem'), JSON.stringify(ko.toJS(this)), function (data, success, sender) {
            if (success && data.Success) {
                self.reload(data);
            }

            ShowGlobalMessages(data);
        });
    }

    self.updateItem = function () {
        ClearGlobalMessages();
        AJAXPost(StorefrontUri('api/storefront/wishlist/updateLineItem'), JSON.stringify(ko.toJS(this)), function (data, success, sender) {
            if (success && data.Success) {
                self.reload(data);
            }

            ShowGlobalMessages(data);
        });
    }
}

function WishListHeadersViewModel(data) {
    var self = this;

    self.wishLists = ko.observableArray();
    $(data.WishLists).each(function () {
        self.wishLists.push(new WishListHeaderViewModel(this));
    });
    self.selectedList = ko.observable(new WishListViewModel());
    self.selectedListId = ko.observable();
    self.selectedListId.subscribe(function (externald) {
        ClearGlobalMessages();
        self.selectedList().load(externald);
    }.bind(this));
    self.showLoader = ko.observable(true);
    self.isNotEmpty = ko.observable(self.wishLists().length !== 0);
    self.isEmpty = ko.observable(self.wishLists().length === 0);

    self.delete = function (item, event) {
        ClearGlobalMessages();
        var deleteButton = $($(event.currentTarget)[0].firstChild);
        deleteButton.removeClass("glyphicon-remove");
        deleteButton.addClass("glyphicon glyphicon-refresh glyphicon-refresh-animate");
        var data = {};
        data.ExternalId = this.externalId();
        AJAXPost(StorefrontUri('api/storefront/wishlist/deleteWishList'), JSON.stringify(data), function (data, success, sender) {
            if (success && data.Success) {
                self.reload(data);
            }

            ShowGlobalMessages(data);
        }, this);
    }

    self.create = function () {
        if ($('#createWishListClose').length > 0) {
            $('#createWishListClose').trigger('click');
        }

        ClearGlobalMessages();
        $("#createWishList").prop("disabled", true);
        $("#createWishList").html($("#createWishList").attr("data-loading-text"));
        var data = {};
        data.Name = $("#wishList-name").val();
        AJAXPost('/api/storefront/wishlist/createWishList', JSON.stringify(data), function (data, success, sender) {
            if (success && data.Success) {
                self.reload(data);
                $('#wishList-name').val('');
                manageWishListActions();
            }

            $("#createWishList").prop("disabled", true);
            $("#createWishList").html($("#createWishList").attr("data-text"));
            ShowGlobalMessages(data);
        }, this);
    }

    self.makeFavorite = function () {
        ClearGlobalMessages();
        var data = {};
        data.ExternalId = this.externalId();
        data.Name = this.name();
        data.IsFavorite = true;
        AJAXPost(StorefrontUri("api/storefront/wishlist/updateWishList"), JSON.stringify(data), function (data, success, sender) {
            if (success && data.Success) {
                self.wishLists.removeAll();
                self.reload(data);
            }

            ShowGlobalMessages(data);
        });
    }

    self.reload = function (data) {
        self.wishLists.removeAll();
        $(data.WishLists).each(function () {
            self.wishLists.push(new WishListHeaderViewModel(this));
        });

        self.isNotEmpty(self.wishLists().length !== 0);
        self.isEmpty(self.wishLists().length === 0);
    }

    self.load = function () {
        ClearGlobalMessages();
        AJAXPost(StorefrontUri("api/storefront/wishlist/activeWishLists"), null, function (data, success, sender) {
            if (success && data.Success) {
                self.reload(data);
            }

            ShowGlobalMessages(data);
        });
    }

    self.editName = function (item, event) {
        var show = item.showEditName();
        item.showEditName(!show);
    }

    self.rename = function (item, event) {
        ClearGlobalMessages();

        var data = {};
        data.ExternalId = item.externalId();
        data.Name = item.name();

        var renameButton = $(event.currentTarget);
        renameButton.button('loading');
        AJAXPost(StorefrontUri("api/storefront/wishlist/updateWishList"), JSON.stringify(data), function (data, success, sender) {
            if (success && data.Success) {
                self.reload(data);
            }

            ShowGlobalMessages(data);
        }, item);
    }
}