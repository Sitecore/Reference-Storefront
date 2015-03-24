//-----------------------------------------------------------------------
// <copyright file="LoyaltyCardsBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the LoyaltyCardsBaseJsonResult class.</summary>
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

namespace Sitecore.Reference.Storefront.Models.JsonResults
{
    using Sitecore.Commerce.Entities.LoyaltyPrograms;
    using Sitecore.Commerce.Services;
    using Sitecore.Diagnostics;
    using System.Collections.Generic;

    /// <summary>
    /// Json result for loyalty cards operations.
    /// </summary>
    public class LoyaltyCardsBaseJsonResult : BaseJsonResult
    {
        private readonly List<LoyaltyCardItemBaseJsonResult> _cards = new List<LoyaltyCardItemBaseJsonResult>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LoyaltyCardsBaseJsonResult"/> class.
        /// </summary>
        public LoyaltyCardsBaseJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoyaltyCardsBaseJsonResult"/> class.
        /// </summary>
        /// <param name="result">The service provider result.</param>
        public LoyaltyCardsBaseJsonResult(ServiceProviderResult result)
            : base(result)
        {
        }

        /// <summary>
        /// Gets the loyalty cards.
        /// </summary>
        public List<LoyaltyCardItemBaseJsonResult> LoyaltyCards
        {
            get
            {
                return this._cards;
            }
        }

        /// <summary>
        /// Initializes the specified loyalty cards.
        /// </summary>
        /// <param name="loyaltyCards">The loyalty cards.</param>
        public virtual void Initialize(IEnumerable<LoyaltyCard> loyaltyCards)
        {
            Assert.ArgumentNotNull(loyaltyCards, "loyaltyCards");
            
            foreach (var card in loyaltyCards)
            {
                var result = new LoyaltyCardItemBaseJsonResult();
                result.Initialize(card);
                this.LoyaltyCards.Add(result);
            }
        }
    }
}