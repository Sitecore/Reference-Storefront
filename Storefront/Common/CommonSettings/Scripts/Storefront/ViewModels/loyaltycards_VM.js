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

function LoyaltyTierViewModel(tier) {
    var self = this;

    self.TierId = ko.observable(tier.TierId);
    self.Description = ko.observable(tier.Description);
    self.TierLevel = ko.observable(tier.TierLevel);
    self.ValidFrom = ko.observable(tier.ValidFrom);
    self.ValidTo = ko.observable(tier.ValidTo);
    self.IsElegible = ko.observable(tier.IsElegible);
}

function LoyaltyProgramViewModel(program) {
    var self = this;

    self.Name = ko.observable(program.Name);
    self.Description = ko.observable(program.Description);
    self.ProgramId = ko.observable(program.ProgramId);
    self.LoyaltyTiers = ko.observableArray(program.LoyaltyTiers);
}

function LoyaltyTransactionViewModel(transaction) {
    var self = this;

    self.ExternalId = ko.observable(transaction.ExternalId);
    self.EntryTime = ko.observable(transaction.EntryTime);
    self.EntryDate = ko.observable(transaction.EntryDate);
    self.EntryType = ko.observable(transaction.EntryType);
    self.ExpirationDate = ko.observable(transaction.ExpirationDate);
    self.Points = ko.observable(transaction.Points);
    self.Store = ko.observable(transaction.Store);
}

function LoyaltyRewardPointViewModel(rewardPoint) {
    var self = this;

    self.RewardPointId = ko.observable(rewardPoint.RewardPointId);
    self.ActivePoints = ko.observable(rewardPoint.ActivePoints);
    self.Description = ko.observable(rewardPoint.Description);
    self.ExpiredPoints = ko.observable(rewardPoint.ExpiredPoints);
    self.IssuedPoints = ko.observable(rewardPoint.IssuedPoints);
    self.RewardPointType = ko.observable(rewardPoint.RewardPointType);
    self.UsedPoints = ko.observable(rewardPoint.UsedPoints);
    self.Transactions = ko.observableArray(rewardPoint.Transactions);
}

function LoyaltyCardViewModel(card) {
    var self = this;

    self.CardNumber = ko.observable(card.CardNumber);
    self.Programs = ko.observableArray(card.Programs);
    self.RewardPoints = ko.observableArray(card.RewardPoints);
}

function LoyaltyCardsListViewModel(data) {
    var self = this;

    self.LoyaltyCards = ko.observableArray(data.LoyaltyCards);
    self.isNotEmpty = ko.observable(self.LoyaltyCards().length !== 0);
    self.isEmpty = ko.observable(self.LoyaltyCards().length === 0);
    self.card = ko.observable();
    self.program = ko.observable();
    self.point = ko.observable();
    self.selectedCard = ko.observable();
    self.selectedCard.subscribe(function (cardNumber) {
        var card = ko.utils.arrayFirst(this.LoyaltyCards(), function (card) {
            if (card.CardNumber === cardNumber) {
                return card;
            }

            return null;
        });

        if (card != null) {
            self.card(card);
            self.program(card.Programs[0]);
            self.point(card.RewardPoints[0]);
        } else {
            self.card('');
        }
    }.bind(this));

    self.changeTiers = function () {
        self.program(this);
    }

    self.changeTransactions = function () {
        self.point(this);
    }

    self.reload = function (data) {
        self.LoyaltyCards.removeAll();
        $(data.LoyaltyCards).each(function () {
            self.LoyaltyCards.push(new LoyaltyCardViewModel(this));
        });
    }

    self.showLoader = ko.observable(true);
}