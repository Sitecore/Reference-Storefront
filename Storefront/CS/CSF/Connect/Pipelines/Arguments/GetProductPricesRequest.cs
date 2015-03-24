//-----------------------------------------------------------------------
// <copyright file="GetProductPricesRequest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>The get product prices request.</summary>
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

namespace Sitecore.Reference.Storefront.Connect.Pipelines.Arguments
{
    using Sitecore.Diagnostics;

    /// <summary>
    /// The get product prices request.
    /// </summary>
    public class GetProductPricesRequest : Sitecore.Commerce.Services.Prices.GetProductPricesRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetProductPricesRequest"/> class.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="productId">The product identifier.</param>
        /// <param name="priceTypeIds">The price type ids.</param>
        public GetProductPricesRequest(string catalogName, string productId, params string[] priceTypeIds)
            : base(productId, priceTypeIds)
        {
            Assert.ArgumentNotNull(catalogName, "catalogName");
            this.ProductCatalogName = catalogName;
            this.IncludeVariantPrices = false;
        }

        /// <summary>
        /// Gets or sets the name of the product catalog.
        /// </summary>       
        public string ProductCatalogName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [include variant prices].
        /// </summary>        
        public bool IncludeVariantPrices { get; set; }
    }
}