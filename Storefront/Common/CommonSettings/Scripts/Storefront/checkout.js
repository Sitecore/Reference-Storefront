//-----------------------------------------------------------------------
// Copyright 2015 Sitecore Corporation A/S
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
var checkoutDataViewModel = null;
var defaultCountryCode = "USA";
var methodsViewModel = null;
var method = null;
var expirationDates = ko.observableArray();
var expirationYears = ko.observableArray();
var shippingMethodsArray = [];

function setupCheckoutPage() {
    $("#orderGetShippingMethods").click(function () {
        ClearGlobalMessages();
        if (checkoutDataViewModel && checkoutDataViewModel.shippingAddress() && checkoutDataViewModel.shippingAddress.errors().length === 0) {
            $("#orderGetShippingMethods").button('loading');
            var party = ko.toJS(checkoutDataViewModel.shippingAddress());
            var data = { "ShippingAddress": party, "ShippingPreferenceType": checkoutDataViewModel.selectedShippingOption(), "Lines": null };
            AJAXPost(StorefrontUri("api/sitecore/checkout/GetShippingMethods"), JSON.stringify(data), function (data, success, sender) {
                if (data.Success && success) {
                    var methods = "";
                    checkoutDataViewModel.shippingMethods.removeAll();
                    $.each(data.ShippingMethods, function (i, v) {
                        checkoutDataViewModel.shippingMethods.push(new method(v.Description, v.ExternalId));
                    });
                }

                ShowGlobalMessages(data);
                $("#orderGetShippingMethods").button('reset');
            }, $(this));
        }
        else {
            checkoutDataViewModel.shippingAddress.errors.showAllMessages();
        }
    });

    $('.temp-click').on('click', changeClass);

    $("body").on("click", ".toBilling", function () { switchingCheckoutStep('billing'); });

    $("body").on("click", ".toShipping", function () { switchingCheckoutStep('shipping'); });

    $("body").on('click', '.lineGetShippingMethods', function () {
        ClearGlobalMessages();
        var lineId = $(this).attr('id').replace('lineGetShippingMethods-', '');
        var line = ko.utils.arrayFirst(checkoutDataViewModel.cart().cartLines(), function (l) {
            return l.externalCartLineId === lineId;
        });

        if (line && line.shippingAddress() && line.shippingAddress.errors().length === 0) {
            $("#lineGetShippingMethods-" + lineId).button('loading');
            var party = ko.toJS(line.shippingAddress());
            var lines = [{ "ExternalCartLineId": lineId, "ShippingPreferenceType": line.selectedShippingOption() }];
            var data = { "ShippingAddress": party, "ShippingPreferenceType": checkoutDataViewModel.selectedShippingOption(), "Lines": lines };
            AJAXPost(StorefrontUri("api/sitecore/checkout/GetShippingMethods"), JSON.stringify(data), function (data, success, sender) {
                var lineId = sender.attr('id').replace('lineGetShippingMethods-', '');
                if (data.Success && success && checkoutDataViewModel != null) {
                    var match = ko.utils.arrayFirst(checkoutDataViewModel.cart().cartLines(), function (item) {
                        return item.externalCartLineId === lineId;
                    });

                    match.shippingMethods.removeAll();
                    $.each(data.LineShippingMethods[0].ShippingMethods, function (i, v) {
                        match.shippingMethods.push(new method(v.Description, v.ExternalId));
                    });
                }

                ShowGlobalMessages(data);
                $("#lineGetShippingMethods-" + lineId).button('reset');
            }, $(this));
        } else {
            line.shippingAddress.errors.showAllMessages();
        }
    });

    $('body').on('click', '.lineSearchStores', function () {
        var lineId = $(this).attr('id').replace('SearchStores-', '');
        searchStores($('#StoreSearchResultsContainer-' + lineId), $('#StoreAddressSearch-' + lineId), 'StoresMap-' + lineId);
    });

    $('body').on('keypress', '.lineStoreAddressSearch', function (e) {
        var lineId = $(this).attr('id').replace('StoreAddressSearch-', '');
        var keycode = (e.keyCode ? e.keyCode : (e.which ? e.which : e.charCode));
        if (keycode == 13) {
            $('#SearchStores-' + lineId).trigger('click');
        }
    });

    $('#SearchStores').click(function () {
        searchStores($('#StoreSearchResultsContainer'), $('#StoreAddressSearch'), 'storesMap');
    });

    $('form').submit(function (e) {
        e.preventDefault();
        return false;
    });

    $('#StoreAddressSearch').keypress(function (e) {
        var keycode = (e.keyCode ? e.keyCode : (e.which ? e.which : e.charCode));
        if (keycode == 13) {
            $('#SearchStores').trigger('click');
        }
    });

    $("body").on('click', '#addLoyaltyCard_Confirm', function () {
        ClearGlobalMessages();
        if ($('#LoyalityCardNumber_Confirm').val().length === 0) {
            return;
        }

        $('#loyaltyCardNumber_Confirm_Added').text($('#LoyalityCardNumber_Confirm').val());
        $(this).button("loading");
        AJAXPost(StorefrontUri('api/sitecore/checkout/updateloyaltycard'), "{'loyaltyCardNumber':'" + $('#LoyalityCardNumber_Confirm').val() + "'}", function (data, success, sender) {
            if (success && data.Success && data.WasUpdated) {
                $('#loyaltyCard-success').show();
            }

            $('#addLoyaltyCard_Confirm').button("reset");
            ShowGlobalMessages(data);
        }, this);
    });

    $("#submitOrder").click(function () {
        submitOrder();
    });
}

