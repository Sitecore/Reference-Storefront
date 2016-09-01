//-----------------------------------------------------------------------
// <copyright file="PricingManager.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>The manager class responsible for encapsulating the pricing business logic for the site.</summary>
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

namespace Sitecore.Reference.Storefront.Managers
{
    using System.Collections.Generic;
    using Sitecore.Commerce.Entities.Prices;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Models.SitecoreItemModels;
    using Sitecore.Commerce.Services.Prices;
    using Sitecore.Commerce.Services;
    using RefSFArgs = Sitecore.Reference.Storefront.Connect.Pipelines.Arguments;

    /// <summary>
    /// Defines the PricingManager class.
    /// </summary>
    public class PricingManager : BaseManager
    {
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
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="productId">The product identifier.</param>
        /// <param name="priceTypeIds">The price type ids.</param>
        /// <returns>
        /// The manager response with the list of prices in the Result.
        /// </returns>
        public virtual ManagerResponse<GetProductPricesResult, IDictionary<string, Price>> GetProductPrices([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, string productId, params string[] priceTypeIds)
        {
            Assert.ArgumentNotNull(storefront, "storefront");

            var request = new GetProductPricesRequest(productId, priceTypeIds);

            if (Sitecore.Context.User.IsAuthenticated)
            {
                request.UserId = visitorContext.GetCustomerId();
            }

            request.CurrencyCode = StorefrontManager.GetCustomerCurrency();

            var result = this.PricingServiceProvider.GetProductPrices(request);

            Helpers.LogSystemMessages(result.SystemMessages, result);
            return new ManagerResponse<GetProductPricesResult, IDictionary<string, Price>>(result, result.Prices ?? new Dictionary<string, Price>());
        }

        /// <summary>
        /// Gets the product bulk prices.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="productIds">The product ids.</param>
        /// <returns>
        /// The manager response with the list of prices in the Result.
        /// </returns>
        public virtual ManagerResponse<GetProductBulkPricesResult, IDictionary<string, Price>> GetProductBulkPrices([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, string catalogName, IEnumerable<string> productIds)
        {
            Assert.ArgumentNotNull(storefront, "storefront");

            var request = new Sitecore.Reference.Storefront.Connect.Pipelines.Arguments.GetProductBulkPricesRequest(catalogName, productIds);
            request.CurrencyCode = StorefrontManager.GetCustomerCurrency();

            var result = this.PricingServiceProvider.GetProductBulkPrices(request);

            Helpers.LogSystemMessages(result.SystemMessages, result);
            return new ManagerResponse<GetProductBulkPricesResult, IDictionary<string, Price>>(result, result.Prices ?? new Dictionary<string, Price>());
        }

        /// <summary>
        /// Gets the supported currencies.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <returns>The manager response.</returns>
        public virtual ManagerResponse<Sitecore.Commerce.Services.Prices.GetSupportedCurrenciesResult, IReadOnlyCollection<string>> GetSupportedCurrencies(CommerceStorefront storefront, string catalogName)
        {
            Assert.ArgumentNotNull(storefront, "storefront");

            var request = new RefSFArgs.GetSupportedCurrenciesRequest(storefront.ShopName, catalogName);
            var result = this.PricingServiceProvider.GetSupportedCurrencies(request);

            return new ManagerResponse<GetSupportedCurrenciesResult, IReadOnlyCollection<string>>(result, result.Currencies);
        }

        /// <summary>
        /// Generates the currency chosen page event.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="currency">The currency.</param>
        /// <returns>The manager response.</returns>
        public virtual ManagerResponse<ServiceProviderResult, bool> CurrencyChosenPageEvent(CommerceStorefront storefront, string currency)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNullOrEmpty(currency, "currency");

            var request = new CurrencyChosenRequest(storefront.ShopName, currency);
            var result = this.PricingServiceProvider.CurrencyChosen(request);

            return new ManagerResponse<ServiceProviderResult, bool>(result, result.Success);
        }

        #endregion
    }
}