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

// Global Vars
var miniCartItemListViewModel = null;
var miniCartUpdateViewModel = null;

//
//Jquery Actions and Functions
//
function manageMiniCartActions() {
    $(document).ready(function () {
        $('.toggle-cart').hover(function () {
            $('.minicart').slideDown(500);
            return false;
        });
        
        $('.minicart').mouseleave(function () {
            $(this).slideUp(500);
            return false;
        });

        $('.minicart-content').on('click', ".minicart-delete", function (event) {
            $(event.currentTarget).find(".glyphicon").removeClass("glyphicon-remove-circle");
            $(event.currentTarget).find(".glyphicon").addClass("glyphicon-refresh");
            $(event.currentTarget).find(".glyphicon").addClass("glyphicon-refresh-animate");
            var lineItem = $(event.currentTarget).parent();
            var lineItemId = lineItem.attr("data-ajax-lineitemid");

            ClearGlobalMessages();
            AJAXPost("api/storefront/cart/DeleteLineItem", "{'ExternalCartLineId':'" + lineItemId + "'}", removeItemResponse, lineItem);
            return false;
        });
    });
}

function removeItemResponse(data, success, sender) {
    if (success && data.Success) {       
        $(sender).slideUp(200);
        miniCartItemListViewModel.reload(data);
    }

    $(sender).find(".glyphicon").removeClass("glyphicon-refresh");
    $(sender).find(".glyphicon").removeClass("glyphicon-refresh-animate");
    $(sender).find(".glyphicon").addClass("glyphicon-remove-circle");
    ShowGlobalMessages(data);
}

function initMiniShoppingCart(sectionId) {
    ClearGlobalMessages();
    AJAXPost(StorefrontUri("api/storefront/cart/getcurrentcart"), null, function (data, success, sender) {
        if (success && data.Success) {
            miniCartItemListViewModel = new MiniCartItemListViewModel(data);
            ko.applyBindings(miniCartItemListViewModel, document.getElementById(sectionId));
            manageMiniCartActions();
        }

        ShowGlobalMessages(data);
    });
}

function UpdateMiniCart(updateCart) {
    ClearGlobalMessages();
    AJAXPost(StorefrontUri("api/storefront/cart/getcurrentcart"), null, function (data, success, sender) {
        if (success && data.Success) {
            miniCartItemListViewModel.reload(data);
        }

        ShowGlobalMessages(data);
    });
}

function initCartAmount(updateAmount) {
    dontBlockUI = true;

    var data = null;
    if (updateAmount != undefined && updateAmount) {
        data = '{ "updateCart" : true}';
    }

    ClearGlobalMessages();
    AJAXPost(StorefrontUri("api/storefront/cart/updateminicart"), null, function (data) {
        if (success && data.Success) {
            miniCartUpdateViewModel = new MiniCartViewModel(data.LineItemCount, data.Total);
            ko.applyBindings(miniCartUpdateViewModel, document.getElementById("B02-Basket"));
        }

        ShowGlobalMessages(data);
    }, null);
}

//
// ViewModel Definitions & ViewModel Logic
//
function basketitem() {
    var self = this;
    self.image = "http://placehold.it/80x80";
    self.displayName = "Empty Element";
    self.quantity = 100;
    self.linePrice = 999.00;
    self.productUrl = "#";
}

function MiniCartViewModel(count, total) {
    this.lineitemcount = count;
    this.total = total;
}


function MiniCartItemViewModel(image, displayName, quantity, linePrice, productUrl, externalCartlineId) {
    var self = this;

    self.image = image;
    self.displayName = displayName;
    self.quantity = quantity;
    self.linePrice = linePrice;
    self.productUrl = productUrl;
    self.externalCartlineId = externalCartlineId;
}

function MiniCartItemListViewModel(data) {
    if (data != null) {
        var self = this;

        self.miniCartItems = ko.observableArray();

        $(data.Lines).each(function () {
            self.miniCartItems.push(new MiniCartItemViewModel(this.Image, this.DisplayName, this.Quantity, this.LinePrice, this.ProductUrl, this.ExternalCartLineId));
        });

        self.lineitemcount = ko.observable(data.Lines.length);
        self.total = ko.observable(data.Subtotal);

        self.reload = function (data) {
            self.miniCartItems.removeAll();

            $(data.Lines).each(function () {
                self.miniCartItems.push(new MiniCartItemViewModel(this.Image, this.DisplayName, this.Quantity, this.LinePrice, this.ProductUrl, this.ExternalCartLineId));
            });
            self.lineitemcount(data.Lines.length);
            self.total(data.Subtotal);
        }
    }
}