// ----- JSON CALLS ----- //
function GetAvailableStates(countryCode) {
    var statesArray = [];
    // Uncomment when the States are available
    //
    //AJAXPost(StorefrontUri("api/sitecore/checkout/getAvailableStates"), '{ "CountryCode": "' + countryCode + '"}', function (data, success, sender){     
    //    if (data.States != null) {
    //        $.each(data.UserAddresses, function (index, value) {         
    //            statesArray.push(new Country(value, index));
    //        });
    //    }  
    //});
    return statesArray;
}

function UpdateAvailableStates(countryCode) {
    checkoutDataViewModel.states(GetAvailableStates(countryCode));
}

function getCheckoutData() {
    AJAXPost(StorefrontUri("api/sitecore/checkout/GetCheckoutData"), null, function (data, success, sender) {
        if (success && data.Success) {
            checkoutDataViewModel = new CheckoutDataViewModel(data);
            ko.applyBindingsWithValidation(checkoutDataViewModel, document.getElementById("checkoutSection"));
            $('#orderShippingPreference').removeAttr('disabled');

            if (checkoutDataViewModel.cartLoyaltyCardNumber && checkoutDataViewModel.cartLoyaltyCardNumber.length > 0) {
                $('#loyaltyCard-success').show();
            }
        }

        ShowGlobalMessages(data);
    });
}

function getExpirationDates() {
    for (var i = 0; i < 12; i++) {
        var index = i + 1;
        expirationDates.push({ Name: index, Value: index });
    }
}

function getExpirationYears() {
    for (var i = 0; i < 10; i++) {
        var currentYear = new Date().getFullYear();
        expirationYears.push({ Year: currentYear + i, Value: currentYear + i });
    }
}

function initObservables() {
    method = function (description, id) {
        this.description = description;
        this.id = id;
    }
    MethodsViewModel = function () {
        var self = this;
        self.methods = ko.observableArray();
    }
    methodsViewModel = new MethodsViewModel();
}

function initCheckoutData() {
    getExpirationDates();
    getExpirationYears();
    getCheckoutData();
}

// ----- SHIPPING ----- //
function InitDeliveryPage() {
    $(document).ready(function () {
        $('#btn-delivery-next').show();
        $('#btn-delivery-prev').show();
        $('#orderShippingPreference').attr('disabled', 'disabled');
        $("#ShipAllItemsInput-ExternalId").val(0);

        $("body").on('click', ".nav li.disabled a", function (e) {
            $(this).parent().removeClass("active");
            e.preventDefault();
            return false;
        });

        $("#deliveryMethodSet").val(false);

        $("#checkoutNavigation2").parent().addClass("disabled");
        $("#checkoutNavigation3").parent().addClass("disabled");

        switchingCheckoutStep("shipping");
        initObservables();
    });
};

