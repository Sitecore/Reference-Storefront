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

var VariantInfoModel = function (variantId, size, color, priceBefore, priceNow, isOnSale, savingsMessage) {
    var self = this;

    self.variantId = variantId;
    self.size = size;
    self.color = color;
    self.priceNow = priceNow;
    self.priceBefore = priceBefore;
    self.isOnSale = isOnSale;
    self.savingsMessage = savingsMessage;
}

var priceInfoVM = null;

var PriceInfoViewModel = function ()
{
    self = this;

    self.priceBefore = ko.observable();
    self.priceNow = ko.observable();
    self.savingsMessage = ko.observable();

    self.switchInfo = function (priceBefore, priceNow, isOnSale, savingsMessage) {
        self.priceNow(priceNow);
        self.priceBefore(priceBefore);
        self.savingsMessage(savingsMessage);

        if (!isOnSale) {
            $("#priceWithSavings").hide()
            $("#priceOnly").show();
        }
        else {
            $("#priceWithSavings").show()
            $("#priceOnly").hide();
        }
    };
}

function AddToCartSuccess(data) {
    if (data.Success) {
        $("#addToCartSuccess").show().fadeOut(4000);
        UpdateMiniCart();
    }

    ShowGlobalMessages(data);
    // Update Button State
    $('#AddToCartButton').button('reset');
}

function AddToCartFailure(data) {
    ShowGlobalMessages(data);
    // Update Button State
    $('#AddToCartButton').button('reset');
}

function AddVariantCombination(size, productColor, id, listPrice, adjustedPrice, isOnSale, savingsMessage) {
    if (!window.variantCombinationsArray) {
        window.variantCombinationsArray = new Array();
    }

    window.variantCombinationsArray[size + '_' + productColor] = new VariantInfoModel(id, size, productColor, listPrice, adjustedPrice, isOnSale, savingsMessage);
}

function VariantSelectionChanged() {
    var size = '';
    var color = '';

    if ($('#variantSize').length) {
        size = $('#variantSize').val();
    }

    if ($('#variantColor').length) {
        color = $('#variantColor').val();
    }

    ClearGlobalMessages();
    $('#AddToCartButton').removeAttr('disabled');
    var variantInfo = GetVariantIdByCombination(size, color);
    if (variantInfo == -1) {
        var data = [];
        data.Success = false;
        data.Errors = [$("#InvalidVariant").text()];
        ShowGlobalMessages(data);
        $('#AddToCartButton').attr('disabled', 'disabled');
    } else {
        $('#VariantId').val(variantInfo.variantId);
        if (stockInfoVM) {
            stockInfoVM.switchInfo();
        }
        if (priceInfoVM) {
            priceInfoVM.switchInfo(variantInfo.priceBefore, variantInfo.priceNow, variantInfo.isOnSale, variantInfo.savingsMessage);
        }
    }
}

function GetVariantIdByCombination(size, productColor) {
    if (!window.variantCombinationsArray || !window.variantCombinationsArray[size + '_' + productColor]) {
        return -1;
    }

    return window.variantCombinationsArray[size + '_' + productColor];
}

function CheckGiftCardBalance() {
    var giftCardId = $("#GiftCardId").val();
    $("#balance-value").html('');
    if (giftCardId.length === 0) {
        return;
    }

    states('CheckGiftCardBalanceButton', 'loading');
    var data = {};
    data.GiftCardId = giftCardId;
    ClearGlobalMessages();
    AJAXPost("/api/storefront/catalog/checkgiftcardbalance", JSON.stringify(data), function (data, success, sender) {
        if (success && data.Success) {
            $("#balance-value").html(data.FormattedBalance);
        }

        $('#CheckGiftCardBalanceButton').button('reset');
        ShowGlobalMessages(data);
    }, this);
}

