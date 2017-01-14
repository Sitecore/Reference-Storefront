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

function selectAllOrderItems(element) {
    var isChecked = $(element).is(':checked');
    $('.item-to-selected').prop('checked', isChecked);
    enableAddOrderItemsToCart();
    enableCancelOrderItems();
}

function enableSelectAllOrderItems() {
    if ($('.item-to-selected').filter(':checked').length === $('.item-to-selected').length) {
        $('#selectAllOrderItems').prop('checked', true);
    } else {
        $('#selectAllOrderItems').removeAttr('checked');
    }
}

function enableAddOrderItemsToCart() {
    $('#addOrderItemsToCart').prop("disabled", $('.item-to-selected').filter(':checked').length == 0);
    enableSelectAllOrderItems();
}

function addOrderItemsToCart() {
    if ($('.item-to-selected').filter(':checked').length === 0) {
        enableAddOrderItemsToCart();
        return;
    }

    $("#addOrderItemsToCart").prop("disabled", true);
    $("#addOrderItemsToCart").children('span[id="buttonText"]').html($("#addOrderItemsToCart").attr("data-loading-text"));

    var reorderLines = [];
    if (!$("#selectAllOrderItems").is(':checked')) {
        $('.item-to-selected').filter(':checked').each(function() {
            reorderLines.push($(this).attr('data-externalId'));
        });
    }

    var data = {
        "OrderId": $("#orderTable").attr("data-orderId"),
        "ReorderLineExternalIds": reorderLines
    };

    ClearGlobalMessages();
    AJAXPost(StorefrontUri('api/storefront/account/reorder'), JSON.stringify(data), function (data, success, sender) {
        if (success && data.Success) {
            $('.item-to-selected').removeAttr('checked');
            $('#selectAllOrderItems').removeAttr('checked');
            $("#addToCartSuccess").show().fadeOut(4000);
            UpdateMiniCart();
        }

        enableAddOrderItemsToCart();
        enableCancelOrderItems();
        $("#addOrderItemsToCart").children('span[id="buttonText"]').html($("#addOrderItemsToCart").attr("data-text"));
        ShowGlobalMessages(data);
    }, this);
}

function enableCancelOrderItems() {
    $('#cancelOrderItems').prop("disabled", ($("#confirm-orderStatus").attr("data-value") == "Cancelled") || !($("#selectAllOrderItems").is(':checked')));
    enableSelectAllOrderItems();
}

function cancelOrderItems() {
    if (!($("#selectAllOrderItems").is(':checked'))) {
        enableCancelOrderItems();
        return;
    }

    $("#cancelOrderItems").prop("disabled", true);
    $("#cancelOrderItems").children('span[id="buttonText"]').html($("#cancelOrderItems").attr("data-loading-text"));

    var cancelLines = [];
    if (!$("#selectAllOrderItems").is(':checked')) {
        $('.item-to-selected').filter(':checked').each(function () {
            cancelLines.push($(this).attr('data-externalId'));
        });
    }

    var data = {
        "OrderId": $("#orderTable").attr("data-orderId"),
        "OrderLineExternalIds": cancelLines
    };

    ClearGlobalMessages();
    AJAXPost(StorefrontUri('api/storefront/account/cancelorder'), JSON.stringify(data), function (data, success, sender) {
        if (success && data.Success) {
            $('.item-to-selected').removeAttr('checked');
            $('#selectAllOrderItems').removeAttr('checked');
            $("#addToCartSuccess").show().fadeOut(4000);
            UpdateMiniCart();
            location.reload(true);
        }

        enableAddOrderItemsToCart();
        enableCancelOrderItems();
        $("#cancelOrderItems").children('span[id="buttonText"]').html($("#cancelOrderItems").attr("data-text"));
        ShowGlobalMessages(data);
    }, this);
}