function setShippingMethods() {
    ClearGlobalMessages();
    var parties = [];
    var shipping = [];
    var orderShippingPreference = checkoutDataViewModel.selectedShippingOption();
    $("#deliveryMethodSet").val(false);

    $("#ToBillingButton").button('loading');
    $("#BackToBillingButton").button('loading');

    if (orderShippingPreference === 1) {
        var partyId = checkoutDataViewModel.shippingAddress().externalId();
        parties.push({
            "Name": checkoutDataViewModel.shippingAddress().name(),
            "Address1": checkoutDataViewModel.shippingAddress().address1(),
            "Country": checkoutDataViewModel.shippingAddress().country(),
            "City": checkoutDataViewModel.shippingAddress().city(),
            "State": checkoutDataViewModel.shippingAddress().state(),
            "ZipPostalCode": checkoutDataViewModel.shippingAddress().zipPostalCode(),
            "ExternalId": partyId,
            "PartyId": partyId
        });

        shipping.push({
            "ShippingMethodID": checkoutDataViewModel.shippingMethod().id,
            "ShippingMethodName": checkoutDataViewModel.shippingMethod().description,
            "ShippingPreferenceType": orderShippingPreference,
            "PartyID": partyId
        });
    }
    else if (orderShippingPreference === 2) {
        var storeId = checkoutDataViewModel.store().externalId();
        parties.push({
            "Name": checkoutDataViewModel.store().name(),
            "Address1": checkoutDataViewModel.store().address().address1(),
            "Country": checkoutDataViewModel.store().address().country(),
            "City": checkoutDataViewModel.store().address().city(),
            "State": checkoutDataViewModel.store().address().state(),
            "ZipPostalCode": checkoutDataViewModel.store().address().zipPostalCode(),
            "ExternalId": storeId,
            "PartyId": storeId
        });

        shipping.push({
            "ShippingMethodID": checkoutDataViewModel.shipToStoreDeliveryMethod().ExternalId,
            "ShippingMethodName": checkoutDataViewModel.shipToStoreDeliveryMethod().Description,
            "ShippingPreferenceType": orderShippingPreference,
            "PartyID": storeId
        });
    }
    else if (orderShippingPreference === 4) {
        $.each(checkoutDataViewModel.cart().cartLines(), function () {
            var lineDeliveryPreference = this.selectedShippingOption();
            var lineId = this.externalCartLineId;

            if (lineDeliveryPreference === 1) {
                var partyId = this.shippingAddress().externalId();
                parties.push({
                    "Name": this.shippingAddress().name(),
                    "Address1": this.shippingAddress().address1(),
                    "Country": this.shippingAddress().country(),
                    "City": this.shippingAddress().city(),
                    "State": this.shippingAddress().state(),
                    "ZipPostalCode": this.shippingAddress().zipPostalCode(),
                    "ExternalId": partyId,
                    "PartyId": partyId
                });

                shipping.push({
                    "ShippingMethodID": this.shippingMethod().id,
                    "ShippingMethodName": this.shippingMethod().description,
                    "ShippingPreferenceType": lineDeliveryPreference,
                    "PartyID": partyId,
                    "LineIDs": [lineId]
                });
            }

            if (lineDeliveryPreference === 2) {
                var storeId = this.store().externalId();
                parties.push({
                    "Name": this.store().name(),
                    "Address1": this.store().address().address1(),
                    "Country": this.store().address().country(),
                    "City": this.store().address().city(),
                    "State": this.store().address().state(),
                    "ZipPostalCode": this.store().address().zipPostalCode(),
                    "ExternalId": storeId,
                    "PartyId": storeId
                });

                shipping.push({
                    "ShippingMethodID": checkoutDataViewModel.shipToStoreDeliveryMethod().ExternalId,
                    "ShippingMethodName": checkoutDataViewModel.shipToStoreDeliveryMethod().Description,
                    "ShippingPreferenceType": lineDeliveryPreference,
                    "PartyID": storeId,
                    "LineIDs": [lineId]
                });
            }

            if (lineDeliveryPreference === 3) {
                shipping.push({
                    "ShippingMethodID": checkoutDataViewModel.emailDeliveryMethod().ExternalId,
                    "ShippingMethodName": checkoutDataViewModel.emailDeliveryMethod().Description,
                    "ShippingPreferenceType": lineDeliveryPreference,
                    "ElectronicDeliveryEmail": this.shippingEmail(),
                    "ElectronicDeliveryEmailContent": this.shippingEmailContent(),
                    "LineIDs": [lineId]
                });
            }
        });
    }
    else if (orderShippingPreference === 3) {
        shipping.push({
            "ShippingMethodID": checkoutDataViewModel.emailDeliveryMethod().ExternalId,
            "ShippingMethodName": checkoutDataViewModel.emailDeliveryMethod().Description,
            "ShippingPreferenceType": orderShippingPreference,
            "ElectronicDeliveryEmail": checkoutDataViewModel.shippingEmail(),
            "ElectronicDeliveryEmailContent": checkoutDataViewModel.shippingEmailContent()
        });
    }

    var data = '{"OrderShippingPreferenceType": "' + orderShippingPreference + '", "ShippingMethods":' + JSON.stringify(shipping) + ', "ShippingAddresses":' + JSON.stringify(parties) + '}';
    AJAXPost(StorefrontUri("api/sitecore/checkout/SetShippingMethods"), data, setShippingMethodsResponse, $(this));
    return false;
}

