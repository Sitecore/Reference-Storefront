//-----------------------------------------------------------------------
// <copyright file="GetProductBulkPricesRequest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>The get product bulk prices request.</summary>
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
    using System.Collections.Generic;

    /// <summary>
    /// The get product bulk prices request.
    /// </summary>
    public class GetProductBulkPricesRequest : Sitecore.Commerce.Services.Prices.GetProductBulkPricesRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetProductBulkPricesRequest"/> class.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="productIds">The product ids.</param>
        /// <param name="priceTypeIds">The price type ids.</param>
        public GetProductBulkPricesRequest(string catalogName, IEnumerable<string> productIds, params string[] priceTypeIds)
            : base(productIds)
        {
            Assert.ArgumentNotNull(catalogName, "catalogName");
            this.ProductCatalogName = catalogName;
            this.PriceTypeIds = priceTypeIds;
        }

        /// <summary>
        /// Gets or sets the name of the product catalog.
        /// </summary>       
        public string ProductCatalogName { get; set; }

        /// <summary>
        /// Gets or sets the price type ids.
        /// </summary>        
        public IEnumerable<string> PriceTypeIds { get; protected set; }
    }
}