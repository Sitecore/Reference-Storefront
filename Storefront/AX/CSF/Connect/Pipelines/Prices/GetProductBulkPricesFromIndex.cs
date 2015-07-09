//-----------------------------------------------------------------------
// <copyright file="GetProductBulkPricesFromIndex.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Pipeline is used to get product bulk prices from the index file.  In AX we only 
// look at the product level adjusted price and list price since AX is not currently 
// supporting variants.</summary>
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

namespace Sitecore.Reference.Storefront.Connect.Pipelines.Prices
{
    using CommerceServer.Core.Catalog;
    using Newtonsoft.Json;
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Catalog;
    using Sitecore.Commerce.Connect.CommerceServer.Catalog.Pipelines;
    using Sitecore.Commerce.Connect.CommerceServer.Search;
    using Sitecore.Commerce.Connect.CommerceServer.Search.Models;
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Entities.Prices;
    using Sitecore.Commerce.Services;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Connect.Models;
    using Sitecore.Reference.Storefront.Connect.Pipelines.Arguments;
    using Sitecore.Reference.Storefront.Search.ComputedFields;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the GetProductBulkPricesFromIndex class.
    /// </summary>
    public class GetProductBulkPricesFromIndex : PricePipelineProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetProductBulkPricesFromIndex"/> class.
        /// </summary>
        /// <param name="entityFactory">The entity factory.</param>
        public GetProductBulkPricesFromIndex([NotNull] IEntityFactory entityFactory)
        {
            Assert.ArgumentNotNull(entityFactory, "entityFactory");

            this.EntityFactory = entityFactory;
        }

        /// <summary>
        /// Gets or sets the entity factory.
        /// </summary>
        /// <value>
        /// The entity factory.
        /// </value>
        public IEntityFactory EntityFactory { get; set; }

        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(Commerce.Pipelines.ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.Request, "args.request");
            Assert.ArgumentCondition(args.Request is GetProductBulkPricesRequest, "args.Request", "args.Request is GetProductBulkPricesRequest");
            Assert.ArgumentCondition(args.Result is Sitecore.Commerce.Services.Prices.GetProductBulkPricesResult, "args.Result", "args.Result is GetProductBulkPricesResult");

            GetProductBulkPricesRequest request = (GetProductBulkPricesRequest)args.Request;
            Sitecore.Commerce.Services.Prices.GetProductBulkPricesResult result = (Sitecore.Commerce.Services.Prices.GetProductBulkPricesResult)args.Result;

            Assert.ArgumentNotNull(request.ProductCatalogName, "request.ProductCatalogName");
            Assert.ArgumentNotNull(request.ProductIds, "request.ProductIds");
            Assert.ArgumentNotNull(request.PriceType, "request.PriceType");

            var uniqueIds = request.ProductIds.ToList().Distinct();
            foreach (var requestedProductId in uniqueIds)
            {
                var productDocument = this.GetProductFromIndex(request.ProductCatalogName, requestedProductId);
                if (productDocument != null)
                {
                    decimal? listPrice = string.IsNullOrWhiteSpace(productDocument.SafeGetFieldValue("listprice")) ? (decimal?)null : Convert.ToDecimal(productDocument["listprice"], CultureInfo.InvariantCulture);
                    decimal? adjustedPrice = string.IsNullOrWhiteSpace(productDocument.SafeGetFieldValue("adjustedprice")) ? (decimal?)null : Convert.ToDecimal(productDocument["adjustedprice"], CultureInfo.InvariantCulture);

                    ExtendedCommercePrice extendedPrice = this.EntityFactory.Create<ExtendedCommercePrice>("Price");

                    if (adjustedPrice.HasValue)
                    {
                        extendedPrice.ListPrice = adjustedPrice.Value;
                    }
                    else
                    {
                        // No base price is defined, the List price is set to the actual ListPrice define in the catalog
                        extendedPrice.ListPrice = listPrice.Value;
                    }

                    // The product list price is the Connect "Adjusted" price.
                    if (listPrice.HasValue)
                    {
                        extendedPrice.Amount = listPrice.Value;
                    }

                    result.Prices.Add(requestedProductId, extendedPrice);
                }
            }
        }

        /// <summary>
        /// Gets the product information from the index file.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="productId">The product identifier.</param>
        /// <returns>The found product document instance; Otherwise null.</returns>
        protected virtual CommerceProductSearchResultItem GetProductFromIndex(String catalogName, string productId)
        {
            var searchManager = CommerceTypeLoader.CreateInstance<ICommerceSearchManager>();
            var searchIndex = searchManager.GetIndex();

            using (var context = searchIndex.CreateSearchContext())
            {
                var searchResults = context.GetQueryable<CommerceProductSearchResultItem>()
                                    .Where(item => item.CommerceSearchItemType == CommerceSearchResultItemType.Product)
                                    .Where(item => item.CatalogName == catalogName)
                                    .Where(item => item.Language == Sitecore.Context.Language.Name)
                                    .Where(item => item.Name == productId)
                                    .Select(p => p);

                var list = searchResults.ToList();
                if (list.Count > 0)
                {
                    return list[0];
                }
            }

            return null;
        }
    }
}
