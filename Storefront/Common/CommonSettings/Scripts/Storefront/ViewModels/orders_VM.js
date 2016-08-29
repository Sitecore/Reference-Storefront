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

var ordersHeaderViewModel = null;

// model
function OrderHeaderModel(orderHeader) {
    var self = this;

    self.externalId = orderHeader.ExternalId;
    self.orderId = orderHeader.OrderId;
    self.status = orderHeader.Status;
    self.lastModified = orderHeader.LastModified;
    self.detailsUrl = orderHeader.DetailsUrl;
}

//viewmodel
function OrderHeaderViewModel(data) {
    var self = this;

    self.orders = ko.observableArray();
    $.each(data.Orders, function () {
        self.orders().push(new OrderHeaderModel(this));
    });

    self.hasOrders = ko.observable(self.orders().length !== 0);
    self.showLoader = ko.observable(true);
}

//data
function initRecentOrders(sectionId) {
    AJAXPost(StorefrontUri("api/storefront/account/recentOrders"), null, function (data, success, sender) {
        if (success && data.Success) {
            ordersHeaderViewModel = new OrderHeaderViewModel(data);
            ko.applyBindings(ordersHeaderViewModel, document.getElementById(sectionId));
            ordersHeaderViewModel.showLoader(false);
        }
        
        ShowGlobalMessages(data);
    });
}