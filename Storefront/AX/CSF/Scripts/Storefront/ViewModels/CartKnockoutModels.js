
function LineItemData(image, displayName, color, lineItemDiscount, discountOfferNames, quantity, linePrice, lineTotal, externalCartlineId, productUrl) {
    //var self = this;

    this.image = image;
    this.displayName = displayName;
    this.color = color;
    this.lineItemDiscount = lineItemDiscount;
    this.quantity = quantity;
    this.linePrice = linePrice;
    this.lineTotal = lineTotal;
    this.externalCartlineId = externalCartlineId;
    this.productUrl = productUrl;
    this.discountOfferNames = discountOfferNames;
    this.lineShippingOptions = ko.observableArray();
    this.shouldShowSavings = ko.observable(this.lineItemDiscount > 0 ? true : false);
    this.shouldShowDiscountOffers = ko.observable(this.discountOfferNames ? true : false);
    this.shippingMethods = ko.observableArray();
}

function Adjustment(description)
{
    this.description = description;
}

function LineItemListViewModel(data) {
    var self = this;

    self.cartlines = ko.observableArray();

    $(data.Lines).each(function () {
        self.cartlines.push(new LineItemData(this.Image, this.DisplayName, this.Color, this.LineDiscount, this.DiscountOfferNames, this.Quantity, this.LinePrice, this.LineTotal, this.ExternalCartLineId, this.ProductUrl));
    });

    self.adjustments = ko.observableArray();

    $(data.Adjustments).each(function () {
        self.adjustments.push(new Adjustment(this.Description));
    })

    self.subTotal = ko.observable(data.Subtotal);
    self.taxTotal = ko.observable(data.TaxTotal);
    self.total = ko.observable(data.Total);
    self.discount = ko.observable(data.Discount);

    self.promoCode = ko.observable("");

    self.setAdjustments = function (data) {
        self.adjustments.removeAll();

        $(data.Adjustments).each(function () {
            self.adjustments.push(new Adjustment(this.Description));
        })
    }

    self.setSummary = function (data) {
        self.subTotal(data.Subtotal);
        self.taxTotal(data.TaxTotal);
        self.total(data.Total);
        self.discount(data.Discount);
    }

    self.reload = function (data) {
        self.cartlines.removeAll();

        $(data.Lines).each(function () {
            self.cartlines.push(new LineItemData(this.Image, this.DisplayName, this.Color, this.LineDiscount, this.DiscountOfferNames, this.Quantity, this.LinePrice, this.LineTotal, this.ExternalCartLineId, this.ProductUrl));
        });

        self.setSummary(data);
        self.setAdjustments(data);

        manageCartActions();
    }

    self.hasPromoCode = ko.computed(function () {
        return self.promoCode();
    });
}

