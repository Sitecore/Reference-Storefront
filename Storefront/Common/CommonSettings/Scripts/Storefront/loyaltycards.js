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

var loyaltyCardListViewModel = null;

function initActiveLoyaltyCards(sectionId) {
    ClearGlobalMessages();
    AJAXPost(StorefrontUri("api/storefront/loyalty/activeLoyaltyCards"), null, function (data, success, sender) {
        if (success && data.Success) {
            loyaltyCardListViewModel = new LoyaltyCardsListViewModel(data);
            ko.applyBindings(loyaltyCardListViewModel, document.getElementById(sectionId));
            loyaltyCardListViewModel.showLoader(false);
        }

        ShowGlobalMessages(data);        
    });
}

function initLoyaltyCards(sectionId) {
    $("#" + sectionId).hide();
    ClearGlobalMessages();

    AJAXPost(StorefrontUri("api/storefront/loyalty/getLoyaltyCards"), null, function (data, success, sender) {
        if (success && data.Success) {
            loyaltyCardListViewModel = new LoyaltyCardsListViewModel(data);
            ko.applyBindings(loyaltyCardListViewModel, document.getElementById(sectionId));
            $("#" + sectionId).show();
        }

        ShowGlobalMessages(data);
    });
}

function joinLoyaltyProgram() {
    ClearGlobalMessages();
    $('#joinLoyaltyProgram').button("loading");
    AJAXPost(StorefrontUri('api/storefront/loyalty/activateAccount'), null, function (data, success, sender) {
        if (success && data.Success) {
            loyaltyCardListViewModel.reload(data);
            $("#loyaltyCards").show();
            $("#loyaltyCardsEmpty").hide();
        }

        $('#joinLoyaltyProgram').button("reset");
        ShowGlobalMessages(data);    
    }, this);
}