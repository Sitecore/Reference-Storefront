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

function CheckoutDataViewModel(data) {
    var self = this;

    self.cart = ko.observable(new LineItemListViewModel(data.Cart));

    // delivery //
    self.isShipAll = ko.observable(false);
    self.isShipToStore = ko.observable(false);
    self.isShipToEmail = ko.observable(false);
    self.isShipItems = ko.observable(false);
    self.emailDeliveryMethod = ko.observable(data.EmailDeliveryMethod);
    self.shipToStoreDeliveryMethod = ko.observable(data.ShipToStoreDeliveryMethod);

    self.orderShippingOptions = ko.observableArray();
    if (data.OrderShippingOptions != null) {
        $.each(data.OrderShippingOptions, function (index, value) {
            self.orderShippingOptions.push(value);
        });
    }
    self.selectedShippingOption = ko.observable('0');
    self.selectedShippingOption.subscribe(function (option) {
        self.isShipAll(option === 1);
        self.isShipToStore(option === 2);
        self.isShipToEmail(option === 3);
        self.isShipItems(option === 4);
        if (option === 2) {
            getMap('storesMap');
            setMyLocation();
        }
    }.bind(this));

    self.isAuthenticated = false;
    self.userEmail = "";
    self.userAddresses = ko.observableArray();
    self.userAddresses.push(new AddressViewModel({ "ExternalId": "UseShipping", "FullAddress": $("#billingAddressSelect").attr("title") }));
    self.userAddresses.push(new AddressViewModel({ "ExternalId": "UseOther", "FullAddress": $("#ShippingAddressSelect").attr("title2") }));
    if (data.IsUserAuthenticated === true && data.UserAddresses.Addresses != null) {
        $.each(data.UserAddresses.Addresses, function () {
            self.userAddresses.push(new AddressViewModel(this));
        });

        self.isAuthenticated = true;
        self.userEmail = data.UserEmail;
    }

    self.shippingMethods = ko.observableArray();
    self.shippingMethod = ko.validatedObservable().extend({ required: true });
    self.shippingAddress = ko.validatedObservable(new AddressViewModel({ "ExternalId": "0" }));
    self.shippingAddressFieldChanged = function () {
        self.shippingMethod("");
        self.shippingMethods.removeAll();
    };
    self.selectedShippingAddress = ko.observable("UseOther");
    self.selectedShippingAddress.subscribe(function (id) {
        var match = self.getAddress(id);
        self.shippingMethod("");
        self.shippingMethods.removeAll();
        if (match != null) {
            self.shippingAddress(match);
        } else {
            self.shippingAddress(new AddressViewModel({ "ExternalId": "0" }));
        }
    }.bind(this));

    self.stores = ko.observableArray();
    self.store = ko.validatedObservable(new StoreViewModel());
    self.changeSelectedStore = function (item, event) {
        self.store(item);
    };

    self.shippingEmail = ko.validatedObservable().extend({ required: true, email: true });
    self.shippingEmailContent = ko.observable("");
    self.setSendToMe = function (item, event) {
        var email = $(event.currentTarget).is(':checked') ? self.userEmail : "";
        item.shippingEmail(email);
    };

    self.enableToBilling = ko.computed({
        read: function () {
            if (self.isShipToEmail()) {
                return self.shippingEmail() && self.shippingEmail.isValid();
            }

            if (self.isShipAll()) {
                return self.shippingMethod.isValid() && self.shippingAddress.isValid()
            }

            if (self.isShipToStore()) {
                return self.store.isValid() && self.store().address.isValid();
            }

            if (self.isShipItems()) {
                var isValid = [];
                $.each(self.cart().cartLines(), function () {
                    if (this.isLineShipToEmail()) {
                        isValid.push(this.shippingEmail() && this.shippingEmail.isValid());
                    }
                    else if (this.isLineShipAll()) {
                        isValid.push(this.shippingMethod.isValid() && this.shippingAddress.isValid());
                    }
                    else if (this.isLineShipToStore()) {
                        isValid.push(this.store.isValid() && this.store().address.isValid());
                    } else {
                        isValid.push(false);
                    }
                });

                return isValid.every(isItemValid);
            }
        },
        write: function (value) {
            return Boolean(value);
        }
    });

    // billing //
    self.billingEmail = ko.validatedObservable(self.userEmail).extend({ required: true, email: true });
    self.billingConfirmEmail = ko.validatedObservable(self.userEmail).extend({ validation: { validator: mustEqual, message: GetMessage('EmailsMustMatchMessage'), params: self.billingEmail } });
    self.payCard = false;
    self.payGiftCard = false;
    self.payLoyaltyCard = false;
    self.payGiftLoyaltyCard = self.payGiftCard || self.payLoyaltyCard ? true : false;
    if (data.PaymentOptions != null) {
        $.each(data.PaymentOptions, function (index, value) {
            if (value.PaymentOptionType.Name === "PayCard") {
                self.payCard = true;
            }
            if (value.PaymentOptionType.Name === "PayGiftCard") {
                self.payGiftCard = true;
            }
            if (value.PaymentOptionType.Name === "PayLoyaltyCard") {
                self.payLoyaltyCard = true;
            }
        });
    }

    var PaymentMethod = function (externalId, description) {
        this.ExternalId = externalId;
        this.Description = description;
    };

    self.paymentMethods = ko.observableArray();
    if (data.PaymentMethods != null) {
        self.paymentMethods.push(new PaymentMethod("0", $("#PaymentMethods").attr("title")));
        $.each(data.PaymentMethods, function (index, value) {
            self.paymentMethods.push(new PaymentMethod(value.ExternalId, value.Description));
        });
    }

    self.cartLoyaltyCardNumber = data.CartLoyaltyCardNumber;
    self.giftCardPayment = ko.validatedObservable(new GiftCardPaymentViewModel());
    self.loyaltyCardPayment = ko.validatedObservable(data.CartLoyaltyCardNumber ? new LoyaltyCardPaymentViewModel({ "CartLoyaltyCardNumber": data.CartLoyaltyCardNumber, "Amount": 0.00 }) : new LoyaltyCardPaymentViewModel());
    self.creditCardPayment = ko.validatedObservable(new CreditCardPaymentViewModel());
    self.creditCardEnable = ko.observable(false);
    self.billingAddress = ko.validatedObservable(new AddressViewModel({ "ExternalId": "1" }));
    self.billingAddressEnable = ko.observable(false);
    self.selectedBillingAddress = ko.observable("UseOther");
    self.selectedBillingAddress.subscribe(function (id) {
        if (id === "UseShipping") {
            self.billingAddressEnable(false);
            self.billingAddress(self.shippingAddress());
        } else {
            var match = self.getAddress(id);
            if (match != null) {
                self.billingAddressEnable(false);
                self.billingAddress(match);
            } else {
                self.billingAddressEnable(true);
                self.billingAddress(new AddressViewModel({ "ExternalId": "1" }));
            }
        }

        $("#billingAddressSelect").prop("disabled", false);
    });
    self.paymentTotal = ko.computed({
        read: function () {
            var ccIsAdded = self.creditCardPayment().isAdded();
            var gcIsAdded = self.giftCardPayment().isAdded();
            var lcIsAdded = self.loyaltyCardPayment().isAdded();
            if (!ccIsAdded && !gcIsAdded && !lcIsAdded) {
                return 0;
            }

            var ccAmount = ccIsAdded ? self.creditCardPayment().creditCardAmount() : 0;
            var gcAmount = gcIsAdded ? self.giftCardPayment().giftCardAmount() : 0;
            var lcAmount = lcIsAdded ? self.loyaltyCardPayment().loyaltyCardAmount() : 0;
            return (parseFloat(ccAmount) + parseFloat(gcAmount) + parseFloat(lcAmount)).toFixed(2);
        },
        write: function () { }
    });
    self.enableToConfirm = ko.computed({
        read: function () {
            var paymentTotalIsValid = parseFloat(self.paymentTotal()) === parseFloat(self.cart().totalAmount());
            if (!paymentTotalIsValid) {
                return false;
            }

            var paymentsAreValid = false;
            if (self.giftCardPayment().isAdded()) {
                paymentsAreValid = self.giftCardPayment.isValid();
            }

            if (self.loyaltyCardPayment().isAdded()) {
                paymentsAreValid = self.loyaltyCardPayment.isValid();
            }

            if (self.creditCardPayment().isAdded()) {
                paymentsAreValid = self.creditCardPayment.isValid() && self.billingAddress.isValid();
            }

            return paymentsAreValid && self.billingEmail.isValid() && self.billingConfirmEmail.isValid();
        },
        write: function (value) {
            return Boolean(value);
        }
    });

    // common //
    self.expirationDates = expirationDates;
    self.expirationYears = expirationYears;
    self.currencyCode = ko.observable(data.CurrencyCode);

    var Country = function (name, code) {
        this.country = name;
        this.code = code;
    };
    self.countries = ko.observableArray();
    if (data.Countries != null) {
        $.each(data.Countries, function (index, value) {
            self.countries.push(new Country(value, index));
        });
    }
    self.states = ko.observableArray(GetAvailableStates(defaultCountryCode));

    self.getAddress = function (id) {
        var match = ko.utils.arrayFirst(self.userAddresses(), function (a) {
            if (a.externalId() === id && id !== "UseOther") {
                return a;
            }

            return null;
        });

        return match;
    };
}

ko.bindingHandlers.checkMe = {
    init: function (element, valueAccessor, all, vm, bindingContext) {
        ko.utils.registerEventHandler(element, "click", function () {
            var checkedValue = valueAccessor(),
                meValue = bindingContext.$data,
                checked = element.checked;
            if (checked && ko.isObservable(checkedValue)) {
                checkedValue(meValue);
            }
        });
    },
    update: function (element, valueAccessor, all, vm, bindingContext) {
        var checkedValue = ko.utils.unwrapObservable(valueAccessor()),
            meValue = bindingContext.$data;

        element.checked = (checkedValue === meValue);
    }
};

Array.prototype.every = function (fun /*, thisp*/) {
    var len = this.length;
    if (typeof fun != "function")
        throw new TypeError();

    var thisp = arguments[1];
    for (var i = 0; i < len; i++) {
        if (i in this &&
            !fun.call(thisp, this[i], i, this))
            return false;
    }

    return true;
};

function isItemValid(element, index, array) {
    return (element === true);
}