function addToWishList(id) {
    var wishListId = "";
    var wishListName = "";

    if (id === "new") {
        $('#createWishListClose').trigger('click');
        wishListName = $("#wishList-name").val();
        $("#wishList-name").val("");
    } else {
        wishListId = id.id;
    }

    var formData = {};
    formData.ProductId = $("#ProductId").val();
    formData.VariantId = $("#VariantId").length > 0 ? $("#VariantId").val() : "";
    formData.ProductCatalog = $("#CatalogName").val();
    formData.Quantity = $("#Quantity").val();
    formData.GiftCardAmount = $("#GiftCard-Amount").length > 0 ? $("#GiftCard-Amount").val() : "";
    formData.WishListId = wishListId;
    formData.WishListName = wishListName;

    ClearGlobalMessages();
    $("#addToWishList").find(".glyphicon").removeClass("glyphicon-heart");
    $("#addToWishList").find(".glyphicon").addClass("glyphicon-refresh");
    $("#addToWishList").find(".glyphicon").addClass("glyphicon-refresh-animate");

    AJAXPost("/api/storefront/WishList/AddToWishList", JSON.stringify(formData), function (data, success, sender) {
        if (success && data.Success) {            
            wishListHeadersListViewModel.reload(data);
        }

        $("#addToWishList").find(".glyphicon").removeClass("glyphicon-refresh");
        $("#addToWishList").find(".glyphicon").removeClass("glyphicon-refresh-animate");
        $("#addToWishList").find(".glyphicon").addClass("glyphicon-heart");
        ShowGlobalMessages(data);
    }, this);
}

function SetAddButton() {
    $(document).ready(function () {
        ClearGlobalMessages();
        $("#AddToCartButton").button('loading');
    });
}

//-----------------------------------------------------------------//
//          SIGN UP FOR NOTIFICATION AND STOCK INFO                //
//-----------------------------------------------------------------//
var StockInfoViewModel = function(info) {
    var populate = info != null;
    var self = this;

    self.productId = populate ? ko.observable(info.ProductId) : ko.observable();
    self.variantId = populate ? ko.observable(info.VariantId) : ko.observable();
    self.status = populate ? ko.observable(info.Status) : ko.observable();
    self.count = populate ? ko.observable(info.Count) : ko.observable();
    self.availabilityDate = populate ? ko.observable(info.AvailabilityDate) : ko.observable();
    self.showSingleLabel = populate ? ko.observable(info.Count === 1) : ko.observable(false);
    self.isOutOfStock = populate ? ko.observable(info.Status === "Out-Of-Stock") : ko.observable(false);
    self.canShowSignupForNotification = populate ? ko.observable(info.CanShowSignupForNotification) : ko.observable(false);

    self.showSignUpForNotification = self.canShowSignupForNotification() && self.isOutOfStock();
}

var StockInfoListViewModel = function () {
    var self = this;

    self.stockInfos = ko.observableArray();
    self.statuses = ko.observableArray();
    self.hasInfo = ko.observable(false);
    self.selectedStockInfo = ko.observable(new StockInfoViewModel());
    self.load = function () {
        ClearGlobalMessages();
        var data = {};
        data.ProductId = $('#product-id').val();
        AJAXPost(StorefrontUri("api/storefront/catalog/GetCurrentProductStockInfo"), JSON.stringify(data), function (data, success, sender) {
            if (success && data && data.Success) {
                $.each(data.StockInformations, function () {
                    self.stockInfos.push(new StockInfoViewModel(this));
                });

                self.selectedStockInfo(new StockInfoViewModel(data.StockInformations[0]));
                self.statuses(data.Statuses);
                self.hasInfo(data.StockInformations.length > 0);

                if (self.selectedStockInfo().isOutOfStock()) {
                    $('#AddToCartButton').attr('disabled', 'disabled');
                }
            }

            ShowGlobalMessages(data);
        });
    };

    self.switchInfo = function () {
        ClearGlobalMessages();

        var productId = $("#product-id").val();
        var variantId = $('#VariantId') && $('#VariantId').length > 0 ? $('#VariantId').val() : "";
        var item = ko.utils.arrayFirst(this.stockInfos(), function (si) {
            if (si.productId() === productId && si.variantId() === variantId) {
                return si;
            }

            return null;
        });

        if (item == null) {
            self.selectedStockInfo(self.stockInfos()[0]);
        } else {
            self.selectedStockInfo(item);
        }

        if (self.selectedStockInfo().isOutOfStock()) {
            $('#AddToCartButton').attr('disabled', 'disabled');
        } else {
            $('#AddToCartButton').removeAttr('disabled');
        }
    };
}

