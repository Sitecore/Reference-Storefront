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

// Localized message dictionary
// -
var messageDictionary = new Array();

function AddMessage(key, value) {
    messageDictionary[key] = value;
}

function GetMessage(key) {
    return messageDictionary[key];
}

// AJAX Extensions
// -
AddAntiForgeryToken = function (data) {
    if (data == null) {
        data = {};
    }

    data.__RequestVerificationToken = $('#_CRSFform input[name=__RequestVerificationToken]').val();
    return data;
};

function AJAXGet(url, data, responseFunction, sender) {
    AJAXCall("GET", url, data, responseFunction, sender);
}

function AJAXPost(url, data, responseFunction, sender) {
    AJAXCall("POST", url, data, responseFunction, sender);
}

function AJAXCall(callType, url, data, responseFunction, sender) {
    var token = $('#_CRSFform input[name=__RequestVerificationToken]').val();
    $.ajax({
        type: callType,
        url: url,
        cache: false,
        headers: { "__RequestVerificationToken": token },
        contentType: "application/json; charset=utf-8",
        data: data,
        success: function (data) {
            dontBlockUI = false;
            if (responseFunction != null) {
                responseFunction(data, true, sender);
            }
        },
        error: function (data) {
            if (responseFunction != null) {
                responseFunction(data, false, sender);
            }
        }
    });
}

// General helper methods
// -
function StorefrontUri(route) {
    //var currentLocation = window.location;
    //var localpathArray = currentLocation.pathname.split("/");

    //var storefrontRouteKey = localpathArray[2];
    ////Check if our storefronts key is the route key
    ////This means that a localization key is in the URL
    //if (storefrontRouteKey.toLowerCase() == "storefronts") {
    //    //If it has an additional key in the route then slide one over
    //    storefrontRouteKey = localpathArray[3];
    //    return "/" + localpathArray[2] + "/" + storefrontRouteKey + "/" + route;
    //}

    //return "/" + localpathArray[1] + "/" + storefrontRouteKey + "/" + route;
    return "/" + route;
}

function ShowGlobalMessages(data) {
    if (data && data.Url) {
        var url = new Uri(StorefrontUri(data.Url));
        window.location.href = url;
    }
    if (errorSummaryViewModel && data && data.Errors && data.Errors.length > 0) {
        errorSummaryViewModel.AddToErrorList(data);
    }    
}

function ClearGlobalMessages() {
    if (errorSummaryViewModel) {
        errorSummaryViewModel.ClearMessages();
    }
}

var toString = Object.prototype.toString;
isString = function (obj) {
    return toString.call(obj) == '[object String]';
}

//
// - 
function productRecommendationClick(e) {
    e.preventDefault();
    var clickedElement = $(this);

    clickedElement.closest("ul").find(".active").removeClass("active");
    clickedElement.closest("li").addClass('active');

    var selector = clickedElement.attr('data-carousel-id');
    var carousel = $('#' + selector);
    var parent = carousel.parent();
    parent.find(".product-slider").each(function () {
        $(this).attr("style", "display:none");
    });
    carousel.attr("style", "");
    $(".product-controls").closest("div").find("a").each(function () {
        $(this).attr("href", '#' + selector);
    });
};

function resetUrl() {
    var url = new Uri(window.location.href)
        .deleteQueryParam(queryStringParamerterSort)
        .deleteQueryParam(queryStringParamerterSortDirection)
        .deleteQueryParam(queryStringParamerterPage)
        .deleteQueryParam(queryStringParamerterPageSize)
        .deleteQueryParam(queryStringParameterSiteContentPage)
        .deleteQueryParam(queryStringParameterSiteContentPageSize)
        .toString();

    window.location.href = url;
}

var queryStringParamerterSort = "s";
var queryStringParamerterSortDirection = "sd";
var queryStringParamerterSortDirectionAsc = "asc";
var queryStringParamerterSortDirectionAscShort = "+";
var queryStringParamerterSortDirectionDesc = "desc";
var queryStringParamerterPage = "pg";
var queryStringParamerterPageSize = "ps";
var queryStringParameterSiteContentPage = "scpg";
var queryStringParameterSiteContentPageSize = "scps";

$(window).on("load", function () {
    setEqualHeight($(".product-list div.col-sm-4"));
});

