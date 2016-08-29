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

var billingViemModel = null;
var shippingMethodViewModel = null;

function GiftCardPaymentViewModel(card) {
    var self = this;
    var populate = card != null;

    self.isAdded = ko.observable(false);
    self.balance = ko.observable(0);
    self.formattedBalance = ko.observable("");
    self.enableAddCard = ko.observable(false);

    self.giftCardNumber = populate ? ko.validatedObservable(card.PaymentMethodID).extend({ required: true }) : ko.validatedObservable("").extend({ required: true });
    self.giftCardNumber.subscribe(function (cn) {
        self.balance(0);
        self.formattedBalance("");
        self.enableAddCard(cn.length > 0 && self.giftCardAmount().length > 0 && self.isAdded() === false);
    }.bind(this));

    self.giftCardAmount = populate ? ko.validatedObservable(card.Amount).extend({ required: true, number: true }) : ko.validatedObservable(0.00).extend({ required: true, number: true });
    self.giftCardAmount.subscribe(function (ca) {
        self.enableAddCard(ca.length > 0 && self.giftCardNumber().length > 0 && self.isAdded() === false);
        self.revalueAmounts();
    }.bind(this));

    self.reload = function (data) {
        self.giftCardNumber(data.ExternalId);
        self.formattedBalance(data.FormattedBalance);
        self.balance(data.Balance);
        self.enableAddCard(false);
    }

    self.getBalance = function () {
        ClearGlobalMessages();
        states('giftCardPayment_GetBalance', 'loading');
        var data = {};
        data.GiftCardId = self.giftCardNumber();
        AJAXPost(StorefrontUri('api/storefront/catalog/checkgiftcardbalance'), JSON.stringify(data), function (data, success, sender) {
            if (success && data.Success) {
                self.reload(data);
            }

            ShowGlobalMessages(data);
            $('#giftCardPayment_GetBalance').button('reset');
        }, this);
    };

    self.applyBalance = function () {
        ClearGlobalMessages();
        var balance = self.balance();
        if (balance <= 0) {
            return;
        }

        var orderTotal = checkoutDataViewModel.cart().totalAmount();
        if (balance > orderTotal) {
            self.giftCardAmount(orderTotal);
        }
        else if (balance <= orderTotal) {
            self.giftCardAmount(balance);
        }

        self.enableAddCard(true);
    };

    self.addCard = function () {
        ClearGlobalMessages();
        self.isAdded(true);
        self.enableAddCard(false);
        self.revalueAmounts();
    };

    self.removeCard = function () {
        ClearGlobalMessages();
        self.isAdded(false);
        self.giftCardNumber("");
        self.formattedBalance("");
        self.giftCardAmount(0.00);
        self.balance(0.00);
    };

    self.revalueAmounts = function () {
        if (!self.isAdded()) {
            return;
        }

        var lcIsAdded = checkoutDataViewModel.loyaltyCardPayment().isAdded();
        var ccIsAdded = checkoutDataViewModel.creditCardPayment().isAdded();
        if (!lcIsAdded && !ccIsAdded) {
            return;
        }

        var ccAmount = parseFloat(checkoutDataViewModel.creditCardPayment().creditCardAmount());
        var lcAmount = parseFloat(checkoutDataViewModel.loyaltyCardPayment().loyaltyCardAmount());
        var total = parseFloat(checkoutDataViewModel.cart().totalAmount());
        var aTotal = parseFloat(parseFloat(self.giftCardAmount()) + lcAmount + ccAmount);

        if (aTotal > total) {
            var count = lcIsAdded && ccIsAdded ? 2 : 1;
            var diff = (aTotal - total) / count;
            lcAmount = lcIsAdded ? lcAmount - diff : 0;
            ccAmount = ccIsAdded ? ccAmount - diff : 0;
            checkoutDataViewModel.loyaltyCardPayment().loyaltyCardAmount((lcAmount).toFixed(2));
            checkoutDataViewModel.creditCardPayment().creditCardAmount((ccAmount).toFixed(2));
        }
    };
}