function setShippingMethodsResponse(data, success, sender) {
    if (success && data.Success) {
        if (checkoutDataViewModel != null) {
            checkoutDataViewModel.cart().setSummary(data);
        }

        updatePaymentAllAmount();
        $("#deliveryMethodSet").val(true);
        $("#billingStep").show();
        $("#reviewStep").hide();
        $("#shippingStep").hide();
        shippingButtons(false);
        billingButtons(true);
        confirmButtons(false);
        $("#checkoutNavigation1").parent().removeClass("active");
        $("#checkoutNavigation2").parent().addClass("active");
        $("#checkoutNavigation3").parent().removeClass("active");
        $("#checkoutNavigation2").parent().removeClass("disabled");
        $("#checkoutNavigation3").parent().removeClass("disabled");
    }

    ShowGlobalMessages(data);
    $("#ToBillingButton").button('reset');
    $("#BackToBillingButton").button('reset');
}

// ----- MAP & STORES ----- //
var map = null;
function getMap(storesMapContainer) {
    if (map) {
        //map.dispose();
    }

    var mapOptions = {
        credentials: "AoDuh-y-4c57psY4ebQAurV-wFTFCfphVB_5TdyjFKv-eBiyU_bnUcMrAPT0BE1k", // TODO this has to be a settings
        zoom: 1,
        disableTouchInput: true
    };

    map = new Microsoft.Maps.Map(document.getElementById(storesMapContainer), mapOptions);
    Microsoft.Maps.loadModule('Microsoft.Maps.Search');
}

function searchStores(searchResultsContainer, addressToSearch, storesMapContainer) {
    searchResultsContainer.children().remove('.toRemove');
    this.getMap(storesMapContainer);
    var searchManager = new Microsoft.Maps.Search.SearchManager(this.map);
    var geocodeRequest = {
        where: addressToSearch.val(),
        count: 1,
        callback: geocodeCallback,
        errorCallback: geocodeError,
        userData: searchResultsContainer
    };
    searchManager.geocode(geocodeRequest);
}

function setMyLocation() {
    var geoLocationProvider = new Microsoft.Maps.GeoLocationProvider(map);

    geoLocationProvider.getCurrentPosition({
        successCallback: displayCenter, showAccuracyCircle: false
    });
}

