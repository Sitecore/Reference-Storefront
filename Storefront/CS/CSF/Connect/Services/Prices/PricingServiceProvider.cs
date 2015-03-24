//-----------------------------------------------------------------------
// <copyright file="PricingServiceProvider.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>The extended Pricing service provider.</summary>
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

namespace Sitecore.Reference.Storefront.Connect.Services.Prices
{
    using Sitecore.Commerce.Services;
    using Sitecore.Reference.Storefront.Connect.Pipelines.Arguments;

    /// <summary>
    /// The extended Pricing service provider.
    /// </summary>
    public class PricingServiceProvider : ServiceProvider
    {
        /// <summary>
        /// Gets the cart total.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Returns Cart totals</returns>
        public virtual Sitecore.Commerce.Services.Prices.GetCartTotalResult GetCartTotal(Sitecore.Commerce.Services.Prices.GetCartTotalRequest request)
        {
            return this.RunPipeline<Sitecore.Commerce.Services.Prices.GetCartTotalRequest, Sitecore.Commerce.Services.Prices.GetCartTotalResult>("commerce.prices.getCartTotal", request);
        }

        /// <summary>
        /// Gets the product bulk prices.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Returns product bulk prices.</returns>
        public virtual GetProductBulkPricesResult GetProductBulkPrices(GetProductBulkPricesRequest request)
        {
            return this.RunPipeline<GetProductBulkPricesRequest, GetProductBulkPricesResult>("commerce.prices.getProductBulkPrices", request);
        }

        /// <summary>
        /// Gets the product prices.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Returns product prices.</returns>
        public virtual GetProductPricesResult GetProductPrices(GetProductPricesRequest request)
        {
            return this.RunPipeline<GetProductPricesRequest, GetProductPricesResult>("commerce.prices.getProductPrices", request);
        }
    }
}