var stockInfoVM = null;

$(function() {
    signForNotificationVM = {
        fullName: ko.validatedObservable().extend({ required: true }),
        email: ko.validatedObservable().extend({ required: true, email: true }),
        messages: ko.observable(new ErrorSummaryViewModel('signForNotificationModalMessages')),

        load: function() {
            this.messages().ClearMessages();
            AJAXPost(StorefrontUri("api/storefront/account/getcurrentuser"), null, function(data, success, sender) {
                if (success && data && data.Success) {
                    if (data.FullName && data.FullName.length > 0) {
                        signForNotificationVM.fullName(data.FullName);
                    }

                    if (data.Email && data.Email.length > 0) {
                        signForNotificationVM.email(data.Email);
                        signForNotificationVM.confirmEmail(data.Email);
                    }
                }

                signForNotificationVM.messages().AddToErrorList(data);
            });
        },

        signUp: function() {
            this.messages().ClearMessages();
            if (this.errors().length === 0) {
                states('signForNotificationButton', 'loading');
                var data = {
                    "ProductId": stockInfoVM.selectedStockInfo().productId(),
                    "CatalogName":  $("#product-catalog").val(),
                    "FullName": this.fullName(),
                    "Email": this.email(),
                    "VariantId": stockInfoVM.selectedStockInfo().variantId()
                };

                AJAXPost(StorefrontUri('api/storefront/Catalog/signupforbackinstocknotification'), JSON.stringify(data), function(data, success, sender) {
                    if (data.Success && success) {
                        // CLEANING MODEL 

                        $('#signForNotificationModal').modal('hide');
                    }

                    $('#signForNotificationButton').button('reset');
                    signForNotificationVM.messages().AddToErrorList(data);
                }, this);
            } else {
                this.errors.showAllMessages();
            }
        },

        close: function() {
            this.messages().ClearMessages();
            $('#signForNotificationModal').modal('hide');
        }
    };

    signForNotificationVM.confirmEmail = ko.validatedObservable().extend({
        validation: {
            validator: mustEqual,
            message: 'Emails do not match.',
            params: signForNotificationVM.email
        }
    });

    signForNotificationVM.errors = ko.validation.group(signForNotificationVM);

    signForNotificationVM.signUpEnable = ko.computed(function() {
        return signForNotificationVM.errors().length === 0;
    });

    if ($('#signForNotificationModal').length > 0) {
        signForNotificationVM.load();
        ko.validation.registerExtenders();
        ko.applyBindingsWithValidation(signForNotificationVM, document.getElementById('signForNotificationModal'));
    }

    if ($('#stock-info').length > 0) {
        stockInfoVM = new StockInfoListViewModel();
        stockInfoVM.load();
        ko.applyBindingsWithValidation(stockInfoVM, document.getElementById('stock-info'));
    }

    if ($('#GiftCardId').length > 0) {
        $('#GiftCardId').keyup(function () {
            $("#balance-value").html('');
            if ($(this).val().trim().length > 0) {
                $('#CheckGiftCardBalanceButton').removeAttr('disabled');
            } else {
                $('#CheckGiftCardBalanceButton').attr('disabled', 'disabled');
            }
        });
    }
});