function geocodeCallback(geocodeResult, userData) {
    // This function is called when a geocode query has successfully executed.
    // Report an error if the geocoding did not return any results.
    // This will be caused by a poorly formed location input by the user.
    if (!geocodeResult.results[0]) {
        alert('Sorry, we were not able to decipher the address you gave us.  Please enter a valid Address.');
        return;
    }

    searchLocation = geocodeResult.results[0].location;

    // Center the map based on the location result returned and a starting (city level) zoom
    // This will trigger the map view change event that will render the store plots
    map.setView({ zoom: 11, center: this.searchLocation });

    //Add a handler for the map change event. This event is used to render the store location plots each time the user zooms or scrolls to a new viewport
    Microsoft.Maps.Events.addHandler(this.mapStoreLocator, 'viewchanged', renderAvailableStores.bind(this));

    // Call the CRT to obtain a list of stores with a radius of the location provided.
    // Note that we request stores for the maximum radius we want to support (200).  The map control
    // is used to determine the "within" scope based on the users zoom settings at runtime.
    getNearbyStores(userData);
}

function geocodeError(request) {
    // This function handles an error from the geocoding service
    // These errors are thrown due to connectivity or system faults, not poorly formed location inputs. 
    alert("Sorry, something went wrong. An error has occured while looking up the address you provided. Please refresh the page and try again.");
}

function getNearbyStores(searchResultsContainer) {
    var data = "{'latitude': '" + searchLocation.latitude + "', 'longitude':" + searchLocation.longitude + "}";

    AJAXPost(StorefrontUri("api/sitecore/checkout/GetNearbyStores"), data, renderAvailableStores, searchResultsContainer);
    return false;
}

function renderAvailableStores(data, success, sender) {
    map.entities.clear();
    sender.hide();

    var lineId = sender.selector.replace('#StoreSearchResultsContainer-', '');

    if (lineId === "#StoreSearchResultsContainer") {
        lineId = "";
    } else {
        lineId = "-" + lineId;
    }

    if (!success) {
        return;
    }

    var storeCount = 0;
    var pin;
    var pinInfoBox;
    var mapBounds = map.getBounds();
    var stores = data.Stores;

    // Display search location
    if (searchLocation != null && searchLocation != undefined && mapBounds.contains(searchLocation)) {
        // Plot the location to the map
        pin = new Microsoft.Maps.Pushpin(searchLocation, { draggable: false, text: "X" });
        map.entities.push(pin);
    }

    // If we have stores, plot them on the map
    if (stores.length > 0) {
        for (var i = 0; i < stores.length; i++) {
            var currentStoreLocation = stores[i];
            currentStoreLocation.location = { latitude: currentStoreLocation.Latitude, longitude: currentStoreLocation.Longitude };

            // Test each location to see if it is within the bounding rectangle
            if (mapBounds.contains(currentStoreLocation.location)) {
                sender.show();

                //  Increment the counter used to manage the sequential entity index
                storeCount++;
                currentStoreLocation.LocationCount = storeCount;

                // This is the html that appears when a push pin is clicked on the map
                var storeAddressText = '<div style="width:80%;height:100%;">\
                    <p style="background-color:gray;color:black;margin-bottom:5px;">\
                        <span style="padding-right:45px;">Store</span>\
                            <span style="font-weight:bold;">Distance</span>\
                                <p><p style="margin-bottom:0px;margin-top:0px;">\
                                    <span style="color:black;padding-right:35px;">'
                                        + currentStoreLocation.Name +
                '</span><span style="color:black;">'
                + currentStoreLocation.Distance +
                ' miles</span>\
                </p><p style="margin-bottom:0px;margin-top:0px;">'
                + currentStoreLocation.Address.Address1 +
                ' </p><p style="margin-bottom:0px;margin-top:0px;">'
                + currentStoreLocation.Address.City + ', '
                + currentStoreLocation.Address.State + ' '
                + currentStoreLocation.Address.ZipPostalCode +
                '</p></div>';

                // Plot the location to the map	
                pin = new Microsoft.Maps.Pushpin(currentStoreLocation.location, { draggable: false, text: "" + storeCount + "" });

                // Populating the Bing map push pin popup with store location data
                pinInfoBox = new Microsoft.Maps.Infobox(currentStoreLocation.location, { width: 225, offset: new Microsoft.Maps.Point(0, 10), showPointer: true, visible: false, description: storeAddressText });

                // Registering the event that fires when a pushpin on a Bing map is clicked
                Microsoft.Maps.Events.addHandler(pin, 'click', (function (pinInfoBox) {
                    return function () {
                        pinInfoBox.setOptions({ visible: true });
                    }
                })(pinInfoBox));

                map.entities.push(pin);
                map.entities.push(pinInfoBox);

                currentStoreLocation.Address.Name = currentStoreLocation.Name;
                lineId = lineId.replace('-', '');
                if (lineId === '') {
                    checkoutDataViewModel.stores.push(new StoreViewModel(currentStoreLocation));
                    if (storeCount === 1) {
                        checkoutDataViewModel.store(new StoreViewModel(currentStoreLocation));
                    }
                } else {
                    var selectedLine = ko.utils.arrayFirst(checkoutDataViewModel.cart().cartLines(), function (line) {
                        return line.externalCartLineId === lineId;
                    });
                    if (selectedLine != null) {
                        selectedLine.stores.push(new StoreViewModel(currentStoreLocation));
                        if (storeCount === 1) {
                            selectedLine.store(new StoreViewModel(currentStoreLocation));
                        }
                    }

                    shippingMethodsArray.push(lineId);
                }
            }
        }
    }
}

