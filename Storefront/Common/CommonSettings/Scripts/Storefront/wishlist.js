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

var wishListHeadersListViewModel = null;
var wishListViewModel = null;

function initWishListHeaders(sectionId, filter) {
    manageWishListActions();

    var data = {};
    data.Filter = filter == null ? false : filter;
    AJAXPost(StorefrontUri("api/storefront/wishlist/activeWishLists"), JSON.stringify(data), function (data, success, sender) {
        if (success && data.Success) {
            wishListHeadersListViewModel = new WishListHeadersViewModel(data);
            if ($("#wishListChange").length > 0) {
                var wishListId = getUrlParameter(document.URL, 'id');
                wishListHeadersListViewModel.selectedListId(wishListId);
            }

            ko.applyBindings(wishListHeadersListViewModel, document.getElementById(sectionId));
            wishListHeadersListViewModel.showLoader(false);
        }

        ShowGlobalMessages(data);
    });
}

function createWishList() {
    if ($('#createWishListClose').length > 0) {
        $('#createWishListClose').trigger('click');
    }

    ClearGlobalMessages();
    var data = {};
    data.Name = $("#wishList-name").val();
    AJAXPost('/api/storefront/wishlist/createWishList', JSON.stringify(data), function (data, success, sender) {
        if (success && data.Success) {
            $('#wishList-name').val('');
            wishListHeadersListViewModel.reload(data);
            $("#wishListsEmpty").hide();
            $("#wishListsSection").show();
            $("#wishLists").show();
        }

        ShowGlobalMessages(data);
    }, this);
}

function manageWishListActions() {
    $(document).ready(function () {
        $('#wishList-name').keyup(function () {
            if ($(this).val().trim().length > 0) {
                $('#createWishList').removeAttr('disabled');
            } else {
                $('#createWishList').attr('disabled', 'disabled');
            }
        });
    });
}

function selectAllItems(element) {
    var isChecked = $(element).is(':checked');
    $('.item-to-selected').prop('checked', isChecked);
    $('#addWishListItemsToCart').prop("disabled", !isChecked);
}

function enableAddItemsToCart() {
    $('#addWishListItemsToCart').prop("disabled", $('.item-to-selected').filter(':checked').length == 0);
}

function addWishListsToCart() {
    var ids = [];

    $('.item-to-selected').each(function () {
        if ($(this).is(':checked')) {
            ids.push($(this).attr('name'));
        }
    });

    ClearGlobalMessages();
    var data = {};
    data.Ids = ids;
    AJAXPost('/api/storefront/wishlist/addWishListsToCart', JSON.stringify(data), addWishListsToCartResponse, this);
}

function addWishListsToCartResponse(data, success, sender) {
    if (success && data.Success) {
        $('.item-to-selected').removeAttr('checked');
        $('#addWishListItemsToCart').attr('disabled', 'disabled');
        initShoppingCart();
    }

    ShowGlobalMessages(data);
}

function addWishListItemsToCart() {
    if ($('.item-to-selected').filter(':checked').length === 0) {
        return;
    }

    $("#addWishListItemsToCart").prop("disabled", true);
    $("#addWishListItemsToCart").children('span[id="buttonText"]').html($("#addWishListItemsToCart").attr("data-loading-text"));
    var data = [];
    $('.item-to-selected').filter(':checked').each(function () {
        var productId = $(this).attr('data-productId');
        data.push({ "ProductId": productId, "VariantId": $(this).attr('data-variantId'), "CatalogName": $(this).attr('data-catalog'), "Quantity": $('#' + productId).val() });
    });

    ClearGlobalMessages();
    AJAXPost(StorefrontUri('api/storefront/cart/addCartLines'), JSON.stringify(data), function (data, success, sender) {
        if (success && data.Success) {
            $('.item-to-selected').removeAttr('checked');
            $('#selectAllItems').removeAttr('checked');
            $('#addWishListItemsToCart').attr('disabled', 'disabled');
            UpdateMiniCart();
        }

        $("#addWishListItemsToCart").prop("disabled", true);
        $("#addWishListItemsToCart").children('span[id="buttonText"]').html($("#addWishListItemsToCart").attr("data-text"));
        ShowGlobalMessages(data);
    }, this);
}