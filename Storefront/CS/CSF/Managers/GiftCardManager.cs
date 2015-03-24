//-----------------------------------------------------------------------
// <copyright file="GiftCardManager.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>The manager class responsible for encapsulating the gift card 
// business logic for the site.</summary>
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

namespace Sitecore.Reference.Storefront.Managers
{
    using Sitecore.Commerce.Entities.GiftCards;
    using Sitecore.Commerce.Services.GiftCards;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Models.SitecoreItemModels;

    /// <summary>
    /// Defines the GiftCardManager class.
    /// </summary>
    public class GiftCardManager : BaseManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GiftCardManager"/> class.
        /// </summary>
        /// <param name="giftCardServiceProvider">The gift card service provider.</param>
        public GiftCardManager([NotNull] GiftCardServiceProvider giftCardServiceProvider)
        {
            Assert.ArgumentNotNull(giftCardServiceProvider, "giftCardServiceProvider");

            this.GiftCardServiceProvider = giftCardServiceProvider;
        }

        /// <summary>
        /// Gets or sets the gift card service provider.
        /// </summary>
        /// <value>
        /// The gift card service provider.
        /// </value>
        public GiftCardServiceProvider GiftCardServiceProvider { get; protected set; }

        /// <summary>
        /// Gets the gift card balance.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="giftCardId">The gift card identifier.</param>
        /// <returns>
        /// The manager response where the gift card balance is returned in the Result.
        /// </returns>
        public ManagerResponse<GetGiftCardResult, decimal> GetGiftCardBalance([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] string giftCardId)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNullOrEmpty(giftCardId, "giftCardId");

            var result = this.GetGiftCard(giftCardId, storefront.ShopName).ServiceProviderResult;

            Helpers.LogSystemMessages(result.SystemMessages, result);
            return new ManagerResponse<GetGiftCardResult, decimal>(result, (result.Success && result.GiftCard != null) ? result.GiftCard.Balance : -1);
        }
        
        /// <summary>
        /// Gets the gift card.
        /// </summary>
        /// <param name="giftCardId">The gift card identifier.</param>
        /// <param name="shopName">Name of the shop.</param>
        /// <returns>
        /// A gift card if found
        /// </returns>
        public ManagerResponse<GetGiftCardResult, GiftCard> GetGiftCard(string giftCardId, string shopName)
        {
            Assert.ArgumentNotNullOrEmpty(giftCardId, "giftCardId");

            var request = new GetGiftCardRequest(giftCardId, shopName);
            var result = this.GiftCardServiceProvider.GetGiftCard(request);

            Helpers.LogSystemMessages(result.SystemMessages, result);
            return new ManagerResponse<GetGiftCardResult, GiftCard>(result, result.GiftCard);
        }
    }
}