// ----- BILLING ----- //
function initBillingPage() {
    $(document).ready(function () {
        $('.accordion-toggle').on('click', function (event) {
            event.preventDefault();

            // create accordion variables
            var accordion = $(this);
            var accordionContent = accordion.closest('.accordion-container').find('.accordion-content');
            var accordionToggleIcon = $(this).children('.toggle-icon');

            // toggle accordion link open class
            accordion.toggleClass("open");

            // toggle accordion content
            accordionContent.slideToggle(250);

            // change plus/minus icon
            if (accordion.hasClass("open")) {
                accordionToggleIcon.html("<span class='glyphicon glyphicon-minus-sign'></span>");
            } else {
                accordionToggleIcon.html("<span class='glyphicon glyphicon-plus-sign'></span>");
            }
        });
    });
}

function updatePaymentAllAmount() {
    var ccIsAdded = checkoutDataViewModel.creditCardPayment().isAdded();
    var gcIsAdded = checkoutDataViewModel.giftCardPayment().isAdded();
    var lcIsAdded = checkoutDataViewModel.loyaltyCardPayment().isAdded();
    if (!ccIsAdded && !gcIsAdded && !lcIsAdded) {
        return;
    }

    var total = parseFloat(checkoutDataViewModel.cart().totalAmount());
    var lcAmount = parseFloat(checkoutDataViewModel.loyaltyCardPayment().loyaltyCardAmount());
    var gcAmount = parseFloat(checkoutDataViewModel.giftCardPayment().giftCardAmount());
    var ccAmount = parseFloat(checkoutDataViewModel.creditCardPayment().creditCardAmount());
    var aTotal = parseFloat(lcAmount + gcAmount + ccAmount);

    if (aTotal === total) {
        return;
    }

    var count = 0
    if (lcIsAdded) {
        ++count;
    }
    if (gcIsAdded) {
        ++count;
    }
    if (ccIsAdded) {
        ++ccount;
    }

    if (aTotal > total) {
        var diff = (aTotal - total) / count;
        lcAmount = lcIsAdded ? lcAmount - diff : 0;
        gcAmount = gcIsAdded ? gcAmount - diff : 0;
        ccAmount = ccIsAdded ? ccAmount - diff : 0;
    } else if (aTotal < total) {
        var diff = (total - aTotal) / count;
        lcAmount = lcIsAdded ? lcAmount + diff : 0;
        gcAmount = gcIsAdded ? gcAmount + diff : 0;
        ccAmount = ccIsAdded ? ccAmount + diff : 0;
    }

    checkoutDataViewModel.loyaltyCardPayment().loyaltyCardAmount((lcAmount).toFixed(2));
    checkoutDataViewModel.giftCardPayment().giftCardAmount((gcAmount).toFixed(2));
    checkoutDataViewModel.creditCardPayment().creditCardAmount((ccAmount).toFixed(2));
}