$(document).ready(function () {
    $('.product-recommendation-click').on('click', productRecommendationClick);

    $(".sortDropdown").change(function () {
        var val = $(this).find("option:selected").attr("value");

        if (val != null && val != "") {
            var fieldName = val.substr(0, val.length - 1);
            var direction = val.charAt(val.length - 1) == queryStringParamerterSortDirectionAscShort ? queryStringParamerterSortDirectionAsc : queryStringParamerterSortDirectionDesc;

            AJAXPost(StorefrontUri("api/storefront/catalog/sortorderapplied"), "{\"sortField\":\"" + fieldName + "\", \"sortDirection\":\"" + direction + "\"}", function (data, success, sender) {
                var url = new Uri(window.location.href)
                    .deleteQueryParam(queryStringParamerterSort)
                    .deleteQueryParam(queryStringParamerterSortDirection)
                    .addQueryParam(queryStringParamerterSort, fieldName)
                    .addQueryParam(queryStringParamerterSortDirection, direction)
                    .deleteQueryParam(queryStringParamerterPage)
                    .toString();

                window.location.href = url;
            });
        }
        else {
            resetUrl();
        }
    });

    $(".changePageSize").change(function () {
        var val = $(this).find("option:selected").attr("value");

        if (val != null && val != "") {
            var url = new Uri(window.location.href)
                .deleteQueryParam(queryStringParamerterPageSize)
                .addQueryParam(queryStringParamerterPageSize, val)
                .deleteQueryParam(queryStringParamerterPage)
                .toString();

            window.location.href = url;
        }
        else {
            resetUrl();
        }
    });

    $(".changeSiteContentPageSize").change(function () {
        var val = $(this).find("option:selected").attr("value");

        if (val != null && val != "") {
            var url = new Uri(window.location.href)
                .deleteQueryParam(queryStringParameterSiteContentPageSize)
                .addQueryParam(queryStringParameterSiteContentPageSize, val)
                .deleteQueryParam(queryStringParamerterPage)
                .toString();

            window.location.href = url;
        }
        else {
            resetUrl();
        }
    });

    $(".thumbnails li a").on('click', function (e) {
        e.preventDefault();
        var activeThumb = $(this);

        activeThumb.closest("ul").find(".selected-thumb").removeClass("selected-thumb");
        activeThumb.closest("li").toggleClass("selected-thumb");

        $('#prod-large-view').attr('src', $(this).attr('href'));
    });
});

function changeClass(e) {
    e.preventDefault();
    var clickedElement = $(this);

    clickedElement.closest("ul").find(".active").removeClass("active");
    clickedElement.closest("li").addClass('active');
};

function states(sender, event) {
    var $btn = $('#' + sender).button(event);
}

// ERROR DIV
function closeErrorMessage() {
    $('.wrap-error').slideUp();
};

function showErrorMessage() {
    $('.wrap-error').slideDown();
};

function getUrlParameter(url, param) {
    url = url.split('?');
    if (url.length === 1) {
        return null;
    }

    var pattern = new RegExp(param + '=(.*?)(;|&|$)', 'gi');
    return url[1].split(pattern)[1];
}

var mustEqual = function (val, other) {
    return val === other;
};

function printPage() {
    var url = window.location.href;
    var hasParams = url.split('?').length > 1;
    var location = hasParams ? url + "&p=1" : url + "?p=1";
    var w = window.open(location);

    w.onload = function () {
        $(document).ajaxStart().ajaxStop(w.print());
    };
}

function setEqualHeight(columns) {
    var tallestcolumn = 0;
    columns.each(function () {
        currentHeight = $(this).height();
        if (currentHeight > tallestcolumn) {
            tallestcolumn = currentHeight;
        }
    });
    columns.height(tallestcolumn);
}

function formatCurrency(x, precision, seperator, isoCurrencySymbol, groupSeperator) {
    var options = {
        precision: precision || 2,
        seperator: seperator || ',',
        groupSeperator: groupSeperator || " "
    }

    var currencyValue = (x.__ko_proto__ === ko.dependentObservable || x.__ko_proto__ === ko.observable) ? x() : x;

    var formatted = parseFloat(currencyValue, 10).toFixed(options.precision);

    var regex = new RegExp('^(\\d+)[^\\d](\\d{' + options.precision + '})$');
    formatted = formatted.replace(regex, '$1' + options.seperator + '$2');
    formatted = formatted.replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1" + options.groupSeperator)

    if (isoCurrencySymbol && isoCurrencySymbol.length > 0) {
        return formatted + " " + isoCurrencySymbol;
    }
    else {
        return formatted;
    }
}


