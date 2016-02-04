//-----------------------------------------------------------------------
// <copyright file="GetProductBulkPricesFromIndex.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Pipeline is used to get product bulk prices from the index file.</summary>
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
    using Sitecore.ContentSearch.Linq.Utilities;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Connect.Models;
    using Sitecore.Reference.Storefront.Connect.Pipelines.Arguments;
    using Sitecore.Reference.Storefront.Search;
    using Sitecore.Reference.Storefront.Search.ComputedFields;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
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

            bool isList = request.PriceTypeIds.FirstOrDefault(x => x.Equals(PriceTypes.List, StringComparison.OrdinalIgnoreCase)) != null;
            bool isAdjusted = request.PriceTypeIds.FirstOrDefault(x => x.Equals(PriceTypes.Adjusted, StringComparison.OrdinalIgnoreCase)) != null;
            bool isLowestPriceVariantSpecified = request.PriceTypeIds.FirstOrDefault(x => x.Equals(PriceTypes.LowestPricedVariant, StringComparison.OrdinalIgnoreCase)) != null;
            bool isLowestPriceVariantListPriceSpecified = request.PriceTypeIds.FirstOrDefault(x => x.Equals(PriceTypes.LowestPricedVariantListPrice, StringComparison.OrdinalIgnoreCase)) != null;
            bool isHighestPriceVariantSpecified = request.PriceTypeIds.FirstOrDefault(x => x.Equals(PriceTypes.HighestPricedVariant, StringComparison.OrdinalIgnoreCase)) != null;

            var uniqueIds = request.ProductIds.ToList().Distinct();
            var productSearchItemList = this.GetProductsFromIndex(request.ProductCatalogName, uniqueIds);
            foreach (var productSearchItem in productSearchItemList)
            {
                var foundId = uniqueIds.SingleOrDefault(x => x == productSearchItem.Name);
                if (foundId == null)
                {
                    continue;
                }

                if (productSearchItem != null)
                {
                    // SOLR returns 0 and not <null> for missing prices.  We set the prices to null when 0 value is encountered.
                    decimal? listPrice = !productSearchItem.OtherFields.ContainsKey("listprice") ? (decimal?)null : ((decimal)productSearchItem.ListPrice == 0M ? (decimal?)null : (decimal)productSearchItem.ListPrice);
                    decimal? basePrice = !productSearchItem.OtherFields.ContainsKey("baseprice") ? (decimal?)null : ((decimal)productSearchItem.BasePrice == 0M ? (decimal?)null : (decimal)productSearchItem.BasePrice);

                    ExtendedCommercePrice extendedPrice = this.EntityFactory.Create<ExtendedCommercePrice>("Price");

                    // The product base price is the Connect "List" price.
                    if (isList)
                    {
                        if (basePrice.HasValue)
                        {
                            extendedPrice.Amount = basePrice.Value;
                        }
                        else
                        {
                            // No base price is defined, the List price is set to the actual ListPrice define in the catalog
                            extendedPrice.Amount = (listPrice.HasValue) ? listPrice.Value : 0M;
                        }
                    }

                    // The product list price is the Connect "Adjusted" price.
                    if (isAdjusted && listPrice.HasValue)
                    {
                        extendedPrice.ListPrice = listPrice.Value;
                    }

                    var variantInfoString = productSearchItem.VariantInfo;
                    if ((isLowestPriceVariantSpecified || isLowestPriceVariantListPriceSpecified || isHighestPriceVariantSpecified) && !string.IsNullOrWhiteSpace(variantInfoString))
                    {
                        List<VariantIndexInfo> variantIndexInfoList = JsonConvert.DeserializeObject<List<VariantIndexInfo>>(variantInfoString);
                        this.SetVariantPricesFromProductVariants(productSearchItem, variantIndexInfoList, extendedPrice, isLowestPriceVariantSpecified, isLowestPriceVariantListPriceSpecified, isHighestPriceVariantSpecified);
                    }

                    result.Prices.Add(foundId, extendedPrice);
                }
            }
        }

        /// <summary>
        /// Builds the product identifier list predicate.
        /// </summary>
        /// <param name="productIdList">The product identifier list.</param>
        /// <returns>
        /// The search predicate ORing the product ids.
        /// </returns>
        protected virtual Expression<Func<PriceSearchResultItem, bool>> BuildProductIdListPredicate(IEnumerable<string> productIdList)
        {
            Expression<Func<PriceSearchResultItem, bool>> predicate = null;

            bool isFirst = true;
            foreach (var productId in productIdList)
            {
                if (isFirst)
                {
                    predicate = PredicateBuilder.Create<PriceSearchResultItem>(p => p.CatalogItemId == productId.ToLowerInvariant());
                }
                else
                {
                    predicate = predicate.Or(p => p.CatalogItemId == productId.ToLowerInvariant());
                }

                isFirst = false;
            }

            return predicate;
        }

        /// <summary>
        /// Gets the product information from the index file.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="productIdList">The product identifier list.</param>
        /// <returns>
        /// The found product document instance; Otherwise null.
        /// </returns>
        protected virtual List<PriceSearchResultItem> GetProductsFromIndex(String catalogName, IEnumerable<string> productIdList)
        {
            var searchManager = CommerceTypeLoader.CreateInstance<ICommerceSearchManager>();
            var searchIndex = searchManager.GetIndex();

            var productPredicate = this.BuildProductIdListPredicate(productIdList);

            using (var context = searchIndex.CreateSearchContext())
            {
                var searchResults = context.GetQueryable<PriceSearchResultItem>()
                                    .Where(item => item.CommerceSearchItemType == CommerceSearchResultItemType.Product)
                                    .Where(item => item.CatalogName == catalogName)
                                    .Where(item => item.Language == Sitecore.Context.Language.Name)
                                    .Where(productPredicate)
                                    .Select(p => new PriceSearchResultItem { OtherFields = p.Fields, ListPrice = p.ListPrice, BasePrice = p.BasePrice, VariantInfo = p.VariantInfo, Name = p.Name });

                return searchResults.ToList();
            }
        }

        /// <summary>
        /// Sets the variant prices by looping through all of the variants returned by the index.
        /// </summary>
        /// <param name="productDocument">The product document.</param>
        /// <param name="variantInfoList">The variant information list.</param>
        /// <param name="extendedPrice">The extended price.</param>
        /// <param name="isLowestPriceVariantSpecified">if set to <c>true</c> the lowest priced variant adjusted price is returned.</param>
        /// <param name="isLowestPriceVariantListPriceSpecified">if set to <c>true</c> the lowest priced variant list price is returned.</param>
        /// <param name="isHighestPriceVariantSpecified">if set to <c>true</c> the highest priced variant adjusted price.</param>
        protected virtual void SetVariantPricesFromProductVariants(
            CommerceProductSearchResultItem productDocument,
            List<VariantIndexInfo> variantInfoList,
            ExtendedCommercePrice extendedPrice,
            bool isLowestPriceVariantSpecified,
            bool isLowestPriceVariantListPriceSpecified,
            bool isHighestPriceVariantSpecified)
        {
            if (variantInfoList != null && variantInfoList.Count > 0)
            {
                decimal highestPrice = 0.0M;
                decimal lowestPrice = 0.0M;
                decimal basePrice = 0.0M;
                bool processingFirstItem = true;

                foreach (var variantInfo in variantInfoList)
                {
                    if (processingFirstItem || variantInfo.ListPrice < lowestPrice)
                    {
                        lowestPrice = variantInfo.ListPrice;
                        basePrice = variantInfo.BasePrice;
                    }

                    if (processingFirstItem || variantInfo.ListPrice > highestPrice)
                    {
                        highestPrice = variantInfo.ListPrice;
                    }

                    processingFirstItem = false;
                }

                if (isLowestPriceVariantSpecified)
                {
                    extendedPrice.LowestPricedVariant = lowestPrice;
                }

                if (isLowestPriceVariantListPriceSpecified)
                {
                    extendedPrice.LowestPricedVariantListPrice = basePrice;
                }

                if (isHighestPriceVariantSpecified)
                {
                    extendedPrice.HighestPricedVariant = highestPrice;
                }
            }
        }
    }
}