function setPaymentMethods() {
    var data = "{";

    if (checkoutDataViewModel.creditCardPayment().isAdded()) {
        var cc = checkoutDataViewModel.creditCardPayment();
        var creditCard = {
            "CreditCardNumber": cc.creditCardNumber(),
            "PaymentMethodID": cc.paymentMethodID(),
            "ValidationCode": cc.validationCode(),
            "ExpirationMonth": cc.expirationMonth(),
            "ExpirationYear": cc.expirationYear(),
            "CustomerNameOnPayment": cc.customerNameOnPayment(),
            "Amount": cc.creditCardAmount(),
            "PartyID": $('#billingAddress-ExternalId').val()
        };

        var ba = checkoutDataViewModel.billingAddress();
        var billingAddress =
        {
            "Name": ba.name(),
            "Address1": ba.address1(),
            "Country": ba.country(),
            "City": ba.city(),
            "State": ba.state(),
            "ZipPostalCode": ba.zipPostalCode(),
            "ExternalId": ba.externalId(),
            "PartyId": ba.externalId()
        };

        if (data.length > 1) {
            data += ",";
        }

        data += '"CreditCardPayment":' + JSON.stringify(creditCard) + ',"BillingAddress":' + JSON.stringify(billingAddress);
    }

    if (checkoutDataViewModel.giftCardPayment().isAdded()) {
        var giftCard = {
            "PaymentMethodID": checkoutDataViewModel.giftCardPayment().giftCardNumber(),
            "Amount": checkoutDataViewModel.giftCardPayment().giftCardAmount()
        };

        if (data.length > 1) {
            data += ",";
        }

        data += '"GiftCardPayment":' + JSON.stringify(giftCard);
    }

    if ($('#addedLoyaltyCard').val() === 'true') {
        var loyaltyCard = {
            "PaymentMethodID": checkoutDataViewModel.loyaltyCardPayment().loyaltyCardNumber(),
            "Amount": checkoutDataViewModel.loyaltyCardPayment().loyaltyCardAmount()
        };

        if (data.length > 1) {
            data += ",";
        }

        data += '"LoyaltyCardPayment":' + JSON.stringify(loyaltyCard);
    }

    data += "}";

    $("#ToConfirmButton").button('loading');

    AJAXPost(StorefrontUri("api/sitecore/checkout/SetPaymentMethods"), data, setPaymentMethodsResponse, $(this));
}

function setPaymentMethodsResponse(data, success, sender) {
    if (data.Success && success) {
        if (checkoutDataViewModel != null) {
            checkoutDataViewModel.cart().setSummary(data);
        }

        switchingCheckoutStep('confirm');
    }

    ShowGlobalMessages(data);
    $("#ToConfirmButton").button('reset');
    $("#PlaceOrderButton").button('reset');
}

// ----- CONFIRM & SUBMIT ----- //
function submitOrder() {
    ClearGlobalMessages();

    var data = "{";
    data += '"userEmail": "' + checkoutDataViewModel.billingEmail() + '"';

    if (checkoutDataViewModel.creditCardPayment().isAdded()) {
        var cc = checkoutDataViewModel.creditCardPayment();
        var creditCard = {
            "CreditCardNumber": cc.creditCardNumber(),
            "PaymentMethodID": cc.paymentMethodID(),
            "ValidationCode": cc.validationCode(),
            "ExpirationMonth": cc.expirationMonth(),
            "ExpirationYear": cc.expirationYear(),
            "CustomerNameOnPayment": cc.customerNameOnPayment(),
            "Amount": cc.creditCardAmount(),
            "PartyID": $('#billingAddress-ExternalId').val()
        };

        var ba = checkoutDataViewModel.billingAddress();
        var billingAddress =
        {
            "Name": ba.name(),
            "Address1": ba.address1(),
            "Country": ba.country(),
            "City": ba.city(),
            "State": ba.state(),
            "ZipPostalCode": ba.zipPostalCode(),
            "ExternalId": ba.externalId(),
            "PartyId": ba.externalId()
        };

        data += ',"CreditCardPayment":' + JSON.stringify(creditCard) + ',"BillingAddress":' + JSON.stringify(billingAddress);
    }

    if (checkoutDataViewModel.giftCardPayment().isAdded()) {
        var giftCard = {
            "PaymentMethodID": checkoutDataViewModel.giftCardPayment().giftCardNumber(),
            "Amount": checkoutDataViewModel.giftCardPayment().giftCardAmount()
        };

        data += ',"GiftCardPayment":' + JSON.stringify(giftCard);
    }

    if (checkoutDataViewModel.loyaltyCardPayment().isAdded()) {
        var loyaltyCard = {
            "PaymentMethodID": checkoutDataViewModel.loyaltyCardPayment().loyaltyCardNumber(),
            "Amount": checkoutDataViewModel.loyaltyCardPayment().loyaltyCardAmount()
        };

        data += ',"LoyaltyCardPayment":' + JSON.stringify(loyaltyCard);
    }

    data += "}";

    $("#PlaceOrderButton").button('loading');

    AJAXPost(StorefrontUri("api/sitecore/checkout/SubmitOrder"), data, submitOrderResponse, $(this));
}

