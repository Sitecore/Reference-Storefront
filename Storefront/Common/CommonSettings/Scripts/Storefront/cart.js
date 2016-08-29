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

var lineItemListViewModel = null;

function manageCartActions() {
    $(document).ready(function () {
        $(".delete-item").click(function () {
            var lineItem = $(this);
            var lineItemId = lineItem.attr("data-ajax-lineitemid");
            $(this).find(".glyphicon").removeClass("glyphicon-remove-circle");
            $(this).find(".glyphicon").addClass("glyphicon-refresh");
            $(this).find(".glyphicon").addClass("glyphicon-refresh-animate");
            ClearGlobalMessages();
            AJAXPost("api/storefront/cart/DeleteLineItem", "{'ExternalCartLineId':'" + lineItemId + "'}", deleteLineItemResponse, lineItem);
            return false;
        });

        $(".form-control-quantity").blur(function () {
            var lineItem = $(this);
            var lineItemId = lineItem.attr("data-ajax-lineitemid");
            var previousQuantity = lineItem.attr("value");
            var currentQuantity = lineItem.val();

            if (previousQuantity !== currentQuantity) {
                ClearGlobalMessages();
                AJAXPost("api/storefront/cart/UpdateLineItem", "{'ExternalCartLineId':'" + lineItemId + "', 'Quantity': " + currentQuantity + "}", updateLineItemResponse, lineItem);
            }
            return false;
        });

        $(".delete-promocode").click(function () {
            var adjustment = $(this);
            var adjustmentDescription = adjustment.attr("data-ajax-promocode");
            $(this).find(".glyphicon").removeClass("glyphicon-remove");
            $(this).find(".glyphicon").addClass("glyphicon-refresh");
            $(this).find(".glyphicon").addClass("glyphicon-refresh-animate");
            ClearGlobalMessages();
            AJAXPost("api/storefront/cart/RemoveDiscount", "{'promoCode':'" + adjustmentDescription + "'}", removePromoCodeResponse, $(this));
        });
    });
}

function manageCartDiscountActions() {
    $(".cart-applydiscount").click(function () {
        $(this).button('loading');
        ClearGlobalMessages();
        AJAXPost("api/storefront/cart/ApplyDiscount", "{'promoCode':'" + $('#discountcode_cart').val() + "'}", addPromoCodeResponse, $(this));
    });
}

function addPromoCodeResponse(data, success, sender) {
    $(sender).button('reset');
    if (success && data.Success) {
        lineItemListViewModel.reload(data);
    }

    ShowGlobalMessages(data);
}

function removePromoCodeResponse(data, success, sender) {
    if (success && data.Success) {
        lineItemListViewModel.reload(data);
    }

    $(sender).find(".glyphicon").removeClass("glyphicon-refresh");
    $(sender).find(".glyphicon").removeClass("glyphicon-refresh-animate");
    $(sender).find(".glyphicon").addClass("glyphicon-remove");
    ShowGlobalMessages(data);
}

function updateLineItemResponse(data, success, sender) {
    if (success && data.Success) {
        lineItemListViewModel.reload(data);
    }

    ShowGlobalMessages(data);
}

function deleteLineItemResponse(data, success, sender) {
    if (success && data.Success) {        
        lineItemListViewModel.reload(data);
    }

    $(sender).find(".glyphicon").removeClass("glyphicon-refresh");
    $(sender).find(".glyphicon").removeClass("glyphicon-refresh-animate");
    $(sender).find(".glyphicon").addClass("glyphicon-remove-circle");
    ShowGlobalMessages(data);
}

function initShoppingCart(sectionId) {
    ClearGlobalMessages();
    AJAXPost(StorefrontUri("api/storefront/cart/getcurrentcart"), null, function (data, success, sender) {
        if (success && data.Success) {
            lineItemListViewModel = new LineItemListViewModel(data);
            if (sectionId) {
                ko.applyBindings(lineItemListViewModel, document.getElementById(sectionId));
            }
            else {
                ko.applyBindings(lineItemListViewModel);
            }

            manageCartActions();
            manageCartDiscountActions();
        }

        ShowGlobalMessages(data);
    });
}

function UpdateShoppingCartView() {
    ClearGlobalMessages();
    AJAXPost(StorefrontUri("api/storefront/cart/getcurrentcart"), null, function (data, success, sender) {
        if (success && data.Success) {
            lineItemListViewModel.reload(data);
        }

        ShowGlobalMessages(data);
    });
}