function LoyaltyCardPaymentViewModel(card) {
    var self = this;
    var populate = card != null;

    self.enableAddCard = ko.observable(false);
    self.isAdded = ko.observable(false);

    self.loyaltyCardNumber = ko.validatedObservable("").extend({ required: true });
    if (populate) {
        if (card.PaymentMethodID) {
            self.loyaltyCardNumber(card.PaymentMethodID);
        }
        else if (card.CartLoyaltyCardNumber) {
            self.loyaltyCardNumber(card.CartLoyaltyCardNumber);
        }
    }
    self.loyaltyCardNumber.subscribe(function (cn) {
        self.enableAddCard(cn.length > 0 && self.loyaltyCardAmount().length > 0 && self.isAdded() === false);
    }.bind(this));

    self.loyaltyCardAmount = populate ? ko.validatedObservable(card.Amount).extend({ required: true, number: true }) : ko.validatedObservable(0.00).extend({ required: true, number: true });
    self.loyaltyCardAmount.subscribe(function (ca) {
        self.enableAddCard(ca.length > 0 && self.loyaltyCardNumber().length > 0 && self.isAdded() === false);
        self.revalueAmounts();
    }.bind(this));

    self.addCard = function () {
        ClearGlobalMessages();
        self.isAdded(true);
        self.enableAddCard(false);
        self.revalueAmounts();
    };

    self.removeCard = function () {
        ClearGlobalMessages();
        self.isAdded(false);
        self.loyaltyCardNumber("");
        self.loyaltyCardAmount(0.00);
    };

    self.revalueAmounts = function () {
        if (!self.isAdded()) {
            return;
        }

        var gcIsAdded = checkoutDataViewModel.giftCardPayment().isAdded();
        var ccIsAdded = checkoutDataViewModel.creditCardPayment().isAdded();
        if (!gcIsAdded && !ccIsAdded) {
            return;
        }

        var ccAmount = parseFloat(checkoutDataViewModel.creditCardPayment().creditCardAmount());
        var gcAmount = parseFloat(checkoutDataViewModel.giftCardPayment().giftCardAmount());
        var total = parseFloat(checkoutDataViewModel.cart().totalAmount());
        var aTotal = parseFloat(parseFloat(self.loyaltyCardAmount()) + gcAmount + ccAmount);

        if (aTotal > total) {
            var count = gcIsAdded && ccIsAdded ? 2 : 1;
            var diff = (aTotal - total) / count;
            gcAmount = gcIsAdded ? gcAmount - diff : 0;
            ccAmount = ccIsAdded ? ccAmount - diff : 0;
            checkoutDataViewModel.giftCardPayment().giftCardAmount((gcAmount).toFixed(2));
            checkoutDataViewModel.creditCardPayment().creditCardAmount((ccAmount).toFixed(2));
        }
    };
}

function CreditCardPaymentViewModel(card) {
    var self = this;
    var populate = card != null;

    self.isAdded = ko.observable(false);
    self.creditCardNumber = populate ? ko.validatedObservable(card.CreditCardNumber).extend({ required: true, number: true }) : ko.validatedObservable().extend({ required: true, number: true });
    self.creditCardNumberMasked = ko.observable();
    self.creditCardNumber.subscribe(function (number) {
        self.creditCardNumberMasked("XXXX XXXX XXXX " + number.substr(number.length - 4));
    });

    self.description = ko.observable();

    self.paymentMethodID = populate ? ko.validatedObservable(card.PaymentMethodID).extend({ required: true }) : ko.validatedObservable().extend({ required: true });
    self.paymentMethodID.subscribe(function (methodId) {
        if (methodId !== "0") {
            self.isAdded(true);

            var paymentMethod = ko.utils.arrayFirst(checkoutDataViewModel.paymentMethods(), function (a) {
                if (a.ExternalId === methodId) {
                    return a;
                }

                return null;
            });

            self.description(paymentMethod ? paymentMethod.Description : "");
            checkoutDataViewModel.creditCardEnable(true);
            checkoutDataViewModel.billingAddressEnable(true);
        } else {
            self.isAdded(false);
            checkoutDataViewModel.creditCardEnable(false);
            checkoutDataViewModel.billingAddressEnable(false);
        }
    });

    self.validationCode = populate ? ko.validatedObservable(card.validationCode).extend({ required: true }) : ko.validatedObservable().extend({ required: true });
    self.expirationMonth = populate ? ko.validatedObservable(card.expirationMonth).extend({ required: true }) : ko.validatedObservable().extend({ required: true });
    self.expirationYear = populate ? ko.validatedObservable(card.expirationYear).extend({ required: true }) : ko.validatedObservable().extend({ required: true });
    self.customerNameOnPayment = populate ? ko.validatedObservable(card.CustomerNameOnPayment).extend({ required: true }) : ko.validatedObservable().extend({ required: true });
    self.creditCardAmount = ko.computed({
        read: function () {
            if (!self.isAdded()) {
                return 0;
            }

            var total = checkoutDataViewModel.cart() ? checkoutDataViewModel.cart().totalAmount() : 0;
            var gcAmount = checkoutDataViewModel.giftCardPayment() ? checkoutDataViewModel.giftCardPayment().giftCardAmount() : 0;
            var lcAmount = checkoutDataViewModel.loyaltyCardPayment() ? checkoutDataViewModel.loyaltyCardPayment().loyaltyCardAmount() : 0;
            return (parseFloat(total) - parseFloat(gcAmount) - parseFloat(lcAmount)).toFixed(2);
        },
        write: function () { }
    });
    self.partyID = populate ? ko.observable(card.PartyID) : ko.observable();
    self.removeCard = function () {
        ClearGlobalMessages();
        self.isAdded(false);
        self.creditCardAmount(0.0);
        $("#ccpayment").trigger('click');
        if (checkoutDataViewModel.cardPaymentResultAccessCode.length == 0) {
            $("#cardPaymentAcceptFrame")[0].src = $("#cardPaymentAcceptFrame")[0].src;
        }
        else {
            getCardPaymentAcceptUrl();
            checkoutDataViewModel.cardPaymentResultAccessCode = "";
        }              
    };
}