function submitOrderResponse(data, success, sender) {
    if (data.Success && success) {
        window.location.href = data.ConfirmUrl;
    }

    ShowGlobalMessages(data);
    $("#PlaceOrderButton").button('reset');
}

// ----- CHECKOUT GENERAL ----- //
function switchingCheckoutStep(step) {
    ClearGlobalMessages();

    if (step === "billing") {
        if ($("#deliveryMethodSet").val() === 'false') {
            setShippingMethods();
        } else {
            $("#billingStep").show();
            $("#reviewStep").hide();
            $("#shippingStep").hide();
            shippingButtons(false);
            billingButtons(true);
            confirmButtons(false);
            $("#checkoutNavigation1").parent().removeClass("active");
            $("#checkoutNavigation2").parent().addClass("active");
            $("#checkoutNavigation3").parent().removeClass("active");
            return;
        }
    }

    if (step === "shipping") {
        $("#deliveryMethodSet").val(false);
        $("#billingStep").hide();
        $("#reviewStep").hide();
        $("#shippingStep").show();
        shippingButtons(true);
        billingButtons(false);
        confirmButtons(false);
        $("#checkoutNavigation1").parent().addClass("active");
        $("#checkoutNavigation2").parent().removeClass("active");
        $("#checkoutNavigation3").parent().removeClass("active");
        $("#checkoutNavigation2").parent().addClass("disabled");
        $("#checkoutNavigation3").parent().addClass("disabled");
    }

    if (step === "confirm") {
        if ($("#deliveryMethodSet").val() === 'true') {
            $("#billingStep").hide();
            $("#reviewStep").show();
            $("#shippingStep").hide();
            shippingButtons(false);
            billingButtons(false);
            confirmButtons(true);
            $("#checkoutNavigation1").parent().removeClass("active");
            $("#checkoutNavigation2").parent().removeClass("active");
            $("#checkoutNavigation3").parent().addClass("active");
        } else {
            $("#checkoutNavigation2").parent().addClass("disabled");
            $("#checkoutNavigation3").parent().addClass("disabled");
            return;
        }
    }

    if (step === "placeOrder") {
        $("#billingStep").hide();
        $("#reviewStep").hide();
        $("#shippingStep").hide();
        shippingButtons(false);
        billingButtons(false);
        confirmButtons(false);
        $("#checkoutNavigation1").parent().removeClass("active");
        $("#checkoutNavigation2").parent().removeClass("active");
        $("#checkoutNavigation3").parent().removeClass("active");
    }
}

function shippingButtons(show) {
    if (!show) {
        $('#btn-delivery-next').hide();
        $('#btn-delivery-prev').hide();
    }
    else {
        $('#btn-delivery-next').show();
        $('#btn-delivery-prev').show();
    }
}

function billingButtons(show) {
    if (!show) {
        $('#btn-billing-next').hide();
        $('#btn-billing-prev').hide();
    }
    else {
        $('#btn-billing-next').show();
        $('#btn-billing-prev').show();
    }
}

function confirmButtons(show) {
    if (!show) {
        $('#btn-confirm-next').hide();
        $('#btn-confirm-prev').hide();
    }
    else {
        $('#btn-confirm-next').show();
        $('#btn-confirm-prev').show();
    }
}

function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}

var toString = Object.prototype.toString;
isString = function (obj) {
    return toString.call(obj) == '[object String]';
}