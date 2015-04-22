//-----------------------------------------------------------------------
// <copyright file="LoyaltyTierItemBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the LoyaltyTierItemBaseJsonResult class.</summary>
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
    using Sitecore.Diagnostics;
    using Sitecore.Commerce.Entities.LoyaltyPrograms;
    using Sitecore.Reference.Storefront.Extensions;

    /// <summary>
    /// Json result for loyalty program tier operations.
    /// </summary>
    public class LoyaltyTierItemBaseJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the tier identifier.
        /// </summary>
        /// <value>
        /// The tier identifier.
        /// </value>
        public string TierId { get; set; }

        /// <summary>
        /// Gets or sets the tier level.
        /// </summary>
        /// <value>
        /// The tier level.
        /// </value>
        public string TierLevel { get; set; }

        /// <summary>
        /// Gets or sets the valid from.
        /// </summary>
        /// <value>
        /// The valid from.
        /// </value>
        public string ValidFrom { get; set; }

        /// <summary>
        /// Gets or sets the valid to.
        /// </summary>
        /// <value>
        /// The valid to.
        /// </value>
        public string ValidTo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is elegible.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is elegible; otherwise, <c>false</c>.
        /// </value>
        public bool IsElegible { get; set; }

        /// <summary>
        /// Initializes the specified tier.
        /// </summary>
        /// <param name="tier">The tier.</param>
        /// <param name="cardTier">The card tier.</param>
        public virtual void Initialize(LoyaltyTier tier, LoyaltyCardTier cardTier)
        {
            Assert.ArgumentNotNull(tier, "tier");

            this.TierId = tier.TierId;
            this.Description = tier.Description;
            this.TierLevel = tier.TierLevel.ToString(Sitecore.Context.Language.CultureInfo);
            this.IsElegible = false;

            if (cardTier == null)
            {
                return;
            }

            this.ValidFrom = cardTier.ValidFrom.ToDisplayedDate();
            this.ValidTo = cardTier.ValidTo.ToDisplayedDate();
            this.IsElegible = true;
        }
    }
}