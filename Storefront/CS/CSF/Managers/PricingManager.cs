//-----------------------------------------------------------------------
// <copyright file="PricingManager.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>The manager class responsible for encapsulating the pricing business logic for the site.</summary>
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
    using Sitecore.Commerce.Entities.Prices;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Connect.Pipelines.Arguments;
    using Sitecore.Reference.Storefront.Connect.Services.Prices;
    using Sitecore.Reference.Storefront.Models.SitecoreItemModels;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the PricingManager class.
    /// </summary>
    public class PricingManager : BaseManager
    {
        private static string[] defaultPriceTypeIds = new string[2] { "List", "Adjusted" };

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PricingManager"/> class.
        /// </summary>
        /// <param name="pricingServiceProvider">The pricing service provider.</param>
        public PricingManager([NotNull] PricingServiceProvider pricingServiceProvider)
        {
            Assert.ArgumentNotNull(pricingServiceProvider, "pricingServiceProvider");

            this.PricingServiceProvider = pricingServiceProvider;
        }

        #endregion

        #region Properties (public)

        /// <summary>
        /// Gets or sets the pricing service provider.
        /// </summary>
        /// <value>
        /// The pricing service provider.
        /// </value>
        public PricingServiceProvider PricingServiceProvider { get; protected set; }

        #endregion

        #region Methods (public, virtual)

        /// <summary>
        /// Gets the product prices.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="productId">The product identifier.</param>
        /// <param name="includeVariants">if set to <c>true</c> [include variants].</param>
        /// <param name="priceTypeIds">The price type ids.</param>
        /// <returns>The manager response with the list of prices in the Result.</returns>
        public virtual ManagerResponse<GetProductPricesResult, IDictionary<string, Dictionary<string, Price>>> GetProductPrices([NotNull] CommerceStorefront storefront, string catalogName, string productId, bool includeVariants, params string[] priceTypeIds)
        {
            Assert.ArgumentNotNull(storefront, "storefront");

            if (priceTypeIds == null)
            {
                priceTypeIds = defaultPriceTypeIds;
            }

            var request = new GetProductPricesRequest(catalogName, productId, priceTypeIds);
            request.IncludeVariantPrices = includeVariants;
            var result = this.PricingServiceProvider.GetProductPrices(request);

            Helpers.LogSystemMessages(result.SystemMessages, result);
            return new ManagerResponse<GetProductPricesResult, IDictionary<string, Dictionary<string, Price>>>(result, result.Prices == null ? new Dictionary<string, Dictionary<string, Price>>() : result.Prices);
        }

        /// <summary>
        /// Gets the product bulk prices.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="productIds">The product ids.</param>
        /// <param name="priceTypeIds">The price type ids.</param>
        /// <returns>The manager response with the list of prices in the Result.</returns>
        public virtual ManagerResponse<GetProductBulkPricesResult, IDictionary<string, Dictionary<string, Price>>> GetProductBulkPrices([NotNull] CommerceStorefront storefront, string catalogName, IEnumerable<string> productIds, params string[] priceTypeIds)
        {
            Assert.ArgumentNotNull(storefront, "storefront");

            if (priceTypeIds == null)
            {
                priceTypeIds = defaultPriceTypeIds;
            }

            var request = new GetProductBulkPricesRequest(catalogName, productIds, priceTypeIds);
            var result = this.PricingServiceProvider.GetProductBulkPrices(request);

            // Currently, both Categories and Products are passed in and are waiting for a fix to filter the categories out.  Until then, this code is commented
            // out as it generates an unecessary Error event indicating the product cannot be found.
            // Helpers.LogSystemMessages(result.SystemMessages, result);
            return new ManagerResponse<GetProductBulkPricesResult, IDictionary<string, Dictionary<string, Price>>>(result, result.Prices == null ? new Dictionary<string, Dictionary<string, Price>>() : result.Prices);
        }

        #endregion
    }
}