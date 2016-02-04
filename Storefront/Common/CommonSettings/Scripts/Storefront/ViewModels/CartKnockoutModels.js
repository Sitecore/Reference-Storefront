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

function LineItemData(line) {
    var self = this;

    // lines //
    self.image = line.Image;
    self.displayName = line.DisplayName;
    self.color = line.Color;
    self.lineItemDiscount = line.LineDiscount;
    self.quantity = line.Quantity;
    self.linePrice = line.LinePrice;
    self.lineTotal = line.LineTotal;
    self.externalCartLineId = line.ExternalCartLineId;
    self.productUrl = line.ProductUrl;
    self.discountOfferNames = line.DiscountOfferNames;
    self.shouldShowSavings = ko.observable(self.lineItemDiscount !== "$0.00" ? true : false);
    self.shouldShowDiscountOffers = ko.observable(self.discountOfferNames.length > 0 ? true : false);

    // shipping //
    self.shippingOptions = ko.observableArray();
    if (line.ShippingOptions !== null) {
        $(line.ShippingOptions).each(function () {
            self.shippingOptions.push(this);
        });
    }

    self.isLineShipAll = ko.observable(false);
    self.isLineShipToStore = ko.observable(false);
    self.isLineShipToEmail = ko.observable(false);
    self.showShipOptionContent = ko.observable(false);
    self.selectedShippingOptionName = ko.observable(GetMessage('SelectDeliveryFirstMessage'));
    self.toggleShipContent = function () {
        self.showShipOptionContent(!self.showShipOptionContent());
    };

    self.selectedShippingOption = ko.observable('0');
    self.selectedShippingOption.subscribe(function (option) {
        self.isLineShipAll(option === 1);
        self.isLineShipToStore(option === 2);
        self.isLineShipToEmail(option === 3);
        self.showShipOptionContent(option !== 0);

        if (option === 2) {
            getMap('StoresMap-' + self.externalCartLineId);
        }

        var match = ko.utils.arrayFirst(self.shippingOptions(), function (o) {
            return o.ShippingOptionType.Value === option;
        });

        if (match != null) {
            self.selectedShippingOptionName(match.Name);
        }
    }.bind(this));

    self.shippingMethods = ko.observableArray();
    self.shippingMethod = ko.validatedObservable().extend({ required: true });
    self.selectShippingMethod = function (shippingMethod) {
        if (shippingMethodsArray.indexOf(self.externalCartLineId) === -1) {
            shippingMethodsArray.push(self.externalCartLineId);
        }
    };
    self.shippingAddress = ko.validatedObservable(new AddressViewModel({ "ExternalId": self.externalCartLineId }));
    self.selectedShippingAddress = ko.observable("UseOther");
    self.selectedShippingAddress.subscribe(function (id) {
        var match = ko.utils.arrayFirst(checkoutDataViewModel.userAddresses(), function (a) {
            if (a.externalId() === id && id !== "UseOther") {
                return a;
            }

            return null;
        });

        self.shippingMethod("");
        self.shippingMethods.removeAll();
        if (match != null) {
            self.shippingAddress(match);
        } else {
            self.shippingAddress(new AddressViewModel({ "ExternalId": self.externalCartLineId }));
        }
    }.bind(this));
    self.shippingAddressFieldChanged = function () {
        var index = shippingMethodsArray.indexOf(self.externalCartLineId);
        if (index !== -1) {
            shippingMethodsArray.splice(index, 1);
        }
        self.shippingMethod("");
        self.shippingMethods.removeAll();
    };

    self.stores = ko.observableArray();
    self.store = ko.validatedObservable(new StoreViewModel());
    self.changeSelectedStore = function (item, event) {
        self.store(item);
    };

    self.shippingEmail = ko.validatedObservable("").extend({ required: true, email: true });
    self.shippingEmail.subscribe(function (email) {
        var index = shippingMethodsArray.indexOf(self.externalCartLineId);
        if (email.trim().length > 0 && index === -1) {
            shippingMethodsArray.push(self.externalCartLineId);
        }
        else if (email.trim().length === 0 && index !== -1) {
            shippingMethodsArray.splice(index, 1);
        }

    }.bind(this));
    self.shippingEmailContent = ko.observable("");
}

function Adjustment(description) {
    this.description = description;
}

function LineItemListViewModel(data) {
    var self = this;

    self.cartLines = ko.observableArray();

    $(data.Lines).each(function () {
        self.cartLines.push(new LineItemData(this));
    });

    self.adjustments = ko.observableArray();

    $(data.Adjustments).each(function () {
        self.adjustments.push(new Adjustment(this.Description));
    });

    self.promoCodes = ko.observableArray();

    $(data.PromoCodes).each(function () {
        self.promoCodes.push(this);
    });

    self.subTotal = ko.observable(data.Subtotal);
    self.taxTotal = ko.observable(data.TaxTotal);
    self.total = ko.observable(data.Total);
    self.totalAmount = ko.observable(data.TotalAmount);
    self.discount = ko.observable(data.Discount);
    self.shippingTotal = ko.observable(data.ShippingTotal);

    self.promoCode = ko.observable("");

    self.setAdjustments = function (data) {
        self.adjustments.removeAll();

        $(data.Adjustments).each(function () {
            self.adjustments.push(new Adjustment(this.Description));
        });
    }

    self.setSummary = function (data) {
        self.subTotal(data.Subtotal);
        self.taxTotal(data.TaxTotal);
        self.total(data.Total);
        self.totalAmount(data.TotalAmount);
        self.discount(data.Discount);
        self.shippingTotal(data.ShippingTotal);
    }

    self.setPromoCodes = function (data) {
        self.promoCodes.removeAll();

        $(data.PromoCodes).each(function () {
            self.promoCodes.push(this);
        });
    }

    self.reload = function (data) {
        self.cartLines.removeAll();

        $(data.Lines).each(function () {
            self.cartLines.push(new LineItemData(this));
        });

        self.setSummary(data);
        self.setAdjustments(data);
        self.setPromoCodes(data);

        manageCartActions();
    }

    self.hasPromoCode = ko.computed(function () {
        return self.promoCode();
    });
}